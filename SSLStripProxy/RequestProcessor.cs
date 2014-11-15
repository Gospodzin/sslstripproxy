using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpHttpsProxy.Http;
using SSLStripProxy.Http;

namespace SSLStripProxy
{
    class RequestProcessor
    {
        public static void Process(HttpRequest httpRequest)
        {
            HandleRequestUri(httpRequest);
            HandleReferer(httpRequest);
            HandleConnection(httpRequest);

        }

        public static void HandleRequestUri(HttpRequest httpRequest)
        {
            httpRequest.RequestUri = new Uri(httpRequest.RequestUri).PathAndQuery;
        }

        private static void HandleReferer(HttpRequest httpRequest)
        {
            if (httpRequest.Headers[HttpHeaderField.Referer.Name] == null) return;

            if (HttpsMapper.IsHttps(httpRequest.Headers[HttpHeaderField.Referer.Name]))
                httpRequest.Headers[HttpHeaderField.Referer.Name] = "https" + httpRequest.Headers[HttpHeaderField.Referer.Name].Substring("http".Length);
        }

        private static void HandleConnection(HttpRequest httpRequest)
        {
            httpRequest.Headers.Remove(HttpHeaderField.Connection.Name);
            httpRequest.Headers.Add(HttpHeaderField.Connection.Name, "close");
        }
    }
}
