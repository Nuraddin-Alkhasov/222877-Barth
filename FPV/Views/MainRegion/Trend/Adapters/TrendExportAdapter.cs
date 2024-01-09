using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using HMI.Services;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using VisiWin.DataAccess;

namespace HMI.Adapter
{
    /// <summary>
    /// Adapter Trendexport
    /// </summary>
    [ExportAdapter("TrendExportAdapter")]
    public class TrendExportAdapter : AdapterBase
    {
        public static readonly DependencyProperty ArchiveNamesProperty = DependencyProperty.Register("ArchiveNames", typeof(List<string>), typeof(TrendExportAdapter),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ExportedTrendsDataViewProperty = DependencyProperty.Register("ExportedTrendsDataView", typeof(DataView), typeof(TrendExportAdapter),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ExportFileNameProperty = DependencyProperty.Register("ExportFileName", typeof(string), typeof(TrendExportAdapter),
            new PropertyMetadata("TrendExport.csv"));

        public static readonly DependencyProperty ExportProgressProperty = DependencyProperty.Register("ExportProgress", typeof(int), typeof(TrendExportAdapter), new PropertyMetadata(0));

        public static readonly DependencyProperty InformationTextProperty = DependencyProperty.Register("InformationText", typeof(string), typeof(TrendExportAdapter),
            new PropertyMetadata(null));

        public static readonly DependencyProperty IsEnabledUnitConversionVariableValueProperty = DependencyProperty.Register("IsEnabledUnitConversionVariableValue", typeof(bool),
            typeof(TrendExportAdapter), new PropertyMetadata(false));

        public static readonly DependencyProperty SelectedArchiveNameIndexProperty = DependencyProperty.Register("SelectedArchiveNameIndex", typeof(int), typeof(TrendExportAdapter),
            new PropertyMetadata(-1));

        public static readonly DependencyProperty SelectedArchiveNameProperty = DependencyProperty.Register("SelectedArchiveName", typeof(string), typeof(TrendExportAdapter),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ShowStatesProperty = DependencyProperty.Register("ShowStates", typeof(bool), typeof(TrendExportAdapter), new PropertyMetadata(false));

        public static readonly DependencyProperty StartExportToFileIsEnabledProperty = DependencyProperty.Register("StartExportToFileIsEnabled", typeof(bool), typeof(TrendExportAdapter),
            new PropertyMetadata(true));

        public static readonly DependencyProperty StartExportToTableIsEnabledProperty = DependencyProperty.Register("StartExportToTableIsEnabled", typeof(bool), typeof(TrendExportAdapter),
            new PropertyMetadata(true));

        public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register("StartTime", typeof(DateTime), typeof(TrendExportAdapter),
            new PropertyMetadata(DateTime.Now.AddHours(-1)));

        public static readonly DependencyProperty StopExportIsEnabledProperty = DependencyProperty.Register("StopExportIsEnabled", typeof(bool), typeof(TrendExportAdapter),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StopTimeProperty = DependencyProperty.Register("StopTime", typeof(DateTime), typeof(TrendExportAdapter), new PropertyMetadata(DateTime.Now));

        public static readonly DependencyProperty TrendDataSecondsProperty = DependencyProperty.Register("TrendDataSeconds", typeof(int), typeof(TrendExportAdapter), new PropertyMetadata(0));

        public static readonly DependencyProperty UnitConversionByVariableProperty = DependencyProperty.Register("UnitConversionByVariable", typeof(bool), typeof(TrendExportAdapter),
            new PropertyMetadata(false));

        public static readonly DependencyProperty UnitConversionOffProperty = DependencyProperty.Register("UnitConversionOff", typeof(bool), typeof(TrendExportAdapter),
            new PropertyMetadata(true));

        /// <summary>
        /// Trendexportservice
        /// </summary>
        private readonly ITrendExportService trendExportService;

        /// <summary>
        /// Einzelvariable
        /// </summary>
        private readonly IVariable unitConversionSetting;

        /// <summary>
        /// Variablenservice
        /// </summary>
        private readonly IVariableService variableService;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public TrendExportAdapter()
        {
            if (ApplicationService.IsInDesignMode)
            {
                return;
            }
            if (this.trendExportService == null)
            {
                this.trendExportService = ApplicationService.GetService<ITrendExportService>();
            }
            if (this.trendExportService != null)
            {
                this.trendExportService.TrendExportCompleted += this.TrendExportService_TrendExportCompleted;
                this.trendExportService.TrendExportProgressChanged += this.TrendExportService_TrendExportProgressChanged;
            }
            if (this.variableService == null)
            {
                this.variableService = ApplicationService.GetService<IVariableService>();
            }
            if (this.variableService != null)
            {
                this.unitConversionSetting = this.variableService.GetVariable("UnitConversionSetting");
                if (this.unitConversionSetting != null)
                {
                    this.unitConversionSetting.Change += this.UnitConversionSetting_Change;
                }
            }
            this.AttachCommands();
        }

        /// <summary>
        /// Namen der Archive
        /// </summary>
        public List<string> ArchiveNames
        {
            get { return (List<string>)this.GetValue(ArchiveNamesProperty); }
            set { this.SetValue(ArchiveNamesProperty, value); }
        }

        /// <summary>
        /// Tabelle: Stellt Exportdaten dar
        /// </summary>
        public DataView ExportedTrendsDataView
        {
            get { return (DataView)this.GetValue(ExportedTrendsDataViewProperty); }
            set { this.SetValue(ExportedTrendsDataViewProperty, value); }
        }

        /// <summary>
        /// Trend: Name der Exportdatei
        /// </summary>
        public string ExportFileName
        {
            get { return (string)this.GetValue(ExportFileNameProperty); }
            set { this.SetValue(ExportFileNameProperty, value); }
        }

        /// <summary>
        /// Wert für Fortschrittsanzeige beim Exportvorgang
        /// </summary>
        public int ExportProgress
        {
            get { return (int)this.GetValue(ExportProgressProperty); }
            set { this.SetValue(ExportProgressProperty, value); }
        }

        /// <summary>
        /// Kommando: Export stoppen
        /// </summary>
        public ICommand ExportStopCommand { get; set; }

        /// <summary>
        /// Kommando: Export in Datei
        /// </summary>
        public ICommand ExportToFileCommand { get; set; }

        /// <summary>
        /// Kommando: Export in Tabelle
        /// </summary>
        public ICommand ExportToTableCommand { get; set; }

        /// <summary>
        /// Information (zum Export)
        /// </summary>
        public string InformationText
        {
            get { return (string)this.GetValue(InformationTextProperty); }
            set { this.SetValue(InformationTextProperty, value); }
        }

        public bool IsEnabledUnitConversionVariableValue
        {
            get { return (bool)this.GetValue(IsEnabledUnitConversionVariableValueProperty); }
            set { this.SetValue(IsEnabledUnitConversionVariableValueProperty, value); }
        }

        /// <summary>
        /// Name gewähltes Archiv
        /// </summary>
        public string SelectedArchiveName
        {
            get { return (string)this.GetValue(SelectedArchiveNameProperty); }
            set { this.SetValue(SelectedArchiveNameProperty, value); }
        }

        /// <summary>
        /// Index: Name gewähltes Archiv
        /// </summary>
        public int SelectedArchiveNameIndex
        {
            get { return (int)this.GetValue(SelectedArchiveNameIndexProperty); }
            set { this.SetValue(SelectedArchiveNameIndexProperty, value); }
        }

        /// <summary>
        /// Flag: Anzeige Statuswerte
        /// </summary>
        public bool ShowStates
        {
            get { return (bool)this.GetValue(ShowStatesProperty); }
            set { this.SetValue(ShowStatesProperty, value); }
        }

        public bool StartExportToFileIsEnabled
        {
            get { return (bool)this.GetValue(StartExportToFileIsEnabledProperty); }
            set { this.SetValue(StartExportToFileIsEnabledProperty, value); }
        }

        public bool StartExportToTableIsEnabled
        {
            get { return (bool)this.GetValue(StartExportToTableIsEnabledProperty); }
            set { this.SetValue(StartExportToTableIsEnabledProperty, value); }
        }

        /// <summary>
        /// Trend: Anfangszeitpunkt
        /// </summary>
        public DateTime StartTime
        {
            get { return (DateTime)this.GetValue(StartTimeProperty); }
            set { this.SetValue(StartTimeProperty, value); }
        }

        public bool StopExportIsEnabled
        {
            get { return (bool)this.GetValue(StopExportIsEnabledProperty); }
            set { this.SetValue(StopExportIsEnabledProperty, value); }
        }

        /// <summary>
        /// Trend: Endezeitpunkt
        /// </summary>
        public DateTime StopTime
        {
            get { return (DateTime)this.GetValue(StopTimeProperty); }
            set { this.SetValue(StopTimeProperty, value); }
        }

        /// <summary>
        /// Abstand der einzelnen Datenpunkte im exportierten Trend (0=alle)
        /// </summary>
        public int TrendDataSeconds
        {
            get { return (int)this.GetValue(TrendDataSecondsProperty); }
            set { this.SetValue(TrendDataSecondsProperty, value); }
        }

        /// <summary>
        /// Flag: Einheitenumschaltung
        /// </summary>
        public bool UnitConversionByVariable
        {
            get { return (bool)this.GetValue(UnitConversionByVariableProperty); }
            set { this.SetValue(UnitConversionByVariableProperty, value); }
        }

        /// <summary>
        /// Flag: Einheitenumschaltung
        /// </summary>
        public bool UnitConversionOff
        {
            get { return (bool)this.GetValue(UnitConversionOffProperty); }
            set { this.SetValue(UnitConversionOffProperty, value); }
        }

        public override void OnViewAttached(IView view)
        {
            base.OnViewAttached(view);
        }

        /// <summary>
        /// Verbindet Command und Action
        /// </summary>
        private void AttachCommands()
        {
            this.ExportToFileCommand = new ActionCommand(this.ExportToFileCommandExecuted);
            this.ExportToTableCommand = new ActionCommand(this.ExportToTableCommandExecuted);
            this.ExportStopCommand = new ActionCommand(this.ExportStopCommandExecuted);
        }

        private void ExportStopCommandExecuted(object parameter)
        {
            this.trendExportService.CancelExport();
        }

        private void ExportToFileCommandExecuted(object parameter)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = DateTime.Now.Year.ToString()+ DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + "_" + SelectedArchiveName; // Default file name
            dlg.DefaultExt = ".csv"; // Default file extension
            dlg.Filter = "CSV (Trennzeichen-getrennt) (.csv)|*.csv"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                ExportFileName = dlg.FileName;
                this.StartExport(TrendExportService.FileFormat.csv);
            }
         
        }

        private void ExportToTableCommandExecuted(object parameter)
        {
            this.StartExport(TrendExportService.FileFormat.datatable);
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="exportCompleted">Flag</param>
        /// <param name="information">Text</param>
        private void InitControls(bool exportCompleted, string information = null)
        {
            this.InformationText = information;
            this.StartExportToTableIsEnabled = exportCompleted;
            this.StartExportToFileIsEnabled = exportCompleted;
            this.StopExportIsEnabled = !exportCompleted;
        }

        /// <summary>
        /// Startfunktion Export Trend
        /// </summary>
        /// <param name="fileFormat">Datenformat</param>
        private void StartExport(TrendExportService.FileFormat fileFormat)
        {
            if (this.trendExportService != null)
            {
                this.ExportProgress = 0;
                this.InitControls(false);

                if (this.ExportedTrendsDataView != null)
                {
                    this.ExportedTrendsDataView.Dispose();
                    this.ExportedTrendsDataView = null;
                }

                var settings = new TrendExportSettings();
                settings.ArchiveName = this.SelectedArchiveName;
                settings.ShowStates = this.ShowStates;
                settings.ExportFileName = this.ExportFileName;
                settings.StartTime = this.StartTime;
                settings.StopTime = this.StopTime;
                settings.Format = fileFormat;

                if (this.TrendDataSeconds == 0)
                {
                    settings.TrendDataSeconds = new TimeSpan(0);
                }
                else
                {
                    settings.TrendDataSeconds = new TimeSpan(0, 0, this.TrendDataSeconds);
                }

                var options = new TrendExportOptions();
                var value = ApplicationService.GetVariableValue<int>("UnitConversionSetting");
                //Werte der ComboBox müssen mit den Werten der Enumeration übereinstimmen
                options.UnitConversionSetting = (TrendExportService.UnitConversionSettings)value;
                value = ApplicationService.GetVariableValue<int>("UnitConversionVariableValue");
                //Werte der ComboBox müssen mit den Werten der variablenabhängigen Einheitenklasse übereinstimmen
                options.UnitConversionVariableValue = value;

                this.trendExportService.ExportTrendData(settings, options);
            }
        }

        private void TrendExportService_TrendExportCompleted(object sender, TrendExportResult e)
        {
            string message;
            if (e.Exception != null)
            {
                message = ApplicationService.GetText("TrendSystemBenutzerText.Views.ExportView.Error");
                this.InitControls(true, string.Format("{0}:{1}", message, e.Exception.Message));
            }
            else if (e.Cancelled)
            {
                message = ApplicationService.GetText("TrendSystemBenutzerText.Views.ExportView.Cancel");
                this.InitControls(true, message);
            }
            else
            {
                if (e.DataTable != null)
                {
                    this.ExportedTrendsDataView = e.DataTable.DefaultView;
                }

                message = ApplicationService.GetText("TrendSystemBenutzerText.Views.ExportView.SuccessMessage"); //Ist ein Formatstring
                message = string.Format(message, e.Informations.Count, e.Informations.TimeForGetTrendData.TotalSeconds, e.Informations.TimeExportData.TotalSeconds,
                    e.Informations.TimeTotal.TotalSeconds);
                this.InitControls(true, message);
            }

            GC.Collect();
        }

        private void TrendExportService_TrendExportProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ExportProgress = e.ProgressPercentage;
        }

        private void UnitConversionSetting_Change(object sender, VariableEventArgs e)
        {
            //Abgestimmt auf die Einträge in der ComboBox
            this.IsEnabledUnitConversionVariableValue = Convert.ToInt32(e.Value) == (int)TrendExportService.UnitConversionSettings.VariableValue;
            if (!this.IsEnabledUnitConversionVariableValue)
            {
                //Als Option hier auch Umschaltvariable der variablenabhängigen Einheitenklasse zurücksetzen
                ApplicationService.SetVariableValue("UnitConversionVariableValue", 0);
            }
        }
    }
}