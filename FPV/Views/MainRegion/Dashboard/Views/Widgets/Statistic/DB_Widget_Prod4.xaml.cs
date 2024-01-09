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
    [ExportDashboardWidget("DB_Widget_Prod4", "Dashboard.Text18", "@Dashboard.Text13", 1, 1)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DB_Widget_Prod4 : View
    {

        public DB_Widget_Prod4()
        {
            InitializeComponent();
            DataTable tempTD = (new localDBAdapter("SELECT * FROM ProductionData WHERE DayOfWeek = '" + DateTime.Now.DayOfWeek.ToString() + "';")).DB_Output();
            DataTable tempYD = (new localDBAdapter("SELECT * FROM ProductionData WHERE DayOfWeek = '" + (DateTime.Now.AddDays(-1)).DayOfWeek + "';")).DB_Output();

            gauge.To = Convert.ToDouble(tempYD.Rows[0][2]);
            gauge.Value = Convert.ToDouble(tempTD.Rows[0][2]);
        }
        
    }
}
