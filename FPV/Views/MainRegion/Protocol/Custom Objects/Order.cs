using System.ComponentModel;


namespace HMI.Views.MainRegion.Protocol
{
    class Order : INotifyPropertyChanged
    {
        public int id;
        private string timestamp;
        private string pon;
        private string bn;
        private string an;
        private string mr;
        private int mrID;
        private int charges;
        private string user;

        public Order() { }

        public Order(int _id, string _timestamp ,string _pon, string _bn, string _an, int _mrID, string _mr, int _charges,string _user)
        {
            id = _id;
            timestamp = _timestamp;
            pon = _pon;
            bn = _bn;
            an = _an;
            mrID = _mrID;
            mr = _mr;
            charges = _charges;
            user = _user;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string TimeStamp
        {
            get { return this.timestamp; }
            set
            {
                this.timestamp = value;
                this.OnPropertyChanged("TimeStamp");
            }
        }

        public string PON
        {
            get { return this.pon; }
            set
            {
                this.pon = value;
                this.OnPropertyChanged("PON");
            }
        }

        public string BN
        {
            get { return this.bn; }
            set
            {
                this.bn = value;
                this.OnPropertyChanged("BN");
            }
        }

        public string AN
        {
            get { return this.an; }
            set
            {
                this.an = value;
                this.OnPropertyChanged("AN");
            }
        }


        public int MRID
        {
            get { return this.mrID; }
            set
            {
                this.mrID = value;
                this.OnPropertyChanged("MRID");
            }
        }
        public string MR
        {
            get { return this.mr; }
            set
            {
                this.mr = value;
                this.OnPropertyChanged("MR");
            }
        }

        public int Charges
        {
            get { return this.charges; }
            set
            {
                this.charges = value;
                this.OnPropertyChanged("Charges");
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
