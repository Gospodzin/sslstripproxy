using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace SSLStripProxy.Http
{
    public class HttpRequest
    {
        private const string CRLF = "\r\n";
        private const string SP = " ";
        private const string HT = "\t";

        private const string HeaderEndTag = CRLF + CRLF;
        private static readonly byte[] HeaderEndTagBytes = Encoding.ASCII.GetBytes(HeaderEndTag);

        private readonly byte[] _httpRequestBytes;

        public HttpRequest(byte[] httpRequestBytes)
        {
            _httpRequestBytes = httpRequestBytes;

            Headers = new WebHeaderCollection();
        }

        #region properties
        public string Method { get; set; }

        public string RequestUri { get; set; }

        public string HttpVersion { get; set; }

        public WebHeaderCollection Headers { get; set; }

        public int ContentStartIdx { get; set; }

        #endregion

        #region compiling
        private byte[] CompileRequestLine()
        {
            // Request-Line   = Method SP Request-URI SP HTTP-Version CRLF
            string requestLine = Method + SP + RequestUri + SP + HttpVersion + CRLF;
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

        public byte[] Compile()
        {
            byte[] requestLineBytes = CompileRequestLine();
            byte[] headersBytes = CompileHeaders();

            byte[] requestBytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(requestLineBytes, 0, requestLineBytes.Length);
                memoryStream.Write(headersBytes, 0, headersBytes.Length);
                memoryStream.Write(_httpRequestBytes, ContentStartIdx, _httpRequestBytes.Length - ContentStartIdx);
                requestBytes = memoryStream.ToArray();
            }

            return requestBytes;
        }
        #endregion

        
        
        
        #region parsing
        private void ParseRequestLine(string requestLine)
        {
            // Request-Line   = Method SP Request-URI SP HTTP-Version CRLF
            var requestLineWords = requestLine.Split(new[] { SP, CRLF }, StringSplitOptions.None);
            Method = requestLineWords[0];
            RequestUri = requestLineWords[1];
            HttpVersion = requestLineWords[2];
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

                Headers.Add(fieldName, fieldValue);
            }
        }

        private int GetHeaderOctetsCount()
        {
            for (int i = 0; i < _httpRequestBytes.Length - HeaderEndTagBytes.Length; i++)
            {
                if (_httpRequestBytes.Skip(i).Take(HeaderEndTag.Length).SequenceEqual(HeaderEndTagBytes))
                {
                    return i + HeaderEndTagBytes.Length;
                }
            }

            // no end tag
            return _httpRequestBytes.Length;
        }

        public void Parse()
        {
            // Request       = Request-Line             
            //                 *(( general-header       
            //                  | request-header       
            //                  | entity-header ) CRLF) 
            //                 CRLF
            //                 [ message-body ]      

            int headerOctetsCount = GetHeaderOctetsCount();
            string header = Encoding.UTF8.GetString(_httpRequestBytes, 0, headerOctetsCount);
            int headerLength = header.Length;

            int requestLineLength = header.IndexOf(CRLF, StringComparison.Ordinal) + CRLF.Length;
            string requestLine = header.Substring(0, requestLineLength);
            ParseRequestLine(requestLine);

            int headersLength = headerLength - requestLineLength;
            string headers = header.Substring(requestLineLength, headersLength);
            // sometimes lacks end tag
            if (!headers.EndsWith(HeaderEndTag))
                headers += HeaderEndTag;
            ParseHeaders(headers);

            ContentStartIdx = headerLength;
        }
        #endregion
    }
}
