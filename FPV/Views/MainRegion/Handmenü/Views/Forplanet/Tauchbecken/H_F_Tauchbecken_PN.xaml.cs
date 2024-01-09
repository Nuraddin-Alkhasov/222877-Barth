using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Handmenu.Forplanet
{
    /// <summary>
    /// Interaction logic for KeyAndSwitchView.xaml
    /// </summary>
    [ExportView("H_F_Tauchbecken_PN")]
    public partial class H_F_Tauchbecken_PN : View
    {
	//	protected Point TouchStart;
      //  protected int CurrentViewID;

        public H_F_Tauchbecken_PN()
        {
            this.InitializeComponent();
 		}
        private void service_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            pn_Tauchbecken.ScrollNext();
        }

        private void pn_Tauchbecken_SelectedPanoramaRegionChanged(object sender, SelectedPanoramaRegionChangedEventArgs e)
        {
            if (pn_Tauchbecken.SelectedPanoramaRegionIndex == 0)
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
            pn_Tauchbecken.ScrollPrevious();
        }


    }
}