using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MyTCPServer
{
    class Client: TcpClient
    {
        public string Name { get; set; }
        public Client(string name)
        {
            this.Name = name;
        }
    }
}
