using System.ComponentModel.Composition;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("Handmenu_Feeding_BRS")]
 
    public partial class Handmenu_Feeding_BRS : View
    {
        public Handmenu_Feeding_BRS()
        {
            this.InitializeComponent();
        }
    }
}