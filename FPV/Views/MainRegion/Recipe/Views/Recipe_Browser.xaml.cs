using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;


namespace HMI.Views.MainRegion.Recipe
{

    [ExportView("Recipe_Browser")]
    public partial class Recipe_Browser : VisiWin.Controls.View
    {

        public Recipe_Browser()
        {
            this.InitializeComponent();
        }

        private void CloseDialog(object sender, RoutedEventArgs e)
        {

            ApplicationService.SetView("DialogRegion", "EmptyView");
        }

        private void LayoutRoot_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                int classID = Convert.ToInt16(ApplicationService.ObjectStore.GetValue("Recipe_Browser_KEY"));
                this.DataContext = new RecipeBrowserAdapter(classID);
                switch (classID)
                {
                    case 0: btnSave.AuthorizationRight = "Article";
                            btnDelete.AuthorizationRight = "Article"; break;
                    case 2:
                    case 3:
                    case 4:
                    case 5: btnSave.AuthorizationRight = "CoatingProgramm";
                            btnDelete.AuthorizationRight = "CoatingProgramm"; break;
                    case 6: btnSave.AuthorizationRight = "MachineRecipe";
                            btnDelete.AuthorizationRight = "MachineRecipe"; break;
                }
                
            }
        }
        private void _PreviewTouchDown(object sender, TouchEventArgs e)
        {
            RBdgv_recipe.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
        }

    }
}