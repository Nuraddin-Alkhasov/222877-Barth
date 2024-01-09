using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.Recipe
{
    [ExportView("Recipe_Coating_StepsName")]
    public partial class Recipe_Coating_StepsName : VisiWin.Controls.View
    {
        IRecipeClass CS = ApplicationService.GetService<IRecipeService>().GetRecipeClass("CoatingSteps");
        string Recipe_Name;
        string Recipe_Descr;
        int StepID;
        int StepNR;
        string RPrevValue;
        IRegionService iRS;
        Recipe_Coating_PR RC_PR;

        public Recipe_Coating_StepsName(int _StepNr)
        {
            StepNR = _StepNr;
            iRS = ApplicationService.GetService<IRegionService>();
            RC_PR = (Recipe_Coating_PR)iRS.GetView("Recipe_Coating_PR");
            this.InitializeComponent();
        }
        public Recipe_Coating_StepsName(string _name, string _descr, int _stepID, int _StepNr)
        {
           // CS.StartEdit();
            Recipe_Name = _name;
            Recipe_Descr = _descr;
            StepID = _stepID;
            StepNR = _StepNr;
            iRS = ApplicationService.GetService<IRegionService>();
            RC_PR = (Recipe_Coating_PR)iRS.GetView("Recipe_Coating_PR");
            this.InitializeComponent();
        }

        private void SetSymbol(int sID)
        {
            switch (sID)
            {
                case 0:
                    RBName.SymbolResourceKey = "";
                    RBName.Text = "";
                    break;
                case 1:
                    RBName.SymbolResourceKey = "DipingS";
                    RBName.LocalizableLabelText = "@Rezeptverwaltung.BSSchritte.lblTauchen" + (StepNR + 1);
                    break;
                case 2:
                    RBName.SymbolResourceKey = "SpinS";
                    RBName.LocalizableLabelText = "@Rezeptverwaltung.BSSchritte.lblSchleidern" + (StepNR + 1);
                    break;
                case 3:
                    RBName.SymbolResourceKey = "RollingS";
                    RBName.LocalizableLabelText = "@Rezeptverwaltung.BSSchritte.lblWaelzen" + (StepNR + 1);
                    break;
            }
        }

        private void name_ValueChanged(object sender, VisiWin.DataAccess.VariableEventArgs e)
        {
            if(RName.Value != RPrevValue)
            {
                if (RName.Value != "" )
                {
                    object _id;
                    CS.GetValue(RName.VariableName + "_ID", out _id);
                    loadStep(_id.ToString());
                }
                else
                {
                    StepID = 0;
                    RDescr.Value = "";
                }
                RC_PR.BlinkONOFF();
                SetSymbol(StepID);
                RPrevValue = RName.Value;
            }
        }

        private void RDel_Click(object sender, RoutedEventArgs e)
        {
            StepID = 0;
            RBName.LocalizableLabelText = "@Rezeptverwaltung.BSSchritte.Text1";
            RName.Value = "";
            RDescr.Value = "";
            RBName.IsChecked = true;
        }

        private void loadStep(string sID)
        {
            switch (sID)
            {
                case "1":
                    RDescr.Value = ApplicationService.GetService<IRecipeService>().GetRecipeClass("D").GetRecipeFile(RName.Value).Description;
                    StepID = 1;
                    break;
                case "2":
                    RDescr.Value = ApplicationService.GetService<IRecipeService>().GetRecipeClass("S").GetRecipeFile(RName.Value).Description;
                    StepID = 2;
                    break;
                case "3":
                    RDescr.Value = ApplicationService.GetService<IRecipeService>().GetRecipeClass("R").GetRecipeFile(RName.Value).Description;
                    StepID = 3;
                    break;
            }
        }

        public void Step_Switcher(int sID)
        {
                object _rname;
                object _rID;
                CS.GetValue("Recipe.CatingSteps.Step" + (sID+1), out _rname);
                CS.GetValue("Recipe.CatingSteps.Step" + (sID+1) + "_ID", out _rID);

                CS.SetValue("Recipe.CatingSteps.Step" + (sID + 1) + "_ID", StepID);
                CS.SetValue("Recipe.CatingSteps.Step" + (sID + 1), RName.Value);

                CS.SetValue("Recipe.CatingSteps.Step" + (StepNR+1).ToString() + "_ID", _rID);
                CS.SetValue("Recipe.CatingSteps.Step" + (StepNR+1).ToString(), _rname);
        }

        public void BlinkON()
        {
            RName.IsBlinkEnabled = true;
            RDescr.IsBlinkEnabled = true;
            RBName.IsChecked = true;
        }

        public void BlinkOFF()
        {
            RName.IsBlinkEnabled = false;
            RDescr.IsBlinkEnabled = false;
        }

        private void RBName_Checked(object sender, RoutedEventArgs e)
        {
            if (RBName.IsChecked == true)
            {
                RC_PR.SelectionChanged(StepNR);
            } 
        }

        private void View_Initialized(object sender, System.EventArgs e)
        {
            RName.VariableName = "Recipe.CoatingSteps.Step" + (StepNR + 1).ToString();
            CS.SetValue("Recipe.CoatingSteps.Step" + (StepNR + 1).ToString(), Recipe_Name);
            CS.SetValue("Recipe.CoatingSteps.Step" + (StepNR + 1).ToString() + "_ID", StepID);
            RDescr.Value = Recipe_Descr;
            SetSymbol(StepID);
        }
    }
}