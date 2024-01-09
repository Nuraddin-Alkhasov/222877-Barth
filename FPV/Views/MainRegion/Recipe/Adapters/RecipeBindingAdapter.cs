
using HMI.Module;
using HMI.Views.MainRegion.Recipe.Custom_Objects;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Commands;

namespace HMI.Views.MainRegion.Recipe.Adapters
{
    class RecipeBindingAdapter : AdapterBase
    {
        ObservableCollection<BCToMR> _BCToMRList;
        BCToMR _SelectedData;
        bool _isBCToMRSelected;
        bool _isEditing;
        Visibility _DialogVisible;
        public RecipeBindingAdapter()
        {
            UpdateBCToMRList();
            this._BCToMRList = new ObservableCollection<BCToMR>();
            this.PropertyChanged += this.OnSelectedDataChanged;
            this.NewCommand = new ActionCommand(NewCommandExecuted);
            this.EditCommand = new ActionCommand(EditCommandExecuted);
            this.CloseDialogCommand = new ActionCommand(CloseDialogCommandExecuted);
            this.CloseDialogViewCommand = new ActionCommand(CloseDialogViewCommandExecuted);
            
            this.SaveCommand = new ActionCommand(SaveCommandExecuted);
            this.DeleteCommand = new ActionCommand(DeleteCommandExecuted);
            _DialogVisible = Visibility.Hidden;
        }

        public BCToMR SelectedData
        {
            get { return this._SelectedData; }
            set
            {
                if (!Equals(value, this._SelectedData))
                {
                    this._SelectedData = value;
                    this.OnPropertyChanged("SelectedData");
                }
            }
        }

        public bool isBCToMRSelected
        {
            get { return this._isBCToMRSelected; }
            set
            {
                this._isBCToMRSelected = value;
                this.OnPropertyChanged("isBCToMRSelected");
            }
        }

        public void OnSelectedDataChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("SelectedData".Equals(e.PropertyName))
            {
                this.isBCToMRSelected = this.SelectedData != null;
            }
        }

        public bool isEditing
        {
            get { return this._isEditing; }
            set
            {
                this._isEditing = value;
                this.OnPropertyChanged("isEditing");
            }
        }

        public Visibility DialogVisible
        {
            get { return this._DialogVisible; }
            set
            {
                if (!Equals(value, this._DialogVisible))
                {
                    this._DialogVisible = value;
                    this.OnPropertyChanged("DialogVisible");
                }
            }
        }

        public ObservableCollection<BCToMR> BCToMRList
        {
            get { return this._BCToMRList; }
            set
            {
                if (!Equals(value, this._BCToMRList))
                {
                    this._BCToMRList = value;
                    this.OnPropertyChanged("BCToMRList");
                }
            }
        }

        private void UpdateBCToMRList()
        {

            Task T = Task.Run(() =>
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    DataTable _BCToMR_DT = (new localDBAdapter("SELECT * FROM BarcodeToMR;")).DB_Output();
                    if (_BCToMR_DT.Rows.Count > 0)
                    {
                        this._BCToMRList.Clear();
                        foreach (DataRow row in _BCToMR_DT.Rows)
                        {
                            _BCToMRList.Add(new BCToMR(Convert.ToInt32(row[0]), row[1].ToString(), row[2].ToString()));
                        }
                        
                    }
                });
            });
        }

        public ICommand NewCommand { get; set; }

        private void NewCommandExecuted(object parameter)
        {
            Task T = Task.Run(() =>
            {
                bool result = (new localDBAdapter("INSERT INTO BarcodeToMR (Barcode, MachineRecipe) VALUES ('','')")).DB_Input();
                UpdateBCToMRList();
            });
        }

        public ICommand EditCommand { get; set; }

        private void EditCommandExecuted(object parameter)
        {
            isEditing = true;
            DialogVisible = Visibility.Visible;
        }

        public ICommand CloseDialogCommand { get; set; }

        private void CloseDialogCommandExecuted(object parameter)
        {
            isEditing = false;
            DialogVisible = Visibility.Hidden;
        }

        public ICommand CloseDialogViewCommand { get; set; }

        private void CloseDialogViewCommandExecuted(object parameter)
        {
            isEditing = false;
            DialogVisible = Visibility.Hidden;
            ApplicationService.SetView("DialogRegion", "EmptyView");
        }

        public ICommand SaveCommand { get; set; }

        private void SaveCommandExecuted(object parameter)
        {
            if (this._SelectedData != null)
            {
                Task T = Task.Run(() =>
                {
                    DataTable temp = (new localDBAdapter("SELECT * FROM BarcodeToMR WHERE Barcode ='"+ _SelectedData.Barcode + "' ; ")).DB_Output();
                    if (temp.Rows.Count == 0)
                    {
                        bool result = (new localDBAdapter("UPDATE BarcodeToMR SET Barcode ='" + _SelectedData.Barcode + "', MachineRecipe = '" + _SelectedData.MR + "' WHERE ID = " + _SelectedData.ID.ToString() + ";")).DB_Input();

                    }
                    else
                    {
                        bool result = (new localDBAdapter("UPDATE BarcodeToMR SET MachineRecipe = '" + _SelectedData.MR + "' WHERE Barcode ='" + _SelectedData.Barcode + "';")).DB_Input();

                    }
                    isEditing = false;
                    DialogVisible = Visibility.Hidden;
                    UpdateBCToMRList();
                });
            }
        }

        public ICommand DeleteCommand { get; set; }

        private void DeleteCommandExecuted(object parameter)
        {
            if (this._SelectedData != null)
            {
                Task T = Task.Run(() =>
                {
                    bool result = (new localDBAdapter("DELETE FROM BarcodeToMR WHERE ID = " + SelectedData.ID.ToString() + ";")).DB_Input();
                    UpdateBCToMRList();
                });
            }
        }

    }
}
