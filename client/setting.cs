using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChattingRoom
{
    internal class setting
    {
        public static string serverIp = "";
        public static int port = 8080;
        public static string Cuser = "";
        public static bool newConnect = false;
        public static string pic_source = "";
        public static string imgFile = "";

        public delegate string StrHandler(string str);

        public class ChatSetting
        {
            public Socket socket;
            public NetworkStream stream;
            public StreamReader reader;
            public StreamWriter writer;
            public StrHandler inHandler;
            public EndPoint remoteEndPoint;
            public bool isDead = false;

            public ChatSetting(Socket s)
            {
                socket = s;
                stream = new NetworkStream(s);
                reader = new StreamReader(stream);
                writer = new StreamWriter(stream);
                remoteEndPoint = socket.RemoteEndPoint;
            }

            public string Get_IP()
            {
                string domain = Dns.GetHostName();
                IPAddress[] ipadrlist = Dns.GetHostAddresses(domain);
                foreach (IPAddress ipv4 in ipadrlist)
                {
                    if (ipv4.AddressFamily == AddressFamily.InterNetwork) return ipv4.ToString();
                }
                return null;
            }
            public string receive()
            {
                return reader.ReadLine();
            }

            public ChatSetting send(string line)
            {
                writer.WriteLine(line);
                writer.Flush();
                return this;
            }

            public static ChatSetting connect(string ip)
            {
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), port);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipep);
                return new ChatSetting(socket);
            }

            public Thread newListener(StrHandler pHandler)
            {
                inHandler = pHandler;

                Thread listenThread = new Thread(new ThreadStart(listen));
                listenThread.Start();
                return listenThread;
            }

            public void listen()
            {
                try
                {
                    while (true)
                    {
                        string line = receive();
                        inHandler(line);
                    }
                }
                catch (Exception ex)
                {
                    isDead = true;
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
