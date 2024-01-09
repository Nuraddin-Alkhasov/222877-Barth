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
    [ExportDashboardWidget("DB_Widget_Prod3", "Dashboard.Text17", "@Dashboard.Text13", 1, 2)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class DB_Widget_Prod3 : View
    {
      
      

        public DB_Widget_Prod3()
        {
            InitializeComponent();
        }

    }
}
