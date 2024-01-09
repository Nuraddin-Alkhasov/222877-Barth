using HMI.Interfaces;
using HMI.Module;
using HMI.Views.MessageBoxRegion;
using System;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisiWin.Alarm;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Recipe;

namespace HMI.Services
{

    [ExportService(typeof(Service_Protocol))]
    [Export(typeof(Service_Protocol))]
    class Service_Protocol : ServiceBase, IProtocol
    {
        IVariableService VS;
        IVariable new_order;
        IVariable new_charge;
        IVariable bs_protocolS;
        IVariable bs_protocolE;
        IVariable bs_protocolF;
        IVariable ofen_protocolS;
        IVariable ofen_protocolE;
        IVariable ofen_protocolF;
        IVariable end_protocol;
        IAlarmItem[] OLD_TT_Oven;
        IAlarmItem[] OLD_TT_BS;
        ICurrentAlarms2 CurrentAlarmList;

        public Service_Protocol()
        {
            if (ApplicationService.IsInDesignMode)
                return;
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
            VS = ApplicationService.GetService<IVariableService>();
            CurrentAlarmList = ApplicationService.GetService<IAlarmService>().GetCurrentAlarms2();
            OLD_TT_Oven = new IAlarmItem[0];
            OLD_TT_BS = new IAlarmItem[0];

            new_order = VS.GetVariable("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.02 Hebekippgerät.Hubwerk.DB HK Hubwerk HMI.PC.Protkollierung.HK Start");
            new_order.Change += NewOrder;

            new_charge = VS.GetVariable("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.04 Befüllen.03 Korb beladen Drehstation.DB KBD HMI.PC.Protkollierung.Charge generieren");
            new_charge.Change += NewCharge;
            
            bs_protocolS = VS.GetVariable("PLC.PLC.Blocks.4 Modul 2 Beschichtung.00 Allgemein.DB Beschichtung Allgemein HMI.PC.Protokollierung.Charge Start");
            bs_protocolS.Change += BSProtocolS;
            bs_protocolE = VS.GetVariable("PLC.PLC.Blocks.4 Modul 2 Beschichtung.00 Allgemein.DB Beschichtung Allgemein HMI.PC.Protokollierung.Charge Ende");
            bs_protocolE.Change += BSProtocolE;
            bs_protocolF = VS.GetVariable("PLC.PLC.Blocks.4 Modul 2 Beschichtung.00 Allgemein.DB Beschichtung Allgemein HMI.PC.Protokollierung.Sammelstörung");
            bs_protocolF.Change += BSProtocolF;

            ofen_protocolS = VS.GetVariable("PLC.PLC.Blocks.5 Modul 3 Trockner.08 Heizung / Ventilatoren.00 Allgemein.DB Heizung / Ventilatoren Allgemein HMI.PC.Protkollierung.Charge Start");
            ofen_protocolS.Change += OfenProtocolS;
            ofen_protocolE = VS.GetVariable("PLC.PLC.Blocks.5 Modul 3 Trockner.08 Heizung / Ventilatoren.00 Allgemein.DB Heizung / Ventilatoren Allgemein HMI.PC.Protkollierung.Charge Ende");
            ofen_protocolE.Change += OfenProtocolE;
            ofen_protocolF = VS.GetVariable("PLC.PLC.Blocks.5 Modul 3 Trockner.08 Heizung / Ventilatoren.00 Allgemein.DB Heizung / Ventilatoren Allgemein HMI.PC.Protkollierung.Sammelstörung");
            ofen_protocolF.Change += OfenProtocolF;

            end_protocol = VS.GetVariable("PLC.PLC.Blocks.5 Modul 3 Trockner.03 Ausschleussen.03 Ausschleusse Band / Hub / Box / Vereinzelung.Ausschleusse Band.DB Ausschleusse Band HMI.PC.Protkollierung.Ende");
            end_protocol.Change += EndProtocol;

            base.OnLoadProjectCompleted();
        }

        #region - - - NEW ORDER RECORD - - -   

        private void NewOrder(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                if ((bool)e.Value)
                {
                    string MR = ApplicationService.GetVariableValue("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.02 Hebekippgerät.DB HK PD.Header.Maschinenprogramm#STRING40").ToString();
                    string PON = ApplicationService.GetVariableValue("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.02 Hebekippgerät.DB HK PD.Header.Produktionsnummer#STRING40").ToString();
                    string BN = "";
                    string AN = ApplicationService.GetVariableValue("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.02 Hebekippgerät.DB HK PD.Header.Artikelmummer#STRING40").ToString();
                    string User = ApplicationService.GetVariableValue("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.02 Hebekippgerät.DB HK PD.Header.User#STRING40").ToString();

                    Task.Run(() => WriteOrderAsync(MR, PON, BN, AN, User));

                    new_order.Value = false;
                }
            }
        }

