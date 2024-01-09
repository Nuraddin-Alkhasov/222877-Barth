using System.ComponentModel.Composition;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("Handmenu_Feeding_PN")]
    public partial class Handmenu_Feeding_PN : View
    {
		
        public Handmenu_Feeding_PN()
        {
            this.InitializeComponent();
            
        }

        private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible && pn_Handmenu_Feeding.SelectedPanoramaRegionIndex != 0)
            {
                pn_Handmenu_Feeding.NavigateToStart();
            } 
        }
    }
}