using EKSEthLib;
using HMI.Interfaces;
using System;
using System.ComponentModel.Composition;
using System.Security.Cryptography;
using System.Text;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.UserManagement;

namespace HMI.Services
{
    [ExportService(typeof(IEKS))]
    [Export(typeof(IEKS))]
    public class Service_EKS : ServiceBase, IEKS
    {

        private EKSETH EKS;
        IUserManagementService userService = ApplicationService.GetService<IUserManagementService>();

        private readonly string IP = "192.168.3.13";
        private readonly string port = "2444";

        private readonly string key = "1234567891234567";

        public string Status { set; get; } = "";

        public Service_EKS()
        {
            if (ApplicationService.IsInDesignMode)
                return;
        }


        public void OpenConnection()
        {
            EKS = new EKSETH
            {
                IPAddress = IP,
                Port = port,
                KeyType = KeyType_def.EKS_KEY_READWRITE,
                StartAdress = 0,
                CountData = 12
            };
            EKS.OnKey += EKS_OnKey;

            if (EKS.Open())
            {
                Status = "Connection opened successfully";
            }
            else
            {
                Status = "Can not open the connection";
            }
        }

        public void CloseConnection()
        {
            if (EKS != null)
            {
                if (EKS.Close())
                {
                    Status = "Connection Closed successfully";
                }
                else
                {
                    Status = "Can not close the connection";
                }
            }
        }

        public string CheckConnection()
        {
            return EKS.LastState.ToString() + " - Check the code in EKS manual.";
        }

        private void EKS_OnKey()
        {

            if (EKS.KeyState == KeyState_def.EKS_KEY_IN)
            {
                userService.LogOn(null, null, Read());
                Status = Read() + " - is in EKS";
            }
            else if (EKS.KeyState == KeyState_def.EKS_KEY_OUT)
            {
                userService.LogOff();
                Status = "No Token in EKS";
            }
        }

        public string Read()
        {
            if (EKS.KeyState == KeyState_def.EKS_KEY_IN)
            {
                byte[] encrData = new byte[12];
                for (short i = 0; i < 12; i++)
                {
                    encrData[i] = Convert.ToByte(EKS.getData(i));
                }
                try
                {
                    Status = "Read";
                    return Decrypt(encrData);
                }
                catch
                {
                    Status = "Problem on read";
                    return "";
                }
            }
            else
            {
                Status = "Problem on read (No Token in EKS)";
                return "";
            }
        }

        public void Write(string data)
        {
            if (EKS.KeyState == KeyState_def.EKS_KEY_IN)
            {
                string a = Encrypt(data);
                byte[] encrData = UTF8Encoding.UTF8.GetBytes(a);

                for (short i = 0; i < 12; i++)
                {
                    EKS.setData(i, encrData[i]);

                }

                EKS.Write();
                Status = "Data written";
            }
        }

        private string Encrypt(string toEncrypt)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private string Decrypt(byte[] Data)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

            byte[] toEncryptArray = Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(Data, 0, Data.Length));

            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public string GetStatus()
        {
            return Status;
        }

        #region OnProject


        // Hier stehen noch keine VisiWin Funktionen zur Verfügung
        protected override void OnLoadProjectStarted()
        {
            base.OnLoadProjectStarted();
        }


        // Hier kann auf die VisiWin Funktionen zugegriffen werden
        protected override void OnLoadProjectCompleted()
        {
            OpenConnection();
            userService.UserLoggedOn += UserService_UserLoggedOn;
            userService.UserLoggedOff += UserService_UserLoggedOff;
            base.OnLoadProjectCompleted();
        }

        private void UserService_UserLoggedOn(object sender, LogOnEventArgs e)
        {
            bool a = e.CurrentUser.RightNames.Contains("MOP Bediener");
            bool b = e.CurrentUser.RightNames.Contains("MOP Einrichten");
            if (a)
            {
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB Benutzer Sync.Benutzer Level.Bediener", true);
            }

            if (b)
            {
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB Benutzer Sync.Benutzer Level.Einrichten", true);
            }

            if (a || b)
            {
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB Benutzer Sync.Anmelden", true);
            }
        }

        private void UserService_UserLoggedOff(object sender, LogOffEventArgs e)
        {
            ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB Benutzer Sync.Abmelden", true);
        }

        // Hier stehen noch die VisiWin Funktionen zur Verfügung
        protected override void OnUnloadProjectStarted()
        {
            CloseConnection();
            base.OnUnloadProjectStarted();
        }

        // Hier sind keine VisiWin Funktionen mehr verfügbar. Bei C/S ist die Verbindung zum Server schon getrennt.
        protected override void OnUnloadProjectCompleted()
        {
            base.OnUnloadProjectCompleted();
        }

      



        #endregion

    }
}
