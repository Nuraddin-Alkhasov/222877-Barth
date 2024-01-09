using System.ComponentModel;

namespace HMI.Views.MainRegion.Protocol
{

    class Charge : INotifyPropertyChanged
    {

        public int id;
        private int chargenr;
        private int orderid;
        private double weight;
        private string optimized;
        private bool error;

        private string start;

        private string c1s;
        private string c1e;
        private string o1s;
        private string o1e;

        private string c2s;
        private string c2e;
        private string o2s;
        private string o2e;

        private string c3s;
        private string c3e;
        private string o3s;
        private string o3e;

        private string c4s;
        private string c4e;
        private string o4s;
        private string o4e;

        private string end;

        public Charge(int _id, int _chargenr, double _weight, string _optimized, int _orderid, bool _error, string _start, string _c1s, string _c1e, string _o1s, string _o1e, string _c2s, string _c2e, string _o2s, string _o2e, string _c3s, string _c3e, string _o3s, string _o3e, string _c4s, string _c4e, string _o4s, string _o4e, string _end)
        {

            id = _id;
            chargenr = _chargenr;
            weight = _weight;
            optimized = _optimized;
            orderid = _orderid;
          
            error = _error;

            start = _start;

            c1s = _c1s;
            c1e = _c1e;
            o1s = _o1s;
            o1e = _o1e;

            c2s = _c2s;
            c2e = _c2e;
            o2s = _o2s;
            o2e = _o2e;

            c3s = _c3s;
            c3e = _c3e;
            o3s = _o3s;
            o3e = _o3e;

            c4s = _c4s;
            c4e = _c4e;
            o4s = _o4s;
            o4e = _o4e;

            end = _end;
       
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int ChargeNr
        {
            get { return this.chargenr; }
            set
            {
                this.chargenr = value;
                this.OnPropertyChanged("ChargeNr");
            }
        }

        public double Weight
        {
            get { return this.weight; }
            set
            {
                this.weight = value;
                this.OnPropertyChanged("Weight");
            }
        }

        public string Optimized
        {
            get { return this.optimized; }
            set
            {
                this.optimized = value;
                this.OnPropertyChanged("Optimized");
            }
        }

        public bool Error
        {
            get { return this.error; }
            set
            {
                this.error = value;
                this.OnPropertyChanged("Error");
            }
        }

        public string Start
        {
            get { return this.start; }
            set
            {
                this.start = value;
                this.OnPropertyChanged("Start");
            }
        }

        public string C1S
        {
            get { return this.c1s; }
            set
            {
                this.c1s = value;
                this.OnPropertyChanged("C1S");
            }
        }

        public string C1E
        {
            get { return this.c1e; }
            set
            {
                this.c1e = value;
                this.OnPropertyChanged("C1E");
            }
        }

        public string O1S
        {
            get { return this.o1s; }
            set
            {
                this.o1s = value;
                this.OnPropertyChanged("O1S");
            }
        }

        public string O1E
        {
            get { return this.o1e; }
            set
            {
                this.o1e = value;
                this.OnPropertyChanged("O1E");
            }
        }

        public string C2S
        {
            get { return this.c2s; }
            set
            {
                this.c2s = value;
                this.OnPropertyChanged("C2S");
            }
        }

        public string C2E
        {
            get { return this.c2e; }
            set
            {
                this.c2e = value;
                this.OnPropertyChanged("C2E");
            }
        }

        public string O2S
        {
            get { return this.o2s; }
            set
            {
                this.o2s = value;
                this.OnPropertyChanged("O2S");
            }
        }

        public string O2E
        {
            get { return this.o2e; }
            set
            {
                this.o2e = value;
                this.OnPropertyChanged("O2E");
            }
        }

        public string C3S
        {
            get { return this.c3s; }
            set
            {
                this.c3s = value;
                this.OnPropertyChanged("C3S");
            }
        }

        public string C3E
        {
            get { return this.c3e; }
            set
            {
                this.c3e = value;
                this.OnPropertyChanged("C3E");
            }
        }

        public string O3S
        {
            get { return this.o3s; }
            set
            {
                this.o3s = value;
                this.OnPropertyChanged("O3S");
            }
        }

        public string O3E
        {
            get { return this.o3e; }
            set
            {
                this.o3e = value;
                this.OnPropertyChanged("O3E");
            }
        }

        public string C4S
        {
            get { return this.c4s; }
            set
            {
                this.c4s = value;
                this.OnPropertyChanged("C4S");
            }
        }

        public string C4E
        {
            get { return this.c4e; }
            set
            {
                this.c4e = value;
                this.OnPropertyChanged("C4E");
            }
        }

        public string O4S
        {
            get { return this.o4s; }
            set
            {
                this.o4s = value;
                this.OnPropertyChanged("O4S");
            }
        }

        public string O4E
        {
            get { return this.o4e; }
            set
            {
                this.o4e = value;
                this.OnPropertyChanged("O4E");
            }
        }

        public string End
        {
            get { return this.end; }
            set
            {
                this.end = value;
                this.OnPropertyChanged("End");
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
