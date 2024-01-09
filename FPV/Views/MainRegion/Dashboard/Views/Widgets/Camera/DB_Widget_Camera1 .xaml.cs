using Dashboard;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardWidgetBar.xaml
    /// </summary>
    [ExportDashboardWidget("DB_Widget_Camera1", "Dashboard.Text24", "@Dashboard.Text25", 1, 2)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DB_Widget_Camera1 : View
    {

        Baumer Cam1 = null;
        bool start;
        BackgroundWorker BW_MT = null;

        private readonly Stopwatch _doubleTapStopwatch = new Stopwatch();
        private Point _lastTapLocation;
        private Point _lastTapLocation2;


        public DB_Widget_Camera1()
        {
            InitializeComponent();
        }

        private void BW_MainThread(object sender, DoWorkEventArgs e)
        {
            Cam1 = new Baumer("Kamera 1");
            if (Cam1.isStarted)
            {
                Task obTask = null;
                obTask = new Task(Do);
                obTask.Start();
                while (start)
                {
                    if (obTask.IsCompleted)
                    {
                        obTask = new Task(Do);
                        obTask.Start();
                    }
                    Thread.Sleep(100);
                }
                while (!obTask.IsCompleted)
                {
                    Thread.Sleep(100);
                }
                if (Cam1 != null)
                {
                    Cam1.Dispose();
                    Cam1 = null;
                }

            }
        }

        private void Do()
        {
            if (Cam1 != null)
            {
                byte[] pic = Cam1.GetPicture();
                if (pic != null)
                {
                    Application.Current.Dispatcher.InvokeAsync((Action)delegate
                    {
                        BitmapSource bitmapSource = BitmapSource.Create(2048, 1536, 300, 300, PixelFormats.Indexed8, BitmapPalettes.Gray256, pic, 2048);
                        IMG.Image = bitmapSource;
                    });
                }
            }
        }

        private void IMG_Loaded(object sender, RoutedEventArgs e)
        {
            Task obTask = Task.Run(() =>
            {

                if (!start)
                {
                    start = true;
                    BW_MT = new BackgroundWorker();
                    BW_MT.DoWork += BW_MainThread;
                    BW_MT.RunWorkerAsync();
                }
            });
        }

        private void IMG_Unloaded(object sender, RoutedEventArgs e)
        {
            Task obTask = Task.Run(() =>
            {
                if (start) start = false;
            });

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!start)
            {
                start = true;
                BW_MT = new BackgroundWorker();
                BW_MT.DoWork += BW_MainThread;
                BW_MT.RunWorkerAsync();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (start) start = false;
        }

        private void IMG_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            if (IsDoubleTap(e))
            {
                _lastTapLocation2 = e.GetTouchPoint(this).Position;
                if (IMG.Width != 2048)
                {
                    IMG.Height = 1536;
                    IMG.Width = 2048;
                }
                else
                {
                    IMG.Height = 425; 
                    IMG.Width = 565; 
                }
            }
        }

        private bool IsDoubleTap(TouchEventArgs e)
        {
            Point currentTapPosition = e.GetTouchPoint(this).Position;
            bool tapsAreCloseInDistance = currentTapPosition.X - _lastTapLocation.X < 50;
            _lastTapLocation = currentTapPosition;

            TimeSpan elapsed = _doubleTapStopwatch.Elapsed;
            _doubleTapStopwatch.Restart();
            bool tapsAreCloseInTime = (elapsed != TimeSpan.Zero && elapsed < TimeSpan.FromSeconds(0.4));

            return tapsAreCloseInDistance && tapsAreCloseInTime;
        }

        private void IMG_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IMG.Width == 2048)
            {
                scroll.ScrollToHorizontalOffset((((_lastTapLocation2.X) * 2048 / 565)) * 1483 / 2048);
                scroll.ScrollToVerticalOffset((((_lastTapLocation2.Y - 35) * 1536 / 425)) * 1111 / 1536);
            }
            else
            {
                scroll.ScrollToHorizontalOffset(0);
                scroll.ScrollToVerticalOffset(0);
            }
        }
    }
}


