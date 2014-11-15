namespace HttpHttpsProxy.Http
{
    public sealed class HttpHeaderField
    {
        public readonly string Name;

        private HttpHeaderField(string name)
        {
            Name = name;
        }

        // general headers
        public static readonly HttpHeaderField CacheControl = new HttpHeaderField("Cache-Control");
        public static readonly HttpHeaderField Connection = new HttpHeaderField("Connection");
        public static readonly HttpHeaderField Date = new HttpHeaderField("Date");
        public static readonly HttpHeaderField Pragma = new HttpHeaderField("Pragma");
        public static readonly HttpHeaderField Trailer = new HttpHeaderField("Trailer");
        public static readonly HttpHeaderField TransferEncoding = new HttpHeaderField("Transfer-Encoding");
        public static readonly HttpHeaderField Upgrade = new HttpHeaderField("Upgrade");
        public static readonly HttpHeaderField Via = new HttpHeaderField("Via");
        public static readonly HttpHeaderField Warning = new HttpHeaderField("Warning");

        // request headers
        public static readonly HttpHeaderField Accept = new HttpHeaderField("Accept");
        public static readonly HttpHeaderField AcceptCharset = new HttpHeaderField("Accept-Charset");
        public static readonly HttpHeaderField AcceptEncoding = new HttpHeaderField("Accept-Encoding");
        public static readonly HttpHeaderField AcceptLanguage = new HttpHeaderField("Accept-Language");
        public static readonly HttpHeaderField Authorization = new HttpHeaderField("Authorization");
        public static readonly HttpHeaderField Expect = new HttpHeaderField("Expect");
        public static readonly HttpHeaderField From = new HttpHeaderField("From");
        public static readonly HttpHeaderField Host = new HttpHeaderField("Host");
        public static readonly HttpHeaderField IfMatch = new HttpHeaderField("If-Match");
        public static readonly HttpHeaderField IfModifiedSince = new HttpHeaderField("If-Modified-Since");
        public static readonly HttpHeaderField IfNoneMatch = new HttpHeaderField("If-None-Match");
        public static readonly HttpHeaderField IfRange = new HttpHeaderField("If-Range");
        public static readonly HttpHeaderField IfUnmodifiedSince = new HttpHeaderField("If-Unmodified-Since");
        public static readonly HttpHeaderField MaxForwards = new HttpHeaderField("Max-Forwards");
        public static readonly HttpHeaderField ProxyAuthorization = new HttpHeaderField("Proxy-Authorization");
        public static readonly HttpHeaderField Range = new HttpHeaderField("Range");
        public static readonly HttpHeaderField Referer = new HttpHeaderField("Referer");
        public static readonly HttpHeaderField TE = new HttpHeaderField("TE");
        public static readonly HttpHeaderField UserAgent = new HttpHeaderField("User-Agent");

        // response headers
        public static readonly HttpHeaderField AcceptRanges = new HttpHeaderField("Accept-Ranges");
        public static readonly HttpHeaderField Age = new HttpHeaderField("Age");
        public static readonly HttpHeaderField ETag = new HttpHeaderField("ETag");
        public static readonly HttpHeaderField Location = new HttpHeaderField("Location");
        public static readonly HttpHeaderField ProxyAuthenticate = new HttpHeaderField("Proxy-Authenticate");
        public static readonly HttpHeaderField RetryAfter = new HttpHeaderField("Retry-After");
        public static readonly HttpHeaderField Server = new HttpHeaderField("Server");
        public static readonly HttpHeaderField Vary = new HttpHeaderField("Vary");
        public static readonly HttpHeaderField WWWAuthenticate = new HttpHeaderField("WWW-Authenticate");

        // entity headers
        public static readonly HttpHeaderField Allow = new HttpHeaderField("Allow");
        public static readonly HttpHeaderField ContentEncoding = new HttpHeaderField("Content-Encoding");
        public static readonly HttpHeaderField ContentLanguage = new HttpHeaderField("Content-Language");
        public static readonly HttpHeaderField ContentLength = new HttpHeaderField("Content-Length");
        public static readonly HttpHeaderField ContentLocation = new HttpHeaderField("Content-Location");
        public static readonly HttpHeaderField ContentMD5 = new HttpHeaderField("Content-MD5");
        public static readonly HttpHeaderField ContentRange = new HttpHeaderField("Content-Range");
        public static readonly HttpHeaderField ContentType = new HttpHeaderField("Content-Type");
        public static readonly HttpHeaderField Expires = new HttpHeaderField("Expires");
        public static readonly HttpHeaderField LastModified = new HttpHeaderField("Last-Modified");

        // state management
        public static readonly HttpHeaderField SetCookie = new HttpHeaderField("Set-Cookie");
        public static readonly HttpHeaderField Cookie = new HttpHeaderField("Cookie");

    }
}
