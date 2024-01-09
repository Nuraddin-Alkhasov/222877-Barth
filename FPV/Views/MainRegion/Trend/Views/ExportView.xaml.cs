using HMI.Adapter;
using System;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Views.MainRegion
{
    /// <summary>
    /// Interaction logic for ExportView.xaml
    /// </summary>
    [ExportView("ExportView")]
    public partial class ExportView : View
    {
        public ExportView()
        {
            this.InitializeComponent();
        }

        private void ExportView_View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                int T_ID = Convert.ToInt16(ApplicationService.GetVariableValue("Trend.ID"));
                switch (T_ID)
                {
                    case 0:
                        ((TrendExportAdapter)this.DataContext).SelectedArchiveName = "Vorzone"; break;
                    case 1:
                        ((TrendExportAdapter)this.DataContext).SelectedArchiveName = "Zwischenzone"; break;
                    case 2:
                        ((TrendExportAdapter)this.DataContext).SelectedArchiveName = "Trockner"; break;
                    case 3:
                        ((TrendExportAdapter)this.DataContext).SelectedArchiveName = "Kühlzone"; break;
                    case 4:
                        ((TrendExportAdapter)this.DataContext).SelectedArchiveName = "Beschichtung"; break;
                }              
            }
        }
    }
}