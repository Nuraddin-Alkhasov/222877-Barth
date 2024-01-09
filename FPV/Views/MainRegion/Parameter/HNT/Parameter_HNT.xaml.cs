using HMI.Views.Parameter.HNT;
using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Parameter.HNT
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("Parameter_HNT")]
 
    public partial class Parameter_HNT : View
    {
        public Parameter_HNT()
        {
            this.InitializeComponent();
        }

        private void btnMH_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            maniReg.Content = new P_HNT_P_PS();
        }

        private void btnMD_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            maniReg.Content = new P_HNT_B_PS();
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