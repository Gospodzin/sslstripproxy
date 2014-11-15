using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Web.UI;
using HttpHttpsProxy.Http;
using HttpRequest = SSLStripProxy.Http.HttpRequest;
using HttpResponse = SSLStripProxy.Http.HttpResponse;

namespace SSLStripProxy
{
    class ClientWebServerBinding
    {
        private static int _idCounter;
        private readonly int _myId;
        private readonly ClientConnection _clientConnection;
        private IWebServerConnection _webServerConnection;

        private bool _shouldPass;

        public ClientWebServerBinding(ClientConnection clientConnection)
        {
            _clientConnection = clientConnection;
            _myId = _idCounter++;
        }

        public void Connect()
        {
            var messageBytes = _clientConnection.Receive();
            if (messageBytes.Length == 0)
            {
                _clientConnection.Close();
                return;
            }

            Http.HttpRequest httpRequest = new Http.HttpRequest(messageBytes);
            httpRequest.Parse();

            if (httpRequest.Method == WebRequestMethods.Http.Connect)
            {
                Logger.LogSetValue("connect", httpRequest.RequestUri);
                Logger.Log("Client requesting CONNECT method. " + httpRequest.RequestUri);

                SendNotFoundResponse();

                _clientConnection.Close();
                return;
            }

            _webServerConnection = GetWebServerConnection(httpRequest);

            try
            {
                _webServerConnection.Connect();
                Begin(httpRequest);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message);
            }
        }

        public void Begin(Http.HttpRequest httpRequest)
        {
            string from = GetTargetUri(httpRequest).ToString();
            RequestProcessor.Process(httpRequest);

            Logger.Log(new Pair(_myId, httpRequest), Logger.LogType.Request);
            Logger.Log(new Pair(_myId, from), Logger.LogType.Uri);
            Logger.Log(new Pair(_myId, _clientConnection.GetClientEndpoint().ToString()), Logger.LogType.Client);

            Http.HttpResponse httpResponse = ReceiveWholeResponse(httpRequest);
            Logger.LogSetValue("statuses", httpResponse.StatusCode + " " + httpResponse.ReasonPhrase);
           
            if (!_shouldPass)
            {
                ResponseProcessor.Process(httpResponse, from);
                Logger.Log(new Pair(_myId, httpResponse), Logger.LogType.Response);
                Logger.Log(new object[]{_clientConnection.GetClientEndpoint().ToString(), from, httpRequest, httpResponse},Logger.LogType.ToFile);
                _clientConnection.Send(httpResponse.Compile());
            }

            Close();
        }

        private void SendNotFoundResponse()
        {
            const string notFoundResponseStr = "HTTP/1.1 404 Not Found\r\n\r\n";
            _clientConnection.Send(Encoding.ASCII.GetBytes(notFoundResponseStr));
        }

        private static Uri GetTargetUri(Http.HttpRequest httpRequest)
        {
            Uri uri;
            if (HttpsMapper.IsHttps(httpRequest.RequestUri))
                uri = new Uri("https" + httpRequest.RequestUri.Substring("http".Length));
            else
                uri = new Uri(httpRequest.RequestUri);

            return uri;
        }

        private IWebServerConnection GetWebServerConnection(Http.HttpRequest httpRequest)
        {
            IWebServerConnection webServerConnection = null;

            Uri uri = GetTargetUri(httpRequest);

            switch (uri.Port)
            {
                case 80:
                    webServerConnection = new WebServerConnection(uri.Host, uri.Port);
                    break;
                case 443:
                    webServerConnection = new SecureWebServerConnection(uri.Host, uri.Port);
                    break;
            }

            return webServerConnection;
        }

