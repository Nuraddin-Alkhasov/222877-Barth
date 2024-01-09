using HMI.Module;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Protocol
{
    [ExportAdapter("ProtocolAdapter")]
    class ProtocolAdapter : AdapterBase
    {
        private ObservableCollection<HistoricalTimeSpanFilterItem> timeSpanFilterTypes;
        private DateTime maxTime;
        private DateTime minTime;
        private int selectedTimeSpanFilterTypeIndex;
        private bool customTimeSpanFilterEnabled;

        private ObservableCollection<Order> orders;
        private ObservableCollection<Charge> charges;
        private ObservableCollection<Error> errors;
        private Order selectedOrder;
        private Charge selectedCharge;
        private string ponFilter;
        private string bnFilter;
        private string anFilter;
        private string mrFilter;
        private string timestamp;
        private string pon;
        public string bn;
        public string an;
        public string article;
        public string user;
        public double weight;

        private string mr;
        private string c1;
        private string c2;
        private string c3;
        private string c4;

        ProtocolAdapter()
        {   
            Orders = new ObservableCollection<Order>();
            Charges = new ObservableCollection<Charge>();
            Errors = new ObservableCollection<Error>();
            timeSpanFilterTypes = new ObservableCollection<HistoricalTimeSpanFilterItem>();
            timeSpanFilterTypes.Add(new HistoricalTimeSpanFilterItem(HistoricalTimeSpanFilterType.Today));
            timeSpanFilterTypes.Add(new HistoricalTimeSpanFilterItem(HistoricalTimeSpanFilterType.Yesterday));
            timeSpanFilterTypes.Add(new HistoricalTimeSpanFilterItem(HistoricalTimeSpanFilterType.ThisWeek));
            timeSpanFilterTypes.Add(new HistoricalTimeSpanFilterItem(HistoricalTimeSpanFilterType.LastWeek));
            timeSpanFilterTypes.Add(new HistoricalTimeSpanFilterItem(HistoricalTimeSpanFilterType.Custom));
            SetTimeSpan(HistoricalTimeSpanFilterType.Today);
        }

        public ObservableCollection<Order> Orders
        {
            get { return this.orders; }
            set
            {
                if (!Equals(value, this.orders))
                {
                    this.orders = value;
                    this.OnPropertyChanged("Orders");
                }
            }
        }

        public ObservableCollection<Charge> Charges
        {
            get { return this.charges; }
            set
            {
                if (!Equals(value, this.charges))
                {
                    this.charges = value;
                    this.OnPropertyChanged("Charges");
                }
            }
        }

        public ObservableCollection<Error> Errors
        {
            get { return this.errors; }
            set
            {
                if (!Equals(value, this.errors))
                {
                    this.errors = value;
                    this.OnPropertyChanged("Errors");
                }
            }
        }

        public Order SelectedOrder
        {
            get { return this.selectedOrder; }
            set
            {
                if (value != this.selectedOrder)
                {
                    this.selectedOrder = value;
                    SetSelectedOrder();
                    this.OnPropertyChanged("SelectedOrder");
                }
            }
        }

        public Charge SelectedCharge
        {
            get { return this.selectedCharge; }
            set
            {
                if (value != this.selectedCharge)
                {
                    this.selectedCharge = value;
                    SetSelectedCharge();
                    this.OnPropertyChanged("SelectedCharge");
                }
            }
        }

        public string PONFilter
        {
            get { return this.ponFilter; }
            set
            {
                if (!Equals(value, this.ponFilter))
                { 
                    this.ponFilter = value;
                    this.OnPropertyChanged("PONFilter");
                }
            }
        }

        public string BNFilter
        {
            get { return this.bnFilter; }
            set
            {
                if (!Equals(value, this.bnFilter))
                {
                    this.bnFilter = value;
                    this.OnPropertyChanged("BNFilter");
                }
            }
        }

        public string ANFilter
        {
            get { return this.bnFilter; }
            set
            {
                if (!Equals(value, this.anFilter))
                {
                    this.anFilter = value;
                    this.OnPropertyChanged("ANFilter");
                }
            }
        }

        public string MRFilter
        {
            get { return this.mrFilter; }
            set
            {
                if (!Equals(value, this.mrFilter))
                {
                    this.mrFilter = value;
                    this.OnPropertyChanged("MRFilter");
                }
            }
        }

        public string TimeStampFilter
        {
            get { return this.timestamp; }
            set
            {
                if (!Equals(value, this.timestamp))
                {
                    this.timestamp = value;
                    this.OnPropertyChanged("TimeStampFilter");
                }
            }
        }

        public DateTime MaxTime
        {
            get { return maxTime; }
            set
            {
                if (value != maxTime)
                {
                    maxTime = value;
                    OnPropertyChanged("MaxTime");
                }
            }
        }

        public DateTime MinTime
        {
            get { return minTime; }
            set
            {
                if (value != minTime)
                {
                    minTime = value;
                    OnPropertyChanged("MinTime");
                }
            }
        }

        public int SelectedTimeSpanFilterTypeIndex
        {
            get { return selectedTimeSpanFilterTypeIndex; }
            set
            {
                if (value != selectedTimeSpanFilterTypeIndex)
                {
                    selectedTimeSpanFilterTypeIndex = value;
                    OnPropertyChanged("SelectedTimeSpanFilterTypeIndex");
                }
            }
        }

        public ObservableCollection<HistoricalTimeSpanFilterItem> TimeSpanFilterTypes
        {
            get
            {
                return this.timeSpanFilterTypes;
            }
        }

        public bool CustomTimeSpanFilterEnabled
        {
            get { return customTimeSpanFilterEnabled; }
            set
            {
                if (value != customTimeSpanFilterEnabled)
                {
                    customTimeSpanFilterEnabled = value;
                    OnPropertyChanged("CustomTimeSpanFilterEnabled");
                }
            }
        }


        public void BW_FilterSQL()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += FilterSQL;
            bw.RunWorkerAsync();
        }

        private void FilterSQL(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate 
            {
                Orders.Clear();
                DataTable DT = (new localDBAdapter(GenerateSQLQuery())).DB_Output();
                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow r in DT.Rows)
                    {
                        Orders.Add(new Order(Convert.ToInt32(r.ItemArray[0]), Convert.ToDateTime(r.ItemArray[1].ToString()).ToString(), r.ItemArray[2].ToString(), r.ItemArray[3].ToString(), r.ItemArray[4].ToString(), Convert.ToInt16(r.ItemArray[5]), r.ItemArray[6].ToString(), Convert.ToInt16(r.ItemArray[7]), r.ItemArray[8].ToString()));
                    }
                }
            });
        }

        private void SetSelectedOrder()
        {
            Task.Run(() => {
                if (selectedOrder != null)
                {
                    TimeStamp = selectedOrder.TimeStamp;
                    PON = selectedOrder.PON;
                    BN = selectedOrder.BN;
                    AN = selectedOrder.AN;
                    MR = selectedOrder.MR;
                    User = selectedOrder.User;
                    Article = "";
                    C1 = "";
                    C2 = "";
                    C3 = "";
                    C4 = "";
                   
                    DataTable DT = (new localDBAdapter("SELECT MR, C1, C2, C3, C4 FROM MachineRecipes WHERE Id = " + selectedOrder.MRID.ToString())).DB_Output();
                    if (DT.Rows.Count > 0)
                    {
                        Article = DT.Rows[0].ItemArray[0].ToString();
                        C1 = DT.Rows[0].ItemArray[1].ToString();
                        C2 = DT.Rows[0].ItemArray[2].ToString();
                        C3 = DT.Rows[0].ItemArray[3].ToString();
                        C4 = DT.Rows[0].ItemArray[4].ToString();
                    }

                    DT = (new localDBAdapter("SELECT * FROM Charges WHERE Order_Id = " + selectedOrder.id.ToString())).DB_Output();

                   
                    Dispatcher.BeginInvoke(new Action(() => 
                    {
                        Charges.Clear();
                        double temp = 0;
                        if (DT.Rows.Count > 0)
                        {
                           
                            foreach (DataRow r in DT.Rows)
                            {
                                Charges.Add(new Charge(Convert.ToInt32(r.ItemArray[0]),
                                     Convert.ToInt32(r.ItemArray[1]),
                                     Convert.ToDouble(r.ItemArray[2]),
                                     Convert.ToBoolean(r.ItemArray[3]) ? "Ja" : "Nein",
                                     Convert.ToInt32(r.ItemArray[4]),
                                     Convert.ToBoolean(r.ItemArray[5]),
                                    r.ItemArray[6].ToString(),
                                    r.ItemArray[7].ToString(),
                                    r.ItemArray[8].ToString(),
                                    r.ItemArray[9].ToString(),
                                    r.ItemArray[10].ToString(),
                                    r.ItemArray[11].ToString(),
                                    r.ItemArray[12].ToString(),
                                    r.ItemArray[13].ToString(),
                                    r.ItemArray[14].ToString(),
                                    r.ItemArray[15].ToString(),
                                    r.ItemArray[16].ToString(),
                                    r.ItemArray[17].ToString(),
                                    r.ItemArray[18].ToString(),
                                    r.ItemArray[19].ToString(),
                                    r.ItemArray[20].ToString(),
                                    r.ItemArray[21].ToString(),
                                    r.ItemArray[22].ToString(),
                                    r.ItemArray[23].ToString()));
                                temp = temp + Convert.ToDouble(r.ItemArray[2]);
                            }
                           
                        }
                        Weight = temp;                     
                    }));
                   
                }
            });
        }

        private void SetSelectedCharge()
        {

            if (selectedCharge != null) { 
                DataTable DT = (new localDBAdapter("SELECT * FROM Errors WHERE Charge_Id = " + selectedCharge.id.ToString())).DB_Output();
                Errors.Clear();
                if (DT.Rows.Count > 0)
                {
                    foreach (DataRow r in DT.Rows)
                    {
                        Errors.Add(new Error(r.ItemArray[2].ToString(),
                            r.ItemArray[3].ToString(),
                             r.ItemArray[4].ToString()));
                    }
                }

            }
        }

        public string TimeStamp
        {
            get { return this.timestamp; }
            set
            {
                if (value != this.timestamp)
                {
                    this.timestamp = value;
                    this.OnPropertyChanged("TimeStamp");
                }
            }
        }

        public double Weight
        {
            get { return this.weight; }
            set
            {
                if (value != this.weight)
                {
                    this.weight = value;
                    this.OnPropertyChanged("Weight");
                }
            }
        }

        public string PON
        {
            get { return this.pon; }
            set
            {
                if (value != this.pon)
                {
                    this.pon = value;
                    this.OnPropertyChanged("PON");
                }
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

        public string MR
        {
            get { return this.mr; }
            set
            {
                this.mr = value;
                this.OnPropertyChanged("MR");
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

        public string Article
        {
            get { return this.article; }
            set
            {
                this.article = value;
                this.OnPropertyChanged("Article");
            }
        }

        public string C1
        {
            get { return this.c1; }
            set
            {
                this.c1 = value;
                this.OnPropertyChanged("C1");
            }
        }

        public string C2
        {
            get { return this.c2; }
            set
            {
                this.c2 = value;
                this.OnPropertyChanged("C2");
            }
        }

        public string C3
        {
            get { return this.c3; }
            set
            {
                this.c3 = value;
                this.OnPropertyChanged("C3");
            }
        }

        public string C4
        {
            get { return this.c4; }
            set
            {
                this.c4 = value;
                this.OnPropertyChanged("C4");
            }
        }

        private string GenerateSQLQuery()
        {
            string T = "Select Orders.Id, Orders.TimeStamp, Orders.PON, Orders.BN, Orders.AN, Orders.MR_ID, MachineRecipes.MR, Orders.Charges, Orders.User From Orders INNER JOIN MachineRecipes ON Orders.MR_ID = MachineRecipes.ID";
            string F = "";
            List<string> FL = new List<string>();

            if (minTime != null && maxTime != null)
            {
                string Minmonth;
                if (minTime.Month.ToString().Length == 1) { Minmonth = "0" + minTime.Month.ToString(); }
                else { Minmonth = minTime.Month.ToString(); }

                string Minday;
                if (minTime.Day.ToString().Length == 1) { Minday = "0" + minTime.Day.ToString(); }
                else { Minday = minTime.Day.ToString(); }

                string Maxmonth;
                if (maxTime.Month.ToString().Length == 1) { Maxmonth = "0" + maxTime.Month.ToString(); }
                else { Maxmonth = maxTime.Month.ToString(); }

                string Maxday;
                if (maxTime.Day.ToString().Length == 1) { Maxday = "0" + maxTime.Day.ToString(); }
                else { Maxday = maxTime.Day.ToString(); }

                FL.Add("datetime(TimeStamp) >= date('" + minTime.Year + "-" + Minmonth + "-" + Minday + "') AND date(TimeStamp) <= date('" + MaxTime.Year + "-" + Maxmonth + "-" + Maxday + "')");

            }

            if (ponFilter!="" && ponFilter != null)
            {
                FL.Add("PON like '%" + ponFilter + "%'");
            }

            if (bnFilter != "" && bnFilter != null)
            {
                FL.Add("BN like '%" + bnFilter + "%'");
            }

            if (anFilter != "" && anFilter != null)
            {
                FL.Add("AN like '%" + anFilter + "%'");
            }

            if (mrFilter != "" && mrFilter != null)
            {
                FL.Add("MachineRecipes.MainR like '%" + mrFilter + "%'");
            }

            if (FL.Count > 0)
            {
                F = F + " WHERE ";
                for (int i = 0; i < FL.Count; i++)
                {
                    if (i == 0)
                    {
                        F = F + FL[i];
                    }
                    else
                    {
                        F = F + " AND " + FL[i];
                    }
                } 
            }
            return T + F + ";";
        }

        public void SetTimeSpan(HistoricalTimeSpanFilterType selectedTimeSpanFilterType)
        {
            switch (selectedTimeSpanFilterType)
            {
                case HistoricalTimeSpanFilterType.Custom:
                    break;
                case HistoricalTimeSpanFilterType.Today:
                    MinTime = DateTime.Now.Date;
                    MaxTime = MinTime.Add(TimeSpan.FromDays(1));
                    break;
                case HistoricalTimeSpanFilterType.Yesterday:
                    MinTime = DateTime.Now.Date.Add(TimeSpan.FromDays(-1.0));
                    MaxTime = MinTime.Add(TimeSpan.FromDays(1));
                    break;
                case HistoricalTimeSpanFilterType.ThisWeek:
                    MinTime = DateTime.Now.Date.Add(TimeSpan.FromDays(-(double)DateTime.Now.Date.DayOfWeek));
                    MaxTime = MinTime.Add(TimeSpan.FromDays(7));
                    break;
                case HistoricalTimeSpanFilterType.LastWeek:
                    MinTime = DateTime.Now.Date.Add(TimeSpan.FromDays(-(double)DateTime.Now.Date.DayOfWeek - 7));
                    MaxTime = MinTime.Add(TimeSpan.FromDays(7));
                    break;
                default:
                    break;
            }

        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == "SelectedTimeSpanFilterTypeIndex")
            {
                if (SelectedTimeSpanFilterTypeIndex >= 0)
                {
                    HistoricalTimeSpanFilterType selectedFilterType = TimeSpanFilterTypes[SelectedTimeSpanFilterTypeIndex].FilterType;
                    CustomTimeSpanFilterEnabled = selectedFilterType == HistoricalTimeSpanFilterType.Custom;
                    SetTimeSpan(selectedFilterType);
                }
            }
        }
    }
}
