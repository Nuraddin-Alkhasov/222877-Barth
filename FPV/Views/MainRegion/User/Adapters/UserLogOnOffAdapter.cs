using System.Collections.Generic;
using System.Linq;
using VisiWin.ApplicationFramework;
using System.ComponentModel;
using VisiWin.UserManagement;
using System.Collections.ObjectModel;
using VisiWin.Language;
using VisiWin.Commands;
using System.Windows.Input;
using HMI.Views.DialogRegion;
using HMI.Views.MessageBoxRegion;

namespace HMI.Views.MainRegion.User
{
    [ExportAdapter("UserLogOnOffAdapter")]
    public class UserLogOnOffAdapter : AdapterBase, INotifyPropertyChanged
	{
		#region Felder

		private IUserManagementService userService;
		private ILanguageService textService;
		private bool renewPassword;

		#endregion Felder

		public UserLogOnOffAdapter()
        {
			if (ApplicationService.IsInDesignMode)
				return;

			this.userService = ApplicationService.GetService<IUserManagementService>();
			this.textService = ApplicationService.GetService<ILanguageService>();

			if (this.userService == null) {
				if (this.textService == null)
					this.Status = "Initialisierungsfehler Benutzerverwaltung";
				else
					this.Status = this.textService.GetText("@UserManagement.InitError");
				return;
			}

			//	Kommandos anbinden
			this.LogOnOffCommand = new ActionCommand(OnLogOnOffCommandExecuted);
			this.ChangeCurrentPasswordCommand = new ActionCommand(OnChangeCurrentPasswordCommandExecuted);

            UserList = new ObservableCollection<string>();

			userService.UserLoggedOn += this.userService_UserLoggedOn;
			userService.UserLoggedOff += this.userService_UserLoggedOff;
			if (userService.CurrentUser != null) {
				this.UserIsLoggedOn = true;
				this.CurrentUserName = this.userService.CurrentUserName;
				this.CurrentUserFullName = this.userService.CurrentUser.FullName;
			} else {
				this.UserIsLoggedOn = false;
				this.CurrentUserName = "";
				this.CurrentUserFullName = "";
			}
		}

		/// <summary>
		/// Benutzer hat sich angemeldet
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void userService_UserLoggedOff(object sender, LogOffEventArgs e)
		{
			this.UserIsLoggedOn = false;
			this.CurrentUserName = "";
			this.CurrentUserFullName = "";
		}

		/// <summary>
		/// Benutzer hat sich abgemeldet
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void userService_UserLoggedOn(object sender, LogOnEventArgs e)
		{
			this.UserIsLoggedOn = true;
			this.CurrentUserName = e.CurrentUser.Name;
			this.CurrentUserFullName = e.CurrentUser.FullName;
		}

		/// <summary>
		/// Benutzerliste auf den neuesten Stand bringen
		/// </summary>
		private void RefreshUserNames()
		{
			//	aktuelle Liste merken
			List<string> oldUserList = UserList.ToList<string>();

			//	die Neuen dazu
			foreach (string user in this.userService.UserNames) {
				if (UserList.Contains(user))
					oldUserList.Remove(user);
				else
					UserList.Add(user);
			}

			//	die nicht mehr Vorhandenen entfernen
			foreach (string user in oldUserList)
				UserList.Remove(user);
		}

		#region Befehle

		/// <summary>
		/// Benutzer an-/abmelden
		/// </summary>
		public ICommand LogOnOffCommand { get; set; }
		private void OnLogOnOffCommandExecuted(object parameter)
		{
			ClearError();

			RefreshUserNames();
			this.renewPassword = false;
			this.Password = "";

			//	solange Dialog zeigen, bis OK oder Abbruch oder Passwort geändert wurde
			while (( DialogView.Show("User_LogOnOff", "@UserManagement.View.LogOnOff", DialogButton.Custom,
									(this.userService.CurrentUser == null) ? DialogResult.Left : DialogResult.Right, true,
									this.CheckLogOnOff, "@UserManagement.View.Buttons.LogOn", null,
									(this.userService.CurrentUser == null) ? null : "@UserManagement.View.Buttons.LogOff") == DialogResult.Left) && this.renewPassword) {
				//	das Passwort muss geändert werden
				this.renewPassword = false;

				if (string.IsNullOrEmpty(this.SelectedUser))
					break;

				UserAdapter ua = (UserAdapter)ApplicationService.GetAdapter("UserAdapter");
				if (ua != null)
					ua.ShowChangePasswordDialog(this.SelectedUser);
			}
		}

