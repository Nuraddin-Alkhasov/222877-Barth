using System;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.DataAccess;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_CrossTransport")]
    public partial class MO_CrossTransport : VisiWin.Controls.View
    {
        IVariableService VS = ApplicationService.GetService<IVariableService>();
        IVariable WM;

        public MO_CrossTransport()
        {
            InitializeComponent();
        }

		private void View_Initialized(object sender, System.EventArgs e)
        {
            WM = VS.GetVariable("PLC.PLC.Blocks.5 Modul 3 Trockner.02 HVT.Fahrwagen.02 Fahrantrieb.DB HVT Fahrwagen HMI.PC.Status Bildanzeige Quertransport");
            WM.Change += WM_Change;
        }
        private void WM_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                if ((short)e.Value == 0 || (short)e.Value == 1)
                {
                    mat_b.Margin = new Thickness(971, 484, 0, 0);
                    mat_b2.Margin = new Thickness(971, 484, 0, 0);

                    mat_t.Margin = new Thickness(933, 366, 0, 0);
                    mat_t2.Margin = new Thickness(933, 386, 0, 0);
                }
                else
                {
                    mat_b.Margin = new Thickness(652, 689, 0, 0);
                    mat_b2.Margin = new Thickness(652, 689, 0, 0);

                    mat_t.Margin = new Thickness(615, 571, 0, 0);
                    mat_t2.Margin = new Thickness(610, 588, 0, 0);
                }
            }

        }

        private void PictureBox_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            Status(sender);
        }
        private void PictureBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Status(sender);
        }

        private void Status(object sender)
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



