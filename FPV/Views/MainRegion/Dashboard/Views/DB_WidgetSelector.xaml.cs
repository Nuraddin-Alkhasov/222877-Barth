
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Dashboard
{
    /// <summary>
    /// Auswahldialog für vorhandene Widgets im Projekt
    /// </summary>
    [ExportView("DB_WidgetSelector")]
    public partial class DB_WidgetSelector : View
    {
        public DB_WidgetSelector()
        {
            this.InitializeComponent();
        }
    }
}