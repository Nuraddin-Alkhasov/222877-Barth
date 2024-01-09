using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.Recipe
{
	/// <summary>
	/// Interaction logic for View1.xaml
	/// </summary>
	[ExportView("Recipe_Coating_PN")]
	public partial class Recipe_Coating_PN : VisiWin.Controls.View
	{
        public Recipe_Coating_PN()
		{
			this.InitializeComponent();
        }

        private void pn_C_SelectedPanoramaRegionChanged(object sender, VisiWin.Controls.SelectedPanoramaRegionChangedEventArgs e)
        {
            IRegionService iRS = ApplicationService.GetService<IRegionService>();
            Recipe_PN R_PN = (Recipe_PN)iRS.GetView("Recipe_PN");
            if (R_PN.pn_recipe.SelectedPanoramaRegionIndex == 2)
            {
                ((Recipe_Coating_PR)pn_coating_BS.PanoramaRegions[0].Content).dgvUpdate();
                if (pn_coating_BS.SelectedPanoramaRegionIndex == 0)
                {
              
                        R_PN.Rname.VariableName = "Recipe.CoatingSteps.Name";
                        R_PN.Descr.VariableName = "Recipe.CoatingSteps.Description";
                        R_PN.HeaderTxt.LocalizableText = "@Rezeptverwaltung.Text4";
                }
                else
                {
                    R_PN.HeaderTxt.LocalizableText = "@Rezeptverwaltung.Text19";
                    Recipe_Coating_Steps R_CS = (Recipe_Coating_Steps)iRS.GetView("Recipe_Coating_Steps");
                        switch (R_CS.selectedR)
                        {
                        case 0:
                            R_PN.Rname.VariableName = "";
                            R_PN.Rname.Value = "";
                            R_PN.Descr.VariableName = "";
                            R_PN.Descr.Value = "";
                            
                            break;
                        case 2:
                            R_PN.Rname.VariableName = "Recipe.D.Name";
                            R_PN.Descr.VariableName = "Recipe.D.Description"; break;
                            case 3:
                            R_PN.Rname.VariableName = "Recipe.S.Name";
                            R_PN.Descr.VariableName = "Recipe.S.Description"; break;
                            case 4:
                            R_PN.Rname.VariableName = "Recipe.R.Name";
                            R_PN.Descr.VariableName = "Recipe.R.Description"; break;
                        }

                    R_CS.weight.Value = 0;
                    Task obTask = Task.Run(() =>
                    {
                        Application.Current.Dispatcher.InvokeAsync((Action)delegate
                        {

                            IRecipeClass T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("Article");
                            object wjk = 0;
                            T.GetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp MR.Feeding.Korbdrehen.Befüllgewicht je Korb", out wjk);
                            if (wjk != null)
                            {
                                R_CS.weight.Value = Convert.ToInt32(wjk);
                            }
                        });
                    });
                }
            }
        }

    }
}