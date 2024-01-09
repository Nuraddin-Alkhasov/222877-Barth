using System;
using HMI.Interfaces;
using HMI.Services;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.User
{
	/// <summary>
	/// Interaction logic for UserAdd.xaml
	/// </summary>
    [ExportView("User_Barcode")]
	public partial class User_Barcode : VisiWin.Controls.View, IView
	{
        IBarcode BarcodeService = ApplicationService.GetService<IBarcode>();
        IVariableService VS;
        IVariable newDataV;

        public User_Barcode()
		{
			this.InitializeComponent();
            
            
                VS = ApplicationService.GetService<IVariableService>();
                newDataV = VS.GetVariable("DataPicker.DatafromScanner");
                newDataV.Change += NewDataV_Change;
               
            }

        private void NewDataV_Change(object sender, VariableEventArgs e)
        {
            if (this.IsVisible && e.Value.ToString() != "")
            {
                status.Value = e.Value.ToString();
                ApplicationService.SetVariableValue("DataPicker.DatafromScanner", "");
            }
        }

        private void OpenConnection_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BarcodeService.OpenConnection();
            
        }

        private void CloseConnection_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            BarcodeService.CloseConnection();
        }

        private void CheckConnection_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            status.Value = BarcodeService.CheckConnection();
        }
    }
}