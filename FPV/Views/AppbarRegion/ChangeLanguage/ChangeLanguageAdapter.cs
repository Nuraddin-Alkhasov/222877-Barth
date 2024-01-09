using HMI.Views.DialogRegion;
using System.Collections.ObjectModel;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;
using VisiWin.Language;

namespace HMI.Views.Common
{
    [ExportAdapter("ChangeLanguageAdapter")]
    public class ChangeLanguageAdapter : AdapterBase
    {
        private string currentLanguageName;

        private readonly ILanguageService languageService;

        private IProjectLanguage selectedLanguage;

        public ChangeLanguageAdapter()
        {
            if (ApplicationService.IsInDesignMode)
            {
                return;
            }

         
            this.languageService = ApplicationService.GetService<ILanguageService>();

            this.SelectedLanguage = this.languageService.CurrentLanguage;
            this.languageService.LanguageChanged += this.OnLanguageChanged;
           
            this.SetCurrentLanguageName();
            this.ChangeLanguageCommand = new ActionCommand(this.OnChangeLanguageCommandExecuted);
        }

        /// <summary>
        /// Zeigt einen Dialog zum Wechseln der Projektsprache
        /// </summary>
        public ICommand ChangeLanguageCommand { get; set; }

        public string CurrentLanguageName
        {
            get { return this.currentLanguageName; }
            set
            {
                this.currentLanguageName = value;
                this.OnPropertyChanged("CurrentLanguageName");
            }
        }

      

        public IProjectLanguage SelectedLanguage
        {
            get { return this.selectedLanguage; }
            set
            {
                this.selectedLanguage = value;
                this.OnPropertyChanged("SelectedLanguage");
            }
        }

       

        public override void OnViewAttached(IView view)
        {
            base.OnViewAttached(view);
            this.SetCurrentLanguageName();
            this.SelectedLanguage = this.languageService.CurrentLanguage;
        }

        private void OnChangeLanguageCommandExecuted(object parameter)
        {
          this.SelectedLanguage = this.languageService.CurrentLanguage;
            if (this.SelectedLanguage.LCID == 1031)
            {
                this.languageService.ChangeLanguage(1033);
            }
            else
            {
                this.languageService.ChangeLanguage(1031);
            }
           
        }

        private void OnLanguageChanged(object sender, LanguageChangedEventArgs e)
        {
            this.SetCurrentLanguageName();
        }

        private void SetCurrentLanguageName()
        {
            var name = this.languageService.CurrentLanguage.Text;
            this.CurrentLanguageName = name.Substring(0, name.IndexOf('(') - 1);
        }

       
    }
}