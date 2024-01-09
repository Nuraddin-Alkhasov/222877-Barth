using HMI.Views.DialogRegion;
using System.Windows;
using System.Windows.Controls;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Protocol
{
	/// <summary>
	/// Interaction logic for Protocol.xaml
	/// </summary>
	[ExportView("Protocol_Orders")]
	public partial class Protocol_Orders : VisiWin.Controls.View
	{
        int oldIndex = 0;
		public Protocol_Orders()
		{
			this.InitializeComponent();
		}

		private void LayoutRoot_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
            if (this.IsVisible)
            {
                ((ProtocolAdapter)this.DataContext).BW_FilterSQL();
                dgv_orders.SelectedIndex = oldIndex;
            }
		}

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            if (DialogView.Show("Protocol_Filter", "@Protocol.Text45", DialogButton.OKCancel, DialogResult.Cancel) == DialogResult.OK)
            {
                ProtocolAdapter temp = (ProtocolAdapter)this.DataContext;
                temp.BW_FilterSQL();
            }
        }

        private void DataGridRow_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            dgv_orders.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
            oldIndex = dgv_orders.SelectedIndex;
        }
    }
}