        private async Task WriteOrderAsync(string _mr, string _pon, string _bn, string _an, string _user)
        {
            object[] values = await GetRecipeDataAsync(_mr);

            long MR_ID;

            DataTable temp = (new localDBAdapter("SELECT * FROM MachineRecipes WHERE MR = '" + _mr + "' AND Article = '" + values[5] + "' AND C1 = '" + values[6] + "' AND C2 = '" + values[7] + "' AND C3 = '" + values[8] + "' AND C4 = '" + values[9] + "'")).DB_Output();
            if (temp.Rows.Count == 0)
            {
                bool result = (new localDBAdapter("INSERT INTO MachineRecipes (MR, Article, C1, C2, C3, C4) VALUES ('" + _mr + "','" + values[5] + "','" + values[6] + "','" + values[7] + "','" + values[8] + "','" + values[9] + "')")).DB_Input();
                MR_ID = (long)(new localDBAdapter("SELECT * FROM MachineRecipes WHERE MR = '" + _mr + "' AND Article = '" + values[5] + "' AND C1 = '" + values[6] + "' AND C2 = '" + values[7] + "' AND C3 = '" + values[8] + "' AND C4 = '" + values[9] + "'")).DB_Output().Rows[0][0];
            }
            else
            {
                MR_ID = Convert.ToInt32(temp.Rows[0][0]);
            }

            temp = (new localDBAdapter("SELECT * FROM Orders WHERE PON = '" + _pon + "' AND BN = '" + _bn + "' AND AN = '" + _an + "' AND MR_ID = '" + MR_ID.ToString() + "'")).DB_Output();

            if (temp.Rows.Count == 0)
            {
                var a = (new localDBAdapter("INSERT INTO Orders (TimeStamp, PON, BN, AN, MR_ID, Charges, User) VALUES ('" + GetDataTimeNowToFormat() + "','" + _pon + "','" + _bn + "','" + _an + "','" + MR_ID.ToString() + "','0','" + _user + "'); ")).DB_Input();
            }
        }

        #endregion

        #region - - - NEW CHARGE RECORD - - -   

