using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Parameter.Forplanet
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("Parameter_Forplanet_PN")]
    public partial class Parameter_Forplanet_PN : View
    {
		
        public Parameter_Forplanet_PN()
        {
            this.InitializeComponent();
            
        }

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible && pn_parameter_forplanet.SelectedPanoramaRegionIndex != 0)
            {
                pn_parameter_forplanet.NavigateToStart();
            }
        }
    }
}