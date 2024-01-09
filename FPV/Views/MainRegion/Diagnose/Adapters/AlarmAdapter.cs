using System;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using System.ComponentModel.Composition;
using VisiWin.Alarm;
using System.Linq;
using System.Collections.Generic;

namespace HMI.Diagnose
{
    [ExportAdapter("AlarmAdapter")]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AlarmAdapter : AdapterBase
    {
 
        private List<IAlarmItem> alarms;
        ICurrentAlarms2 CurrentAlarmList;
        private IAlarmItem alarm;

        public AlarmAdapter()
        {
            if (ApplicationService.IsInDesignMode)
                return;
            CurrentAlarmList = ApplicationService.GetService<IAlarmService>().GetCurrentAlarms2();

            CurrentAlarmList.ChangeAlarm += SetAlarmData;
            CurrentAlarmList.NewAlarm += SetAlarmData;
            CurrentAlarmList.ClearAlarm += SetAlarmData;

            alarms = CurrentAlarmList.Alarms.Where(x => (x.Group.Name == "Alarm" || x.Group.Name == "Bedienerführung") && x.AlarmState == AlarmState.Active).ToList();
          
            this.AcknowledgeAll = new ActionCommand(this.AcknowledgeAllCommandExecuted);
            this.AcknowledgeAlarm = new ActionCommand(this.AcknowledgeAlarmCommandExecuted);
        }

        private void AcknowledgeAlarmCommandExecuted(object obj)
        {
            if (alarm != null) alarm.Acknowledge();
        }

        private void AcknowledgeAllCommandExecuted(object obj)
        {
            CurrentAlarmList.AcknowledgeAll();
        }

        public ICommand AcknowledgeAll { get; set; }
        public ICommand AcknowledgeAlarm { get; set; }

        public List<IAlarmItem> Alarms
        {
            get 
            { 
                return this.alarms; 
            }
            private set
            {
                if (this.alarms != value)
                {
                    this.alarms = value;
                    this.OnPropertyChanged("Alarms");
                }
            }
        }

        public IAlarmItem Alarm
        {
            get
            {
                return this.alarm;
            }
            set
            {
                if (this.alarm != value)
                {
                    this.alarm = value;
                    this.OnPropertyChanged("Alarm");
                }
            }
        }

        void SetAlarmData(object sender, AlarmEventArgs e)
        {
            Alarms = CurrentAlarmList.Alarms.Where(x => (x.Group.Name == "Alarm" || x.Group.Name == "Bedienerführung") && x.AlarmState == AlarmState.Active).ToList();
        }



    }

}
