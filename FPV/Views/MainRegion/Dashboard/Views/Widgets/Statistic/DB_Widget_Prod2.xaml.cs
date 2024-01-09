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
using System.Data;
using HMI.Module;

namespace HMI.Dashboard
{
    /// <summary>
    /// Interaction logic for DashboardWidgetBar.xaml
    /// </summary>
    [ExportDashboardWidget("DB_Widget_Prod2", "Dashboard.Text16", "@Dashboard.Text13", 1, 2)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DB_Widget_Prod2 : View
    {
      
      

        public DB_Widget_Prod2()
        {
            InitializeComponent();
            DataTable temp = (new localDBAdapter("SELECT * FROM ProductionData;")).DB_Output();
           Convert.ToDouble(temp.Rows[0][2]);

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title ="Wochen Daten",
                    Values = new ChartValues<double>
                    {
                        Convert.ToDouble(temp.Rows[0][2]),
                        Convert.ToDouble(temp.Rows[1][2]),
                        Convert.ToDouble(temp.Rows[2][2]),
                        Convert.ToDouble(temp.Rows[3][2]),
                        Convert.ToDouble(temp.Rows[4][2]),
                        Convert.ToDouble(temp.Rows[5][2]),
                        Convert.ToDouble(temp.Rows[6][2])
                    }
                }
            };

         
            //also adding values updates and animates the chart automatically
 

            Labels = new[] { "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag", "Samstag", "Sonntag" };
            //Formatter = value => value.ToString("N");
            
            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
    }
}
