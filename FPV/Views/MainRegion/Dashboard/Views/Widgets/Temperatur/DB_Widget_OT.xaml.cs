using System.ComponentModel.Composition;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Wpf;
using VisiWin.DataAccess;
using System.Collections.Generic;
using System.Windows.Media;

namespace HMI.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardWidgetBar.xaml
    /// </summary>
    [ExportDashboardWidget("DB_Widget_OT", "Dashboard.Text6", "@Dashboard.Text11", 1, 1)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DB_Widget_OT : View, INotifyPropertyChanged
    {
        private double _lastLecture;
        IVariableService VS = ApplicationService.GetService<IVariableService>();
        IVariable WM;

        public DB_Widget_OT()
        {
            InitializeComponent();
            
            WM = VS.GetVariable("PLC.PLC.Blocks.5 Modul 3 Trockner.08 Heizung / Ventilatoren.03 Trockner.DB Trockner HMI.Istwerte.Temperatur");
          

            LastHourSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Ofen Temperatur",
                    AreaLimit = -10,
                    StrokeThickness = 5,
                    PointGeometrySize = 0,
                    LineSmoothness = 1,
                    Stroke = new SolidColorBrush(Color.FromRgb(255,188,73)),
                   
                    Values = new ChartValues<double>
                    {
                        Convert.ToDouble(WM.Value)-20,
                        Convert.ToDouble(WM.Value),
                        Convert.ToDouble(WM.Value),
                        Convert.ToDouble(WM.Value)
                    }
                }


            };
            Labels = new List<string>();
            Labels.Add(DateTime.Now.ToLongTimeString());

            DataContext = this;
            WM.Change += WM_Change;
        }

        public List<string> _Labels;
        public List<string> Labels
        {
            get { return _Labels; }
            set
            {
                _Labels = value;
                this.OnPropertyChanged("Labels");
            }
        }

        public SeriesCollection LastHourSeries { get; set; }

        public double LastLecture
        {
            get { return _lastLecture; }
            set
            {
                _lastLecture = value;
                this.OnPropertyChanged("LastLecture");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void WM_Change(object sender, VariableEventArgs e)
        {
            if (LastHourSeries != null)
            {
                if (Labels.Count == 20)
                {
                    Labels.RemoveAt(1);
                    LastHourSeries[0].Values.RemoveAt(1);
                }

                Labels.Add(DateTime.Now.ToLongTimeString());
                LastHourSeries[0].Values.Add(Convert.ToDouble(e.Value));
                LastLecture = Convert.ToDouble(e.Value);
            }
        }
    }
}
