using ChattingRoom;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

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

            string imagePath = AppDomain.CurrentDomain.BaseDirectory + "\\" + $"{setting.imgFile}.jpg";
            if (System.IO.File.Exists(imagePath))
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                    bitmap.EndInit();

                    show.Source = bitmap;
                }
                catch { }
            }

        }

    }
}
