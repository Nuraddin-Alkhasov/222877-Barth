using System.ComponentModel;
using VisiWin.ApplicationFramework;
using VisiWin.Language;

namespace HMI.Components
{
    /// <summary>
    /// Wrapperklasse für die Infos über die Rezeptklassen
    /// </summary>
    public class RecipeClassInfo : INotifyPropertyChanged
    {
        protected static ILanguageService LanguageService = ApplicationService.GetService<ILanguageService>();
        //An dieser Position müssen die sprachumschaltbaren Namen mit den System-Klassennamen hinterlegt sein
        //
        //Beispiel:
        //Für die Klasse MainRecipe sind die sprachumschaltbaren Texte unter @RecipeClasses.MainRecipe abgelegt.
        //
        //Sollten hier Tippfehler bei den Namen auftreten wird die Sprachumschaltung nicht funktionieren!
        private static readonly string localizableTextPreSequenz = "@RecipeClasses.";
        private readonly ILocalizedText localizedClassName;

        public RecipeClassInfo(string recipeClassName)
        {
            this.ClassName = recipeClassName;
            //Der Sprachumschaltbare Text muss an dieser Stelle selbst gebaut werden, da die Sprachumschaltung
            //der Klassennamen durch sprachumschaltbare Benutzertexte realisiert wird.
            this.localizedClassName = LanguageService.GetLocalizedText(localizableTextPreSequenz + this.ClassName);
            this.localizedClassName.Changed += this.LocalizedClassName_Changed;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ClassName { get; private set; }

        /// <summary>
        /// Falls der Klassenname sprachumschaltbar ist, wird der Klassenname in der derzeit
        /// ausgewählten Sprache zurückgegeben. Ansonsten wird der dem System-Klassenname zurückgegeben.
        /// </summary>
        public string LocalizableClassName
        {
            get { return this.localizedClassName.DisplayText.StartsWith("@Application.") ? this.ClassName : this.localizedClassName.DisplayText; }
        }

        public override bool Equals(object obj)
        {
            var recipeClassInfo = obj as RecipeClassInfo;
            return recipeClassInfo != null && this.ClassName.Equals(recipeClassInfo.ClassName);
        }

        public override int GetHashCode()
        {
            return this.ClassName.GetHashCode();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void LocalizedClassName_Changed(object sender, LocalizedTextChangedEventArgs e)
        {
            this.OnPropertyChanged("LocalizableClassName");
        }
    }
}