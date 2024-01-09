using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Handemnu.Forplanet
{
    /// <summary>
    /// Interaction logic for KeyAndSwitchView.xaml
    /// </summary>
    [ExportView("H_F_Planet_PN")]

    public partial class H_F_Planet_PN : View
    {
        protected TouchPoint TouchStart;

        public H_F_Planet_PN()
        {
            this.InitializeComponent();
	
 		}

        private void service_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            pn_planet.ScrollNext();
        }

        private void pn_planet_SelectedPanoramaRegionChanged(object sender, SelectedPanoramaRegionChangedEventArgs e)
        {
            if (pn_planet.SelectedPanoramaRegionIndex == 0)
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
            pn_planet.ScrollPrevious();
        }

    }
}