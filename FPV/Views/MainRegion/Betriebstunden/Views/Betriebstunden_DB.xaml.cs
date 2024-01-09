using HMI.Views.DialogRegion;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Views.MainRegion.Betriebstunden
{
    /// <summary>
    /// Interaction logic for ButtonsView.xaml
    /// </summary>
    [ExportView("Betriebstunden_DB")]
 
    public partial class Betriebstunden_DB : View
    {
        public Betriebstunden_DB()
        {
            this.InitializeComponent();
        }

        private void NavigationButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogView.Show("BS_HK", "Betriebstunden", DialogButton.Close, DialogResult.Cancel);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogView.Show("BS_Mani", "Betriebstunden", DialogButton.Close, DialogResult.Cancel);
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogView.Show("BS_KAS", "Betriebstunden", DialogButton.Close, DialogResult.Cancel);
            
        }

        private void Button_Click_2(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogView.Show("BS_Forplanet", "Betriebstunden", DialogButton.Close, DialogResult.Cancel);
            
        }

        private void Button_Click_3(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogView.Show("BS_HVT", "Betriebstunden", DialogButton.Close, DialogResult.Cancel);
        }

        private void Button_Click_5(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogView.Show("BS_Ventilator", "Betriebstunden", DialogButton.Close, DialogResult.Cancel);
        }

        private void Button_Click_6(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogView.Show("BS_HNT", "Betriebstunden", DialogButton.Close, DialogResult.Cancel);
        }

        private void Button_Click_7(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogView.Show("BS_QT", "Betriebstunden", DialogButton.Close, DialogResult.Cancel);
            
        }
    }
}