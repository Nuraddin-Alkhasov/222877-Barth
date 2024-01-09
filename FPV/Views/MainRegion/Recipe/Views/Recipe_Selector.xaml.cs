using HMI.Views.DialogRegion;
using HMI.Views.MainRegion.MachineOverview;
using System.Windows;
using System.Windows.Controls;
using VisiWin.ApplicationFramework;


namespace HMI.Views.MainRegion.Recipe
{

    [ExportView("Recipe_Selector")]
    public partial class Recipe_Selector : VisiWin.Controls.View
    {

        public Recipe_Selector()
        {
            this.InitializeComponent();
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetVariableValue("Recipe.Article.Name", tbName.Value.ToString());
            ApplicationService.SetVariableValue("Recipe.Article.Description", tbDescription.Value.ToString());
            IRegionService iRS = ApplicationService.GetService<IRegionService>();
            if (iRS.GetCurrentViewName("DialogRegion") == "MO_DataOverwrite")
            {
                MO_DO_Data MODOData = (MO_DO_Data)iRS.GetView("MO_DO_Data");
                MODOData.UpdateDatapickerDataSelection();
            }
            if (iRS.GetCurrentViewName("DialogRegion") == "DialogView")
            {
                MO_DataPicker MODataPicker = (MO_DataPicker)iRS.GetView("MO_DataPicker");
                MODataPicker.UpdateDatapickerDataSelection();
            }

            if (iRS.GetCurrentViewName("DialogRegion") == "Recipe_Binding")
            {
                Recipe_Binding RecipeBinding = (Recipe_Binding)iRS.GetView("Recipe_Binding");
                RecipeBinding.SetData(tbName.Value.ToString(), tbDescription.Value.ToString());
            }


            RecipeSelectorAdapter RSA = (RecipeSelectorAdapter)DataContext;
            RSA.LoadFromFileToBuffer();

            ApplicationService.SetView("MessageBoxRegion", "EmptyView");
        }

        private void CancelButton_Click_1(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MessageBoxRegion", "EmptyView");
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MessageBoxRegion", "EmptyView");
        }

        private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                this.DataContext = new RecipeSelectorAdapter();
            }
        }

        private void DataGridRow_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            RSdgv_recipe.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
        }

    }
}