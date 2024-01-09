using HMI.Views.DialogRegion;
using HMI.Views.MessageBoxRegion;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using VisiWin.Controls;
using VisiWin.Trend;

namespace HMI
{
    [ExportAdapter("TrendAdapter")]
    public class TrendAdapter : AdapterBase, INotifyPropertyChanged
    {
        private ITrendService trendService;

        #region Initialisierung

        public TrendAdapter()
        {
            if (ApplicationService.IsInDesignMode)
                return;

            this.ShowTrendCurveConfigurationCommand = new ActionCommand(OnShowTrendCurveConfigurationCommandExecuted);
            this.trendService = ApplicationService.GetService<ITrendService>();
            this.Archives = this.trendService.ArchiveNames;
        }

        private void GetTrends()
        {
            if (this.SelectedCurve != null)
                this.Trends = this.trendService.GetTrendNames(this.SelectedArchiveName);
        }

        private void InitDialog()
        {
            if (this.SelectedCurve != null)
            {
                this.SelectedArchiveName = this.SelectedCurve.ArchiveName;
                this.SelectedTrendName = null;
                this.SelectedTrendName = this.SelectedCurve.TrendName;

                this.IsCurveVisible = this.SelectedCurve.Visibility == System.Windows.Visibility.Visible;
                this.IsScaleVisible = this.SelectedCurve.ScaleY.Visibility == System.Windows.Visibility.Visible;

                this.ScaleMinValue = this.SelectedCurve.ScaleY.MinValue;
                this.ScaleMaxValue = this.SelectedCurve.ScaleY.MaxValue;

                this.CurveLineBrush = this.SelectedCurve.LineBrush;
            }
        }

        #endregion

        #region Befehle

        public ICommand ShowTrendCurveConfigurationCommand { get; set; }

        public void OnShowTrendCurveConfigurationCommandExecuted(object parameter)
        {
            if (this.SelectedCurve != null)
            {
                this.InitDialog();
                DialogView.Show("TrendCurveConfigurationView", "Kurve konfigurieren", VerifyDialogResultFunction: this.CommitTrendCurveChanges);
            }
            else
            {
                new MessageBoxTask("@TrendSystem.Results.NoCurveSelected", "@TrendSystem.Results.NoCurveSelectedCaption",  MessageBoxIcon.Information);
            }
        }

        #endregion

        #region Dialogfunktionen

        private void CommitTrendCurveChanges(object sender, DialogResultEventArgs e)
        {
            if (e.Result == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(this.SelectedArchiveName) && !string.IsNullOrEmpty(this.SelectedTrendName))
                {
                    this.SelectedCurve.ArchiveName = this.SelectedArchiveName;
                    this.SelectedCurve.TrendName = this.SelectedTrendName;

                    this.SelectedCurve.Visibility = this.IsCurveVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                    this.SelectedCurve.ScaleY.Visibility = this.IsScaleVisible ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;

                    this.SelectedCurve.ScaleY.MinValue = this.ScaleMinValue;
                    this.SelectedCurve.ScaleY.MaxValue = this.ScaleMaxValue;

                    this.SelectedCurve.LineBrush = this.CurveLineBrush;
                }
                else
                {
                    e.CancelDialogClosing = true;
                }
            }
        }

        #endregion

        #region Bindungsmöglichkeiten

        private TrendCurve selectedCurve;
        public TrendCurve SelectedCurve
        {
            get { return this.selectedCurve; }
            set
            {
                this.selectedCurve = value;
                OnPropertyChanged("SelectedCurve");
            }
        }

        private string selectedArchiveName;
        public string SelectedArchiveName
        {
            get { return this.selectedArchiveName; }
            set
            {
                this.selectedArchiveName = value;
                this.GetTrends();
                OnPropertyChanged("SelectedArchiveName");
            }
        }

        private string selectedTrendName;
        public string SelectedTrendName
        {
            get { return this.selectedTrendName; }
            set
            {
                this.selectedTrendName = value;
                OnPropertyChanged("SelectedTrendName");
            }
        }

        private bool isCurveVisible;
        public bool IsCurveVisible
        {
            get { return this.isCurveVisible; }
            set
            {
                this.isCurveVisible = value;
                OnPropertyChanged("CurveVisible");
            }
        }

        private bool isScaleVisible;
        public bool IsScaleVisible
        {
            get { return this.isScaleVisible; }
            set
            {
                this.isScaleVisible = value;
                OnPropertyChanged("IsScaleVisible");
            }
        }

        private double scaleMinValue;
        public double ScaleMinValue
        {
            get { return this.scaleMinValue; }
            set
            {
                this.scaleMinValue = value;
                OnPropertyChanged("ScaleMinValue");
            }
        }

        private double scaleMaxValue;
        public double ScaleMaxValue
        {
            get { return this.scaleMaxValue; }
            set
            {
                this.scaleMaxValue = value;
                OnPropertyChanged("ScaleMaxValue");
            }
        }

        private Brush curveLineBrush;
        public Brush CurveLineBrush
        {
            get { return this.curveLineBrush; }
            set
            {
                this.curveLineBrush = value;
                OnPropertyChanged("CurveLineBrush");
            }
        }

        private ReadOnlyCollection<string> archives;
        public ReadOnlyCollection<string> Archives
        {
            get { return this.archives; }
            set
            {
                this.archives = value;
                OnPropertyChanged("Archives");
            }
        }

        private ReadOnlyCollection<string> trends;
        public ReadOnlyCollection<string> Trends
        {
            get { return this.trends; }
            set
            {
                this.trends = value;
                OnPropertyChanged("Trends");
            }
        }

        #endregion
    }
}
