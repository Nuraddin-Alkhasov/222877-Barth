using HMI.Views.MessageBoxRegion;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.Helper;
using VisiWin.Language;
using VisiWin.Logging;

namespace HMI.Handmenu.Forplanet
{
    /// <summary>
    /// Interaction logic for KeyAndSwitchView.xaml
    /// </summary>
    [ExportView("H_F_Tauchbecken_Service")]
    public partial class H_F_Tauchbecken_Service : View
    {
        private readonly ILoggingService loggingService;

        public H_F_Tauchbecken_Service()
        {
            this.InitializeComponent();
            this.loggingService = ApplicationService.GetService<ILoggingService>();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@HandMenu.Text1", "@HandMenu.Text2", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.4 Modul 2 Beschichtung.05 Tauchbecken.DB LTB HMI.PC.Drive.Vorwahl Referenz", 1);
                ILanguageService textService = ApplicationService.GetService<ILanguageService>();

                string txt = textService.GetText("@Logging.Service.Text4");
                this.loggingService.Log("Service", "NewReference", txt, FastDateTime.Now);
            }
        }
    }
}