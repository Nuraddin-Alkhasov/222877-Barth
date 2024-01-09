using HMI.Views.MessageBoxRegion;
using System;
using System.Windows;
using System.Windows.Threading;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion
{

    [ExportView("WaitScreen")]
    public partial class WaitScreen : VisiWin.Controls.View
    {

        DispatcherTimer _timer_Backup;
        DispatcherTimer _timer_Dataload;

        public WaitScreen()
        {
            this.InitializeComponent(); 
        }

        private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            int param = Convert.ToInt16(ApplicationService.ObjectStore.GetValue("WaitScreen_KEY"));
            if (this.IsVisible)
            {
                switch (param)
                {
                    case 0:
                        TextBlockText.LocalizableText = "@WaitScreen.Text1";
                        Backup_GOGO();
                        break;
                    case 1:
                        TextBlockText.LocalizableText = "@WaitScreen.Text2";
                        LoadData_GOGO();
                        break;
                }
            }
            else
            {
                switch (param)
                {
                    case 0:
                        _timer_Backup.Stop();
                        break;
                    case 1:
                        _timer_Dataload.Stop();
                        break;
                }
            }
        }

        private void Backup_GOGO()
        {
            TimeSpan _time = TimeSpan.FromSeconds(120);

            _timer_Backup = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                {
                    if (this.IsVisible)
                    {
                        if (_time == TimeSpan.Zero)
                        {
                            ApplicationService.SetView("TouchpadRegion", "EmptyView");
                            new MessageBoxTask("@Backup.Text2", "@Backup.Text1", MessageBoxIcon.Exclamation);
                        }
                        _time = _time.Add(TimeSpan.FromSeconds(-1));
                    }
                    else
                    {
                        return;
                    }
                    
                }, Application.Current.Dispatcher);
            _timer_Backup.Start();
        }

        private void LoadData_GOGO()
        {
            TimeSpan _time = TimeSpan.FromSeconds(10);
            _timer_Dataload = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                if (_time == TimeSpan.Zero)
                {
                    //ApplicationService.SetVariableValue("Recipe not loaded", true);
                    ApplicationService.SetView("TouchpadRegion", "EmptyView");
                    new MessageBoxTask("@RecipeSystem.Results.PLCWriteError", "@RecipeSystem.Results.Text1", MessageBoxIcon.Exclamation);
                }
                _time = _time.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
            _timer_Dataload.Start();
        }
    }
}