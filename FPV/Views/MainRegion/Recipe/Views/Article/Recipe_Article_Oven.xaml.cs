using HMI.Views.MainRegion.MachineOverview;
using System.Windows;
using VisiWin.ApplicationFramework;

namespace HMI.Views.MainRegion.Recipe
{
	/// <summary>
	/// Interaction logic for BSStepEdit.xaml
	/// </summary>
	[ExportView("Recipe_Article_Oven")]
	public partial class Recipe_Article_Oven : VisiWin.Controls.View
	{

        public Recipe_Article_Oven()
		{
			this.InitializeComponent();
        }
   }
}