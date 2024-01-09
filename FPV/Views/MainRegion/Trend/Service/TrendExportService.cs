using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Trend;

namespace HMI.Services
{
    /// <summary>
    /// Interface Service TrendExport
    /// </summary>
    public interface ITrendExportService
    {
        /// <summary>
        /// Fertigevent Trendexport
        /// </summary>
        event EventHandler<TrendExportResult> TrendExportCompleted;

        /// <summary>
        /// Fortschrittsevent Trendexport
        /// </summary>
        event ProgressChangedEventHandler TrendExportProgressChanged;

        /// <summary>
        /// Liste mit Namen der VisiWin-Archive
        /// </summary>
        string[] ArchiveNames { get; }

        /// <summary>
        /// Abbruch der Exportfunktion
        /// </summary>
        void CancelExport();

        /// <summary>
        /// Ausführung der Exportfunktion
        /// </summary>
        /// <param name="settings">Einstellungen</param>
        /// <param name="options">Optionen</param>
        void ExportTrendData(TrendExportSettings settings, TrendExportOptions options = null);
    }

    /// <summary>
    /// Service Trendexport
    /// </summary>
    [ExportService(typeof(ITrendExportService))]
    public class TrendExportService : ServiceBase, ITrendExportService
    {
        private readonly List<MyDataSample> listIDataSample = new List<MyDataSample>();
        private readonly Stopwatch stopWacth = new Stopwatch();
        private IArchive archive;
        private string archiveName;
        private string[] archiveNames;
        private BackgroundWorker backgroundWorker;
        private DataTable dataTable;
        private Dictionary<int, MyDataSample> emptyRecordSet;
        private DateTime endTime;
        private FileFormat fileFormat;
        private string fileName;
        private Dictionary<int, string> headerTexts;
        private int recortSetCount;
        private DateTime startTime;
        private List<MyStepDataSample> stepDataRecordSets;
        private TimeSpan stepTimeSpan;
        private TextWriter textWriter;
        private TimeSpan trendDataCalculateTime;
        private TimeSpan trendDataServerGetTime;
        private readonly ITrendService trendService;
        private bool trendStates;
        private UnitConversionSettings unitConversionSetting = UnitConversionSettings.Off;
        private Dictionary<int, IVariable> variables = new Dictionary<int, IVariable>();

        /// <summary>
        /// Konstruktor
        /// </summary>
        public TrendExportService()
        {
            this.trendService = ApplicationService.GetService<ITrendService>();
            this.InitBackgroundWorker();
        }

        /// <summary>
        ///     <see cref="ITrendExportService.TrendExportCompleted" />
        /// </summary>
        public event EventHandler<TrendExportResult> TrendExportCompleted;

        /// <summary>
        ///     <see cref="ITrendExportService.TrendExportProgressChanged" />
        /// </summary>
        public event ProgressChangedEventHandler TrendExportProgressChanged;

        /// <summary>
        /// Dateiformate (für den Trendexport)
        /// </summary>
        public enum FileFormat
        {
            csv,
            datatable
        }

        /// <summary>
        /// Status der Einheitenumschaltung
        /// </summary>
        public enum UnitConversionSettings
        {
            Off,
            VariableValue
        }

        /// <summary>
        ///     <see cref="ITrendExportService.ArchiveNames" />
        /// </summary>
        public string[] ArchiveNames
        {
            get
            {
                if ((this.archiveNames == null) && (this.trendService != null) && (this.trendService.ArchiveNames != null) && (this.trendService.ArchiveNames.Count > 0))
                {
                    this.archiveNames = this.trendService.ArchiveNames.ToArray();
                }

                return this.archiveNames;
            }
        }

        private Dictionary<int, string> HeaderTexts
        {
            get
            {
                if (this.headerTexts == null)
                {
                    this.headerTexts = new Dictionary<int, string>();
                }

                return this.headerTexts;
            }
        }

        private List<MyStepDataSample> StepDataRecordSets
        {
            get
            {
                if (this.stepDataRecordSets == null)
                {
                    this.stepDataRecordSets = new List<MyStepDataSample>();
                }

                return this.stepDataRecordSets;
            }
        }

        private Dictionary<int, IVariable> Variables
        {
            get
            {
                if (this.variables == null)
                {
                    this.variables = new Dictionary<int, IVariable>();
                }

                return this.variables;
            }
        }

        public void CancelExport()
        {
            this.backgroundWorker.CancelAsync();
        }

