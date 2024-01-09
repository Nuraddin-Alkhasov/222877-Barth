using HMI.Views.MessageBoxRegion;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Handemnu.Forplanet
{
    /// <summary>
    /// Interaction logic for KeyAndSwitchView.xaml
    /// </summary>
    [ExportView("Handmenu_Feeding_BRS_Service")]

    public partial class Handmenu_Feeding_BRS_Service : View
    {
        public Handmenu_Feeding_BRS_Service()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@HandMenu.Text1", "@HandMenu.Text2", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.4 Modul 2 Beschichtung.10 Zentrifuge.5 Planet.DB BS Planet HMI.PC.Drive.Vorwahl Referenz", 1);

            }
        }
    }
}