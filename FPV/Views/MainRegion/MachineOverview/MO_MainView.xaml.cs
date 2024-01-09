using HMI.Views.DialogRegion;
using HMI.Views.MessageBoxRegion;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Helper;
using VisiWin.Language;
using VisiWin.Logging;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_MainView")]
    public partial class MO_MainView : VisiWin.Controls.View
    {
        IVariableService VS;
        IVariable status;
        private ILoggingService loggingService;
        ILanguageService textService;
        public MO_MainView()
        {
            InitializeComponent();
            VS = ApplicationService.GetService<IVariableService>();
            status = VS.GetVariable("PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Steuerspannung Status");
            status.Change += Status_Change;
            loggingService = ApplicationService.GetService<ILoggingService>();
            textService = ApplicationService.GetService<ILanguageService>();
        }

        private void Status_Change(object sender, VariableEventArgs e)
        {
            loggingService = ApplicationService.GetService<ILoggingService>();
            textService = ApplicationService.GetService<ILanguageService>();
            if ((short)e.Value == 2 || (short)e.Value == 3)
            {
                ONOFF.VariableName = "PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Steuerspannung Aus";
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Steuerspannung Ein", 0);
                string txt = textService.GetText("@Logging.Service.Text18");
                this.loggingService.Log("Service", "Anlage Ein/Aus", txt, FastDateTime.Now);
            }
            else
            {
                ONOFF.VariableName = "PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Steuerspannung Ein";
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Steuerspannung Aus", 0);
                string txt = textService.GetText("@Logging.Service.Text19");
                this.loggingService.Log("Service", "Anlage Ein/Aus", txt, FastDateTime.Now);
            }
            if ((short)e.Value == 3)
            {
                powerOFF.Visibility = Visibility.Visible;
            }
            else
            {
                powerOFF.Visibility = Visibility.Hidden;
            }
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            if (DialogView.Show("MO_DataPicker", "Datenauswahl", DialogButton.OKCancel, DialogResult.Cancel) == DialogResult.OK)
            {
                if ((bool)ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data to PC.MR loading enable") && !(bool)ApplicationService.GetVariableValue("Temp.RecipeIsLaoding"))
                {
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Header.Artikelmummer#STRING40", MO_DataPicker.AN);
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Header.Produktionsnummer#STRING40", MO_DataPicker.PON);
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Header.Maschinenprogramm#STRING40", MO_DataPicker.MR);
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Header.User#STRING40", MO_DataPicker.USER);
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Status.Kleinst Mengen", MO_DataPicker.SmallCharge);
                }
                else
                {
                    new MessageBoxTask("Kein freigabe daten zu laden.", "Kein freigabe.", MessageBoxIcon.Exclamation);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogView.Show("MO_Status", "Status", DialogButton.OKCancel, DialogResult.Cancel);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogView.Show("MO_AutoONOFF", "Zeitschaltuhr", DialogButton.Close, DialogResult.Cancel);
        }

        private void BtnMH_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Manuell Leerfahren", false);
        }

        private void BtnMD_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Automatisch Leerfahren", false);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DialogView.Show("MO_Status_KBD", "Korb beladen Drehstation", DialogButton.Close, DialogResult.Cancel);
        }

        
    }
}



