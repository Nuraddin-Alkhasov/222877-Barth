using System;
using System.ComponentModel;
using System.Windows.Media;
using VisiWin.ApplicationFramework;
using VisiWin.Language;

namespace HMI.Components
{
    public class LocalizableParameterNameWrapper : INotifyPropertyChanged
    {
        protected static ILanguageService LanguageService = ApplicationService.GetService<ILanguageService>();

        private string localizableText;
        private ILocalizedText localizedText;

        public event PropertyChangedEventHandler PropertyChanged;

        public string LocalizableParameterName
        {
            get { return this.localizedText.DisplayText; }
            set
            {
                if (this.localizableText != value)
                {
                    this.localizableText = value;

                    //EventListener vom alten ILocalizedText abhängen
                    if (this.localizedText != null)
                    {
                        this.localizedText.Changed -= this.localizedText_Changed;
                    }

                    this.localizedText = LanguageService.GetLocalizedText(this.localizableText);
                    this.OnPropertyChanged("LocalizableParameterName");

                    this.localizedText.Changed += this.localizedText_Changed;
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void localizedText_Changed(object sender, LocalizedTextChangedEventArgs e)
        {
            this.OnPropertyChanged("LocalizableParameterName");
        }
    }

    /// <summary>
    /// Wrapperklasse für die Werte der Variablen, damit eine Sprachumschaltung gewährleistet ist.
    /// </summary>
    public class RecipeVariableValue : INotifyPropertyChanged
    {
        private static readonly ILanguageService languageService = ApplicationService.GetService<ILanguageService>();
        private readonly object value;

        public RecipeVariableValue(object value)
        {
            this.value = value;
            languageService.LanguageChanged += this._languageService_LanguageChanged;
        }

        //Benötigt um die EventListener abzuhängen
        ~RecipeVariableValue()
        {
            languageService.LanguageChanged -= this._languageService_LanguageChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayText
        {
            get { return this.value.ToString(); }
        }

        public override bool Equals(object obj)
        {
            var recipeVariableValue = (RecipeVariableValue)obj;
            return recipeVariableValue != null && this.value.Equals(recipeVariableValue.value);
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        private void _languageService_LanguageChanged(object sender, LanguageChangedEventArgs e)
        {
            this.OnPropertyChanged("DisplayText");
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    /// <summary>
    /// Information über eine Rezeptvariable
    /// </summary>
    public class RecipeVariableInfo : LocalizableParameterNameWrapper
    {
        public string Description { get; set; }
        public object Maximum { get; set; }
        public object Minimum { get; set; }
        public string Name { get; set; }
        public object RawType { get; set; }
        public object Type { get; set; }
        public string Unit { get; set; }
        public RecipeVariableValue Value { get; set; }
    }

    public class CompareRecipeVarialbenInfo : RecipeVariableInfo
    {
        private bool shouldBeFiltered;
        public SolidColorBrush background2 { get; set; }
        public SolidColorBrush background3 { get; set; }
        public SolidColorBrush background4 { get; set; }

        public bool ShouldBeFiltered
        {
            get { return this.shouldBeFiltered; }
            set
            {
                if (this.shouldBeFiltered != value)
                {
                    this.shouldBeFiltered = value;
                    this.OnPropertyChanged("ShouldBeFiltered");
                }
            }
        }

        public RecipeVariableValue Value2 { get; set; }
        public RecipeVariableValue Value3 { get; set; }
        public RecipeVariableValue Value4 { get; set; }
    }

    /// <summary>
    /// Informationen über eine Rezeptänderung
    /// </summary>
    public class RecipeChangeInfo : LocalizableParameterNameWrapper
    {
        /// <summary>
        /// Die Repräsentation einer Rezeptänderung
        /// </summary>
        /// <param name="user">Benutzername</param>
        /// <param name="timestamp">Zeitstempel</param>
        /// <param name="variable">geänderte Rezeptvariable</param>
        /// <param name="oldValue">alter Wert</param>
        /// <param name="newValue">neuer Wert</param>
        /// <param name="localizableParameterName"></param>
        public RecipeChangeInfo(string user, DateTime timestamp, string variable, object oldValue, object newValue, string localizableParameterName)
        {
            this.User = user;
            this.Timestamp = timestamp.ToString();
            this.Variable = variable;
            this.OldValue = oldValue.ToString();
            this.NewValue = newValue.ToString();
            this.LocalizableParameterName = localizableParameterName;
        }

        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public string Timestamp { get; set; }

        public string User { get; set; }
        public string Variable { get; set; }
    }
}