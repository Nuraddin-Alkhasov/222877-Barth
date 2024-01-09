using System.Collections.ObjectModel;
using System.Windows;
using HMI.Components;
using HMI.Views.DialogRegion;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Recipe;
using System.Threading.Tasks;
using HMI.Views.MessageBoxRegion;

namespace HMI.Views.MainRegion.Recipe
{
    class RecipeAdapter
    {
        #region Declaration

        private IRecipeClass recipeClass { get; set; }
        public IRecipeFile Recipe { get; set; }
        public string Path { get; set; }
        public ObservableCollection<RecipeVariableInfo> recipeVariables { get; set; }

        public string status { get; set; }

        #endregion

        #region Constructor / Destructor

        public RecipeAdapter(string ClassName, string RecipeName)
        {
            this.recipeClass = ApplicationService.GetService<IRecipeService>().GetRecipeClass(ClassName);
          //  this.recipeClass.StartEdit();
            this.Path = recipeClass.CurrentPath;
            this.Recipe = recipeClass.GetRecipeFile(RecipeName);
            this.recipeVariables = GetVariableList();
            this.status = "";
        }


        #endregion

        #region Methods

        #region Tasks

        public async Task SaveRecipe()
        {
            var e = (await this.recipeClass.SaveToFileFromBufferAsync(this.Recipe.FileName, this.Recipe.Description, true));

            if (e.Result == SaveRecipeResult.Succeeded)
            {
                this.SetError("SaveOK");
            }
            else
            {
                this.SetError("SaveError", e.Result.ToString());
            }
        }

        public void DeleteRecipe()
        {
            if (this.Recipe != null)
            {
                if (MessageBoxView.Show("@RecipeSystem.Results.DeleteFileQuery", "@RecipeSystem.Results.DeleteFile", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.Yes)
                {
                    if (this.recipeClass.DeleteFile(this.Recipe.FileName))
                    {


                        this.SetError("DeleteOK");
                    }
                    else
                    {
                        this.SetError("DeleteError");
                    }
                }
            }
        }

        public async Task LoadFromFileToBuffer()
        {
            var e = (await this.recipeClass.LoadFromFileToBufferAsync(this.Recipe.FileName));

            if (e.Result == LoadRecipeResult.Succeeded)
            {
                var r = this.recipeClass.GetRecipeFile(Recipe.FileName);
                if (r != null)
                {
                    this.SetError("LoadOK");
                }
                else
                {
                    this.SetError("LoadError", "Can't get RecipeClass");
                }
            }
            else
            {
                this.SetError("LoadError", e.Result.ToString());
            }
        }

        public async Task LoadFromFileToProcess()
        {
            var e = (await this.recipeClass.LoadFromFileToProcessAsync(this.Recipe.FileName));
            
            if (e.Result == SendRecipeResult.Succeeded)
            {
                this.SetError("PLCWriteOK");
            }
            else
            {
                this.SetError("PLCWriteError", e.Result.ToString());
            }
        }

        public async Task LoadFromProcessToBuffer()
        {
            var e = (await this.recipeClass.ReadProcessToBufferAsync());

            if (e.Result == GetRecipeResult.Succeeded)
            {
                this.SetError("PLCReadOK");
            }
            else
            {
                this.SetError("PLCReadError", e.Result.ToString());
            }
        }

        public async Task LoadFromBufferToProcess()
        {
            var e = (await this.recipeClass.WriteBufferToProcessAsync());

            if (e.Result == SetRecipeResult.Succeeded)
            {
                this.SetError("PLCWriteOK");
            }
            else
            {
                this.SetError("PLCWriteError", e.Result.ToString());
            }
        }

        #endregion

        private ObservableCollection<RecipeVariableInfo> GetVariableList()
        {
            ObservableCollection<RecipeVariableInfo> RL = new ObservableCollection<RecipeVariableInfo>();
            IVariableService variableService = ApplicationService.GetService<IVariableService>();

            if (this.recipeClass != null)
            {
                var recipeVars = this.recipeClass.GetVariableNames();

                foreach (var nextVar in recipeVars)
                {
                    var rv = variableService.GetVariable(nextVar);
                    var rvDef = variableService.GetVariableDefinition(nextVar);
                    if ((rv != null) && (rvDef != null))
                    {
                        RL.Add(new RecipeVariableInfo
                        {
                            Value = new RecipeVariableValue(rv.Value),
                            Minimum = rv.TypeCode.ToString().Equals("String") ? "" : rv.MinValue,
                            Maximum = rv.TypeCode.ToString().Equals("String") ? "" : rv.MaxValue,
                            Description = rv.Comment,
                            Unit = rv.UnitText,
                            Type = rv.TypeCode,
                            RawType = rv.RawTypeCode,
                            Name = rv.Name,
                            LocalizableParameterName = rvDef.LocalizableText
                        });
                    }
                }
            }
            return RL;
        }

        #endregion

        #region Error Handle

        private void SetError(string szErrorCode)
        {
            this.status = szErrorCode;
        }

        private void SetError(string szErrorCode, object param)
        {
            this.status = szErrorCode + ": " + param;
        }

        private void ClearError()
        {
            this.status = "";
        }

        #endregion

    }


}

