using HMI.Views.MainRegion.Recipe.Custom_Objects;
using System.ComponentModel;
using System.Windows.Controls;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;


namespace HMI.Views.MainRegion.Recipe
{

	[ExportView("Recipe_Binding")]
	public partial class Recipe_Binding : VisiWin.Controls.View
	{

        IVariableService VS;
        IVariable newDataV;
        public Recipe_Binding()
		{
			this.InitializeComponent();
         
        }

        private void NewDataV_Change(object sender, VariableEventArgs e)
        {
            if (this.IsVisible && newDataV.Value.ToString() != "")
            {
                if (dataedit.IsVisible)
                {
                    barcode.Value = e.Value.ToString();
                }
                else
                {
                    dgv_bctor.ScrollIntoView(GetItem(e.Value.ToString()));
                }
                ApplicationService.SetVariableValue("DataPicker.DatafromScanner", "");
            }
         
        }


        private BCToMR GetItem(string a)
        {
            for (int i=0;i< dgv_bctor.Items.Count; i++)
            {
                if (((BCToMR)dgv_bctor.Items[i]).Barcode == a)
                {
                    return (BCToMR)dgv_bctor.Items[i];
                }
            }
            return null;
        }

        private void DataGridRow_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            dgv_bctor.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
        }

        private void TextVarOut_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ApplicationService.SetView("MessageBoxRegion", "Recipe_Selector");
        }

        public void SetData(string a, string b)
        {
            mr.Value = a;
        }

        private void Dataedit_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            VS = ApplicationService.GetService<IVariableService>();
            newDataV = VS.GetVariable("DataPicker.DatafromScanner");
            newDataV.Change += NewDataV_Change;
            
        }

        private void dataedit_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            newDataV.Detach();
            newDataV = null;
        }

       

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            dgv_bctor.Items.SortDescriptions.Clear();
            dgvBarcode.SortDirection = System.ComponentModel.ListSortDirection.Ascending;
            dgv_bctor.Items.SortDescriptions.Add(new SortDescription("Barcode", ListSortDirection.Ascending));
        }
    }
}