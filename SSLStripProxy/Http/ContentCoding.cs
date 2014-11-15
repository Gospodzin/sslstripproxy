namespace SSLStripProxy.Http
{
    public sealed class ContentCoding
    {
        public readonly string Name;

        private ContentCoding(string name)
        {
            Name = name;
        }

        public static readonly ContentCoding GZip = new ContentCoding("gzip");
        public static readonly ContentCoding Compress = new ContentCoding("compress");
        public static readonly ContentCoding Deflate = new ContentCoding("deflate");
        public static readonly ContentCoding Identity = new ContentCoding("identity");
    }
}
