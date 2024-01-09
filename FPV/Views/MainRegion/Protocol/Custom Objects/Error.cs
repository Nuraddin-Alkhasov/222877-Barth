using System.ComponentModel;

namespace HMI.Views.MainRegion.Protocol
{
    class Error
    {
        private string time;
        private string text;
        private string user;

        public Error(string _time, string _text, string _user)
        {
            time = _time;
            text = _text;
            user = _user;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public string Time
        {
            get { return this.time; }
            set
            {
                this.time = value;
                this.OnPropertyChanged("Time");
            }
        }

        public string Text
        {
            get { return this.text; }
            set
            {
                this.text = value;
                this.OnPropertyChanged("Text");
            }
        }

        public string User
        {
            get { return this.user; }
            set
            {
                this.user = value;
                this.OnPropertyChanged("User");
            }
        }


        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
