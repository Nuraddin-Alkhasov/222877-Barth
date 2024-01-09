using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Parameter.Feeding
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("Parameter_Feeding_PN")]
    public partial class Parameter_Feeding_PN : View
    {
		
        public Parameter_Feeding_PN()
        {
            this.InitializeComponent();
            
        }

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible && pn_parameter_feeding.SelectedPanoramaRegionIndex != 0)
            {
                pn_parameter_feeding.NavigateToStart();
            }
        }
    }
}