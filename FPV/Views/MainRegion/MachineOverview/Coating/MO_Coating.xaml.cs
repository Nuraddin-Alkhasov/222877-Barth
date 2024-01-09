using HMI.Views.DialogRegion;
using System;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;


namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_Coating")]
    public partial class MO_Coating : VisiWin.Controls.View
    {

        public MO_Coating()
        {
            InitializeComponent();
        }

        private void NumericVarOut_ValueChanged(object sender, VisiWin.DataAccess.VariableEventArgs e)
        {
            object lid = ApplicationService.GetVariableValue("PLC.PLC.Blocks.4 Modul 2 Beschichtung.05 Tauchbecken.DB LTB HMI.Actual value.Dipping Vat.LTB Lacktyp");
            if (Convert.ToInt32(lid) >= 1 && Convert.ToInt32(lid) <= 9)
            {
                paintTYP2.Value = ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.00 Allgemein.Lacktyp Namen.Lacktyp Name[" + lid.ToString() + "]").ToString();
            }
            else
            {
                paintTYP2.Value = "";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetVariableValue("Trend.ID", ((System.Windows.Controls.Button)sender).Tag);
            ApplicationService.SetView("MainRegion", "TrendView");
        }



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string[] m = (((Button)sender).Tag.ToString()).Split('*');
            SP temp = new SP(m[0], m[1], m[2], m[3]);
        }
    }
}