		/// <summary>
		/// Testen auf gültige Eingaben beim Anmelden
		/// </summary>
		/// <returns></returns>
		void CheckLogOnOff(object sender, DialogResultEventArgs e)
		{
			//	Dialog muss bleiben
			e.CancelDialogClosing = true;

			//	Anmelden oder Abmelden?
			if (e.Result == DialogResult.Left) {
				//	Anmelden

				//	kein Benutzer ausgewählt?
				if (string.IsNullOrEmpty(this.SelectedUser)) {
                    new MessageBoxTask("@UserManagement.Results.ChooseUser", "@UserManagement.View.LogOnOff",  MessageBoxIcon.Information);
					return;
				}

				//	Passwort leer?
				if (string.IsNullOrEmpty(this.Password)) {
                    new MessageBoxTask("@UserManagement.Results.EnterPassword", "@UserManagement.View.LogOnOff",  MessageBoxIcon.Information);
					return;
				}

				//	Benutzer anmelden
				LogOnSuccess success = this.userService.LogOn(this.SelectedUser, this.Password);
				if (success != LogOnSuccess.Succeeded) {
					string errorText = 	GetErrorText("LogOnError", "LogOnSuccess", success.ToString());
                    new MessageBoxTask(errorText, "@UserManagement.View.LogOnOff",  MessageBoxIcon.Exclamation);

					//	Ergebnis Passwort ändern?
					if (success == LogOnSuccess.RenewPassword) {
						this.renewPassword = true;
						e.CancelDialogClosing = false;
					}

					return;
				}

				SetError("LogOnOK");
			} else if (e.Result == DialogResult.Right) {
				//	Benutzer abmelden

				if (!this.userService.LogOff()) {
                    new MessageBoxTask("@UserManagement.Results.LogOffError", "@UserManagement.View.LogOnOff",  MessageBoxIcon.Exclamation);
					return;
				}

				SetError("LogOffOK");
			}

			//	fertig, Dialog muss doch nicht bleiben
			e.CancelDialogClosing = false;
		}

		/// <summary>
		/// aktuelles Passwort ändern
		/// </summary>
		public ICommand ChangeCurrentPasswordCommand { get; set; }
		private void OnChangeCurrentPasswordCommandExecuted(object parameter)
		{
			bool result = false;

			ClearError();

			if (string.IsNullOrEmpty(this.CurrentUserName))
				return;

			UserAdapter ua = (UserAdapter) ApplicationService.GetAdapter("UserAdapter");
			if (ua != null)
				result = ua.ShowChangePasswordDialog(this.CurrentUserName);

			SetError((result) ? "ChangePasswordOK" : "ChangePasswordError");
		}

		#endregion Befehle

        #region Bindungsmöglichkeiten

        private ObservableCollection<string> userlist;
        public  ObservableCollection<string> UserList
        {
            get { return userlist;; }
            set { userlist = value; OnPropertyChanged("UserList"); }
        }

		private string status = "";
		public string Status
		{
			get { return this.status; }
			set { this.status = value; OnPropertyChanged("Status"); }
		}

		private bool userIsLoggedOn;
		public bool UserIsLoggedOn
		{
			get { return this.userIsLoggedOn; }
			set { this.userIsLoggedOn = value; OnPropertyChanged("UserIsLoggedOn"); }
		}

		private string selectedUser;
        public string SelectedUser
        {
            get { return this.selectedUser; }
            set 
            {
                if (this.selectedUser != value)
                {
                    this.selectedUser = value;
                    OnPropertyChanged("SelectedUser");

                    if (!string.IsNullOrEmpty(this.password))
                    {
                        this.password = string.Empty;
                        OnPropertyChanged("Password");
                    }
                }
            }
        }

		private string currentUserFullName;
		public string CurrentUserFullName
		{
			get { return this.currentUserFullName; }
			set { this.currentUserFullName = value; OnPropertyChanged("CurrentUserFullName"); }
		}

		private string currentUserName;
		public string CurrentUserName
		{
			get { return this.currentUserName; }
			set { this.currentUserName = value; OnPropertyChanged("CurrentUserName"); }
		}

		private string password;
        public string Password
        {
            get { return this.password; }
            set { this.password = value; OnPropertyChanged("Password"); }
        }
		
		private string machinecode;
        public string MachineCode
        {
            get { return this.machinecode; }
            set { this.machinecode = value; OnPropertyChanged("MachineCode"); }
        }
		
		#endregion

		#region Dialogtextverwaltung

		private void ClearError()
		{
			this.Status = "";
		}

		private void SetError(string szErrorCode)
		{
			if (this.textService == null)
				this.Status = "@UserManagement.Results." + szErrorCode;
			else
				this.Status = this.textService.GetText("@UserManagement.Results." + szErrorCode);
		}

		private void SetError(string szErrorCode, object param)
		{
			if (this.textService == null)
				this.Status = "@UserManagement.Results." + szErrorCode + ": " + param.ToString();
			else
				this.Status = this.textService.GetText("@UserManagement.Results." + szErrorCode, param);
		}

		private string GetErrorText(string error, string errorCode, string errorResult)
		{
			string errorText;
			if (textService == null)
				errorText = string.Format("@UserManagement.Results.{0}: {1}.{2}", error, errorCode, errorResult);
			else
				errorText = /*this.textService.GetText("@UserManagement.Results." + error) + ": " + */this.textService.GetText(string.Format("@UserManagement.Results.{0}.{1}", errorCode, errorResult));
			return errorText;
		}

		#endregion Dialogtextverwaltung
	}
}
