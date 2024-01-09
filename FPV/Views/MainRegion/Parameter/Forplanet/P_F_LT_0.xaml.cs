using VisiWin.ApplicationFramework;
using VisiWin.Controls;

namespace HMI.Views.Parameter.Forplanet
{
	/// <summary>
	/// Interaction logic for BSStepEdit.xaml
	/// </summary>
	[ExportView("P_F_LT_0")]
	public partial class P_F_LT_0 : VisiWin.Controls.View
	{

        public P_F_LT_0()
		{
			this.InitializeComponent();
        }

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                StateCollection Temp_SC = new StateCollection();
                for (int i = 1; i <= 9; i++)
                {
                    string temp = ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.00 Allgemein.Lacktyp Namen.Lacktyp Name[" + i.ToString() + "]").ToString();
                    if (temp != "")
                    {
                        Temp_SC.Add(new State()
                        {
                            Text = temp,
                            Value = i.ToString()
                        });
                    }

                }
                cb1.StateList = Temp_SC;
                cb2.StateList = Temp_SC;
                cb3.StateList = Temp_SC;
                cb4.StateList = Temp_SC;
                cb5.StateList = Temp_SC;
                cb6.StateList = Temp_SC;
                cb7.StateList = Temp_SC;
            }
        }
    }
}