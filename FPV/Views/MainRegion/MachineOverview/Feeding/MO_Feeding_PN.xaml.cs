using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.MachineOverview
{
	/// <summary>
	/// Interaction logic for ProtocolPN.xaml
	/// </summary>
	[ExportView("MO_Feeding_PN")]
	public partial class MO_Feeding_PN : VisiWin.Controls.View
	{
		public MO_Feeding_PN()
		{
			this.InitializeComponent();
		}

        private void Pn_MO_Feeding_SelectedPanoramaRegionChanged(object sender, VisiWin.Controls.SelectedPanoramaRegionChangedEventArgs e)
        {
            switch (pn_MO_Feeding.SelectedPanoramaRegionIndex)
            {
                case 0: header.LocalizableText = "@MainOverview.Text6"; break;
                case 1: header.LocalizableText = "@MainOverview.Text7"; break;
                case 2: header.LocalizableText = "@MainOverview.Text1"; break;
                case 3: header.LocalizableText = "@MainOverview.Text8"; break;
            }
        }

    }
}