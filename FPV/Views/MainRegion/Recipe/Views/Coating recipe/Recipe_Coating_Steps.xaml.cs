using HMI.Views.MainRegion.MachineOverview;
using System;
using System.Threading.Tasks;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.Recipe
{
	/// <summary>
	/// Interaction logic for BSStepEdit.xaml
	/// </summary>
	[ExportView("Recipe_Coating_Steps")]
	public partial class Recipe_Coating_Steps : VisiWin.Controls.View
	{
       public int selectedR = 0;
        IRegionService iRS;
        Recipe_PN R_PN;
        public Recipe_Coating_Steps()
		{
			this.InitializeComponent();
            iRS = ApplicationService.GetService<IRegionService>();
            R_PN = (Recipe_PN)iRS.GetView("Recipe_PN");
        }

        public void btndiping_Click(object sender, RoutedEventArgs e)
        {
            selectedR = 2;
            
                R_PN.Rname.VariableName = "Recipe.D.Name";
                R_PN.Descr.VariableName = "Recipe.D.Description";
           

            regionXXX.Content = new Recipe_Coating_D();
        }

        private void btnspining_Click(object sender, RoutedEventArgs e)
        {
            selectedR = 3;
            if (R_PN.pn_recipe.SelectedPanoramaRegionIndex != 1)
            {
                R_PN.Rname.VariableName = "Recipe.S.Name";
                R_PN.Descr.VariableName = "Recipe.S.Description";
            }

            regionXXX.Content = new Recipe_Coating_S();
        }

        private void btnrolling_Click(object sender, RoutedEventArgs e)
        {
            selectedR = 4;
            R_PN.Rname.VariableName = "Recipe.R.Name";
            R_PN.Descr.VariableName = "Recipe.R.Description";
            regionXXX.Content = new Recipe_Coating_R();
        }

        private void Verwalt_Click(object sender, RoutedEventArgs e)
        {
            switch (selectedR)
            {
                case 2: ApplicationService.SetView("DialogRegion", "Recipe_Browser", 2); break;
                case 3: ApplicationService.SetView("DialogRegion", "Recipe_Browser", 3); break;
                case 4: ApplicationService.SetView("DialogRegion", "Recipe_Browser", 4); break;
            }
        }

        private void NumericVarIn_ValueChanged(object sender, VisiWin.DataAccess.VariableEventArgs e)
        {
            mDreh.Value = 0.068 * weight.Value * weight.Value - 6.8 * weight.Value + 271.2;
        }

        private void Btndiping_Loaded(object sender, RoutedEventArgs e)
        {
            Task obTask = Task.Run(() =>
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    btnspining.IsChecked = true;
                });
            }); 
        }

    }
}