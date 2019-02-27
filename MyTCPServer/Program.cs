using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace MyTCPServer
{
    class Program
    {
        static string name = "alon", clientName = "";
        static ConsoleColor cc;
        static void Main(string[] args)
        {
            cc = Console.ForegroundColor;
            while (true)
            {
            START:
                Console.Write("~ ");
                string x = Console.ReadLine();
                if (x.First() == '-')
                {
                    string[] command = x.Split('-');
                    command = command[1].Split('(');
                    if (command.Length == 1)
                    {
                        goto START;
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
                                        goto START;
                                    }
                                }
                                break;
                            }
                        default:
                            break;
                    }
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


                // Buffer for reading data
                byte[] bytes = new byte[256];
                string data;

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;
                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        if (data.First() == '?')
                        {
                            byte[] returnName = Encoding.ASCII.GetBytes("~" + name);

                            // Send back the name.
                            stream.Write(returnName, 0, returnName.Length);
                            Console.WriteLine("Sent: {0}", "~" + name);
                        }
                        else if (data.First() == '~')
                        {
                            clientName = data.Split('~')[1];
                        }
                        else if (data.First() == '-')
                        {
                            data = data.Split('-')[1];
                            Console.WriteLine("{}: {0}", data);

                            byte[] msg = Encoding.ASCII.GetBytes("-" + "trying");

                            // Send back the name.
                            stream.Write(msg, 0, msg.Length);
                            Console.WriteLine("Sent: {0}", msg);
                        }
                    }

                    // Shutdown and end connection
                    client.Close();
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
