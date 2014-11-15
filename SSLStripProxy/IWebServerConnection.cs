using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSLStripProxy
{
    interface IWebServerConnection
    {
        void Connect();
        void Send(byte[] message);
        byte[] Receive();
        void Close();
    }
}
