using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Handmenu
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("Handmenu_Manipulator_PN")]
    public partial class Handmenu_Manipulator_PN : View
    {
		
        public Handmenu_Manipulator_PN()
        {
            this.InitializeComponent();
            
        }

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible && pn_manipulator.SelectedPanoramaRegionIndex != 0)
            {
                pn_manipulator.NavigateToStart();
            }
        }
    }
}