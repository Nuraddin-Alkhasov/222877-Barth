using HMI.Interfaces;
using VisiWin.ApplicationFramework;

namespace HMI.User
{
	/// <summary>
	/// Interaction logic for UserAdd.xaml
	/// </summary>
    [ExportView("User_EKS")]
	public partial class User_EKS : VisiWin.Controls.View, IView
	{
        IEKS EKSService = ApplicationService.GetService<IEKS>();
        public User_EKS()
		{
			this.InitializeComponent();
       
        }

        private void OpenConnection_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EKSService.OpenConnection();
            
        }

        private void CloseConnection_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EKSService.CloseConnection();
           
        }

        private void CheckConnection_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            status.Value = EKSService.CheckConnection();
        }

        private void ReadData_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            data.Value = EKSService.Read();
           
        }

        private void WriteData_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            EKSService.Write(data.Value.ToString());
           
        }

        private void UpdateStatus_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            status.Value = EKSService.GetStatus();
        }
    }
}