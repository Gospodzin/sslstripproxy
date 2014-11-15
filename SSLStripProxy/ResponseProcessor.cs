using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using HttpHttpsProxy.Http;
using SSLStripProxy.Http;
using HttpResponse = SSLStripProxy.Http.HttpResponse;

namespace SSLStripProxy
{
    class ResponseProcessor
    {
        private static void Handle(Http.HttpResponse httpResponse, string from)
        {
            string contentTypeStr = httpResponse.Headers.Get(HttpHeaderField.ContentType.Name);
            if (contentTypeStr == null) return;

            ContentType contentType = new ContentType(contentTypeStr);

            if (httpResponse.Content.Length == 0)
                return;

            var contentBytes = httpResponse.Content;

            Encoding httpHeaderEncoding = null;
            if (contentType.CharSet != null)
            {
                httpHeaderEncoding = Encoding.GetEncoding(contentType.CharSet);
            }

            Encoding docEncoding = httpHeaderEncoding ?? Encoding.UTF8;
            string content = docEncoding.GetString(contentBytes);

            httpResponse.Content = docEncoding.GetBytes(HandleHttps(content, new Uri(from)));
            httpResponse.Headers.Set(HttpHeaderField.ContentLength.Name, httpResponse.Content.Length.ToString(CultureInfo.InvariantCulture));
        }

        private static string HandleHttps(string content, Uri fromUri)
        {
            {
                StringBuilder contentBuilder = new StringBuilder();
                const string pattern = @"https://[-.a-z0-9]+(/[-.a-z0-9_]*)*";

                Regex r = new Regex(pattern, RegexOptions.IgnoreCase);

                Match m = r.Match(content);
                int offset = 0;
                while (m.Success)
                {
                    Group group = m.Groups[0];
                    contentBuilder.Append(content.Substring(offset, group.Index - offset));
                    var doneUri = "http" + group.Value.Substring("https".Length);
                    HttpsMapper.AddHttps(doneUri);
                    contentBuilder.Append(doneUri);
                    offset = group.Index + group.Length;
                    m = m.NextMatch();
                }
                contentBuilder.Append(content.Substring(offset));

                content = contentBuilder.ToString();
            }
            {
                StringBuilder contentBuilder = new StringBuilder();
                const string pattern = @"https:\\/\\/[-.a-z0-9]+(\\/[-.a-z0-9_]*)*";

                Regex r = new Regex(pattern, RegexOptions.IgnoreCase);

                Match m = r.Match(content);
                int offset = 0;
                while (m.Success)
                {
                    Group group = m.Groups[0];
                    contentBuilder.Append(content.Substring(offset, group.Index - offset));
                    var doneUri = "http" + group.Value.Substring("https".Length);
                    HttpsMapper.AddHttps(doneUri.Replace("\\/", "/"));
                    contentBuilder.Append(doneUri);
                    offset = group.Index + group.Length;
                    m = m.NextMatch();
                }
                contentBuilder.Append(content.Substring(offset));

                content = contentBuilder.ToString();
            }

            if (fromUri.Scheme == "https")
            {
                {
                    const string pattern = "\"(/[-.a-z0-9]+(/[-.a-z0-9_]*)*)\"";

                    Regex r = new Regex(pattern, RegexOptions.IgnoreCase);

                    Match m = r.Match(content);
                    while (m.Success)
                    {
                        Group group = m.Groups[1];
                        var doneUri = new Uri(fromUri, group.Value).ToString();
                        HttpsMapper.AddHttps(doneUri);
                        m = m.NextMatch();
                    }
                }

                {
                    const string pattern = "\'(/[-.a-z0-9]+(/[-.a-z0-9_]*)*)\'";

                    Regex r = new Regex(pattern, RegexOptions.IgnoreCase);

                    Match m = r.Match(content);
                    while (m.Success)
                    {
                        Group group = m.Groups[1];
                        var doneUri = new Uri(fromUri, group.Value).ToString();
                        HttpsMapper.AddHttps(doneUri);
                        m = m.NextMatch();
                    }
                }

                {
                    const string pattern = "(src|action|href|formaction)=\"(.+?)\"";

                    Regex r = new Regex(pattern, RegexOptions.IgnoreCase);

                    Match m = r.Match(content);

                        while (m.Success)
                        {
                            Group group = m.Groups[2];
                            try
                            {
                                if (!group.Value.StartsWith("http:"))
                                {
                                    var doneUri = new Uri(fromUri, group.Value).ToString();
                                    HttpsMapper.AddHttps(doneUri);
                                }
                            }
                            catch
                            {
                            }
                            m = m.NextMatch();
                        }
                    
                }
            }

            return content;
        }

