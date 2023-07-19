using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ADB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region data
        Bitmap TOP_UP_BMP;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        void LoadData()
        {
            TOP_UP_BMP = (Bitmap)Bitmap.FromFile("Data//congcu.png");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task t = new Task(() =>
            {
                isStop = false;
                Auto();
            });
            t.Start();
        }

        bool isStop = false;
        void Auto()
        {
            isStop = false;
            List<string> devices = new List<string>();
            devices = KAutoHelper.ADBHelper.GetDevices();
            MessageBox.Show("Tìm thấy " + devices.Count + " thiết bị", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            // chạy từng device để điều khiển theo kịch bản bên trong
            foreach (var deviceID in devices)
            {
                // tạo ra một luồng xử lý riêng biệt để xử lý cho device này
                Task t = new Task(() =>
                {
                    // lặp kịch bản quài quài
                    while (true)
                    {
                        // nếu có lệnh stop thì dừng toàn bộ luồng chạy
                        if (isStop)
                            return;
                        var screen = KAutoHelper.ADBHelper.ScreenShoot(deviceID);
                        var pos = KAutoHelper.ImageScanOpenCV.FindOutPoint(screen, TOP_UP_BMP);
                        if (pos != null)
                        {
                            MessageBox.Show("Tìm thấy");
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy");
                        }
                    }
                });
                t.Start();
            }
        }

        void Delay(int delay)
        {
            while (delay > 0)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                delay--;
                if (isStop)
                    break;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            isStop = true;
        }
    }
}