        private void NewCharge(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                if ((bool)new_charge.Value)
                {
                    Task.Run(() => WriteCharge());
                }
                new_charge.Value = false;
            }
        }

        private void WriteCharge()
        {
            string _mr = ApplicationService.GetVariableValue("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.04 Befüllen.03 Korb beladen Drehstation.DB KBD PD.Header.Maschinenprogramm#STRING40").ToString();
            string _pon = ApplicationService.GetVariableValue("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.04 Befüllen.03 Korb beladen Drehstation.DB KBD PD.Header.Produktionsnummer#STRING40").ToString();
            string _bn = "";
            string _an = ApplicationService.GetVariableValue("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.04 Befüllen.03 Korb beladen Drehstation.DB KBD PD.Header.Artikelmummer#STRING40").ToString();
            string _user = ApplicationService.GetVariableValue("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.04 Befüllen.03 Korb beladen Drehstation.DB KBD PD.Header.User#STRING40").ToString();
            string _weight = ApplicationService.GetVariableValue("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.04 Befüllen.03 Korb beladen Drehstation.DB KBD PD.Status.Charge.Chargen Gewicht").ToString();
            _weight = System.Math.Round(double.Parse(_weight), 1).ToString();
            bool temp_OP = (bool)ApplicationService.GetVariableValue("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.04 Befüllen.03 Korb beladen Drehstation.DB KBD PD.Status.Charge.Gewicht wurde Restmengenoptimiert");
            int OP = temp_OP ? 1 : 0;

            DataTable temp = (new localDBAdapter("SELECT Orders.Id, Orders.Charges FROM Orders INNER JOIN MachineRecipes ON Orders.MR_ID = MachineRecipes.ID WHERE Orders.PON = '" + _pon + "' AND Orders.BN = '" + _bn + "' AND Orders.AN = '" + _an + "' AND MachineRecipes.MR = '" + _mr + "';")).DB_Output();
            if (temp.Rows.Count != 0)
            {
                var a = (new localDBAdapter("INSERT INTO Charges (Charge, Optimized, Weight, Order_Id, Start) VALUES (" + (Convert.ToInt32(temp.Rows[0][1]) + 1).ToString() + "," + OP + "," + _weight.Replace(',', '.') + ",'" + temp.Rows[0][0].ToString() + "','" + GetDataTimeNowToFormat() + "');")).DB_Input();
                var b = (new localDBAdapter("UPDATE Orders SET Charges = " + (Convert.ToInt32(temp.Rows[0][1]) + 1).ToString() + " WHERE Id = " + temp.Rows[0][0].ToString() + ";").DB_Input());
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.04 Befüllen.03 Korb beladen Drehstation.DB KBD PD.Status.Charge.Chargen Nummer", (Convert.ToInt32(temp.Rows[0][1]) + 1));
            }

            DataTable temp_2 = (new localDBAdapter("SELECT * FROM ProductionData WHERE DayOfWeek = '" + DateTime.Now.DayOfWeek.ToString() + "';")).DB_Output();
            var c = (new localDBAdapter("UPDATE ProductionData SET TotalWeight = " + (Convert.ToDouble(temp_2.Rows[0][2]) + Convert.ToDouble(_weight)).ToString().Replace(',','.') + " WHERE DayOfWeek = '" + DateTime.Now.DayOfWeek.ToString() + "';").DB_Input());

        }

        #endregion

        #region - - - BS PROTOCOL - - -   

        private void BSProtocolS(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                if ((bool)bs_protocolS.Value)
                {
                    bs_protocolS.Value = false;
                    Task.Run(() => WriteBSProtocol("S"));
                }
            }
        }

        private void BSProtocolE(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                if ((bool)bs_protocolE.Value)
                {
                    bs_protocolE.Value = false;
                    Task.Run(() => WriteBSProtocol("E"));
                }
            }
        }

        private void BSProtocolF(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                if ((bool)bs_protocolF.Value)
                {
                    bs_protocolF.Value = false;
                    Task.Run(() => WriteBSError());
                }
            }
        }

        private void WriteBSProtocol(string C)
        {
            string MR = ApplicationService.GetVariableValue("PLC.PLC.Blocks.4 Modul 2 Beschichtung.00 Allgemein.DB Beschichtung PD.Header.Maschinenprogramm#STRING40").ToString();
            string PON = ApplicationService.GetVariableValue("PLC.PLC.Blocks.4 Modul 2 Beschichtung.00 Allgemein.DB Beschichtung PD.Header.Produktionsnummer#STRING40").ToString();
            string BN = "";
            string AN = ApplicationService.GetVariableValue("PLC.PLC.Blocks.4 Modul 2 Beschichtung.00 Allgemein.DB Beschichtung PD.Header.Artikelmummer#STRING40").ToString();
            string charge_NR = ApplicationService.GetVariableValue("PLC.PLC.Blocks.4 Modul 2 Beschichtung.00 Allgemein.DB Beschichtung PD.Status.Charge.Chargen Nummer").ToString();
            string layer = ApplicationService.GetVariableValue("PLC.PLC.Blocks.4 Modul 2 Beschichtung.00 Allgemein.DB Beschichtung PD.Status.Charge.Aktueller Durchlauf").ToString();

            WriteDateTimeTOCharge(MR, PON, BN, AN, charge_NR, "C" + layer + "_" + C);
        }

        private void WriteBSError()
        {
            IAlarmItem[] TT = CurrentAlarmList.Alarms.Where(x => x.Group.Name == "Alarm" && x.AlarmState == AlarmState.Active && x.Param2 >= 2500 && x.Param2 <= 2999).ToArray();
            if(TT.Length!=0)
                if (!Enumerable.SequenceEqual(TT, OLD_TT_BS))
                {
                    OLD_TT_BS = TT;
                    string MR = ApplicationService.GetVariableValue("PLC.PLC.Blocks.4 Modul 2 Beschichtung.00 Allgemein.DB Beschichtung PD.Header.Maschinenprogramm").ToString();
                    string PON = ApplicationService.GetVariableValue("PLC.PLC.Blocks.4 Modul 2 Beschichtung.00 Allgemein.DB Beschichtung PD.Header.Produktionsnummer").ToString();
                    string BN = ApplicationService.GetVariableValue("PLC.PLC.Blocks.4 Modul 2 Beschichtung.00 Allgemein.DB Beschichtung PD.Header.Batchnummer").ToString();
                    string AN = ApplicationService.GetVariableValue("PLC.PLC.Blocks.4 Modul 2 Beschichtung.00 Allgemein.DB Beschichtung PD.Header.Artikelmummer").ToString();
                    string User = ApplicationService.GetVariableValue("PLC.PLC.Blocks.4 Modul 2 Beschichtung.00 Allgemein.DB Beschichtung PD.Header.User").ToString();
                    string charge_NR = ApplicationService.GetVariableValue("PLC.PLC.Blocks.4 Modul 2 Beschichtung.00 Allgemein.DB Beschichtung PD.Status.Charge.Chargen Nummer").ToString();

                    WriteErrorToCharge(MR, PON, BN, AN, User, charge_NR, TT);
                }
               
        }

        #endregion

        #region - - - Oven PROTOCOL - - -   

        private void OfenProtocolS(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                if ((bool)ofen_protocolS.Value)
                {
                    ofen_protocolS.Value = false;
                    Task.Run(() => WriteOvenProtocolS());
                }
            }
        }
        private void OfenProtocolE(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {

                if ((bool)ofen_protocolE.Value)
                {
                    ofen_protocolE.Value = false;
                    Task.Run(() => WriteOvenProtocolE());
                }

            }
        }
        private void OfenProtocolF(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                if ((bool)ofen_protocolF.Value)
                {
                    ofen_protocolF.Value = false;
                    Task.Run(() => WriteOvenError());
                }
            }
        }

        private void WriteOvenProtocolS()
        {
            string MR = ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[12].Header.Maschinenprogramm#STRING40").ToString();
            string PON = ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[12].Header.Produktionsnummer#STRING40").ToString();
            string BN = "";
            string AN = ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[12].Header.Artikelmummer#STRING40").ToString();
            string charge_NR = ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[12].Status.Charge.Chargen Nummer").ToString();
            string layer = ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[12].Status.Charge.Aktueller Durchlauf").ToString();

            WriteDateTimeTOCharge(MR, PON, BN, AN, charge_NR, "OvenC" + layer + "_S");
        }

        private void WriteOvenProtocolE()
        {
            string MR = ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[30].Header.Maschinenprogramm#STRING40").ToString();
            string PON = ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[30].Header.Produktionsnummer#STRING40").ToString();
            string BN = "";
            string AN = ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[30].Header.Artikelmummer#STRING40").ToString();
            string charge_NR = ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[30].Status.Charge.Chargen Nummer").ToString();
            string layer = ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[30].Status.Charge.Aktueller Durchlauf").ToString();


            WriteDateTimeTOCharge(MR, PON, BN, AN, charge_NR, "OvenC" + layer + "_E");
        }

        private void WriteOvenError()
        {
            IAlarmItem[] TT = CurrentAlarmList.Alarms.Where(x => x.Group.Name == "Alarm" && x.AlarmState == AlarmState.Active && x.Param2 >= 5244 && x.Param2 <= 5371).ToArray();
            if (TT.Length != 0)
                if (!Enumerable.SequenceEqual(TT, OLD_TT_Oven))
                {
                    OLD_TT_Oven = TT;
                    for (int i = 12; i <= 29; i++)
                    {
                        if ((bool)ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[" + i.ToString() + "].Status.Charge.Material vorhanden"))
                        {
                            string MR = ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[" + i.ToString() + "].Header.Maschinenprogramm#STRING40").ToString();

                            string PON = ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[" + i.ToString() + "].Header.Produktionsnummer#STRING40").ToString();
                            string BN = "";
                            string AN = ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[" + i.ToString() + "].Header.Artikelmummer#STRING40").ToString();
                            string User = ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[" + i.ToString() + "].Header.User#STRING40").ToString();
                            string charge_NR = ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[" + i.ToString() + "].Status.Charge.Chargen Nummer").ToString();

                            WriteErrorToCharge(MR, PON, BN, AN, User, charge_NR, TT);
                        }
                    }
                }
        }

        #endregion

        #region - - - END PROTOCOL - - -   

        private void EndProtocol(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                if ((bool)e.Value)
                {
                    string MR = ApplicationService.GetVariableValue("PLC.PLC.Blocks.5 Modul 3 Trockner.03 Ausschleussen.03 Ausschleusse Band / Hub / Box / Vereinzelung.Ausschleusse Band.DB Ausschleusse Band PD.Header.Maschinenprogramm#STRING40").ToString();
                    string PON = ApplicationService.GetVariableValue("PLC.PLC.Blocks.5 Modul 3 Trockner.03 Ausschleussen.03 Ausschleusse Band / Hub / Box / Vereinzelung.Ausschleusse Band.DB Ausschleusse Band PD.Header.Produktionsnummer#STRING40").ToString();
                    string BN = "";
                    string AN = ApplicationService.GetVariableValue("PLC.PLC.Blocks.5 Modul 3 Trockner.03 Ausschleussen.03 Ausschleusse Band / Hub / Box / Vereinzelung.Ausschleusse Band.DB Ausschleusse Band PD.Header.Artikelmummer#STRING40").ToString();
                    string charge_NR = ApplicationService.GetVariableValue("PLC.PLC.Blocks.5 Modul 3 Trockner.03 Ausschleussen.03 Ausschleusse Band / Hub / Box / Vereinzelung.Ausschleusse Band.DB Ausschleusse Band PD.Status.Charge.Chargen Nummer").ToString();

                    Task.Run(() => WriteDateTimeTOCharge(MR, PON, BN, AN, charge_NR, "End"));
                }
                end_protocol.Value = 0;
            }
        }

        #endregion

        private void WriteErrorToCharge(string _mr, string _pon, string _bn, string _an, string _user, string chargeNR, IAlarmItem [] TT )
        {
            DataTable temp1 = (new localDBAdapter("SELECT Orders.Id FROM Orders INNER JOIN MachineRecipes ON Orders.MR_ID = MachineRecipes.ID WHERE Orders.PON = '" + _pon + "' AND Orders.BN = '" + _bn + "' AND Orders.AN = '" + _an + "' AND MachineRecipes.MR = '" + _mr + "';")).DB_Output();
            if (temp1.Rows.Count != 0)
            {
                var a = (new localDBAdapter("UPDATE Charges SET Error = 1 WHERE Charge = " + chargeNR + " AND Order_Id = " + temp1.Rows[0][0].ToString() + ";").DB_Input());

                DataTable temp2 = (new localDBAdapter("SELECT Id FROM Charges WHERE Charge = " + chargeNR + " AND Order_Id = " + temp1.Rows[0][0].ToString() + ";").DB_Output());
                if (chargeNR != "0")
                { 
                    foreach (IAlarmItem AI in TT)
                    {
                        bool result = (new localDBAdapter("INSERT INTO Errors (Charge_Id, Time, Text, User) VALUES ("+ temp2.Rows[0][0].ToString() + ",'" + GetDataTimeNowToFormat() + "','" + AI.Text + "','" + ApplicationService.GetVariableValue("__CURRENT_USER.FULLNAME").ToString() + "')")).DB_Input();
                    }
                }
            }    
        }

        private void WriteDateTimeTOCharge(string _mr, string _pon, string _bn, string _an, string chargeNR, string clmn)
        {
            DataTable temp = (new localDBAdapter("SELECT Orders.Id FROM Orders INNER JOIN MachineRecipes ON Orders.MR_ID = MachineRecipes.ID WHERE Orders.PON = '" + _pon + "' AND Orders.BN = '" + _bn + "' AND Orders.AN = '" + _an + "' AND MachineRecipes.MR = '" + _mr + "';")).DB_Output();
            if (temp.Rows.Count != 0)
            {
                var b = (new localDBAdapter("UPDATE Charges SET " + clmn + " = '" + GetDataTimeNowToFormat() + "' WHERE Charge = " + chargeNR + " AND Order_Id = " + temp.Rows[0][0].ToString() + ";").DB_Input());
            }
        }

        private string GetDataTimeNowToFormat()
        {
            return DateTime.Now.ToString("yyyy-MM-dd") + " "+DateTime.Now.ToLongTimeString();
        }

        private async System.Threading.Tasks.Task<object[]> GetRecipeDataAsync(string RecipeName)
        {
            IRecipeClass MachineRecipe = ApplicationService.GetService<IRecipeService>().GetRecipeClass("MachineRecipe");
            await MachineRecipe.LoadFromFileToBufferAsync(RecipeName);

            object[] values;
            MachineRecipe.GetValues(MachineRecipe.GetVariableNames().ToArray(), out values);
            return values;
        }

        // Hier stehen noch die VisiWin Funktionen zur Verfügung
        protected override void OnUnloadProjectStarted()
        {
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
