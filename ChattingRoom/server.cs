using System.Net;
using System.Net.Sockets;
using System.Drawing;
using static ChattingRoom.setting;
using System.Security.Cryptography.X509Certificates;

namespace server
{
    class server
    {
        List<ChatSetting> clientList = new List<ChatSetting>();     // 存放連線者
        List<string> users = new List<string>();        // 存放連線進來的用戶所使用的名稱
        List<string> ips = new List<string>();          // 存放連線者的IP位址

        IPEndPoint ip = new IPEndPoint(IPAddress.Any, 8080);    // 建立服務器端點於 8080 port

        // 創建一個TCP socket 主機 //
        Socket nsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            // 啟動服務器 //
            server newserver = new server();
            Console.WriteLine("Startup...");
            Console.WriteLine("Server is running...");
            string domain = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(domain);
            foreach (IPAddress ipv4 in ipadrlist)
            {
                if (ipv4.AddressFamily == AddressFamily.InterNetwork)
                    Console.WriteLine("您的伺服器連線號碼是： " + ipv4.ToString() + " 請分享給您的朋友以加入聊天。");
            }
            Console.WriteLine("==========Chatting Room Server==========\nWaitting user...\n");
            newserver.run();
        }

        public void run()
        {
            nsocket.Bind(ip);
            nsocket.Listen(10);

            while (true)
            {
                // 接受用戶的連線 //
                Socket socket = nsocket.Accept();
                Console.WriteLine("New Connect......");

                ChatSetting client = new ChatSetting(socket);

                try
                {
                    clientList.Add(client);     // 將用戶加入到服務器底下
                    client.newListener(processMsgComeIn);       // 監聽此用戶
                }
                catch { }
            }
        }

        public string processMsgComeIn(string msg)
        {
            // 判斷用戶朝服務器發送何種指令 //
            if (msg.Contains("./testConnect"))      // 若用戶朝服務器發送 測試連線 的請求
            {
                Console.WriteLine("=> " + msg + "  at " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                bool name_ok = true;    // 檢測名稱是否通過之變數
                // 判斷當前連線用戶是否滿載，若滿載朝該用戶發送 連線失敗 的關鍵字 //
                if (users.Count >= 10) clientList[clientList.Count - 1].send("./fullConnect");
                else
                {
                    // 若尚未滿載 //
                    string this_user = msg.Replace("./testConnect", "");    // 取得連線新用戶的名稱
                    foreach (var user in users)
                    {
                        if (this_user == user)      // 若該新用戶名稱與服務器底下保存的名稱之一個相同
                        {
                            clientList[clientList.Count - 1].send("./nameFail");    // 朝該用戶發送 重名 的關鍵字
                            name_ok = false;    // 設定名稱不通過
                            break;  // 不需再做其他驗證
                        }
                    }
                }
                // 判斷名稱驗證通過，朝用戶發送 可以連線 的關鍵字 //
                if (name_ok) clientList[clientList.Count - 1].send("./canConnect");
            }
            // 若用戶發送 滿載 或是 重名 的請求 //
            else if (msg.Contains("./fullConnect") || msg.Contains("./nameFail"))
            {
                clientList.Remove(clientList[clientList.Count - 1]);    // 則將此用戶自服務器底下移除監聽
            }
            // 若通過所有驗證 //
            else if (msg.Contains("./newConnect"))      // 若用戶發送 新連線 的請求
            {
                Console.WriteLine("=> " + msg + "  at " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                users.Add(msg.Replace("./newConnect", ""));     // 將該用戶名稱加入到名稱串列保存
                string client_ip = clientList[clientList.Count - 1].remoteEndPoint.ToString().Split(':')[0];
                ips.Add(client_ip);
                Console.Write("ip address : " + client_ip + "\n All IPs : ");
                foreach(var ip in ips) Console.Write("[" + ip + "] ");
                Console.WriteLine();
                Console.WriteLine("Connect user:" + users.Count);
                // 朝每一位用戶發送有新連線的廣播 //
                string u = "./newConnect";
                foreach (var user in users) u += user + ",";
                broadCast(u, "");
            }
            // 若有任意用戶終止連線 //
            else if (msg.Contains("./disConnect"))
            {
                Console.WriteLine("=> " + msg + "  at " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                users.Remove(msg.Replace("./disConnect", "").Split(':')[0]);      // 將該用戶名稱自串列中移除
                ips.Remove(msg.Replace("./disConnect", "").Split(':')[1]);          // 將該用戶IP自串列中移除
                // 朝每一位用戶發送有用戶離開的廣播 //
                string u = "./disConnect";
                foreach (var user in users) u += user + ",";
                u += msg.Replace("./disConnect", "");
                broadCast(u, "");
            }
            // 傳送圖片 //
            else if (msg.Contains("./sendImage"))
            {
                string base64String = msg.Split(':')[3];
                Console.WriteLine("Solve Image File from " + msg.Split(":")[2]);
                Image img = Image.FromStream(new MemoryStream(Convert.FromBase64String(base64String)));
                string imgFile = DateTime.Now.ToString("yyyyMMddHHmmss");
                img.Save(AppDomain.CurrentDomain.BaseDirectory + "\\" + $"{imgFile}.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                broadCast("./sendImage:" + msg.Split(":")[2] + ":" + imgFile, base64String);
            }
            // 其餘用戶之間正常傳送訊息聊天的情況 //
            else if (msg.Contains("./newMessage"))
            {
                Console.WriteLine("=> " + msg + "  at " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                broadCast(msg.Replace("./newMessage", ""), "");
            }
            return "OK";
        }

        /// <summary>
        /// 朝每位用戶廣播收到的訊息
        /// </summary>
        /// <param name="msg"></param>
        public void broadCast(string msg, string base64String)
        {
            foreach (ChatSetting client in clientList)
            {
                if (!client.isDead)
                {
                    if(msg.Contains("./sendImage")) client.send("./getImage:" + msg.Split(":")[1] + ":" + msg.Split(":")[2] + ":" + base64String);
                    else client.send(msg);
                }
            }
                
        }
    }
}
