using System.ComponentModel.Composition;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("Handmenu_Feeding_Lift")]
 
    public partial class Handmenu_Feeding_Lift : View
    {
        public Handmenu_Feeding_Lift()
        {
            this.InitializeComponent();
        }
    }
}