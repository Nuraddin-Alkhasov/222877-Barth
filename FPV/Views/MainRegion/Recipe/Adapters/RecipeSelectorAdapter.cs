using HMI.Views.MessageBoxRegion;
using System.Collections.ObjectModel;
using VisiWin.ApplicationFramework;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.Recipe
{
   
    [ExportAdapter("RecipeSelectorAdapter")]
    public class RecipeSelectorAdapter : AdapterBase
    {
        public IRecipeClass recipeClass;
        private ObservableCollection<RecipeFileInfo> recipeFiles;

        private RecipeFileInfo selectedFile;
        private string recipeDescriptionBuffer = "";
        private string recipeNameBuffer = "";

        public RecipeSelectorAdapter()
        {
            this.recipeClass = ApplicationService.GetService<IRecipeService>().GetRecipeClass("MachineRecipe");
            this.RecipeFiles = new ObservableCollection<RecipeFileInfo>();
            this.UpdateFileList();
            SelectedFile = GetSelectedR(ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Header.Maschinenprogramm#STRING40").ToString());
        }

        private RecipeFileInfo GetSelectedR(string _r)
        {
            foreach (RecipeFileInfo rf in recipeFiles)
            {
                if (rf.FileName == _r)
                {
                    return rf;
                }
            }
            return null;
        }

        public ObservableCollection<RecipeFileInfo> RecipeFiles
        {
            get { return this.recipeFiles; }
            set
            {
                if (!Equals(value, this.recipeFiles))
                {
                    this.recipeFiles = value;
                    this.OnPropertyChanged("RecipeFiles");
                }
            }
        }

        public void UpdateFileList()
        {
            if (this.recipeClass != null)
            {
                this.RecipeFiles.Clear();
                foreach (var nextFile in this.recipeClass.FileNames)
                {
                    var r = this.recipeClass.GetRecipeFile(nextFile);
                    this.RecipeFiles.Add(new RecipeFileInfo(recipeClass.Name, nextFile, r.Description, r.TimeOfLastChange, r.WhoSavedRecipe()));
                }
            }
        }

        public async void LoadFromFileToBuffer()
        {
            var e = (await this.recipeClass.LoadFromFileToBufferAsync(this.SelectedFile.FileName));

            if (e.Result != LoadRecipeResult.Succeeded)
            {
                new MessageBoxTask("Error loading in buffer"," Error", MessageBoxIcon.Error);
            }
        }

        public RecipeFileInfo SelectedFile
        {
            get { return this.selectedFile; }
            set
            {
                this.selectedFile = value;
                if (selectedFile != null)
                {
                    this.RecipeNameBuffer = selectedFile.FileName;
                    this.RecipeDescriptionBuffer = selectedFile.Description;
                }
                else
                {
                    this.RecipeNameBuffer = "";
                    this.RecipeDescriptionBuffer = "";
                }

                this.OnPropertyChanged("SelectedFile");
            }
        }

        public string RecipeNameBuffer
        {
            get { return this.recipeNameBuffer; }
            set
            {
                this.recipeNameBuffer = value;
                this.OnPropertyChanged("RecipeNameBuffer");
            }
        }

        public string RecipeDescriptionBuffer
        {
            get { return this.recipeDescriptionBuffer; }
            set
            {
                this.recipeDescriptionBuffer = value;
                this.OnPropertyChanged("RecipeDescriptionBuffer");
            }
        }

    }
}
