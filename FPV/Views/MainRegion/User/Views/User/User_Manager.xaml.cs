using HMI.Views.DialogRegion;
using System.Windows;
using VisiWin.ApplicationFramework;

namespace HMI.User
{
	/// <summary>
	/// Interaction logic for UserManager.xaml
	/// </summary>
	[ExportView("User_Manager")]
	public partial class User_Manager : VisiWin.Controls.View
	{
		public User_Manager()
		{
			this.InitializeComponent();
		}
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;

        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogView.Show("User_EKS","EKS", DialogButton.OK, DialogResult.OK);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogView.Show("User_Barcode", "Barcode", DialogButton.OK, DialogResult.OK);
        }
    }
}