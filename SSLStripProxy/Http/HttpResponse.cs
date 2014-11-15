using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using HttpHttpsProxy.Http;

namespace SSLStripProxy.Http
{
    public class HttpResponse
    {
        private const string CRLF = "\r\n";
        private const string SP = " ";
        private const string HT = "\t";

        private const string HeaderEndTag = CRLF + CRLF;
        private static readonly byte[] HeaderEndTagBytes = Encoding.ASCII.GetBytes(HeaderEndTag);


        public HttpResponse(byte[] originalHttpResponseBytes)
        {
            OriginalHttpResponseBytes = originalHttpResponseBytes;

            Headers = new WebHeaderCollection();

            Cookies = new List<string>();
        }

        #region properties
        public byte[] OriginalHttpResponseBytes { get; set; }

        public string HttpVersion { get; set; }

        public string StatusCode { get; set; }

        public string ReasonPhrase { get; set; }

        public WebHeaderCollection Headers { get; set; }

        public List<String> Cookies { get; set; }

        public byte[] Content { get; set; }

        public int HeaderOctetsCount { get; set; }
        #endregion

        #region compiling
        private byte[] CompileStatusLine()
        {
            // Status-Line = HTTP-Version SP Status-Code SP Reason-Phrase CRLF
            string requestLine = HttpVersion + SP + StatusCode + SP + ReasonPhrase + CRLF;
            return Encoding.ASCII.GetBytes(requestLine);
        }

        private byte[] CompileHeaders()
        {
            // Headers
            // message-header = field-name ":" [ field-value ]
            // field-name     = token
            // field-value    = *( field-content | LWS )
            // field-content  = <the OCTETs making up the field-value
            //                 and consisting of either *TEXT or combinations
            //                 of token, separators, and quoted-string>

            return Headers.ToByteArray();
        }

        private byte[] CompileSetCookies()
        {
            StringBuilder setCookies = new StringBuilder();
            foreach (var cookie in Cookies)
            {
                string setCookie = HttpHeaderField.SetCookie.Name + ":" + cookie + CRLF;
                setCookies.Append(setCookie);
            }

            var setCookiesBytes = Encoding.UTF8.GetBytes(setCookies.ToString());
            return setCookiesBytes;
        }

        public byte[] Compile()
        {
            byte[] statusLineBytes = CompileStatusLine();
            byte[] setCookiesBytes = CompileSetCookies();
            byte[] headersBytes = CompileHeaders();

            byte[] responseBytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(statusLineBytes, 0, statusLineBytes.Length);
                memoryStream.Write(setCookiesBytes, 0, setCookiesBytes.Length);
                memoryStream.Write(headersBytes, 0, headersBytes.Length);
                memoryStream.Write(Content, 0, Content.Length);
                responseBytes = memoryStream.ToArray();
            }

            return responseBytes;
        }
        #endregion

        #region parsing
        private void ParseStatusLine(string requestLine)
        {
            // Status-Line = HTTP-Version SP Status-Code SP Reason-Phrase CRLF
            var requestLineWords = requestLine.Split(new[] { SP, CRLF }, 3, StringSplitOptions.None);
            HttpVersion = requestLineWords[0];
            StatusCode = requestLineWords[1];
            ReasonPhrase = requestLineWords[2].Remove(requestLineWords[2].Length - CRLF.Length);
        }


        private void ParseHeaders(string headers)
        {
            // Headers
            // message-header = field-name ":" [ field-value ]
            // field-name     = token
            // field-value    = *( field-content | LWS )
            // field-content  = <the OCTETs making up the field-value
            //                 and consisting of either *TEXT or combinations
            //                 of token, separators, and quoted-string>

            for (int offset = 0; headers.Substring(offset, 2) != CRLF; )
            {
                int fieldNamelength = headers.IndexOf(":", offset, StringComparison.Ordinal) - offset;
                string fieldName = headers.Substring(offset, fieldNamelength);
                offset += fieldNamelength + ":".Length;

                string fieldValue = String.Empty;
                do
                {
                    int fieldValuePartLength = headers.IndexOf(CRLF, offset, StringComparison.Ordinal) - offset;
                    fieldValue += headers.Substring(offset, fieldValuePartLength);
                    offset += fieldValuePartLength;
                } while (headers.Substring(offset + CRLF.Length, 1) == SP || headers.Substring(offset + CRLF.Length, 1) == HT);
                offset += CRLF.Length;

                if (fieldName == HttpHeaderField.SetCookie.Name)
                    ParseSetCookie(fieldValue);
                else
                    Headers.Add(fieldName, fieldValue);
            }
        }

        private void ParseSetCookie(string setCookieVal)
        {
            Cookies.Add(setCookieVal);
        }

        private int GetHeaderOctetsCount()
        {
            for (int i = 0; i < OriginalHttpResponseBytes.Length - HeaderEndTagBytes.Length; i++)
            {
                if (OriginalHttpResponseBytes.Skip(i).Take(HeaderEndTag.Length).SequenceEqual(HeaderEndTagBytes))
                {
                    return i + HeaderEndTagBytes.Length;
                }
            }

            // no end tag
            return OriginalHttpResponseBytes.Length;
        }

        public void Parse()
        {
            // Request       = Request-Line             
            //                 *(( general-header       
            //                  | request-header       
            //                  | entity-header ) CRLF) 
            //                 CRLF
            //                 [ message-body ]      

            HeaderOctetsCount = GetHeaderOctetsCount();
            string header = Encoding.UTF8.GetString(OriginalHttpResponseBytes, 0, HeaderOctetsCount);
            int headerLength = header.Length;

            int statusLineLength = header.IndexOf(CRLF, StringComparison.Ordinal) + CRLF.Length;
            string statusLine = header.Substring(0, statusLineLength);
            ParseStatusLine(statusLine);

            int headersLength = headerLength - statusLineLength;
            string headers = header.Substring(statusLineLength, headersLength);
            // sometimes lacks end tag
            if (!headers.EndsWith(HeaderEndTag))
                headers += HeaderEndTag;
            ParseHeaders(headers);

            Content = OriginalHttpResponseBytes.Skip(HeaderOctetsCount).ToArray();
        }
        #endregion
    }
}
