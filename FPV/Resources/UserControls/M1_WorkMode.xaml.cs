using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;

namespace HMI.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class M1_WorkMode : UserControl
    {

        public string TouchClick_WM = "PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Modul 1 Mani ZF Status an BP.Anzeige_Betriebsart";
        public string var_Automaric = "PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Zufürhung Mani Korbauskippen.Automatik";
        public string var_Manual = "PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Zufürhung Mani Korbauskippen.Hand";
        public string var_SetUP = "PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Zufürhung Mani Korbauskippen.Einrichten";
        public string var_START = "PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Zufürhung Mani Korbauskippen.Automatik_Start";
        public string var_STOP= "PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Zufürhung Mani Korbauskippen.Automatik_Stop";

        ColorAnimation CA = new ColorAnimation() {Duration = new Duration(TimeSpan.FromSeconds(0.5))};
        IVariableService VS = ApplicationService.GetService<IVariableService>();
        IVariable WM;

        public M1_WorkMode()
        {
            InitializeComponent();
            
        }

        private void WM_Change(object sender, VariableEventArgs e)
        {
                this.IsEnabled = true;
                DoWork((short)e.Value);
        }


        private void WorkingMode_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            switch (Convert.ToInt32(ApplicationService.GetVariableValue(TouchClick_WM)))
            {
                case 1: SetUP(); CA.To = (Color)ColorConverter.ConvertFromString("#FFBC49"); break;
                case 2: Automaric(); CA.To = (Color)ColorConverter.ConvertFromString("#FF025502"); break;
                case 3: Manunal(); CA.To = (Color)ColorConverter.ConvertFromString("#FF006AAD"); break;
            }
        }

        private void WorkingMode_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            switch (Convert.ToInt32(ApplicationService.GetVariableValue(TouchClick_WM)))
            {
                case 1: SetUP(); CA.To = (Color)ColorConverter.ConvertFromString("#FFBC49"); break;
                case 2: Automaric(); CA.To = (Color)ColorConverter.ConvertFromString("#FF025502"); break;
                case 3: Manunal(); CA.To = (Color)ColorConverter.ConvertFromString("#FF006AAD"); break;
            }
        }

        private void DoWork(int a)
        {
            switch (a)
            {
                case 0:
                    //no Mode
                    lbl.LocalizableText = "@MainOverview.Text22";
                    btnstart.Visibility = Visibility.Hidden;
                    btnstop.Visibility = Visibility.Hidden;
                    CA.To = (Color)ColorConverter.ConvertFromString("#FFBE2828");
                    break;
                case 1:
                    //hand Mode
                    lbl.LocalizableText = "@MainOverview.Text20";
                    btnstart.Visibility = Visibility.Hidden;
                    btnstop.Visibility = Visibility.Hidden;
                    CA.To = (Color)ColorConverter.ConvertFromString("#FF006AAD");
                    break;
                case 2:
                    //setUP Mode
                    lbl.LocalizableText = "@MainOverview.Text21";
                    btnstart.Visibility = Visibility.Hidden;
                    btnstop.Visibility = Visibility.Hidden;
                    CA.To = (Color)ColorConverter.ConvertFromString("#FFBC49");
                    break;
                case 3:
                    //Automatic
                    lbl.LocalizableText = "@MainOverview.Text19";
                    btnstart.Visibility = Visibility.Visible;
                    btnstop.Visibility = Visibility.Visible;
                    CA.To = (Color)ColorConverter.ConvertFromString("#FF025502");
                    break;
            }
            BtnShadow1.Visibility = Visibility.Visible;
            this.WorkingMode.Fill.BeginAnimation(SolidColorBrush.ColorProperty, CA);
        }

        private void lbl_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            BtnShadow1.Visibility = Visibility.Hidden;
        }

        private void Manunal()
        {
            ApplicationService.SetVariableValue(var_Automaric, false);
            ApplicationService.SetVariableValue(var_Manual, true);
            ApplicationService.SetVariableValue(var_SetUP, false);
        }

        private void SetUP()
        {
            ApplicationService.SetVariableValue(var_Automaric, false);
            ApplicationService.SetVariableValue(var_Manual, false);
            ApplicationService.SetVariableValue(var_SetUP, true);
        }

        private void Automaric()
        {
            ApplicationService.SetVariableValue(var_Automaric, true);
            ApplicationService.SetVariableValue(var_Manual, false);
            ApplicationService.SetVariableValue(var_SetUP, false);
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
           
            WM = VS.GetVariable(TouchClick_WM);
            WM.Change += WM_Change;
            btnstart.VariableName = var_START;
            btnstop.VariableName = var_STOP;
        }
    }
}
