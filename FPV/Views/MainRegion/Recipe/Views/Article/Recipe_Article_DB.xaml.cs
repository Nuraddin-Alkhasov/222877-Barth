using HMI.Views.MainRegion.MachineOverview;
using System.Windows;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Recipe
{
	/// <summary>
	/// Interaction logic for BSStepEdit.xaml
	/// </summary>
	[ExportView("Recipe_Article_DB")]
	public partial class Recipe_Article_DB : VisiWin.Controls.View
	{

        public Recipe_Article_DB()
		{
			this.InitializeComponent();
        }
   }
}