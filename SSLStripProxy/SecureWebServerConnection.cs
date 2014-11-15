using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace SSLStripProxy
{
    class SecureWebServerConnection : IWebServerConnection
    {
        private readonly string _server;
        private readonly int _port;

        private const int BufferSize = 8 * 1024;
        public readonly byte[] BufferBytes = new byte[BufferSize];

        private SslStream _sslStream;

        public SecureWebServerConnection(string server, int port)
        {
            _server = server;
            _port = port;
        }

        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Logger.Log("Certificate error: " + sslPolicyErrors);

            return true;
        }

        public void Connect()
        {
            TcpClient client = new TcpClient(_server, _port);

            _sslStream = new SslStream(client.GetStream(), false, ValidateServerCertificate, null);

            try
            {
                _sslStream.AuthenticateAsClient(_server);
            }
            catch (AuthenticationException e)
            {
                Logger.Log(e.Message);
                client.Close();
            }
        }

        public void Send(byte[] message)
        {
            _sslStream.Write(message, 0, message.Length);
            _sslStream.Flush();
        }

        public byte[] Receive()
        {
            var readStream = _sslStream;
            MemoryStream messageBytes = new MemoryStream();
            int bytesRead;
            do
            {
                bytesRead = readStream.Read(BufferBytes, 0, BufferBytes.Length);
                messageBytes.Write(BufferBytes, 0, bytesRead);
            } while (bytesRead == BufferBytes.Length);

            return messageBytes.ToArray();
        }

        public void Close()
        {
            if (_sslStream != null)
            {
                _sslStream.Close();
            }
        }
    }


}
