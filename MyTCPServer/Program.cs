using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MyTCPServer
{
    class Program
    {
        static ConsoleColor cc;
        static List<Client> clients = new List<Client>();
        static void Main(string[] args)
        {
            cc = Console.ForegroundColor;
            while (true)
            {
                UI();
            }
        }

        static void SendToAll(DetailedMessage dm)
        {
            foreach (Client item in clients)
            {
                if (item.Name != dm.Name)
                {
                    item.GetStream().Write(dm, 0, dm.Length);
                }
            }
        }

        static void UI()
        {
            Console.Write("~ ");
            string x = Console.ReadLine();
            if (x.First() == '-')
            {
                string[] command = x.Split('-');
                command = command[1].Split('(');
                if (command.Length == 1)
                {
                    return;
                }
                switch (command[0])
                {
                    case "start":
                        {
                            command = command[1].Split(')');
                            if (command[0] == "")
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("Server started at {0} on port {1}", GetLocalIP(), 13000);
                                Console.ForegroundColor = cc;
                                Start(IPAddress.Parse(GetLocalIP()));
                            }
                            else
                            {
                                try
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Server started at {0} on port {1}", GetLocalIP(), int.Parse(command[0]));
                                    Console.ForegroundColor = cc;
                                    Start(IPAddress.Parse(GetLocalIP()), int.Parse(command[0]));
                                }
                                catch
                                {
                                    return;
                                }
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
        }
        static void Start(IPAddress localAddr, int port = 13000)
        {
            TcpListener server = null;
            try
            {
                // Set the TcpListener
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    Client client = server.AcceptTcpClient() as Client;
                    new Thread(HandleClient).Start(client);
                    clients.Add(client);
                    Console.WriteLine("Connected to {0}", client.Client.AddressFamily);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }
            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        private static void HandleClient(object aclient)
        {
            Client client = aclient as Client;
            // Buffer for reading data
            byte[] bytes = new byte[256];
            string data;
            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();

            int i;
            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = Encoding.ASCII.GetString(bytes, 0, i);
                if (data.First() == '~')
                {
                    //the client sent the name
                    client.Name = data.Split('~')[1];
                }
                else if (data.First() == '-')
                {
                    //the client sent a message
                    data = data.Split('-')[1];
                    DetailedMessage dm = data;
                    SendToAll(dm);
                    Console.WriteLine(dm.ToString());
                }
            }

            // Shutdown and end connection
            client.Close();
        }

        static string GetMyPublicIP()
        {
            string ip = new WebClient().DownloadString("http://icanhazip.com").Replace("\n", "");
            return ip;
        }

        public static string GetLocalIP()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static string GetFriendName(string name, string[] clients)
        {
            for (int i = 0; i < clients.Length; i++)
            {
                if (clients[i] != name)
                {
                    return clients[i];
                }
            }
            return "";
        }
    }
}
