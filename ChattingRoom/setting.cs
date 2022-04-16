using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChattingRoom
{
    internal class setting
    {
        // server settings ////
        public static string serverIp = "192.168.0.132";
        public static int port = 8080;
        public static string Cuser = "";
        public static bool newConnect = false;
        public static List<string> Cusers = new List<string>();
        public static string Cip = "";

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
