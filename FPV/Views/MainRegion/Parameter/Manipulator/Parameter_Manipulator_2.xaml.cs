using HMI.Views.Parameter.Manipulator;
using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Parameter.Manipulator
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("Parameter_Manipulator_2")]
 
    public partial class Parameter_Manipulator_2 : View
    {
        public Parameter_Manipulator_2()
        {
            this.InitializeComponent();
        }
        private void btnMH_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            maniReg.Content = new P_M_Drehen();
        }


        private void btnMS_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            maniReg.Content = new P_M_BDrehen();
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