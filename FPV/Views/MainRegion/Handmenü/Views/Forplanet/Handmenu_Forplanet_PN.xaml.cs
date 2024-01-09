using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Handmenu
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("Handmenu_Forplanet_PN")]
    public partial class Handmenu_Forplanet_PN : View
    {
		
        public Handmenu_Forplanet_PN()
        {
            this.InitializeComponent();
            
        }

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible && pn_forplanet.SelectedPanoramaRegionIndex != 0)
            {
                pn_forplanet.NavigateToStart();
            }
        }
    }
}