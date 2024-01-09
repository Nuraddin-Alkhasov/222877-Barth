using HMI.Views.MessageBoxRegion;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.Helper;
using VisiWin.Language;
using VisiWin.Logging;

namespace HMI.Handemnu.Forplanet
{
    /// <summary>
    /// Interaction logic for KeyAndSwitchView.xaml
    /// </summary>
    [ExportView("H_F_Planet_Service")]

    public partial class H_F_Planet_Service : View
    {
        private readonly ILoggingService loggingService;

        public H_F_Planet_Service()
        {
            this.InitializeComponent();
            this.loggingService = ApplicationService.GetService<ILoggingService>();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@HandMenu.Text1", "@HandMenu.Text2", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.4 Modul 2 Beschichtung.10 Zentrifuge.5 Planet.DB BS Planet HMI.PC.Drive.Vorwahl Referenz", 1);
                ILanguageService textService = ApplicationService.GetService<ILanguageService>();

                string txt = textService.GetText("@Logging.Service.Text7");
                this.loggingService.Log("Service", "NewReference", txt, FastDateTime.Now);
            }
        }

        private void Key_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@HandMenu.Text6", "@HandMenu.Text2", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.4 Modul 2 Beschichtung.10 Zentrifuge.5 Planet.DB BS Planet HMI.PC.Generel.Refernez löschen", true);
                ILanguageService textService = ApplicationService.GetService<ILanguageService>();

                string txt = textService.GetText("@Logging.Service.Text24");
                this.loggingService.Log("Service", "NewReference", txt, FastDateTime.Now);
            }
        }
    }
}