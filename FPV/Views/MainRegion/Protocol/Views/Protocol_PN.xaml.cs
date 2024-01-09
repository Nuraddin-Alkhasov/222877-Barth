using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Protocol
{
	/// <summary>
	/// Interaction logic for ProtocolPN.xaml
	/// </summary>
	[ExportView("Protocol_PN")]
	public partial class Protocol_PN : VisiWin.Controls.View
	{
		public Protocol_PN()
		{
			this.InitializeComponent();
		}

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible && pn_protocol.SelectedPanoramaRegionIndex!=0)
            {
                pn_protocol.ScrollPrevious();
            }
        }
    }
}