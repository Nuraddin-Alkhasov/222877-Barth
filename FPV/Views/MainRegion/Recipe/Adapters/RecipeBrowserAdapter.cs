using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using HMI.Views.MainRegion.Recipe.Custom_Objects;
using HMI.Views.MessageBoxRegion;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.Recipe
{
   
    [ExportAdapter("RecipeBrowserAdapter")]
    public class RecipeBrowserAdapter : AdapterBase
    {

        private ObservableCollection<RecipeFileInfo> recipeFiles;
        IRecipeClass recipeClass;

        private string recipeDescriptionBuffer = "";
        private string recipeNameBuffer = "";
        private bool recipeIsSelected;
        private string RecipeClassName = "";
      
        private RecipeFileInfo selectedFile;

        private int ClassID;

        public RecipeBrowserAdapter(int _classID)
        {
            ClassID = _classID;
            RecipeClassName = ApplicationService.GetService<IRecipeService>().RecipeClassNames[ClassID];
            recipeClass = ApplicationService.GetService<IRecipeService>().GetRecipeClass(RecipeClassName);
          //  recipeClass.StartEdit();
            
            this.RecipeFiles = new ObservableCollection<RecipeFileInfo>();
            this.PropertyChanged += this.OnSelectedFileChanged;

            this.UpdateFileList();

            this.LoadFileToBuffer = new ActionCommand(this.LoadRecipeToBufferCommandExecuted);
            this.SaveFileCommand = new ActionCommand(this.SaveRecipeCommandExecuted);
            this.DeleteFileCommand = new ActionCommand(this.OnDeleteFileCommandExecuted);

            SelectedFile = GetSelectedR();
        }

        private RecipeFileInfo GetSelectedR()
        {
            string _r = "";
            switch (ClassID)
            {
                case 0:
                    _r= ApplicationService.GetVariableValue("Recipe.Article.Name").ToString(); break;
                case 2:
                    _r = ApplicationService.GetVariableValue("Recipe.D.Name").ToString();break;
                case 3:
                    _r = ApplicationService.GetVariableValue("Recipe.S.Name").ToString(); break;
                case 4:
                    _r = ApplicationService.GetVariableValue("Recipe.R.Name").ToString(); break;
                case 5:
                    _r = ApplicationService.GetVariableValue("Recipe.CoatingSteps.Name").ToString(); break;
                case 6:
                    _r = ApplicationService.GetVariableValue("Recipe.MachineRecipe.Name").ToString(); break;
            }

            foreach (RecipeFileInfo rf in recipeFiles)
            {
                if (rf.FileName == _r)
                {
                    return rf;
                }
            }
            return null;
        }

        public ICommand LoadFileToBuffer { get; set; }
        public ICommand SaveFileCommand { get; set; }
        public ICommand DeleteFileCommand { get; set; }

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

        public bool RecipeIsSelected
        {
            get { return this.recipeIsSelected; }
            set
            {
                this.recipeIsSelected = value;
                this.OnPropertyChanged("RecipeIsSelected");
            }
        }

        public RecipeFileInfo SelectedFile
        {
            get { return this.selectedFile; }
            set
            {
                if (!Equals(value, this.selectedFile))
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
        }

        public void OnSelectedFileChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("SelectedFile".Equals(e.PropertyName))
            {
                this.RecipeIsSelected = this.SelectedFile != null;
            }
        }

        private void LoadRecipeToBufferCommandExecuted(object parameter)
        {
            if (this.SelectedFile != null)
            {
               LoadFromFileToBufferAsync();
            }
        }

        public async void LoadFromFileToBufferAsync()
        {
            if (recipeClass.Name == "MachineRecipe")
            {
               
                (new AutoRecipeLoad(SelectedFile.FileName)).LoadStackAsync();
            }
            else
            {
                var e = (await this.recipeClass.LoadFromFileToBufferAsync(this.SelectedFile.FileName));

                if (e.Result == LoadRecipeResult.Succeeded)
                {
                    this.RecipeNameBuffer = SelectedFile.FileName;
                    this.RecipeDescriptionBuffer = SelectedFile.Description;
                    setNameAndDescr();
                }
                else
                {
                    new MessageBoxTask("Load Error", "Load",  MessageBoxIcon.Error);
                }
            }
        }

        private void SaveRecipeCommandExecuted(object parameter)
        {
            bool saveR = true;
            if (this.recipeClass != null)
            {
                IRegionService iRS = ApplicationService.GetService<IRegionService>();
                switch (ClassID)
                {
                    case 6:
                       
                        //if (!((Recipe_Machine)iRS.GetView("Recipe_Machine")).saveActive)
                        //{  saveR = false;  }
                        break;
                    case 5:
                        if (!((Recipe_Coating_PR)iRS.GetView("Recipe_Coating_PR")).saveActive)
                        { saveR = false; }
                        break;
                }
                if (saveR)
                {
                    SaveRecipe();
                }
                else
                {
                   new MessageBoxTask("Dieses Rezept ist nicht gültig", "Speichern nicht möglich", MessageBoxIcon.Exclamation);
                }
            }
        }

        private void setNameAndDescr()
        {
            switch (ClassID)
            {
                case 0:
                    ApplicationService.SetVariableValue("Recipe.Article.Name", RecipeNameBuffer);
                    ApplicationService.SetVariableValue("Recipe.Article.Description", RecipeDescriptionBuffer); break;
                case 2:
                    ApplicationService.SetVariableValue("Recipe.D.Name", RecipeNameBuffer);
                    ApplicationService.SetVariableValue("Recipe.D.Description", RecipeDescriptionBuffer); break;
                case 3:
                    ApplicationService.SetVariableValue("Recipe.S.Name", RecipeNameBuffer);
                    ApplicationService.SetVariableValue("Recipe.S.Description", RecipeDescriptionBuffer); break;
                case 4:
                    ApplicationService.SetVariableValue("Recipe.R.Name", RecipeNameBuffer);
                    ApplicationService.SetVariableValue("Recipe.R.Description", RecipeDescriptionBuffer); break;
                case 5:
                    ApplicationService.SetVariableValue("Recipe.CoatingSteps.Name", RecipeNameBuffer);
                    ApplicationService.SetVariableValue("Recipe.CoatingSteps.Description", RecipeDescriptionBuffer); break;
                case 6:
                    ApplicationService.SetVariableValue("Recipe.MachineRecipe.Name", RecipeNameBuffer);
                    ApplicationService.SetVariableValue("Recipe.MachineRecipe.Description", RecipeDescriptionBuffer); break;
                   
                 
            }
        }

        public async void SaveRecipe()
        {

            if (string.IsNullOrEmpty(this.RecipeNameBuffer))
            {
                new MessageBoxTask("@RecipeSystem.Results.EnterFilename", "@RecipeSystem.Results.SaveFile", MessageBoxIcon.Information);
                return;
            }

            if (this.recipeClass.FileNames.Contains(this.RecipeNameBuffer))
            {
                if (MessageBoxView.Show("@RecipeSystem.Results.OverwriteFileQuery", "@RecipeSystem.Results.SaveFile", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.No)
                {
                    return;
                }
            }

            var e = (await this.recipeClass.SaveToFileFromBufferAsync(this.RecipeNameBuffer, this.RecipeDescriptionBuffer, true));

            if (e.Result == SaveRecipeResult.Succeeded)
            {
                setNameAndDescr();
            }
            else
            {
               
            }
        }

        private void OnDeleteFileCommandExecuted(object parameter)
        {
            if (this.SelectedFile != null)
            {
                if (MessageBoxView.Show("@RecipeSystem.Results.DeleteFileQuery", "@RecipeSystem.Results.DeleteFile", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.Yes)
                {
                    if (this.recipeClass.DeleteFile(this.SelectedFile.FileName))
                    {
                        this.UpdateFileList();
                        this.RecipeNameBuffer = "";
                        this.RecipeDescriptionBuffer = "";
                    }
                }
            }
        }

        private void UpdateFileList()
        {
            this.RecipeFiles.Clear();
            foreach (var nextFile in recipeClass.FileNames)
            {
                var r = recipeClass.GetRecipeFile(nextFile);
                this.RecipeFiles.Add(new RecipeFileInfo(recipeClass.Name, nextFile, r.Description, r.TimeOfLastChange, r.WhoSavedRecipe()));
            }
        }

    }
}