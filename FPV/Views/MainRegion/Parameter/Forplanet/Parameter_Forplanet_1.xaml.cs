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
    [ExportView("Parameter_Forplanet_1")]
 
    public partial class Parameter_Forplanet_1 : View
    {
        public Parameter_Forplanet_1()
        {
            this.InitializeComponent();
        }

        private void Ventilator1_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Task obTask = Task.Run(() =>
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    Ventilator1.Content = new P_F_Abluftvent();
                });
            });
             
        }

        private void Ventilator2_Loaded(object sender, RoutedEventArgs e)
        {
            Task obTask = Task.Run(() =>
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    Ventilator2.Content = new P_F_Zuluftvent();
                });
            });
        }
    }
}