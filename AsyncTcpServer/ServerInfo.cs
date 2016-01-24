using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Comm;

namespace AsyncTcpServer
{
    public class ServerInfo:SocketInfo
    {
        public IPEndPoint Ip { get; set; }
    }
}
