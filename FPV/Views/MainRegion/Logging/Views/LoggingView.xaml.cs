using System.Windows;
using VisiWin.ApplicationFramework;

namespace HMI
{
    /// <summary>
    /// Interaction logic for AlarmHistoryView.xaml
    /// </summary>
    [ExportView("LoggingView")]
    public partial class LoggingView : VisiWin.Controls.View
    {
        public LoggingView()
        {
            this.InitializeComponent();
        }
    }
}