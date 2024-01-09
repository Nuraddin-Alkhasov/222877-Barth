using System.ComponentModel.Composition;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("Handmenu_Feeding_Belts")]
 
    public partial class Handmenu_Feeding_Belts : View
    {
        public Handmenu_Feeding_Belts()
        {
            this.InitializeComponent();
        }
    }
}