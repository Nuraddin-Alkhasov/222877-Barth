using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.DataAccess;

namespace HMI.Handemnu.Forplanet
{
    /// <summary>
    /// Interaction logic for KeyAndSwitchView.xaml
    /// </summary>
    [ExportView("Handmenu_Feeding_BRS_PN")]

    public partial class Handmenu_Feeding_BRS_PN : View
    {
        protected TouchPoint TouchStart;

        public Handmenu_Feeding_BRS_PN()
        {
            this.InitializeComponent();
	
 		}

        private void service_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IVariableService VS = ApplicationService.GetService<IVariableService>();
            pn_brs.ScrollNext();
        }

        private void pn_planet_SelectedPanoramaRegionChanged(object sender, SelectedPanoramaRegionChangedEventArgs e)
        {
            if (pn_brs.SelectedPanoramaRegionIndex == 0)
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
            pn_brs.ScrollPrevious();
        }

    }
}