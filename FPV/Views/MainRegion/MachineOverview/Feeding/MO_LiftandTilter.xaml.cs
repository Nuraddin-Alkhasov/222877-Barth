using HMI.Views.DialogRegion;
using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_LiftandTilter")]
    public partial class MO_LiftandTilter : VisiWin.Controls.View
    {

        public MO_LiftandTilter()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string[] m = (((Button)sender).Tag.ToString()).Split('*');
            SP temp = new SP(m[0], m[1], m[2], m[3]);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DialogView.Show("MO_Status_KBD", "Korb beladen Drehstation", DialogButton.Close, DialogResult.Cancel);
        }

        private void Key_Click(object sender, RoutedEventArgs e)
        {
            if (gew.Value >= 5 && kba.Value >= 1 && kba.Value <= 10)
            {
                Task obTask = Task.Run(() =>
                {
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.04 Befüllen.01 Bunkerband.DB BE BB HMI.PC.Gerneral.Freigabe KM", true);
                    System.Threading.Thread.Sleep(300);
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.04 Befüllen.01 Bunkerband.DB BE BB HMI.PC.Gerneral.Freigabe KM", false);
                });
            }
        }

        private void Gew_ValueChanged(object sender, VisiWin.DataAccess.VariableEventArgs e)
        {
            if (gew.Value > 0 && kba.Value > 0)
            {
                CharW.Value = gew.Value / kba.Value;
                BasketW.Value = gew.Value / kba.Value / 2;
            }
            else
            {
                CharW.Value = 0;
                BasketW.Value = 0;
            }
               
            if (BasketW.Value > 0 && BasketW.Value < 50)
            {
                FG.IsEnabled = true;
            }
            else
            {
                FG.IsEnabled = false;
            }
        }
    }
}



