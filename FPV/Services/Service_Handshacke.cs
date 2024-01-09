using HMI.Interfaces;
using HMI.Views.MessageBoxRegion;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Recipe;

namespace HMI.Services
{
    [ExportService(typeof(IHandshacke))] 
    [Export(typeof(IHandshacke))]
    public class Service_Handshacke : ServiceBase, IHandshacke
    {
        IVariableService VS;
        IVariable DataToPC ;
        IVariable Freez;
        IVariable RnL;
        IVariable RL;
        IVariable RHK;
        IRecipeClass CS;
        IRecipeClass C;
        BackgroundWorker loadArticle;
        BackgroundWorker loadC1;
        BackgroundWorker loadC2;
        BackgroundWorker loadC3;
        BackgroundWorker loadC4;
        DispatcherTimer _timer;
        public Service_Handshacke()
        {
            if (ApplicationService.IsInDesignMode)
                return;
        }

        private void DataToPC_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                if (VisiWin.Helper.BitHelper.GetBit((Int16)e.Value, 9))
                {
                    DataToPC.SetBit(9, false);
                    loadArticle.RunWorkerAsync();
                  
                }
                if (VisiWin.Helper.BitHelper.GetBit((Int16)e.Value, 10))
                {
                    DataToPC.SetBit(10, false);
                    loadC1.RunWorkerAsync();
                }
                if (VisiWin.Helper.BitHelper.GetBit((Int16)e.Value, 11))
                {
                    DataToPC.SetBit(11, false);
                    loadC2.RunWorkerAsync();
                }

                if (VisiWin.Helper.BitHelper.GetBit((Int16)e.Value, 12))
                {
                    DataToPC.SetBit(12, false);
                    loadC3.RunWorkerAsync();
                }
                if (VisiWin.Helper.BitHelper.GetBit((Int16)e.Value, 13))
                {
                    DataToPC.SetBit(13, false);
                    loadC4.RunWorkerAsync();
                }

