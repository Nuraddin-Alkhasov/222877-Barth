using HMI.Views.Parameter.HVT;
using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Parameter.HVT
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("Parameter_HVT")]
 
    public partial class Parameter_HVT : View
    {
        public Parameter_HVT()
        {
            this.InitializeComponent();
        }

        private void btnMH_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            maniReg.Content = new P_HVT_P_PS();
        }

        private void btnMD_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            maniReg.Content = new P_HVT_B_PS();
        }

        private void BtnMH_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task obTask = Task.Run(() =>
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    btnMH.IsChecked = true;
                });
            });
        }
    }
}