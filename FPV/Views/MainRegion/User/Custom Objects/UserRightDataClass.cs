using System;
using System.ComponentModel;


namespace HMI.Views.MainRegion.User
{
    public class UserRightDataClass : INotifyPropertyChanged
    {
        public UserRightDataClass(bool on, string right)
        {
            this.on = on;
            this.Right = right;
        }

        private string right;
        public string Right
        {
            get { return this.right; }
            set { this.right = value; OnPropertyChanged("Right"); }
        }

        private bool on;
        public bool On
        {
            get { return this.on; }
            set { this.on = value; OnPropertyChanged("On"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
}