        /// <summary>
        ///     <see cref="ITrendExportService.ExportTrendData" />
        /// </summary>
        public void ExportTrendData(TrendExportSettings settings, TrendExportOptions options = null)
        {
            if (settings != null)
            {
                this.archiveName = settings.ArchiveName;
                this.trendStates = settings.ShowStates;
                this.fileName = settings.ExportFileName;
                this.fileFormat = settings.Format;
                this.startTime = settings.StartTime;
                this.endTime = settings.StopTime;
                this.stepTimeSpan = settings.TrendDataSeconds;

                this.unitConversionSetting = UnitConversionSettings.Off;

                if (options != null)
                {
                    this.unitConversionSetting = options.UnitConversionSetting;
                }

                this.ExportTrendData();
            }
        }

        private void AddRecordSet(DateTime dateTimeValue, Dictionary<int, MyDataSample> recordSets)
        {
            if (this.fileFormat == FileFormat.csv)
            {
                this.AddRecordSetToCsvFile(dateTimeValue, recordSets);
                this.recortSetCount++;
            }
            else if (this.fileFormat == FileFormat.datatable)
            {
                this.AddRecordSetToDataTable(dateTimeValue, recordSets);
                this.recortSetCount++;
            }
        }

        private void AddRecordSetToCsvFile(DateTime dateTimeValue, Dictionary<int, MyDataSample> recordSets)
        {
            if (this.textWriter == null)
            {
                if (File.Exists(this.fileName))
                {
                    File.Delete(this.fileName);
                }
                this.textWriter = File.CreateText(this.fileName);
                var recordSetValue = ApplicationService.GetText("@Application.TrendSystemBenutzerText.Views.ExportView.DataGridColumnTime");

                foreach (var trendIndex in recordSets.Keys)
                {
                    var headerText = this.GetTrendHeaderText(trendIndex);
                    recordSetValue = recordSetValue + ";" + headerText;

                    if (this.trendStates)
                    {
                        recordSetValue = recordSetValue + ";" + this.GetTrendStateHeaderText(headerText);
                    }
                }

                this.textWriter.WriteLine(recordSetValue);
            }

            var sB = new StringBuilder();
            sB.Append(dateTimeValue.ToString(CultureInfo.InvariantCulture));

            foreach (var trendIndex in recordSets.Keys)
            {
                sB.Append(";");
                sB.Append(recordSets[trendIndex] == null ? "" : this.GetTrendValue(trendIndex, recordSets[trendIndex].DataSample));

                if (this.trendStates)
                {
                    sB.Append(";");
                    if ((recordSets[trendIndex] != null) && (recordSets[trendIndex].DataSample.State != SampleState.Standard))
                    {
                        sB.Append(recordSets[trendIndex].DataSample.State);
                    }
                    else
                    {
                        sB.Append("");
                    }
                }
            }

            this.textWriter.WriteLine(sB.ToString());
        }

        private void AddRecordSetToDataTable(DateTime dateTimeValue, Dictionary<int, MyDataSample> recordSets)
        {
            if (this.dataTable == null)
            {
                this.dataTable = new DataTable();

                this.dataTable.Columns.Add(new DataColumn { ColumnName = ApplicationService.GetText("@Application.TrendSystemBenutzerText.Views.ExportView.DataGridColumnTime"),  DataType = typeof(string) });

                foreach (var trendIndex in recordSets.Keys)
                {
                    var headerText = this.GetTrendHeaderText(trendIndex);

                    this.dataTable.Columns.Add(new DataColumn { ColumnName = headerText, DataType = typeof(string) });

                    if (this.trendStates)
                    {
                        this.dataTable.Columns.Add(new DataColumn { ColumnName = this.GetTrendStateHeaderText(headerText), DataType = typeof(string) });
                    }
                }

                this.dataTable.RejectChanges();
            }

            if (this.dataTable != null)
            {
                var dataRow = this.dataTable.NewRow();

                dataRow[0] = string.Format("{0:MM/dd/yy hh:mm:ss fff}", dateTimeValue);

                foreach (var trendIndex in recordSets.Keys)
                {
                    if (recordSets[trendIndex] != null)
                    {
                        var trendHeaderText = this.GetTrendHeaderText(trendIndex);
                        dataRow[trendHeaderText] = this.GetTrendValue(trendIndex, recordSets[trendIndex].DataSample);
                        if (this.trendStates && (recordSets[trendIndex].DataSample.State != SampleState.Standard))
                        {
                            dataRow[this.GetTrendStateHeaderText(trendHeaderText)] = recordSets[trendIndex].DataSample.State.ToString();
                        }
                    }
                }

                this.dataTable.Rows.Add(dataRow);
            }
        }

