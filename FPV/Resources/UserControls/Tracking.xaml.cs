using HMI.Views.MainRegion.MachineOverview;
using System.Windows.Controls;
using VisiWin.ApplicationFramework;

namespace HMI.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class Tracking : UserControl

    {
        public Tracking()
        {
            InitializeComponent();
        }

        private void UserControl_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                if(this.Tag!=null)
                    pon.Text = ((Track)this.Tag).pon;
            }
        }       
    }
}
