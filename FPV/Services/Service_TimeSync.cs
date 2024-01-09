using HMI.Interfaces;
using HMI.Module;
using HMI.Views.MessageBoxRegion;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.Services
{
    [ExportService(typeof(ITimeSync))] 
    [Export(typeof(ITimeSync))]
    public class Service_TimeSync : ServiceBase, ITimeSync
    {
        IVariableService VS;
        IVariable sync ;
        BackgroundWorker SNK;

        public Service_TimeSync()
        {
            if (ApplicationService.IsInDesignMode)
                return;
        }

        private void sync_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                if (VisiWin.Helper.BitHelper.GetBit((Int16)e.Value, 9))
                {
                    sync.SetBit(11, false);
                    SNK.RunWorkerAsync();
                }
                
            }
            
        }
     

        #region OnProject

      
        // Hier stehen noch keine VisiWin Funktionen zur Verfügung
        protected override void OnLoadProjectStarted()
        {
            int i = 0;
            while (CheckVisiWin())
            {
                if (i == 10)
                {
                    Task obTask = Task.Run(() =>
                    {
                        Thread.Sleep(4000);
                        ProcessStartInfo proc = new ProcessStartInfo();

                        proc.WindowStyle = ProcessWindowStyle.Hidden;
                        proc.FileName = "cmd";
                        proc.Arguments = "/C shutdown -f -r -t 10";
                        Process.Start(proc);
                    });

                    MessageBox.Show("Houston we have a problem. VisiWin process is running. Our space fleet was unable to destroy it. Admiral SpongeBob has pressed on the big red button which will restart this panel shortly.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                   
                }

                KillVisiWin();
                Thread.Sleep(500);
                i++;
            }

            base.OnLoadProjectStarted();
        }

        private void KillVisiWin()
        {
            Process[] prs = Process.GetProcesses();

            foreach (Process pr in prs)
            {
                if (pr.ProcessName.Contains("VisiWin"))
                {
                    try
                    {
                        pr.Kill();
                    }
                    catch
                    {
                        MessageBox.Show("Houston we have a problem. VisiWin process is running. Our space fleet was unable to destroy it. Admiral SpongeBob requires some backup.Try to run the visualisation as Administrator.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                }

            }
        }
        private bool CheckVisiWin()
        {
            Process[] prs = Process.GetProcesses();

            foreach (Process pr in prs)
            {
                if (pr.ProcessName.Contains("VisiWin"))
                {
                    return true;
                }
            }
            return false;
        }

        // Hier kann auf die VisiWin Funktionen zugegriffen werden
        protected override void OnLoadProjectCompleted()
        {
            VS = ApplicationService.GetService<IVariableService>();
            //sync = VS.GetVariable(" ");
            //sync.Change += sync_Change;

            //SNK = new BackgroundWorker();
            //SNK.DoWork += W1_DoWork;
            //SNK.WorkerReportsProgress = true;
            SetUpTimer(new TimeSpan(00, 00, 00));
            SomeMethodRunsAt();
            base.OnLoadProjectCompleted();
        }

        // Hier stehen noch die VisiWin Funktionen zur Verfügung
        protected override void OnUnloadProjectStarted()
        {
            base.OnUnloadProjectStarted();
        }

        // Hier sind keine VisiWin Funktionen mehr verfügbar. Bei C/S ist die Verbindung zum Server schon getrennt.
        protected override void OnUnloadProjectCompleted()
        {
            base.OnUnloadProjectCompleted();
        }

        private void W1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ApplicationService.SetVariableValue("",DateTime.Now.Hour);
            ApplicationService.SetVariableValue("", DateTime.Now.Minute);
            ApplicationService.SetVariableValue("", DateTime.Now.Second);
            ApplicationService.SetVariableValue("", true);
        }

        private System.Threading.Timer timer;
        private void SetUpTimer(TimeSpan alertTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                return;
            }
            this.timer = new System.Threading.Timer(x =>
            {
                this.SomeMethodRunsAt();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        private void SomeMethodRunsAt()
        {
            Task obTask = Task.Run(() =>
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    DataTable temp = (new localDBAdapter("SELECT Value FROM Config WHERE Variable = 'TimePDWasReset';")).DB_Output();

                    if (Convert.ToDateTime(temp.Rows[0][0]).DayOfWeek != DateTime.Now.DayOfWeek)
                    {
                        var a = (new localDBAdapter("UPDATE ProductionData SET TotalWeight = 0 WHERE DayofWeek = '" + DateTime.Now.DayOfWeek.ToString() + "';").DB_Input());
                        var b = (new localDBAdapter("UPDATE Config SET Value = '"+ GetDataTimeNowToFormat() + "' WHERE Variable = 'TimePDWasReset' ;").DB_Input());
                    } 
                });
            });
        }

        private string GetDataTimeNowToFormat()
        {
            return DateTime.Now.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToLongTimeString();
        }

        #endregion

    }
}
