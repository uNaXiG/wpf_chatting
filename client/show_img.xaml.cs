using System;
using System.Windows;
using System.Windows.Media.Imaging;
using static ChattingRoom.setting;

namespace client
{
    /// <summary>
    /// show_img.xaml 的互動邏輯
    /// </summary>
    public partial class show_img : Window
    {
        public show_img()
        {
            InitializeComponent();

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(pic_source, UriKind.RelativeOrAbsolute);
            bi.EndInit();

            show.Source = bi;
        }
    }
}
