using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Handmenu.Manipulator
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("H_M_Lifting_PN")]
    public partial class H_M_Lifting_PN : View
    {
		
        public H_M_Lifting_PN()
        {
            this.InitializeComponent();
            
        }

        private void service_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            pn_lift.ScrollNext();
        }

        private void pn_lift_SelectedPanoramaRegionChanged(object sender, SelectedPanoramaRegionChangedEventArgs e)
        {
            if (pn_lift.SelectedPanoramaRegionIndex == 0)
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
            pn_lift.ScrollPrevious();
        }


    }
}