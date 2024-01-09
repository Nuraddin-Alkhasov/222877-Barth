using System;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_Werehaus")]
    public partial class MO_Werehaus : VisiWin.Controls.View
    {

        public MO_Werehaus()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string[] m = (((Button)sender).Tag.ToString()).Split('*');
            SP temp = new SP(m[0], m[1], m[2], m[3]);
        }
    }
}



