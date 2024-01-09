using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Handmenu.Drayer
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("H_D_KAS_PN")]
    public partial class H_D_KAS_PN : View
    {
		
        public H_D_KAS_PN()
        {
            this.InitializeComponent();
            
        }

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible && pn_H_D_KAS.SelectedPanoramaRegionIndex != 0)
            {
                pn_H_D_KAS.NavigateToStart();
            }
        }
    }
}