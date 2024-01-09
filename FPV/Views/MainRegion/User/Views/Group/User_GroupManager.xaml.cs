using System.Windows;
using VisiWin.ApplicationFramework;

namespace HMI.User
{
	/// <summary>
	/// Interaction logic for GroupManager.xaml
	/// </summary>
	[ExportView("User_GroupManager")]
	public partial class User_GroupManager : VisiWin.Controls.View
	{
		public User_GroupManager()
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
    }
}