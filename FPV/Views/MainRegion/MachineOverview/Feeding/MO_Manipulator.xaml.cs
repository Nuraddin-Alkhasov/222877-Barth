using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using HMI.Views.DialogRegion;
using VisiWin.Controls;
using System;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_Manipulator")]
    public partial class MO_Manipulator : VisiWin.Controls.View
    {

        IVariableService VS = ApplicationService.GetService<IVariableService>();
        IVariable WM;

        public MO_Manipulator()
        {
            InitializeComponent();
        }

        private void View_Initialized(object sender, System.EventArgs e)
        {
            WM = VS.GetVariable("PLC.PLC.Blocks.3 Modul 1 ZF MA Auskippen.08 Manipulator.00 Allgemein.DB Mani Allgemein HMI.PC.Status Bildanzeige Mani");
            WM.Change += WM_Change;
        }
        private void WM_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                switch ((short)e.Value)
                {
                    case 3: mani_B.Margin = new Thickness(843, 568, 0, 0); break;
                    case 4: mani_B.Margin = new Thickness(985, 648, 0, 0); break;
                    case 7: mani_B.Margin = new Thickness(843, 238, 0, 0); break;
                    case 8: mani_B.Margin = new Thickness(985, 318, 0, 0); break;
						default : break;
                }
                
            }

        }
        private void PictureBox_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            string[] m = (((PictureBox)sender).Tag.ToString()).Split('*');
            SP temp = new SP(m[0], m[1], m[2], m[3]);
        }

        private void PictureBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string[] m = (((PictureBox)sender).Tag.ToString()).Split('*');
            SP temp = new SP(m[0], m[1], m[2], m[3]);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string[] m = (((Button)sender).Tag.ToString()).Split('*');
            SP temp = new SP(m[0], m[1], m[2], m[3]);
        }
    }
}



