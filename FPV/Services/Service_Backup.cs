using HMI.Interfaces;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using VisiWin.ApplicationFramework;

namespace HMI.Services
{
    [ExportService(typeof(IBackup))] 
    [Export(typeof(IBackup))]
    public class Service_Backup : ServiceBase, IBackup
    {

        public Service_Backup()
        {
            if (ApplicationService.IsInDesignMode)
                return;
        }    

        #region OnProject

        // Hier stehen noch keine VisiWin Funktionen zur Verfügung
        protected override void OnLoadProjectStarted()
        {
            base.OnLoadProjectStarted();
        }

        // Hier kann auf die VisiWin Funktionen zugegriffen werden
        protected override void OnLoadProjectCompleted()
        {
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

        private static Timer timer;

        private void SetUpTimer(TimeSpan alertTime)
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;
            if (timeToGo < TimeSpan.Zero)
            {
                timeToGo = new TimeSpan(24, 0, 0) - current.TimeOfDay + alertTime;
            }
            timer = new Timer(x => { this.SomeMethodRunsAt(); }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        private void SomeMethodRunsAt()
        {
            SetUpTimer(new TimeSpan(22, 0, 0));
            new DoBackup();
        }

        private string GetDataTimeNowToFormat()
        {
            return DateTime.Now.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToLongTimeString();
        }

        #endregion

    }

}
