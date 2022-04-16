using System;
using System.Windows;
using System.Windows.Input;
using static ChattingRoom.setting;

namespace client
{
    /// <summary>
    /// set_name.xaml 的互動邏輯
    /// </summary>
    public partial class set_name : Window
    {
        public set_name()
        {
            InitializeComponent();
            name_box.Focus();
            bar.PreviewMouseDown += (s, e) => DragMove();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            enter_room();
        }
        void enter_room()
        {
            if (name_box.Text != "")
            {
                Cuser = name_box.Text;
                serverIp = ip_box.Text;
                Close();
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Cuser = "";
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                enter_room();
            }
        }
    }
}
