using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MyTCPServer
{
    class Client
    {
        public string Name { get; set; }
        public TcpClient tcpClient { get; set; }

        public Client(string name, TcpClient tcpclient)
        {
            this.Name = name;
            this.tcpClient = tcpclient;
        }
    }
}
