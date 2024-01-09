using HMI.Interfaces;
using System.ComponentModel.Composition;
using System.Text;
using VisiWin.ApplicationFramework;
using HHP.PointOfService.ServiceObjects.Scanner;
using Microsoft.PointOfService;
using HMI.Views.MainRegion.MachineOverview;
using HMI.Views.MessageBoxRegion;
using VisiWin.DataAccess;
using System.Management;
using System;

namespace HMI.Services
{
    [ExportService(typeof(IBarcode))]
    [Export(typeof(IBarcode))]
    public class Service_Barcode : ServiceBase, IBarcode
    {


        HandHeldScanner mScanner = new HandHeldScanner();
        string Data = "";

        public static System.EventHandler<DeviceErrorEventArgs> update;

        public Service_Barcode()
        {
            if (ApplicationService.IsInDesignMode)
                return;
        }

        public void OpenConnection()
        {
            mScanner = new HandHeldScanner();
            mScanner.DataEvent += new DataEventHandler(DataEvent);
            try
            {
                if (mScanner.State == ControlState.Closed)
                {
                    mScanner.AssignLogicalname = "BCS";
                    mScanner.Open();

                    if (mScanner.Claimed == false)
                    {
                        mScanner.Claim(5000);
                    }

                    if (mScanner.DeviceEnabled == false)
                    {
                        mScanner.DeviceEnabled = true;
                    }

                    mScanner.DataEventEnabled = true;
                }
            }
            catch (Exception e)
            {
                var a = e;
            }

        }

        public void CloseConnection()
        {
            try
            {
                if (mScanner.DeviceEnabled == true)
                {
                    mScanner.DeviceEnabled = false;
                }
                if (mScanner.Claimed == true)
                {
                    mScanner.Release();
                }

                if (mScanner.State == ControlState.Idle)
                {
                    mScanner.Close();
                }
            }
            catch 
            {

            }

        }

        public string CheckConnection()
        {
            return mScanner.State.ToString();
        }

        private void DataEvent(object sender, Microsoft.PointOfService.DataEventArgs e)
        {
            ASCIIEncoding txt = new ASCIIEncoding();
            ApplicationService.SetVariableValue("DataPicker.DatafromScanner", txt.GetString(mScanner.ScanData).Substring(4));
            mScanner.DataEventEnabled = true; 
        }


        private void Watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (mScanner.State == ControlState.Error)
            {
                mScanner = null;
                OpenConnection();
            }

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
            OpenConnection();

            WqlEventQuery w = new WqlEventQuery
            {
                EventClassName = "__InstanceCreationEvent",
                Condition = "TargetInstance ISA 'Win32_USBControllerDevice'",
                WithinInterval = new TimeSpan(0, 0, 2)
            };

            ManagementEventWatcher watcher = new ManagementEventWatcher(w);
            watcher.EventArrived += new EventArrivedEventHandler(Watcher_EventArrived);
            watcher.Start();

            base.OnLoadProjectCompleted();
        }

    

        // Hier stehen noch die VisiWin Funktionen zur Verfügung
        protected override void OnUnloadProjectStarted()
        {
            CloseConnection();
            base.OnUnloadProjectStarted();
        }

        // Hier sind keine VisiWin Funktionen mehr verfügbar. Bei C/S ist die Verbindung zum Server schon getrennt.
        protected override void OnUnloadProjectCompleted()
        {
            base.OnUnloadProjectCompleted();
        }

       



        #endregion
    }
}
