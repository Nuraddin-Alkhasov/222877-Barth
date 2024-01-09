using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Betriebstunden
{
	/// <summary>
	/// Interaction logic for BSStepEdit.xaml
	/// </summary>
	[ExportView("BS_HNT")]
	public partial class BS_HNT : VisiWin.Controls.View
	{
       
        public BS_HNT()
		{
			this.InitializeComponent();
        }

        private void Btn1_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            btn1.IsSelected = true;
        }

        private void Btn3_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            btn2.IsSelected = true;
        }
    }
}