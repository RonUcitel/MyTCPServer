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
        static bool running = false;
        const int k_port = 13000;
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
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Name != dm.Name)
                {
                    try
                    {
                        clients[i].tcpClient.GetStream().Write(dm, 0, dm.Length + 1);
                    }
                    catch
                    {
                        string name = clients[i].Name;
                        clients.Remove(clients[i]);
                        i--;
                    }

                }
            }
            //foreach (Client item in clients)
            //{
            //    if (item.Name != dm.Name)
            //    {
            //        try
            //        {
            //            item.tcpClient.GetStream().Write(dm, 0, dm.Length + 1);
            //        }
            //        catch
            //        {
            //            string name = item.Name;
            //            clients.Remove(item);
            //            SendToAll(new DetailedMessage("Server", name + " had exit the chatroom"));
            //        }

            //    }
            //}
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
                            if (!running)
                            {
                                command = command[1].Split(')');
                                if (command[0] == "")
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine("Server started at {0} on port {1}", GetLocalIP(), k_port);
                                    Console.ForegroundColor = cc;
                                    new Thread(Start).Start(new object[] { IPAddress.Parse(GetLocalIP()), k_port });
                                }
                                else
                                {
                                    try
                                    {
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine("Server started at {0} on port {1}", GetLocalIP(), int.Parse(command[0]));
                                        Console.ForegroundColor = cc;
                                        new Thread(Start).Start(new object[] { IPAddress.Parse(GetLocalIP()), int.Parse(command[0]) });
                                    }
                                    catch
                                    {
                                        return;
                                    }
                                }
                            }
                            break;
                        }
                    case "send":
                        {
                            command = command[1].Split(')');
                            if (command[0] == "")
                            {

                            }
                            else
                            {
                                SendToAll(new DetailedMessage("Server", command[0]));
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
        }
        static void Start(object a)//IPAddress localAddr, int port = 13000)
        {
            object[] par = (object[])a;
            IPAddress localAddr = (IPAddress)par[0];
            int port = (int)par[1];
            TcpListener server = null;
            try
            {
                // Set the TcpListener
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();
                running = true;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    Client client = new Client("", server.AcceptTcpClient());
                    new Thread(HandleClient).Start(client);
                    clients.Add(client);
                    Console.WriteLine("Connected to {0}", client.tcpClient.Client.AddressFamily);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                running = false;
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
                running = false;
            }
            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        private static void HandleClient(object oclient)
        {
            Client client = (Client)oclient;
            // Buffer for reading data
            byte[] bytes = new byte[256];
            string data;
            // Get a stream object for reading and writing
            NetworkStream stream = client.tcpClient.GetStream();

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
            client.tcpClient.Close();
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
    }
}
