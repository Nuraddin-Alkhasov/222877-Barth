using System;
using System.ComponentModel;


namespace HMI.Views.MainRegion.Recipe
{
    public class RecipeFileInfo : INotifyPropertyChanged
    {
        private string desc;

        private string fileName;

        private DateTime lastChanged;

        private string whoChanged;

        private string recipeclassName;

        public RecipeFileInfo() { fileName = ""; desc = ""; }
        public RecipeFileInfo(string recipeclassName, string fileName, string description, DateTime lastChanged, string whoChanged)
        {
            this.recipeclassName = recipeclassName;
            this.fileName = fileName;
            this.desc = description;
            this.lastChanged = lastChanged;
            this.whoChanged = whoChanged;
        }

        public RecipeFileInfo(string recipeclassName, string fileName, string description)
        {
            this.recipeclassName = recipeclassName;
            this.fileName = fileName;
            this.desc = description;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public string RecipeCLassName
        {
            get { return this.recipeclassName; }
            set
            {
                this.recipeclassName = value;
                this.OnPropertyChanged("RecipeClassName");
            }
        }

        public string Description
        {
            get { return this.desc; }
            set
            {
                this.desc = value;
                this.OnPropertyChanged("Description");
            }
        }

        public string FileName
        {
            get { return this.fileName; }
            set
            {
                this.fileName = value;
                this.OnPropertyChanged("Filename");
            }
        }

        public DateTime LastChanged
        {
            get { return this.lastChanged; }
            set
            {
                this.lastChanged = value;
                this.OnPropertyChanged("LastChanged");
            }
        }

        public string WhoChanged
        {
            get { return this.whoChanged; }
            set
            {
                this.whoChanged = value;
                this.OnPropertyChanged("WhoChanged");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            if (fileName != "")
            {
                return fileName.ToString() + "-" + desc.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}
