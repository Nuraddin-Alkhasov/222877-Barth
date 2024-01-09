using HMI.Views.DialogRegion;
using VisiWin.ApplicationFramework;
using VisiWin.Helper;
using VisiWin.Language;
using VisiWin.Logging;

namespace HMI.Views.MainRegion.MachineOverview
{
    class SP_Logging
    {
        private readonly ILoggingService loggingService;
        
        string Modul;
        string Station;
        string Tablet;

        public SP_Logging(int _FunctionID)
        {
            ILanguageService textService = ApplicationService.GetService<ILanguageService>();
            this.loggingService = ApplicationService.GetService<ILoggingService>();

            string Modul = ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC Status Platz.Daten von BP.Anlagenteil.Nummer").ToString();
            string Station = ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC Status Platz.Daten von BP.Gewerk im Anlagenteil." + Modul).ToString();
            string Tablet = ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC Status Platz.Daten von BP.Platz im Trockner / Kühlzone.Nummer").ToString();
            string txt = "";

            if ((Modul == "3" && Station == "8") || (Modul == "3" && Station == "9"))
            {
                txt = GetHead(Modul, Station) + " Tablett : " + Tablet;
            }
            else
            {
                txt = GetHead(Modul, Station);
            }
                
            switch (_FunctionID)
            {
                //save
                case 0:
                    txt+= " -> " + textService.GetText("@Logging.Service.Text20");
                    this.loggingService.Log("Service", "ChargeDataChenged", txt, FastDateTime.Now);
                    break;
                //delete
                case 1:
                    txt += " -> " + textService.GetText("@Logging.Service.Text21");
                    this.loggingService.Log("Service", "ChargeDataChenged", txt, FastDateTime.Now);
                    break;
            }
            
        }
        string GetHead(string M, string P)
        {
            string ret_val = "";
            switch (M)
            {
                case "1": ret_val ="Modul 1 - ";
                    switch (P)
                    {
                        case "1": ret_val = ret_val + " Hebekippgerät"; break;
                        case "2": ret_val = ret_val + " Bunkerband"; break;
                        case "3": ret_val = ret_val + " Dosierband"; break;
                        case "4": ret_val = ret_val + " Korbdrehstation"; break;
                        case "5": ret_val = ret_val + " Manipulator"; break;
                        case "6": ret_val = ret_val + " Korbspeicher unten"; break;
                        case "7": ret_val = ret_val + " Korbspeicher oben"; break;
                        case "8": ret_val = ret_val + " Korbwechsel"; break;
                        case "9": ret_val = ret_val + " Korb Auskippstation"; break;
                        case "10": ret_val = ret_val + " Ofenband 1"; break;
                    }
                    break;
                case "2": ret_val = "Modul 2 - ";
                    switch (P)
                    {
                        case "1": ret_val = ret_val + " Forplanet"; break;
                    }
                    break;
                case "3": ret_val = "Modul 3 - ";
                    switch (P)
                    {
                        case "1": ret_val = ret_val + " HVT Fahrwagen Korb"; break;
                        case "2": ret_val = ret_val + " HVT Fahrwagen Tablett"; break;
                        case "3": ret_val = ret_val + " Ofenband 2"; break;
                        case "4": ret_val = ret_val + " Ausschleussen Tablett auskippen"; break;
                        case "5": ret_val = ret_val + " Ausschleusse Band"; break;
                        case "6": ret_val = ret_val + " Box"; break;
                        case "7": ret_val = ret_val + " HNT Fahrwagen Tablett"; break;
                        case "8": ret_val = ret_val + " Trockner"; break;
                        case "9": ret_val = ret_val + " Kühlzone"; break;
                    }
                    break;
            }
            return ret_val;
        }
    }
}
