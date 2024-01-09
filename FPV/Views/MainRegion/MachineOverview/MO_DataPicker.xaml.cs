using HMI.Module;
using HMI.Views.MessageBoxRegion;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.MachineOverview
{

	[ExportView("MO_DataPicker")]
	public partial class MO_DataPicker : VisiWin.Controls.View
	{
        public static string AN ="";
        public static string PON = "";
        public static string MR = "";
        public static string USER = "";
        public static bool SmallCharge = false;
        int scanCounter=0;
        IVariableService VS;
        IVariable newDataV;

        public MO_DataPicker()
		{
			this.InitializeComponent();
           
        }

        private void NewDataV_Change(object sender, VariableEventArgs e)
        {
            if (this.IsVisible && e.Value.ToString() != "")
            {
                if (ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString() != "")
                {
                    switch (scanCounter)
                    {
                        case 0:
                            pon.Value = e.Value.ToString();
                            scanCounter++;
                            user.Value = ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString();
                            break;
                        case 1:
                            an.Value = e.Value.ToString();
                            scanCounter--;
                            break;
                    }
                }
                else
                {
                    new MessageBoxTask("@Datenauswahl.Text13", "@Datenauswahl.Text15", MessageBoxIcon.Warning);
                }

                ApplicationService.SetVariableValue("DataPicker.DatafromScanner","");

            }
        }

        private void _IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                pon.Value = ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Header.Produktionsnummer#STRING40").ToString();
                an.Value = ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Header.Artikelmummer#STRING40").ToString();
                mr.Value = ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Header.Maschinenprogramm#STRING40").ToString();
                user.Value = ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Header.User#STRING40").ToString();
                sChargen.IsChecked = (bool)ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Status.Kleinst Mengen");
            }
            else
            {
                AN = an.Value;
                PON = pon.Value;
                MR = mr.Value;
                USER = user.Value;
                SmallCharge = (bool)sChargen.IsChecked;
            }
        }

        private void Mr_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            ApplicationService.SetView("MessageBoxRegion", "Recipe_Selector");   
        }

        private void Mr_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
          ApplicationService.SetView("MessageBoxRegion", "Recipe_Selector");
        }

        public void UpdateDatapickerDataSelection()
        {
            mr.Value = ApplicationService.GetVariableValue("Recipe.Article.Name").ToString();
            rd.Value = ApplicationService.GetVariableValue("Recipe.Article.Description").ToString();
            user.Value = ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString();
        }

        private void LoadMR(string a)
        {
            Task obTask = Task.Run(() =>
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    DataTable temp = (new localDBAdapter("SELECT * FROM BarcodeToMR WHERE Barcode ='" + a + "' ; ")).DB_Output();
                    if (temp.Rows.Count == 0)
                    {
                        new MessageBoxTask("@Datenauswahl.Text16", "@Datenauswahl.Text15", MessageBoxIcon.Warning);
                    }
                    else
                    {
                        mr.Value= temp.Rows[0][2].ToString();
                        user.Value = ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString();
                    }
                });
            });
        }

        private void Pon_Loaded(object sender, RoutedEventArgs e)
        {
            VS = ApplicationService.GetService<IVariableService>();
            newDataV = VS.GetVariable("DataPicker.DatafromScanner");
            newDataV.Change += NewDataV_Change;
        }
        private void pon_Unloaded(object sender, RoutedEventArgs e)
        {
            newDataV.Detach();
            newDataV = null;
            scanCounter = 0;


        }
        private void An_ValueChanged(object sender, VariableEventArgs e)
        {
            if(e.Value.ToString()!= "" && newDataV.Value.ToString()!="")
            LoadMR(newDataV.Value.ToString());
        }

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MessageBoxRegion", "MO_Status_Recipe", mr.Value);
        }

        private void Mr_ValueChanged(object sender, VariableEventArgs e)
        {
            IRecipeClass T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("MachineRecipe");
            if (T.IsExistingRecipeFile(mr.Value))
            {
                rd.Value = T.GetRecipeFile(mr.Value).Description;
            }
            else { rd.Value = ""; }

        }

       
    }
}