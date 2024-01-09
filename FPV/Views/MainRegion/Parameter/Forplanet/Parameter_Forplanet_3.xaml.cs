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
    [ExportView("Parameter_Forplanet_3")]
 
    public partial class Parameter_Forplanet_3 : View
    {
        public Parameter_Forplanet_3()
        {
            this.InitializeComponent();
        }

        private void btnLT_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            LTBregion.Content = new P_F_LT();
        }

        private void btnLTZ0_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            LTBregion.Content = new P_F_LT_0();
        }

        private void btnLTZ50_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            LTBregion.Content = new P_F_LT_50();
        }

        private void BtnLT_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task obTask = Task.Run(() =>
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    btnLT.IsChecked = true;
                });
            });
        }
    }
}