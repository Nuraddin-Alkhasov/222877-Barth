namespace HMI.Interfaces
{
    interface IBarcode
    {
        void OpenConnection();
        void CloseConnection();
        string CheckConnection();
 
    }
}
