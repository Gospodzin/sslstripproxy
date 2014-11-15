using System.IO;
using System.Net;
using System.Net.Sockets;

namespace SSLStripProxy
{
    class ClientConnection
    {
        private const int BufferSize = 8*1024;
        public readonly byte[] BufferBytes = new byte[BufferSize];

        private readonly TcpClient _tcpClient;

        public ClientConnection(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
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

        public void Send(byte[] message)
        {
             _tcpClient.GetStream().Write(message, 0, message.Length);
        }

        public EndPoint GetClientEndpoint()
        {
            return _tcpClient.Client.RemoteEndPoint;
        }

        public void Close()
        {
            _tcpClient.Close();
        }
    }
}
