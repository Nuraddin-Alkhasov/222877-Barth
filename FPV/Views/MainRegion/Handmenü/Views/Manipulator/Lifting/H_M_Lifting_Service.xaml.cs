using HMI.Views.MessageBoxRegion;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.Helper;
using VisiWin.Language;
using VisiWin.Logging;

namespace HMI.Handmenu.Manipulator
{
    /// <summary>
    /// Interaction logic for KeyAndSwitchView.xaml
    /// </summary>
    [ExportView("H_M_Lifting_Service")]

    public partial class H_M_Lifting_Service : View
    {
        private readonly ILoggingService loggingService;

        public H_M_Lifting_Service()
        {
            this.InitializeComponent();
            this.loggingService = ApplicationService.GetService<ILoggingService>();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
           
            if (MessageBoxView.Show("@HandMenu.Text1", "@HandMenu.Text2", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                ILanguageService textService = ApplicationService.GetService<ILanguageService>();

                string txt = textService.GetText("@Logging.Service.Text1");
                 
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.08 Manipulator.01 Mani Hub.DB Mani Hub HMI.PC.Drive.Vorwahl Referenz", 1);
                this.loggingService.Log("Service", "NewReference", txt, FastDateTime.Now);
            }
        }
    }
}