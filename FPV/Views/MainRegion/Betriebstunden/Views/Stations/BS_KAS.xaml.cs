using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Betriebstunden
{
	/// <summary>
	/// Interaction logic for BSStepEdit.xaml
	/// </summary>
	[ExportView("BS_KAS")]
	public partial class BS_KAS : VisiWin.Controls.View
	{
       
        public BS_KAS()
		{
			this.InitializeComponent();
        }

        private void Btn1_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            btn1.IsSelected = true;
        }

        private void Btn2_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            btn2.IsSelected = true;
        }

        private void Btn3_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            btn3.IsSelected = true;
        }

        private void Btn4_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            btn4.IsSelected = true;
        }
    }
}