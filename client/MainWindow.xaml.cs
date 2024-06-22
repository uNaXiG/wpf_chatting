using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using static ChattingRoom.setting;
using System.Drawing;
using ChattingRoom;

namespace client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    ///

    // 禁止客戶端多開 //
    
    public partial class App : Application
    {
        private static System.Threading.Mutex mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            mutex = new System.Threading.Mutex(true, "OnlyRun_CRNS");
            if(mutex.WaitOne(0, false)) base.OnStartup(e);
            else Current.Shutdown();
        }
    }

    public partial class MainWindow : Window
    {
        ChatSetting client_;    // 建立一個用戶端類別        
        StrHandler msgHandler;  // 用來捕捉訊息事件
        string user_name = "";  // 定義用戶名稱  
        string disConnect_user = "";    // 設置哪位用戶關閉客戶端的名稱
        string new_msg_sound = "\\NewMessageSound.wav";     // 新訊息音效
        string new_connect_sound = "\\NewConnectSound.wav";      // 新用戶音效
        bool first_connect = true;  // 決定該用戶是否為第一次連線
        bool is_send_img = false;
        int user_count = 0;     // 統計用戶數量

        public MainWindow()
        {
            InitializeComponent();

            msg_contant.Focus();    // 將焦點置於訊息框上
            bar.PreviewMouseDown += (s, e) => DragMove();   // window title bar
            // 開啟一個輸入用戶名稱的視窗 //
            set_name sn = new set_name();
            sn.ShowDialog();

            // 預設一個音效參數 //
            sound.LoadedBehavior = MediaState.Manual;
            

            // 當設置名稱的頁面離開(沒輸入名稱)時，直接關閉客戶端 //
            if (Cuser == "") Environment.Exit(0);
            else
            {
                // 將用戶名稱設定成輸入的名稱 //
                user_name = Cuser;  // setting中的名稱字串
                bar.Content = "NaXiG Chatting : " + Cuser;    //

                // 將用戶端進行與服務器的連接之相關設定 //
                try
                {
                    client_ = ChatSetting.connect(serverIp);
                    client_.newListener(processMsgComeIn);
                    msgHandler = show_msg;  // 委派顯示訊息框函數
                    send_msg();  // 傳送訊息函數
                }
                catch (Exception ex) 
                { 
                    MessageBox.Show("IP 輸入錯誤，或是該聊天伺服器尚未開啟。");
                    Environment.Exit(0);
                }
                
            }
        }

        /// <summary>
        /// 委派訊息的處理事件
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string processMsgComeIn(string msg)
        {
            Dispatcher.Invoke(msgHandler, new object[] { msg });
            return "OK";
        }

        /// <summary>
        /// 取得訊息傳送的時間
        /// </summary>
        /// <returns></returns>
        string msg_time()
        {
            int h =Convert.ToInt32(DateTime.Now.Hour.ToString());
            string hour = "";
            if (h >= 12) hour = "下午 " + DateTime.Now.Hour.ToString().PadLeft(2, '0');
            else hour = "上午 " + DateTime.Now.Hour.ToString().PadLeft(2, '0');

            return "【" + hour + "：" + 
                DateTime.Now.Minute.ToString().PadLeft(2,'0') + "】";
        }

        /// <summary>
        /// 顯示訊息在對話框上
        /// </summary>
        /// <param name="msg">服務器傳遞的訊息</param>
        /// <returns></returns>
        public string show_msg(string msg)
        {
            // 判定該用戶為第一次連線 //
            if (msg.Contains("./fullConnect")){     // 若收到服務器回傳滿載的關鍵字
                MessageBox.Show("聊天室已滿人，請稍後再試。");
                client_.send("./fullConnect");      // 朝服務器發送滿載的關鍵字
                Environment.Exit(0);
            }
            else if (msg.Contains("./nameFail"))    // 若服務器回傳重名的關鍵字
            {
                MessageBox.Show("名稱已重複，請重新輸入。");
                client_.send("./nameFail");         // 朝服務器發送重名的關鍵字
                Environment.Exit(0);
            }
            else if (msg.Contains("./canConnect"))  // 若服務器回傳可以連線的關鍵字
            {
                client_.send("./newConnect" + user_name);       // 朝服務器發送新連線的關鍵字
            }
            else if (msg.Contains("./newConnect"))  // 若收到服務器回傳新連線的關鍵字
            {
                reflash_users(msg, "./newConnect"); // 更新線上成員
                string[] s = msg.Replace("./newConnect", "").Split(',');
                // 對每位用戶提示有新用戶連線 //
                sound_play(new_connect_sound);
                AppendColoredText(msg_time() + "歡迎新朋友 【" + s[s.Length - 2] + "】 的加入！", Colors.Black);

            }
            // 判定用戶關閉用戶端 //
            else if (msg.Contains("./disConnect"))
            {
                reflash_users(msg, "./disConnect"); // 更新線上成員
                // 對每位用戶提示有用戶離線 //
                AppendColoredText(msg_time() + "成員 【" + disConnect_user + "】 離開了！", Colors.Red);
                sound_play(new_connect_sound);
                disConnect_user = "";
            }
            // 判定用戶傳送圖片 //
            else if (msg.Contains("./getImage"))
            {
                string sendBy = msg.Split(":")[1];
                setting.imgFile = msg.Split(":")[2];
                string base64String = msg.Split(":")[3];

                System.Drawing.Image img = System.Drawing.Image.FromStream(new MemoryStream(Convert.FromBase64String(base64String)));
                img.Save(AppDomain.CurrentDomain.BaseDirectory + "\\" + $"{setting.imgFile}.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);

                images.Items.Add(setting.imgFile);

                if (sendBy == user_name)
                {
                    AppendColoredText(msg_time() + "你 傳送了圖片！", Colors.Green);
                }
                else
                {
                    sound_play(new_msg_sound);
                    AppendColoredText(msg_time() + sendBy + " 傳送了圖片！", Colors.Black);
                    MessageBoxResult re = MessageBox.Show(sendBy + " 傳送了圖片，請點擊查看。", "Chatting Room : " + user_name, MessageBoxButton.YesNo, MessageBoxImage.None);
                    
                    if(re == MessageBoxResult.Yes)
                    {
                        show_img show_Img = new show_img();
                        show_Img.ShowDialog();
                    }
                    
                } 
            }
            // 非以上情況(用戶之間正常的訊息聊天) //
            else
            {
                // 判定訊息是否為自己所傳送，是自己傳送不撥放音效，且將傳送者名稱改為 "你" //
                if (msg.Split(':')[0].Replace(" ", "") == user_name)
                {
                    // 只取代第一次遇到自己名稱的方法 //
                    var regex = new Regex(Regex.Escape(user_name));
                    msg = regex.Replace(msg, "你", 1);
                    AppendColoredText(msg_time() + msg, Colors.Green);
                }
                else
                {
                    sound_play(new_msg_sound);    // 並非自己所傳送，其他用戶撥放音效  
                    AppendColoredText(msg_time() + msg, Colors.Black);
                }
            }
            msg_box.ScrollToEnd();  // 將對話框捲動至最底部            
            return "OK";
        }

        void AppendColoredText(string text, System.Windows.Media.Color color)
        {
            var run = new Run(text)
            {
                Foreground = new SolidColorBrush(color)
            };
            var paragraph = new Paragraph(run)
            {
                Margin = new Thickness(0) // 设置Margin为0以减少行距
            };
            msg_box.Document.Blocks.Add(paragraph);
        }

        /// <summary>
        /// 更新線上成員
        /// </summary>
        /// <param name="msg">服務器傳來的訊息，當中包含哪位用戶所傳送</param>
        /// <param name="command">相關服務器下達的命令</param>
        void reflash_users(string msg, string command)
        {
            string[] user = msg.Replace(command, "").Split(',');    // 取得最新用戶串列中各別的名稱            

            grid.Children.RemoveRange(0, grid.Children.Count);  // 先將目前所有用戶移除            
            user_count = 0;
            // 創建用戶Label //
            for (int i = 0; i < user.Length - 1; i++)
            {
                Label user_lb = new Label();
                user_lb.Name = "user_" + i;
                user_lb.Content = user[i];
                user_lb.Margin = new Thickness(10, 10 + i * 20, 0, 0);
                user_lb.FontWeight = FontWeights.Bold;
                if (user_name == user[i]) user_lb.Foreground = System.Windows.Media.Brushes.Green;
                else user_lb.Foreground = System.Windows.Media.Brushes.Black;
                grid.Children.Add(user_lb);
                user_count++;   // 用戶數 + 1
            }
            user_lb.Content = "線上成員共 " + user_count.ToString() + " / 10 人";
            disConnect_user = user[user.Length - 1];    // 取得關閉程式的用戶名稱

        }

        /// <summary>
        /// 按下送出按鈕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void send_btn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            send_msg();
        }

        /// <summary>
        /// 向服務器發送主要訊息
        /// </summary>
        public void send_msg()
        {
            string ip = "";
            string domain = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(domain);

            foreach (IPAddress ipv4 in ipadrlist) if (ipv4.AddressFamily == AddressFamily.InterNetwork) ip = ipv4.ToString();

            if (first_connect)
            {
                // 首次連線 //
                client_.send("./testConnect" + user_name);   // 向服務器發送測試連線的關鍵字
                first_connect = false; // 之後為非首次連線 //
            }   
            else if (is_send_img)
            {
                // 傳送圖片 //
                // 將圖片讀取並轉Base64
                byte[] imageArray = File.ReadAllBytes(pic_source);
                string base64Image = Convert.ToBase64String(imageArray); 
                client_.send("./sendImage:" + ip + ":" + user_name + ":" + base64Image);      // 項服務器發送傳送圖片的關鍵字
                is_send_img = false;
                //send_img();
            }
            else if (user_name != "" && msg_contant.Text != "")
            {
                // 其他正常發送訊息的情況 //
                client_.send("./newMessage" + user_name + " : " + msg_contant.Text);
                msg_contant.Text = "";  // 清空訊息輸入欄
            }
        }

        /// <summary>
        /// 關閉聊天室按鈕按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void close_Click(object sender, RoutedEventArgs e)
        {
            exit();
        }

        /// <summary>
        /// 離開聊天室的確認函數
        /// </summary>
        void exit()
        {
            // 確定離開聊天室，向服務器發送離開的關鍵字 //
            MessageBoxResult re = MessageBox.Show("確定要離開？", user_name, MessageBoxButton.OKCancel, MessageBoxImage.None);
            if (re == MessageBoxResult.OK)
            {
                client_.send("./disConnect" + user_name + ":" + client_.Get_IP());
                Environment.Exit(0);
            }
            else return;            
        }

        /// <summary>
        /// 捕捉用戶按下的鍵盤指令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter) send_msg();   // 發送訊息快捷鍵
            else if(e.Key == Key.Escape) exit(); // 離開聊天室快捷鍵
        }

        void sound_play(string sound_name)
        {
            sound.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + sound_name);
            sound.Play();
        }

        /// <summary>
        /// 設置音效可重複使用的函數
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sound_MediaEnded(object sender, RoutedEventArgs e)
        {
            sound.Position = TimeSpan.Zero;
            sound.Pause();
        }
        
        /// <summary>
        /// 輸入框於每次輸入時自動捲至底部輸入文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void msg_contant_KeyDown(object sender, KeyEventArgs e)
        {
            msg_contant.ScrollToEnd();
        }

        private void min_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 選擇圖片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_btn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".jpg";
            dialog.Filter = "Picture File|*.jpg;*.png;*.bmp";
            bool? result = dialog.ShowDialog();
            if(result == true)
            {
                is_send_img = true;
                pic_source = dialog.FileName;
                MessageBoxResult re = MessageBox.Show("確定要傳送圖片嗎？", "傳送圖片", MessageBoxButton.YesNo, MessageBoxImage.None);
                if (re == MessageBoxResult.Yes) send_msg();
                else return;
            }
        }

        private void images_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setting.imgFile = images.SelectedValue.ToString();
            show_img show_Img = new show_img();
            show_Img.ShowDialog();
        }
    }
}
