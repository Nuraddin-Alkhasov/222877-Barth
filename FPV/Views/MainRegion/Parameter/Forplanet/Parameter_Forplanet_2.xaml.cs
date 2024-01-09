using HMI.Views.Parameter.Forplanet;
using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Parameter.Forplanet
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("Parameter_Forplanet_2")]
 
    public partial class Parameter_Forplanet_2 : View
    {
        public Parameter_Forplanet_2()
        {
            this.InitializeComponent();
        }

        private void btnLTBB_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            LTBregion.Content = new P_F_BLTB();
        }

        private void btnLTBAP_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            LTBregion.Content = new P_F_LTB();
        }

        private void BtnLTBAP_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task obTask = Task.Run(() =>
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    btnLTBAP.IsChecked=true;
                });
            });
        }
    }
}