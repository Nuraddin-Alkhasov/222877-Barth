using System;
using System.Threading.Tasks;
using VisiWin.ApplicationFramework;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.Recipe.Custom_Objects
{
    public class AutoRecipeLoad
    {

        IRecipeClass MRC;
        IRecipeClass BSC;
        string MR;
      

        public AutoRecipeLoad(string _MR)
        {
            MRC = ApplicationService.GetService<IRecipeService>().GetRecipeClass("MachineRecipe");
            MR = _MR;
        }

        public async void LoadStackAsync()
        {
            if ((await MRC.LoadFromFileToBufferAsync(MR)).Result == LoadRecipeResult.Succeeded)
            {
                ApplicationService.SetVariableValue("Recipe.MachineRecipe.Name", MR);
                ApplicationService.SetVariableValue("Recipe.MachineRecipe.Description", MRC.GetRecipeFile(MR).Description);

                await LoadFromFIleTOBufferArticleAsync();
                await LoadFromFileTOBufferC1Async();
                await LoadFromFileToBufferCoatingSteps();
            }
        }

        private async Task LoadFromFIleTOBufferArticleAsync()
        {
            object Article_N = "" ;
            MRC.GetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Main Recipe.Machine Recipe", out Article_N);
            if (Article_N.ToString() != "")
            {
                IRecipeClass T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("Article");
                if ((await T.LoadFromFileToBufferAsync(Article_N.ToString())).Result == LoadRecipeResult.Succeeded)
                {
                    ApplicationService.SetVariableValue("Recipe.Article.Name", Article_N.ToString());
                    ApplicationService.SetVariableValue("Recipe.Article.Description", T.GetRecipeFile(Article_N.ToString()).Description);
                }
            }
        }

        private async Task LoadFromFileTOBufferC1Async()
        {
            object C1_N = "";
            MRC.GetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Main Recipe.C1", out C1_N);
            if (C1_N.ToString() != "")
            {
                BSC = ApplicationService.GetService<IRecipeService>().GetRecipeClass("CoatingSteps");
                if ((await BSC.LoadFromFileToBufferAsync(C1_N.ToString())).Result == LoadRecipeResult.Succeeded)
                {
                    ApplicationService.SetVariableValue("Recipe.CoatingSteps.Name", C1_N.ToString());
                    ApplicationService.SetVariableValue("Recipe.CoatingSteps.Description", BSC.GetRecipeFile(C1_N.ToString()).Description);
                }
                
            }
        }

        private async Task LoadFromFileToBufferCoatingSteps()
        {
            string[] RN = CoatingProgramLoader();
            await LoadFromFIleTOPRocessloadCS_TAsync(RN[0],"D");
            await LoadFromFIleTOPRocessloadCS_TAsync(RN[1], "S");
            await LoadFromFIleTOPRocessloadCS_TAsync(RN[2], "R");
        }

        private async Task LoadFromFIleTOPRocessloadCS_TAsync(string RN, string Class)
        {
            IRecipeClass T = ApplicationService.GetService<IRecipeService>().GetRecipeClass(Class);
            if (RN != "")
            {
                RecipeErrorParam [] a = (await T.LoadFromFileToBufferAsync(RN)).ErrorParams;
                ApplicationService.SetVariableValue("Recipe." + Class + ".Name", RN);
                ApplicationService.SetVariableValue("Recipe." + Class + ".Description", T.GetRecipeFile(RN).Description);
            }
            else
            {
                RecipeErrorParam[] a = (await T.SetDefaultValuesToBufferAsync()).ErrorParams;
                ApplicationService.SetVariableValue("Recipe." + Class + ".Name", "");
                ApplicationService.SetVariableValue("Recipe." + Class + ".Description", "");
            }
        }

        private string[] CoatingProgramLoader()
        {
            string[] ret_val = { "","",""};

            object[] values;

            BSC.GetValues(BSC.GetVariableNames().ToArray(), out values);

            for (int i = 1; i <= 16;)
            {


                if (values[i].ToString() != "")
                {
                    switch (Convert.ToInt16(values[i + 1]))
                    {
                        case 1:
                            if (ret_val[0] == "")
                            {
                                ret_val[0] = values[i].ToString();
                            }

                            break;
                        case 2:
                            if (ret_val[1] == "")
                            {
                                ret_val[1] = values[i].ToString();
                            }
                            break;

                        case 3:
                            if (ret_val[2] == "")
                            {
                                ret_val[2] = values[i].ToString();
                            }
                            break;
                        default: break;
                    }

                }
                else { break; }

                i = i + 2;
            }
            return ret_val;
        }

    }
}
