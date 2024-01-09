using System.ComponentModel.Composition;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Handemnu.Forplanet
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("H_F_Planet")]
 
    public partial class H_F_Planet : View
    {
        public H_F_Planet()
        {
            this.InitializeComponent();
        }
    }
}