        private static bool ShouldPass(Http.HttpResponse httpResponse)
        {
            string contentTypeStr = httpResponse.Headers.Get(HttpHeaderField.ContentType.Name);
            if (contentTypeStr != null)
            {
                ContentType contentType = new ContentType(contentTypeStr);
                Logger.LogSetValue("content_type", contentType.MediaType);
                if (contentType.MediaType != "text/html"
                    && contentType.MediaType != "text/plain"
                    && contentType.MediaType != "text/css"
                    && contentType.MediaType != "text/javascript"
                    && contentType.MediaType != "application/x-javascript"
                    && contentType.MediaType != "application/javascript")
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        private Http.HttpResponse ReceiveWholeResponse(Http.HttpRequest httpRequest)
        {
            _webServerConnection.Send(httpRequest.Compile());

            var responseBytes = ReceiveWholeHeader();
            Http.HttpResponse responseHeader = new Http.HttpResponse(responseBytes);
            responseHeader.Parse();

            _shouldPass = ShouldPass(responseHeader);

            if (_shouldPass)
                _clientConnection.Send(responseBytes);
            byte[] received = _webServerConnection.Receive();
            if (_shouldPass)
                _clientConnection.Send(received);
            while (received.Length > 0)
            {
                responseBytes = responseBytes.Concat(received).ToArray();
                received = _webServerConnection.Receive();
                if (_shouldPass)
                    _clientConnection.Send(received);
            }

            Http.HttpResponse httpResponse = new Http.HttpResponse(responseBytes);
            httpResponse.Parse();

            if (httpResponse.Headers[HttpHeaderField.TransferEncoding.Name] != null)
                ReceiveByChunkedEncoding(httpResponse);
            else if (httpResponse.Headers[HttpHeaderField.ContentLength.Name] != null)
                ReceiveByContentLength(httpResponse);

            return httpResponse;
        }

        private byte[] ReceiveWholeHeader()
        {
            List<byte> responseBytes = new List<byte>();
            do
            {
                responseBytes.AddRange(_webServerConnection.Receive());
            } while (!Encoding.UTF8.GetString(responseBytes.ToArray()).Contains("\r\n\r\n"));

            return responseBytes.ToArray();
        }

        private void ReceiveByContentLength(Http.HttpResponse httpResponse)
        {
            int contentLength = int.Parse(httpResponse.Headers[HttpHeaderField.ContentLength.Name]);
            int bytesRead = httpResponse.Content.Length;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(httpResponse.Content, 0, httpResponse.Content.Length);
                while (bytesRead < contentLength)
                {
                    var responsePart = _webServerConnection.Receive();
                    if (_shouldPass)
                        _clientConnection.Send(responsePart);
                    bytesRead += responsePart.Length;
                    memoryStream.Write(responsePart, 0, responsePart.Length);
                }
                httpResponse.Content = memoryStream.ToArray();
            }
        }

        private void ReceiveByChunkedEncoding(Http.HttpResponse httpResponse)
        {
            //Chunked-Body   = *chunk
            //                last-chunk
            //                trailer
            //                CRLF
            //chunk          = chunk-size [ chunk-extension ] CRLF
            //                chunk-data CRLF
            //chunk-size     = 1*HEX
            //last-chunk     = 1*("0") [ chunk-extension ] CRLF
            //chunk-extension= *( ";" chunk-ext-name [ "=" chunk-ext-val ] )
            //chunk-ext-name = token
            //chunk-ext-val  = token | quoted-string
            //chunk-data     = chunk-size(OCTET)
            //trailer        = *(entity-header CRLF)

            byte[] crlf = Encoding.ASCII.GetBytes("\r\n");

            List<byte> content = new List<byte>();

            List<byte> chunkedBody = new List<byte>();
            chunkedBody.AddRange(httpResponse.Content);
            int chunkSize = -1;
            while (chunkSize != 0)
            {
                int metaEndIdx = chunkedBody.IndexOf(crlf[0]);
                while (metaEndIdx < 0)
                {
                    var chunk = _webServerConnection.Receive();
                    chunkedBody.AddRange(chunk);
                    if (_shouldPass)
                        _clientConnection.Send(chunk);
                    metaEndIdx = chunkedBody.IndexOf(crlf[0]);
                }

                var chunkExtTagIdx = chunkedBody.IndexOf(Encoding.ASCII.GetBytes(";")[0]);
                if (chunkExtTagIdx > -1 && chunkExtTagIdx < metaEndIdx)
                {
                    Logger.Log("[Chunk Extension]");
                }
                var chunkSizeStr = Encoding.ASCII.GetString(chunkedBody.Take(metaEndIdx).ToArray());
                chunkSize = int.Parse(chunkSizeStr, NumberStyles.HexNumber);

                while (chunkedBody.Count < metaEndIdx + crlf.Length + chunkSize + crlf.Length)
                {
                    var chunk = _webServerConnection.Receive();
                    chunkedBody.AddRange(chunk);
                    if (_shouldPass)
                        _clientConnection.Send(chunk);
                }

                content.AddRange(chunkedBody.Skip(metaEndIdx + crlf.Length).Take(chunkSize));
                chunkedBody.RemoveRange(0, metaEndIdx + crlf.Length + chunkSize + crlf.Length);
            }

            httpResponse.Content = content.ToArray();

            httpResponse.Headers.Remove(HttpHeaderField.TransferEncoding.Name);
            httpResponse.Headers.Add(HttpHeaderField.ContentLength.Name, httpResponse.Content.Length.ToString(CultureInfo.InvariantCulture));
        }

        public void Close()
        {
            _clientConnection.Close();
            _webServerConnection.Close();
        }
    }
}
