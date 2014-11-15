using System;
using System.IO;
using System.Net.Sockets;

namespace SSLStripProxy
{
    class WebServerConnection : IWebServerConnection
    {
        private readonly string _host;
        private readonly int _port;

        private TcpClient _tcpClient;

        private const int BufferSize = 8 * 1024;
        public readonly byte[] BufferBytes = new byte[BufferSize];

        public WebServerConnection(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public void Connect()
        {
            try
            {
                _tcpClient = new TcpClient(_host, _port);
            }
            catch (Exception e)
            {
                Logger.Log(e.ToString());
            }
        }

        public void Send(byte[] message)
        {
            _tcpClient.GetStream().Write(message, 0, message.Length);
        }

        public byte[] Receive()
        {
            var readStream = _tcpClient.GetStream();
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
            _tcpClient.Close();
        }
    }


}