        private void archive_GetTrendsDataCompleted(object sender, GetTrendsDataCompletedEventArgs e)
        {
            this.archive.GetTrendsDataCompleted -= this.archive_GetTrendsDataCompleted;

            this.stopWacth.Stop();
            this.trendDataServerGetTime = this.stopWacth.Elapsed;
            this.stopWacth.Reset();
            this.stopWacth.Start();

            this.SaveExportData(e);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var trendsData = e.Argument as GetTrendsDataCompletedEventArgs;

            if (trendsData != null)
            {
                this.recortSetCount = 0;
                var recordSetValues = new Dictionary<int, MyDataSample>();

                for (var n = 0; n < this.archive.TrendNames.Count; n++)
                {
                    if (this.backgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    var dataSample = trendsData.Data.GetValue(n) as IDataSample[];

                    if (dataSample != null)
                    {
                        this.listIDataSample.AddRange(dataSample.Select(ds => new MyDataSample(n, ds)));
                    }
                }

                var dataSamplesSortedByTime = this.listIDataSample.OrderBy(dataSample => dataSample.DataSample.Time).ToArray();
                var samplesCount = this.listIDataSample.Count;

                var recordChangedTime = DateTime.MinValue;

                var trendNamesCount = this.archive.TrendNames.Count;

                for (var index = 0; index < samplesCount; index++)
                {
                    if (this.backgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    var sortedValue = dataSamplesSortedByTime[index];

                    if ((recordSetValues.ContainsKey(sortedValue.TrendIndex) == false) && !this.IsOutsideInterval(sortedValue.DataSample))
                    {
                        recordSetValues.Add(sortedValue.TrendIndex, sortedValue);
                    }

                    if (sortedValue.DataSample.Time.CompareTo(recordChangedTime) > 0)
                    {
                        //Zeitwechsel Datensatz abspeichern.
                        if (recordSetValues.Count == trendNamesCount)
                        {
                            if ((this.stepTimeSpan == null) || (this.stepTimeSpan.Ticks == 0))
                            {
                                this.AddRecordSet(recordChangedTime, recordSetValues);
                            }
                            else
                            {
                                //Datensatz zwischenspeichern, für Schrittweise (StepTime) zu exportieren
                                this.StepDataRecordSets.Add(this.GetDataSampleStepData(recordChangedTime, recordSetValues));
                            }
                        }

                        //Aktuelle Zeit ändern
                        recordChangedTime = sortedValue.DataSample.Time;
                    }

                    if (!this.IsOutsideInterval(sortedValue.DataSample))
                    {
                        recordSetValues[sortedValue.TrendIndex] = sortedValue;
                    }
                    var percent = Convert.ToInt32(Math.Truncate((double)100 / samplesCount * index));
                    this.backgroundWorker.ReportProgress(percent);
                }
            }

            if (this.StepDataRecordSets.Count > 0)
            {
                this.MakeStepData(e);
            }
            this.backgroundWorker.ReportProgress(100);
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (this.TrendExportProgressChanged != null)
            {
                this.TrendExportProgressChanged(sender, e);
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.DisposeTextWriter();
            this.DisposeVariables();
            this.DisposeHeaderTexts();
            this.DisposeStepDataRecordSets();

            this.stopWacth.Stop();
            this.trendDataCalculateTime = this.stopWacth.Elapsed;
            this.stopWacth.Reset();

            var informations = new Informations(this.recortSetCount, this.trendDataServerGetTime, this.trendDataCalculateTime);

            if (this.TrendExportCompleted != null)
            {
                this.listIDataSample.Clear();

                TrendExportResult trendExportResult = null;

                if (e.Error == null)
                {
                    if (this.fileFormat == FileFormat.csv)
                    {
                        trendExportResult = new TrendExportResult(e.Cancelled, informations);
                    }
                    else if (this.fileFormat == FileFormat.datatable)
                    {
                        trendExportResult = new TrendExportResult(this.dataTable, e.Cancelled, informations);
                    }
                }
                else
                {
                    trendExportResult = new TrendExportResult(e.Error, false);
                }

                this.TrendExportCompleted(this, trendExportResult);
            }

            Debug.WriteLine("backgroundWorker_RunWorkerCompleted");
        }

        private void DisposeDataTable()
        {
            if (this.dataTable != null)
            {
                if (this.dataTable.DefaultView != null)
                {
                    this.dataTable.DefaultView.Dispose();
                }

                this.dataTable.Dispose();
                this.dataTable = null;
            }
        }

        private void DisposeHeaderTexts()
        {
            if (this.headerTexts != null)
            {
                this.headerTexts.Clear();
                this.headerTexts = null;
            }
        }

        private void DisposeStepDataRecordSets()
        {
            if (this.stepDataRecordSets != null)
            {
                this.stepDataRecordSets.Clear();
                this.stepDataRecordSets = null;
            }
        }

        private void DisposeTextWriter()
        {
            if (this.textWriter != null)
            {
                this.textWriter.Close();
                this.textWriter.Dispose();
                this.textWriter = null;
            }
        }

        private void DisposeVariables()
        {
            if (this.variables != null)
            {
                this.variables.Clear();
                this.variables = null;
            }
        }

        private void ExportTrendData()
        {
            Guard.ThrowIfArgumentNullOrEmpty("archiveName", this.archiveName);

            if (this.trendService != null)
            {
                this.stopWacth.Start();
                this.DisposeDataTable();
                this.archive = this.trendService.GetArchive(this.archiveName);
                this.archive.GetTrendsDataCompleted += this.archive_GetTrendsDataCompleted;
                this.archive.GetTrendsDataAsync(this.archive.TrendNames.ToArray(), this.startTime, this.endTime, null, 0);
            }
        }

        private DateTime FillStepDataEmptyValues(DateTime startTime, DateTime lastTime)
        {
            var stepTime = startTime;
            while (stepTime < lastTime)
            {
                //Diese darf aufgenommen werden
                this.AddRecordSet(stepTime, this.GetEmptyRecordSet());

                //Zeit hoch zählen
                stepTime = stepTime.Add(this.stepTimeSpan);
            }

            return stepTime;
        }

        private MyStepDataSample GetDataSampleStepData(DateTime recordChangedTime, Dictionary<int, MyDataSample> recordSetValues)
        {
            var dicCopyRecordSet = new Dictionary<int, MyDataSample>();
            foreach (var trendIndex in recordSetValues.Keys)
            {
                dicCopyRecordSet.Add(trendIndex, recordSetValues[trendIndex]);
            }

            return new MyStepDataSample { RecordSetTime = recordChangedTime, MyDataSamples = dicCopyRecordSet };
        }

        private Dictionary<int, MyDataSample> GetEmptyRecordSet()
        {
            if (this.emptyRecordSet == null)
            {
                this.emptyRecordSet = new Dictionary<int, MyDataSample>();
                for (var n = 0; n < this.archive.TrendNames.Count; n++)
                {
                    this.emptyRecordSet.Add(n, null);
                }
            }

            return this.emptyRecordSet;
        }

        private string GetTrendHeaderText(int trendIndex)
        {
            if (this.HeaderTexts.ContainsKey(trendIndex))
            {
                return this.HeaderTexts[trendIndex];
            }

            var headerText = this.archive.GetTrend(this.archive.TrendNames[trendIndex]).Text;
            //Daten aus Archiven müssen bei Anwendung einer Einheitenumschaltung explizit berechnet werden
            if (this.unitConversionSetting > UnitConversionSettings.Off)
            {
                var variable = this.GetVariable(trendIndex);
                if ((variable != null) && !string.IsNullOrEmpty(variable.UnitText))
                {
                    if (this.fileFormat == FileFormat.csv)
                    {
                        headerText = headerText + " " + "[" + variable.UnitText + "]";
                    }
                    else
                    {
                        headerText = headerText + " " + "(" + variable.UnitText + ")";
                    }
                }
            }

            //Ab in die Liste
            this.HeaderTexts.Add(trendIndex, headerText);

            return headerText;
        }

        private string GetTrendStateHeaderText(string headerText)
        {
            return headerText + " " + "States";
        }

        private string GetTrendValue(int trendIndex, IDataSample dataSample)
        {
            var result = dataSample.YValue.ToString();
            //Daten aus Archiven müssen bei Anwendung einer Einheitenumschaltung explizit berechnet werden
            if (this.unitConversionSetting > UnitConversionSettings.Off)
            {
                var variable = this.GetVariable(trendIndex);
                if ((variable.UnitClass != null) && (variable.UnitClass.CurrentUnit != null))
                {
                    result = variable.UnitClass.CurrentUnit.ToValue(dataSample.YValue).ToString();
                }
            }

            return result;
        }

        private IVariable GetVariable(int trendIndex)
        {
            if (this.Variables.ContainsKey(trendIndex) == false)
            {
                var trend = this.archive.GetTrend(this.archive.TrendNames[trendIndex]);
                var variableService = ServiceLocator.Current.GetInstance<IVariableService>();
                var variable = variableService.GetVariable(trend.GetDefinition().TrendVariableName);

                this.Variables.Add(trendIndex, variable);
            }

            return this.Variables[trendIndex];
        }

        private void InitBackgroundWorker()
        {
            this.backgroundWorker = new BackgroundWorker();
            this.backgroundWorker.DoWork += this.backgroundWorker_DoWork;
            this.backgroundWorker.RunWorkerCompleted += this.backgroundWorker_RunWorkerCompleted;
            this.backgroundWorker.ProgressChanged += this.backgroundWorker_ProgressChanged;
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.WorkerSupportsCancellation = true;
        }

        private bool IsOutsideInterval(IDataSample dataSample)
        {
            if (dataSample != null)
            {
                return (dataSample.State & SampleState.OutsideInterval) == SampleState.OutsideInterval;
            }

            return false;
        }

        private bool IsRecordingInterval(MyStepDataSample stepDataSample)
        {
            var result = true;

            foreach (var myDataSample in stepDataSample.MyDataSamples.Values)
            {
                if (myDataSample != null)
                {
                    if (!((myDataSample.DataSample.State & SampleState.StartedRecording) == SampleState.StartedRecording))
                    {
                        //Es ist ein Recordset gestartet
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        private void MakeStepData(DoWorkEventArgs e)
        {
            var recordStepTimeStart = this.StepDataRecordSets[0].RecordSetTime;
            var recordTimeEnd = this.StepDataRecordSets[this.StepDataRecordSets.Count - 1].RecordSetTime;

            var nStart = 0;
            var rightIndex = 0;

            while (recordStepTimeStart < recordTimeEnd)
            {
                if (this.backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                //Den Datensatz für die stepedTime finden
                for (var n = nStart; n < this.StepDataRecordSets.Count; n++)
                {
                    //Aktuellen Datensatz merken, damit die nächste Datensatzsuche
                    //schneller geht.
                    nStart = n;

                    if (this.StepDataRecordSets[n].RecordSetTime > recordStepTimeStart)
                    {
                        //Datensatz zu diesem stepedTime gefunden
                        if (n - 1 > -1)
                        {
                            rightIndex = n - 1;
                        }

                        break;
                    }
                }

                //Man muss nun herausfinden, ob ein Dateiwechsel vorhanden ist
                if (this.IsRecordingInterval(this.StepDataRecordSets[rightIndex + 1]))
                {
                    //Dateiwechsel
                    if (recordStepTimeStart <= this.StepDataRecordSets[rightIndex].RecordSetTime)
                    {
                        this.AddRecordSet(recordStepTimeStart, this.StepDataRecordSets[rightIndex].MyDataSamples);

                        recordStepTimeStart = this.FillStepDataEmptyValues(recordStepTimeStart, this.StepDataRecordSets[rightIndex + 1].RecordSetTime);
                    }
                    else
                    {
                        this.AddRecordSet(recordStepTimeStart, this.GetEmptyRecordSet());
                    }
                }
                else
                {
                    //Kein Dateiwechsel
                    this.AddRecordSet(recordStepTimeStart, this.StepDataRecordSets[rightIndex].MyDataSamples);
                }

                recordStepTimeStart = recordStepTimeStart.Add(this.stepTimeSpan);
            }
        }

        private void SaveExportData(GetTrendsDataCompletedEventArgs e)
        {
            if (this.backgroundWorker.IsBusy)
            {
                Debug.WriteLine("this.backgroundWorker.IsBusy=" + this.backgroundWorker.IsBusy);
                return;
            }

            this.backgroundWorker.RunWorkerAsync(e);
        }

        private class MyDataSample
        {
            public MyDataSample(int trendIndex, IDataSample dataSample)
            {
                this.TrendIndex = trendIndex;
                this.DataSample = dataSample;
            }

            public IDataSample DataSample { get; private set; }

            public int TrendIndex { get; private set; }
        }

        private class MyStepDataSample
        {
            public Dictionary<int, MyDataSample> MyDataSamples { get; set; }
            public DateTime RecordSetTime { get; set; }
        }
    }

    #region Hilfsklassen

    /// <summary>
    /// Einstellungen Trendexport
    /// </summary>
    public class TrendExportSettings
    {
        private DateTime stopTime = DateTime.MaxValue;
        private DateTime startTime = DateTime.MinValue;
        private string exportFileName = "TrendExport.csv";

        /// <summary>
        /// Archivename
        /// </summary>
        public string ArchiveName { get; set; }

        /// <summary>
        /// Name der Exportdatei (Default: "TrendExport.csv")
        /// </summary>
        public string ExportFileName
        {
            get { return this.exportFileName; }
            set { this.exportFileName = value; }
        }

        /// <summary>
        /// Dateiformat
        /// </summary>
        public TrendExportService.FileFormat Format { get; set; }

        /// <summary>
        /// Flag: Darstellung Status
        /// </summary>
        public bool ShowStates { get; set; }

        /// <summary>
        /// Startzeitpunkt Export (Default: DateTime.MinValue)
        /// </summary>
        public DateTime StartTime
        {
            get { return this.startTime; }
            set { this.startTime = value; }
        }

        /// <summary>
        /// Startzeitpunkt Export (Default: DateTime.MaxValue)
        /// </summary>
        public DateTime StopTime
        {
            get { return this.stopTime; }
            set { this.stopTime = value; }
        }

        /// <summary>
        /// Zeitspanne zwischen den Datenpunkten (0=alle)
        /// </summary>
        public TimeSpan TrendDataSeconds { get; set; }
    }

    /// <summary>
    /// Optionen Trendexport
    /// </summary>
    public class TrendExportOptions
    {
        /// <summary>
        /// Daten Einheitenumschaltung aktivieren?
        /// </summary>
        public TrendExportService.UnitConversionSettings UnitConversionSetting { get; set; }

        /// <summary>
        /// Wert der Umschaltvariable bei aktivierter Einheitenumschaltung
        /// </summary>
        public int UnitConversionVariableValue { get; set; }
    }

    /// <summary>
    /// Beinhaltet Ergebnis eines Exportvorgangs
    /// </summary>
    public class TrendExportResult : EventArgs
    {
        public TrendExportResult(Exception exception, bool cancelled)
        {
            this.Exception = exception;
            this.Cancelled = cancelled;
        }

        public TrendExportResult(bool cancelled, Informations informations)
        {
            this.Cancelled = cancelled;
            this.Informations = informations;
        }

        public TrendExportResult(DataTable dataTable, bool cancelled, Informations informations)
        {
            this.DataTable = dataTable;
            this.Cancelled = cancelled;
            this.Informations = informations;
        }

        public bool Cancelled { get; private set; }

        public DataTable DataTable { get; private set; }

        public Exception Exception { get; private set; }

        public Informations Informations { get; private set; }
    }

    /// <summary>
    /// Überwachungen
    /// </summary>
    internal static class Guard
    {
        public static void ThrowIfArgumentNull(string argumentName, object argumentValue)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void ThrowIfArgumentNullOrEmpty(string argumentName, string argumentValue)
        {
            if (string.IsNullOrEmpty(argumentValue))
            {
                throw new ArgumentNullException(argumentName, "parameter is null or empty.");
            }
        }

        public static void ThrowIfFormatNotSupported(TrendExportService.FileFormat fileFormat)
        {
            throw new Exception(string.Format("FileFormat='{0}' wird zur Zeit nicht unterstützt.", fileFormat));
        }

        public static void ThrowIfMemberIsNull(string memberName, object memberValue)
        {
            if (memberValue == null)
            {
                throw new InvalidOperationException(string.Format("'{0}' is null.", memberName));
            }
        }
    }

    /// <summary>
    /// Container für Informationen
    /// </summary>
    public class Informations
    {
        public Informations(int count, TimeSpan trendDataServerTime, TimeSpan trendDataCalculateTime)
        {
            this.Count = count;
            this.TimeForGetTrendData = trendDataServerTime;
            this.TimeExportData = trendDataCalculateTime;
        }

        public int Count { get; private set; }
        public TimeSpan TimeExportData { get; private set; }
        public TimeSpan TimeForGetTrendData { get; private set; }

        public TimeSpan TimeTotal
        {
            get { return this.TimeForGetTrendData + this.TimeExportData; }
        }
    }

    #endregion
}