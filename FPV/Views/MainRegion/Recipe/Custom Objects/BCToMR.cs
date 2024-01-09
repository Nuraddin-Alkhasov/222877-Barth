using System.ComponentModel;

namespace HMI.Views.MainRegion.Recipe.Custom_Objects
{
    class BCToMR : INotifyPropertyChanged
    {
        public int _ID { set; get; }
        public string _Barcode { set; get; }
        public string _MR { set; get; }
        public BCToMR(int _id, string _bc, string _mr)
        {
            _ID = _id;
            _Barcode = _bc;
            _MR = _mr;
        }

        public int ID
        {
            get { return this._ID; }
            set
            {
                this._ID = value;
                this.OnPropertyChanged("ID");
            }
        }

        public string Barcode
        {
            get { return this._Barcode; }
            set
            {
                this._Barcode = value;
                this.OnPropertyChanged("Barcode");
            }
        }

        public string MR
        {
            get { return this._MR; }
            set
            {
                this._MR = value;
                this.OnPropertyChanged("MR");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