        private static bool ShouldProcess(Http.HttpResponse httpResponse)
        {
            string contentTypeStr = httpResponse.Headers.Get(HttpHeaderField.ContentType.Name);
            if (contentTypeStr != null)
            {
                ContentType contentType = new ContentType(contentTypeStr);
                Logger.LogSetValue("content_type", contentType.MediaType);
                if (!(IsJavaType(httpResponse)||IsTextCssType(httpResponse)||IsTextHtmlType(httpResponse)))
                {
                    return false;
                }
                return true;
            }
            return true;
        }

        public static void Process(Http.HttpResponse httpResponse, string from)
        {
            if(ShouldProcess(httpResponse))
            {
                DecodeContent(httpResponse);
                Handle(httpResponse, from);
            }

            HandleSecureCookies(httpResponse);

            HandleHttpsRedirect(httpResponse);
        }

        private static void HandleSecureCookies(Http.HttpResponse httpResponse)
        {
            List<string> setCookies = new List<string>();
            foreach (var cookie in httpResponse.Cookies)
            {
                setCookies.Add(cookie.Replace("; Secure", "").Replace(";Secure", "")
                    .Replace("; secure", "").Replace(";secure", ""));
            }
            httpResponse.Cookies = setCookies;
        }

        private static void HandleHttpsRedirect(Http.HttpResponse httpResponse)
        {
            if (httpResponse.StatusCode != ((int)HttpStatusCode.Found).ToString(CultureInfo.InvariantCulture)
                && httpResponse.StatusCode != ((int)HttpStatusCode.MovedPermanently).ToString(CultureInfo.InvariantCulture))
                return;

            var uriStr = httpResponse.Headers.Get(HttpHeaderField.Location.Name);
            if (uriStr.StartsWith("https:"))
                //httpResponse.Headers[HttpHeaderField.Location.Name] = HttpsMapper.ConvertHttpsToXHttpUri(uriStr);
            {
                httpResponse.Headers[HttpHeaderField.Location.Name] = "Http" + uriStr.Substring("https".Length);
            HttpsMapper.AddHttps(uriStr);
            }
        }

        private static void DecodeContent(Http.HttpResponse httpResponse)
        {
            if (httpResponse.Content == null)
            {
                return;
            }

            var contentEncodingStr = httpResponse.Headers.Get(HttpHeaderField.ContentEncoding.Name);
            if (contentEncodingStr == null)
            {
                return;
            }

            byte[] decodedContent;
            if (contentEncodingStr == ContentCoding.GZip.Name)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                using (GZipStream gZipStream = new GZipStream(new MemoryStream(httpResponse.Content),
                    CompressionMode.Decompress))
                {

                    byte[] bufferBytes = new byte[1024];
                    int readCount = gZipStream.Read(bufferBytes, 0, bufferBytes.Length);
                    while (readCount > 0)
                    {
                        memoryStream.Write(bufferBytes, 0, readCount);
                        readCount = gZipStream.Read(bufferBytes, 0, bufferBytes.Length);
                    }
                    decodedContent = memoryStream.ToArray();
                }
            }
            else
            {
                throw new NotImplementedException("Not implemented content-coding.");
            }

            httpResponse.Content = decodedContent;
            httpResponse.Headers.Remove(HttpHeaderField.ContentEncoding.Name);
            httpResponse.Headers.Set(HttpHeaderField.ContentLength.Name, httpResponse.Content.Length.ToString(CultureInfo.InvariantCulture));
        }

        public static bool IsTextHtmlType(Http.HttpResponse httpResponse)
        {
            string contentTypeStr = httpResponse.Headers.Get(HttpHeaderField.ContentType.Name);
            if (contentTypeStr == null) return false;

            ContentType contentType = new ContentType(contentTypeStr);
            if (contentType.MediaType != "text/html") return false;

            return true;
        }

        public static bool IsTextCssType(Http.HttpResponse httpResponse)
        {
            string contentTypeStr = httpResponse.Headers.Get(HttpHeaderField.ContentType.Name);
            if (contentTypeStr == null) return false;

            ContentType contentType = new ContentType(contentTypeStr);
            if (contentType.MediaType != "text/css") return false;

            return true;
        }

        public static bool IsJavaType(Http.HttpResponse httpResponse)
        {
            string contentTypeStr = httpResponse.Headers.Get(HttpHeaderField.ContentType.Name);
            if (contentTypeStr == null) return false;

            ContentType contentType = new ContentType(contentTypeStr);
            if (contentType.MediaType != "application/javascript" && contentType.MediaType != "application/x-javascript" && contentType.MediaType != "text/javascript" && contentType.MediaType != "text/plain") return false;

            return true;
        }
    }
}