                if (VisiWin.Helper.BitHelper.GetBit((Int16)e.Value, 14))
                {
                    DataToPC.SetBit(14, false);
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Recipe loaded", true);
                    ApplicationService.SetVariableValue("Temp.RecipeIsLaoding", false);
                }
            }
            
        }

        private void Freez_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                if ((bool)e.Value)
                {
                    TimeSpan _time = TimeSpan.FromSeconds(15);

                    _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
                    {
                        if (_time == TimeSpan.Zero) {Freez.Value = false; new MessageBoxTask("Ladezeit überschritten.", "Rezept laden fehler", MessageBoxIcon.Exclamation); _timer.Stop(); }
                            _time = _time.Add(TimeSpan.FromSeconds(-1));
                    }, Application.Current.Dispatcher);

                    _timer.Start();
                }
                else
                {
                    _timer.Stop();
                }
            }

        }

        private async Task<int> LoadFromFIleTOPRocessMRAsync()
        {
            IRecipeClass TT = ApplicationService.GetService<IRecipeService>().GetRecipeClass("MachineRecipe");
            await TT.LoadFromFileToProcessAsync(ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Header.Maschinenprogramm#STRING40").ToString());

            string MR_Name = ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Main Recipe.Machine Recipe").ToString();

            IRecipeClass T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("Article");
            if ((await T.LoadFromFileToProcessAsync(MR_Name)).Result != SendRecipeResult.Succeeded)
            {
                return 0;
            }
            return 1;
        }

        private async Task<int> LoadFromFileTOPRocessC1Async()
        {
           return await CoatingProgramLoader(ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Main Recipe.C1").ToString());
        }

        private async Task<int> LoadFromFileTOPRocessC2Async()
        {
            string MR_Name = ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Main Recipe.C2").ToString();
            return (MR_Name.Length > 2) ? (await CoatingProgramLoader(MR_Name)) : 2;
        }

        private async Task<int> LoadFromFileTOPRocessC3Async()
        {
            string MR_Name = ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Main Recipe.C3").ToString();
            return (MR_Name.Length > 2) ? (await CoatingProgramLoader(MR_Name)) : 2;
        }

        private async Task<int> LoadFromFileTOPRocessC4Async()
        {
            string MR_Name = ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Main Recipe.C4").ToString();
            return (MR_Name.Length > 2) ? (await CoatingProgramLoader(MR_Name)) : 2;
        }

        #region OnProject

        private async Task<int> CoatingProgramLoader(string R_Name)
        {
            if ((await CS.LoadFromFileToProcessAsync(R_Name)).Result != SendRecipeResult.Succeeded)
            {
                return 0;
            }
            else
            {
                await C.SetDefaultValuesToBufferAsync();

                C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Lacktyp", ApplicationService.GetVariableValue("Recipe.CoatingSteps.Lacktyp"));

                for (int i = 1; i < 9; i++)
                {
                    string MR_Name = ApplicationService.GetVariableValue("Recipe.CoatingSteps.Step" + i.ToString()).ToString();

                    int MR_Type = Convert.ToInt16(ApplicationService.GetVariableValue("Recipe.CoatingSteps.Step" + i.ToString() + "_ID"));
                    if (MR_Name != "")
                    {
                        IRecipeClass T;

                        switch (MR_Type)
                        {
                            case 1:
                                T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("D");

                                if (!T.IsExistingRecipeFile(MR_Name))
                                {
                                    return 0;
                                }
                                else
                                {
                                    var r_val = T.GetRecipeFile(MR_Name).GetValues();

                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen / Schleudern / Wälzen", 1);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.LTB Hub Geschw", r_val["Recipe.D.LTB Hub Geschw"]);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.Rpm Körbe austauchen", r_val["Recipe.D.Rpm Körbe austauchen"]);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.Rpm Körbe eintauchen", r_val["Recipe.D.Rpm Körbe eintauchen"]);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.Rpm Körbe tauchen", r_val["Recipe.D.Rpm Körbe tauchen"]);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.Reversierzeit", r_val["Recipe.D.Reversierzeit"]);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.Tauchzeit", r_val["Recipe.D.Tauchzeit"]);

                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Planet Drehzahl 2", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Planet Drehzeit 2", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Rotor Drehzahl 1", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Rotor Drehzahl 2", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Rotor Drehzahl 3", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Rotor Drehzeit 1", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Rotor Drehzeit 3", 0);

                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Wälzen.Kippwinkel", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Wälzen.Korbdrehzahl", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Wälzen.Reversierzeit", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Wälzen.Wälzzeit", 0);


                                }
                                break;
                            case 2:
                                T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("S");

                                if (!T.IsExistingRecipeFile(MR_Name))
                                {
                                    return 0;
                                }
                                else
                                {
                                    var r_val = T.GetRecipeFile(MR_Name).GetValues();

                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.LTB Hub Geschw", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.Rpm Körbe austauchen", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.Rpm Körbe eintauchen", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.Rpm Körbe tauchen", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.Reversierzeit", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.Tauchzeit", 0);


                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen / Schleudern / Wälzen", 2);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Planet Drehzahl 2", r_val["Recipe.S.Planet Drehzahl 2"]);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Planet Drehzeit 2", r_val["Recipe.S.Planet Drehzeit 2"]);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Rotor Drehzahl 1", r_val["Recipe.S.Rotor Drehzahl 1"]);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Rotor Drehzahl 2", r_val["Recipe.S.Rotor Drehzahl 2"]);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Rotor Drehzahl 3", r_val["Recipe.S.Rotor Drehzahl 3"]);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Rotor Drehzeit 1", r_val["Recipe.S.Rotor Drehzeit 1"]);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Rotor Drehzeit 3", r_val["Recipe.S.Rotor Drehzeit 3"]);

                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Wälzen.Kippwinkel", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Wälzen.Korbdrehzahl", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Wälzen.Reversierzeit", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Wälzen.Wälzzeit", 0);
                                }
                                break;
                            case 3:
                                T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("R");

                                if (!T.IsExistingRecipeFile(MR_Name))
                                {
                                    return 0;
                                }
                                else
                                {
                                    var r_val = T.GetRecipeFile(MR_Name).GetValues();
                                    
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.LTB Hub Geschw", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.Rpm Körbe austauchen", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.Rpm Körbe eintauchen", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.Rpm Körbe tauchen", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.Reversierzeit", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen.Tauchzeit", 0);

                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Planet Drehzahl 2", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Planet Drehzeit 2", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Rotor Drehzahl 1", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Rotor Drehzahl 2", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Rotor Drehzahl 3", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Rotor Drehzeit 1", 0);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Schleudern.Rotor Drehzeit 3", 0);

                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Tauchen / Schleudern / Wälzen", 3);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Wälzen.Kippwinkel", r_val["Recipe.R.Kippwinkel"]);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Wälzen.Korbdrehzahl", r_val["Recipe.R.Korbdrehzahl"]);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Wälzen.Reversierzeit", r_val["Recipe.R.Reversierzeit"]);
                                    C.SetValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Temp C.Step[" + i.ToString() + "].Wälzen.Wälzzeit", r_val["Recipe.R.Wälzzeit"]);
                                }
                                break;
                            default: break;
                        }
                    }
 
                if ((await C.WriteBufferToProcessAsync()).Result != SetRecipeResult.Succeeded)
                {
                    return 0;
                }
            }
            return 1;
            }
        }
        // Hier stehen noch keine VisiWin Funktionen zur Verfügung
        protected override void OnLoadProjectStarted()
        {
            base.OnLoadProjectStarted();
        }

        // Hier kann auf die VisiWin Funktionen zugegriffen werden
        protected override void OnLoadProjectCompleted()
        {
            IRecipeClass T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("MachineRecipe");
            T.StartEdit();
            T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("Article");
            T.StartEdit();
            T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("CoatingSteps");
            T.StartEdit();
            T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("D");
            T.StartEdit();
            T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("R");
            T.StartEdit();
            T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("S");
            T.StartEdit();

            VS = ApplicationService.GetService<IVariableService>();
            DataToPC = VS.GetVariable("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data to PC.Variable");
            DataToPC.Change += DataToPC_Change;

            Freez = VS.GetVariable("Temp.RecipeIsLaoding");
            Freez.Change += Freez_Change;

            CS = ApplicationService.GetService<IRecipeService>().GetRecipeClass("CoatingSteps");
            C = ApplicationService.GetService<IRecipeService>().GetRecipeClass("Coating");
            C.StartEdit();

            loadArticle = new BackgroundWorker();
            loadArticle.DoWork += W1_DoWork;
            loadArticle.ProgressChanged += W1_DoWorkProgressChanged;
            loadArticle.WorkerReportsProgress = true;

            loadC1 = new BackgroundWorker();
            loadC1.DoWork += W2_DoWork;
            loadC1.ProgressChanged += W2_DoWorkProgressChanged;
            loadC1.WorkerReportsProgress = true;

            loadC2 = new BackgroundWorker();
            loadC2.DoWork += W3_DoWork;
            loadC2.ProgressChanged += W3_DoWorkProgressChanged;
            loadC2.WorkerReportsProgress = true;

            loadC3 = new BackgroundWorker();
            loadC3.DoWork += W4_DoWork;
            loadC3.ProgressChanged += W4_DoWorkProgressChanged;
            loadC3.WorkerReportsProgress = true;

            loadC4 = new BackgroundWorker();
            loadC4.DoWork += W5_DoWork;
            loadC4.ProgressChanged += W5_DoWorkProgressChanged;
            loadC4.WorkerReportsProgress = true;

            RnL = VS.GetVariable("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Recipe not loaded");
            RnL.Change += RnL_Change;

            RL = VS.GetVariable("PLC.PLC.Blocks.Standard Bausteine.Test.Loaded MR.Fehler im Durchlauf");
            RL.Change += RL_Change;

            RHK = VS.GetVariable("PLC.PLC.Blocks.Standard Bausteine.Test.HK.Fehler im Durchlauf");
            RHK.Change += RHK_Change;

            base.OnLoadProjectCompleted();
        }

        private void RHK_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && (short)e.Value!=0)
            {
                string Message = "Fehler im Rezept" + Environment.NewLine + " * * * * * " + Environment.NewLine;
                Message += (bool)ApplicationService.GetVariableValue("PLC.PLC.Blocks.Standard Bausteine.Test.HK.Fehler beim Tauchen") ? "beim : Tauchen!" : "";
                Message += (bool)ApplicationService.GetVariableValue("PLC.PLC.Blocks.Standard Bausteine.Test.HK.Fehler beim Wälzen") ? "beim : Wälzen!" : "";
                Message += (bool)ApplicationService.GetVariableValue("PLC.PLC.Blocks.Standard Bausteine.Test.HK.Fehler beim Schleudern") ? "beim : Schleudern!" : "";

                Message += Environment.NewLine + "Schritt Nr: " + ApplicationService.GetVariableValue("PLC.PLC.Blocks.Standard Bausteine.Test.HK.Frehler im Step").ToString();
                Message += Environment.NewLine + "Beschichtungs Programm Nr: " + e.Value.ToString();
                Message += Environment.NewLine + " * * * * * " + Environment.NewLine + "Bitte kontaktieren Sie ein Softwareentwikler von Firma Forplan Technology AG.";

                new MessageBoxTask(Message, "Rezept laden fehler", MessageBoxIcon.Exclamation);
            }
        }

        private void RL_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue && (short)e.Value != 0)
            {
                string Message = "Fehler im Rezept" + Environment.NewLine + " * * * * * " + Environment.NewLine;
                var a = (bool)ApplicationService.GetVariableValue("PLC.PLC.Blocks.Standard Bausteine.Test.Loaded MR.Fehler beim Tauchen");
                Message += (bool)ApplicationService.GetVariableValue("PLC.PLC.Blocks.Standard Bausteine.Test.Loaded MR.Fehler beim Tauchen") ? "beim : Tauchen!" : "";
                Message += (bool)ApplicationService.GetVariableValue("PLC.PLC.Blocks.Standard Bausteine.Test.Loaded MR.Fehler beim Wälzen") ? "beim : Wälzen!" : "";
                Message += (bool)ApplicationService.GetVariableValue("PLC.PLC.Blocks.Standard Bausteine.Test.Loaded MR.Fehler beim Schleudern") ? "beim : Schleudern!" : "";

                Message += Environment.NewLine + "Schritt Nr: " + ApplicationService.GetVariableValue("PLC.PLC.Blocks.Standard Bausteine.Test.Loaded MR.Frehler im Step").ToString();
                Message += Environment.NewLine + "Beschichtungs Programm Nr: " + e.Value.ToString();
                Message += Environment.NewLine + " * * * * * " + Environment.NewLine + "Bitte kontaktieren Sie ein Softwareentwikler von Firma Forplan Technology AG.";
                new MessageBoxTask(Message, "Rezept laden fehler", MessageBoxIcon.Exclamation);
            }
        }

        private void RnL_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                if ((bool)e.Value)
                {
                    new MessageBoxTask("Error beim Rezept laden. Bitte kontaktieren Sie ein Softwareentwikler von Firma Forplan Technology AG.", "Rezept laden fehler", MessageBoxIcon.Exclamation);
                }
            }
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

        private void W1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ApplicationService.SetVariableValue("Temp.RecipeIsLaoding", true);
            try {
            loadArticle.ReportProgress(LoadFromFIleTOPRocessMRAsync().Result);
            }
            catch { loadArticle.ReportProgress(0); }
        }

        private void W1_DoWorkProgressChanged(object sender, ProgressChangedEventArgs e)
            {
                switch (e.ProgressPercentage)
                {
                    case 1 :
                        ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Coating number", 0);
                        ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.MR loaded", true);
                        break;
                    case 0 :
                        ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Recipe not loaded", true);
                        ApplicationService.SetVariableValue("Temp.RecipeIsLaoding", false);
                        new MessageBoxTask("Error beim Artikel laden.", "Rezept laden fehler", MessageBoxIcon.Exclamation);
                        break;
                    default: break;
                }
            }

        private void W2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try { loadC1.ReportProgress(LoadFromFileTOPRocessC1Async().Result); }
            catch { loadC1.ReportProgress(0); }
        }

        private void W2_DoWorkProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 1:
                        ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Coating number", 1);
                        ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.CR loaded", true);
                        break;
                case 0:
                        ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Recipe not loaded", true);
                        ApplicationService.SetVariableValue("Temp.RecipeIsLaoding", false);
                        new MessageBoxTask("Error beim C1 laden.", "Rezept laden fehler", MessageBoxIcon.Exclamation);
                        break;
                default: break;
            }
        }

        private void W3_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try { loadC2.ReportProgress(LoadFromFileTOPRocessC2Async().Result); }
            catch { loadC2.ReportProgress(0); }
        }

        private void W3_DoWorkProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 2:
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Recipe loaded", true);
                    ApplicationService.SetVariableValue("Temp.RecipeIsLaoding", false);
                    break;
                case 1:
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Coating number", 2);
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.CR loaded", true);
                    break;
                case 0:
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Recipe not loaded", true);
                    ApplicationService.SetVariableValue("Temp.RecipeIsLaoding", false);
                    new MessageBoxTask("Error beim C2 laden.", "Rezept laden fehler", MessageBoxIcon.Exclamation);
                    break;
                default: break;
            }
        }

        private void W4_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try { loadC3.ReportProgress(LoadFromFileTOPRocessC3Async().Result); }
            catch { loadC3.ReportProgress(0); }
        }

        private void W4_DoWorkProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 2:
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Recipe loaded", true);
                    ApplicationService.SetVariableValue("Temp.RecipeIsLaoding", false);
                    break;
                case 1:
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Coating number", 3);
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.CR loaded", true);
                    break;
                case 0:
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Recipe not loaded", true);
                    ApplicationService.SetVariableValue("Temp.RecipeIsLaoding", false);
                    new MessageBoxTask("Error beim C3 laden.", "Rezept laden fehler", MessageBoxIcon.Exclamation);
                    break;
                default: break;
            }
        }

        private void W5_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try { loadC4.ReportProgress(LoadFromFileTOPRocessC4Async().Result); }
            catch { loadC4.ReportProgress(0); }
        }

        private void W5_DoWorkProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 2:
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Recipe loaded", true);
                    ApplicationService.SetVariableValue("Temp.RecipeIsLaoding", false);
                    break;
                case 1:
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Coating number", 4);
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.CR loaded", true);
                    break;
                case 0:
                    ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Recipe not loaded", true);
                    ApplicationService.SetVariableValue("Temp.RecipeIsLaoding", false);
                    new MessageBoxTask("Error beim C4 laden.", "Rezept laden fehler", MessageBoxIcon.Exclamation);
                    break;
                default: break;
            }
        }

        #endregion

    }
}
