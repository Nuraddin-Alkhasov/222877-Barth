using HMI.Module;
using HMI.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.UserManagement;

namespace HMI.Diagnose
{
	/// <summary>
	/// Interaction logic for View1.xaml
	/// </summary>
	[ExportView("Diagnose_Alarms")]
	public partial class Diagnose_Alarms : VisiWin.Controls.View
	{

        private readonly Stopwatch _doubleTapStopwatch = new Stopwatch();
        private Point _lastTapLocation;
        private bool run = false;
        public Diagnose_Alarms()
		{
			this.InitializeComponent();
		}

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //alarmList.AcknowledgeAllAlarms();
        }
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;

        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;

        }


        private bool IsDoubleTap(TouchEventArgs e)
        {
            Point currentTapPosition = e.GetTouchPoint(this).Position;
            bool tapsAreCloseInDistance = currentTapPosition.X - _lastTapLocation.X < 40;
            _lastTapLocation = currentTapPosition;

            TimeSpan elapsed = _doubleTapStopwatch.Elapsed;
            _doubleTapStopwatch.Restart();
            bool tapsAreCloseInTime = (elapsed != TimeSpan.Zero && elapsed < TimeSpan.FromSeconds(0.2));

            return tapsAreCloseInDistance && tapsAreCloseInTime;
        }

        private void OnPreviewTouchDown(object sender, TouchEventArgs e)
        {
            if (IsDoubleTap(e))
            {
                IUserManagementService userService = ApplicationService.GetService<IUserManagementService>();
                if (userService.CurrentUser != null && userService.CurrentUser.RightNames.Contains("Diagnose"))
                {
                    Task taskA = new Task(() => 
                    {
                        ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Störung Quittieren", true);
                        Task.Delay(250);
                        ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Störung Quittieren", false);
                    });
                    taskA.Start();
                }
            }
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IUserManagementService userService = ApplicationService.GetService<IUserManagementService>();
            if (userService.CurrentUser!=null && userService.CurrentUser.RightNames.Contains("Diagnose"))
            {
                Task taskA = new Task(() =>
                {
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Störung Quittieren", true);
                    Task.Delay(250);
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Störung Quittieren", false);
                });
                taskA.Start();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.WindowStyle = ProcessWindowStyle.Hidden;
            proc.FileName = "cmd";
            proc.Arguments = "/C shutdown -f -r -t 0";
            Process.Start(proc);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            IRegionService iRS = ApplicationService.GetService<IRegionService>();
            if (iRS.GetCurrentViewName("TouchpadRegion") != "WaitScreen")
            {
               new DoBackup();
               //  var b = (new localDBAdapter("UPDATE Config SET Value = '" + GetDataTimeNowToFormat() + "' WHERE Variable = 'TimeBackuped' ;").DB_Input());
            }
        }
        private string GetDataTimeNowToFormat()
        {
            return DateTime.Now.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToLongTimeString();
        }
    }
}