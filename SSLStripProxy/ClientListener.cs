using System;
using System.Net;
using System.Net.Sockets;

namespace SSLStripProxy
{
    class ClientListener
    {
        private const int Backlog = 0;

        private readonly int _listeningPort;

        public ClientListener(int listeningPort)
        {
            _listeningPort = listeningPort;
        }

        public void StartListening()
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, _listeningPort);
            try
            {
                tcpListener.Start(Backlog);
                tcpListener.BeginAcceptTcpClient(AcceptCallback, tcpListener);
            }
            catch (Exception e)
            {
                Logger.Log(e.ToString());
            }
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            var listener = (TcpListener)ar.AsyncState;

            listener.BeginAcceptTcpClient(AcceptCallback, listener);

            TcpClient tcpClient = listener.EndAcceptTcpClient(ar);

            ClientConnection clientConnection = new ClientConnection(tcpClient);
            ClientWebServerBinding clientWebServerBinding = new ClientWebServerBinding(clientConnection);
            clientWebServerBinding.Connect();
        }
    }
}
