using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Handmenu.Forplanet
{
    /// <summary>
    /// Interaction logic for KeyAndSwitchView.xaml
    /// </summary>
    [ExportView("H_F_Rotor_PN")]

    public partial class H_F_Rotor_PN : View
    {

        public H_F_Rotor_PN()
        {
            this.InitializeComponent();
	
 		}

        private void service_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            pn_rotor.ScrollNext();
        }

        private void pn_rotor_SelectedPanoramaRegionChanged(object sender, SelectedPanoramaRegionChangedEventArgs e)
        {
            if (pn_rotor.SelectedPanoramaRegionIndex == 0)
            {
                service.Visibility = System.Windows.Visibility.Visible;
                handmenu.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                service.Visibility = System.Windows.Visibility.Hidden;
                handmenu.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void handmenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            pn_rotor.ScrollPrevious();
        }

    }
}