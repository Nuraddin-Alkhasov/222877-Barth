using System.ComponentModel.Composition;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("Handmenu_Feeding_Tilt")]
 
    public partial class Handmenu_Feeding_Tilt : View
    {
        public Handmenu_Feeding_Tilt()
        {
            this.InitializeComponent();
        }
    }
}