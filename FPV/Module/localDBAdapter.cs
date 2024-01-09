using System;
using System.Data;
using System.Data.SQLite;

using HMI.Views.MessageBoxRegion;

namespace HMI.Module
{
    class localDBAdapter
    {
        SQLiteConnection con {get;set;}
        SQLiteCommand cmd { get; set; }
        SQLiteDataAdapter DA { get; set; }

        string sql { get; set; }

        public localDBAdapter(string _sql)
        {
            con = new SQLiteConnection("data source=" + System.IO.Directory.GetCurrentDirectory() + "\\DB\\localDB.db");
            sql = _sql;
        }
       public bool DB_Input()
        {

            try
            {
                con.Open();
                cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception e)
            {
                new MessageBoxTask(e, sql, MessageBoxIcon.Error);
                return false;
            }

        }

        public DataTable DB_Output()
        {
            DataTable DT = new DataTable();
            try
            {
                con.Open();
                cmd = con.CreateCommand();

                DA = new SQLiteDataAdapter(sql, con);
                DA.Fill(DT);
                con.Close();
            }
            catch (Exception e)
            {
                new MessageBoxTask(e, sql, MessageBoxIcon.Error);
            }
            return DT;
        }
    }
}
