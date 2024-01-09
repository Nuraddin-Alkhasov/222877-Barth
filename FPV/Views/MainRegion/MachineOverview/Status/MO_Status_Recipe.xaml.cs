using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.MachineOverview
{

	[ExportView("MO_Status_Recipe")]
	public partial class MO_Status_Recipe : VisiWin.Controls.View
	{

        public MO_Status_Recipe()
		{
			this.InitializeComponent();
        }


        private void CloseDialog(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MessageBoxRegion", "EmptyView");
        }

        private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                string MR_Name = ApplicationService.ObjectStore.GetValue("MO_Status_Recipe_KEY").ToString();
                DoWork(MR_Name);
            }
        }
        private async void DoWork(string _MR)
        {
            IRecipeClass MR = ApplicationService.GetService<IRecipeService>().GetRecipeClass("MachineRecipe");
            await MR.LoadFromFileToBufferAsync(_MR);
            if (BS2.Value != "")
            {
                BS2.Visibility = Visibility.Visible;
                BS2descr.Visibility = Visibility.Visible;
            }
            else
            {
                BS2.Visibility = Visibility.Collapsed;
                BS2descr.Visibility = Visibility.Collapsed;
            }
            if (BS3.Value != "")
            {
                BS3.Visibility = Visibility.Visible;
                BS3descr.Visibility = Visibility.Visible;
            }
            else
            {
                BS3.Visibility = Visibility.Collapsed;
                BS3descr.Visibility = Visibility.Collapsed;
            }
            if (BS4.Value != "")
            {
                BS4.Visibility = Visibility.Visible;
                BS4descr.Visibility = Visibility.Visible;
            }
            else
            {
                BS4.Visibility = Visibility.Collapsed;
                BS4descr.Visibility = Visibility.Collapsed;
            }
        }
    }
}