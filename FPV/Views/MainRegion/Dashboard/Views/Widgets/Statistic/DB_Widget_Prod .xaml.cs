using System.ComponentModel.Composition;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using System;
using System.ComponentModel;
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using System.Data;
using HMI.Module;

namespace HMI.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardWidgetBar.xaml
    /// </summary>
    [ExportDashboardWidget("DB_Widget_Prod", "Dashboard.Text15", "@Dashboard.Text13", 1, 2)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DB_Widget_Prod : View
    {
      
      

        public DB_Widget_Prod()
        {
            InitializeComponent();
            PointLabel = chartPoint => string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
            pieeee.ChartLegend.FontSize = 14;
            pieeee.ChartLegend.Foreground = Brushes.White;
            DataContext = this;
            DataTable tempTD = (new localDBAdapter("SELECT * FROM ProductionData WHERE DayOfWeek = '" + DateTime.Now.DayOfWeek.ToString() + "';")).DB_Output();
            DataTable tempYD = (new localDBAdapter("SELECT * FROM ProductionData WHERE DayOfWeek = '" + (DateTime.Now.AddDays(-1)).DayOfWeek + "';")).DB_Output();

            pieeee.Series[0].Values[0]= Convert.ToDouble(tempYD.Rows[0][2]);
            pieeee.Series[1].Values[0] = Convert.ToDouble(tempTD.Rows[0][2]);
        }

        public Func<ChartPoint, string> PointLabel { get; set; }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }
    }
}
