using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Parameter
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("Parameter_Oven_PN")]
    public partial class Parameter_Oven_PN : View
    {
		
        public Parameter_Oven_PN()
        {
            this.InitializeComponent();
            
        }

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible && pn_parameter_ofen.SelectedPanoramaRegionIndex != 0)
            {
                pn_parameter_ofen.NavigateToStart();
            }
        }
    }
}