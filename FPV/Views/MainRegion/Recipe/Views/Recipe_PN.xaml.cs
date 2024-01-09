using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Views.MainRegion.Recipe
{

    [ExportView("Recipe_PN")]
    public partial class Recipe_PN : VisiWin.Controls.View
    {
        bool pr_initialized = false;
        public Recipe_PN()
        {
            this.InitializeComponent();

        }

        private void pn_recipe_SelectedPanoramaRegionChanged(object sender, SelectedPanoramaRegionChangedEventArgs e)
        {
            IRegionService iRS = ApplicationService.GetService<IRegionService>();
            switch (pn_recipe.SelectedPanoramaRegionIndex)
            {
                case 0:
                    HeaderTxt.LocalizableText = "@Rezeptverwaltung.Text5";
                    Rname.VariableName = "Recipe.Article.Name";
                    Descr.VariableName = "Recipe.Article.Description";
                    break;
                case 1:
                    HeaderTxt.LocalizableText = "@Rezeptverwaltung.Text3";
                    Rname.VariableName = "Recipe.MachineRecipe.Name";
                    Descr.VariableName = "Recipe.MachineRecipe.Description";
                    
                    Recipe_Machine MR = (Recipe_Machine)iRS.GetView("Recipe_Machine");
                    MR.dgvUpdate();
                    if (!pr_initialized)
                    {
                        Task obTask = Task.Run(() =>
                        {
                            Application.Current.Dispatcher.InvokeAsync((Action)delegate
                            {
                                pn_recipe.PanoramaRegions[0].Content = new Recipe_Article();
                            });
                        });
                        Task obTask2 = Task.Run(() =>
                        {
                            Application.Current.Dispatcher.InvokeAsync((Action)delegate
                            {
                                pn_recipe.PanoramaRegions[2].Content = new Recipe_Coating_PN();
                            });
                        });

                        
                        pr_initialized = true;
                    }
                   
                    break;
                case 2:
                    if (((Recipe_Coating_PN)pn_recipe.PanoramaRegions[2].Content).pn_coating_BS.SelectedPanoramaRegionIndex != 0)
                    {
                        ((Recipe_Coating_PN)pn_recipe.PanoramaRegions[2].Content).pn_coating_BS.ScrollPrevious(true);
                    }
                    HeaderTxt.LocalizableText = "@Rezeptverwaltung.Text4";
                    Rname.VariableName = "Recipe.CoatingSteps.Name";
                    Descr.VariableName = "Recipe.CoatingSteps.Description";
                    break;

            }
          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (pn_recipe.SelectedPanoramaRegionIndex)
            {
                case 0:
                    ApplicationService.SetView("DialogRegion", "Recipe_Browser", 0);
                    break;
                case 1:
                    ApplicationService.SetView("DialogRegion", "Recipe_Browser", 6);
                    break;
                case 2:
                    IRegionService iRS = ApplicationService.GetService<IRegionService>();
                    if (((Recipe_Coating_PN)pn_recipe.PanoramaRegions[2].Content).pn_coating_BS.SelectedPanoramaRegionIndex == 1)
                    { 
                        Recipe_Coating_Steps R_CS = (Recipe_Coating_Steps)iRS.GetView("Recipe_Coating_Steps");
                        switch (R_CS.selectedR)
                        {
                            case 2: ApplicationService.SetView("DialogRegion", "Recipe_Browser", 2); break;
                            case 3: ApplicationService.SetView("DialogRegion", "Recipe_Browser", 3); break;
                            case 4: ApplicationService.SetView("DialogRegion", "Recipe_Browser", 4); break;
                        }
                    }
                    else
                    { 
                        ApplicationService.SetView("DialogRegion", "Recipe_Browser", 5);
                    }
                    break;
            }
        }

    }
}