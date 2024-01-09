using VisiWin.ApplicationFramework;
using VisiWin.Alarm;
using System.Linq;
using VisiWin.UserManagement;
using System.ComponentModel;
using System.Windows;
using HMI.Views.MessageBoxRegion;
using System.Data;
using HMI.Module;
using System;

namespace HMI
{

    [ExportView("HeaderView")]
    public partial class HeaderView : VisiWin.Controls.View
    {
        ICurrentAlarms2 CurrentAlarmList;
        IAlarmItem CurrentAlarm;

        public HeaderView()
        {
            this.InitializeComponent();
            
            CurrentAlarmList = ApplicationService.GetService<IAlarmService>().GetCurrentAlarms2();

            CurrentAlarmList.ChangeAlarm +=  SetAlarmLineData;
            CurrentAlarmList.NewAlarm += SetAlarmLineData;
            CurrentAlarmList.ClearAlarm += SetAlarmLineData;

            SetAlarmLineData(null,null);


            BackgroundWorker PLC_Connection_Status = new BackgroundWorker();
            PLC_Connection_Status.DoWork += W5_DoWork;
            PLC_Connection_Status.RunWorkerAsync();



        }

        private void W5_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                this.Dispatcher.Invoke(() => {
                    if (xxx.IsQualityGood)
                    {
                        if (aaa.Visibility!= System.Windows.Visibility.Hidden)
                        aaa.Visibility = System.Windows.Visibility.Hidden;
                    }
                    else
                    {
                        if (aaa.Visibility != System.Windows.Visibility.Visible)
                            aaa.Visibility = System.Windows.Visibility.Visible;
                    }
                });
                System.Threading.Thread.Sleep(500);
            }
        }

        private void AlarmLabel_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            IUserManagementService userService = ApplicationService.GetService<IUserManagementService>();
            if (userService.CurrentUser != null && userService.CurrentUser.RightNames.Contains("Diagnose"))
            {
                CurrentAlarm.Acknowledge();
            }
            
        }

        void SetAlarmLineData(object sender, AlarmEventArgs e)
        {
            IAlarmItem[] TT = CurrentAlarmList.Alarms.Where(x => x.Group.Name == "Alarm" && x.AlarmState == AlarmState.Active).ToArray();
            CurrentAlarm = (TT.Length > 0) ? TT[0] : null;

            if (CurrentAlarm != null)
            {
                AlarmLabel.Visibility = System.Windows.Visibility.Visible;
                AlarmText.Content = CurrentAlarm.ActivationTime.ToString() + "  -  " + CurrentAlarm.Text;                
            }
            else
            {
                AlarmLabel.Visibility = System.Windows.Visibility.Hidden;
            }
        }

    }
}
