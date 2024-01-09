using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.Recipe
{
	/// <summary>
	/// Interaction logic for View1.xaml
	/// </summary>
	[ExportView("Recipe_Article")]
	public partial class Recipe_Article : VisiWin.Controls.View
	{

        public Recipe_Article()
		{
            //ApplicationService.GetService<IRecipeService>().GetRecipeClass("Article").StartEdit();
            this.InitializeComponent();
        }

		private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("DialogRegion", "Recipe_Browser", 0);
        }


        private void btnLTD_Checked(object sender, RoutedEventArgs e)
        {
            region_Article.Content = new Recipe_Article_LT();
        }

        private void btnBB_Checked(object sender, RoutedEventArgs e)
        {
            region_Article.Content = new Recipe_Article_BB();
        }

        private void btnDB_Checked(object sender, RoutedEventArgs e)
        {
            region_Article.Content = new Recipe_Article_DB();
        }

        private void btnKDS_Checked(object sender, RoutedEventArgs e)
        {
            region_Article.Content = new Recipe_Article_KDS();
        }

        private void btnAK_Checked(object sender, RoutedEventArgs e)
        {
            region_Article.Content = new Recipe_Article_AK();
        }

        private void btnOven_Checked(object sender, RoutedEventArgs e)
        {
            region_Article.Content = new Recipe_Article_Oven();
        }

        private void btnKAB1_Checked(object sender, RoutedEventArgs e)
        {
            region_Article.Content = new Recipe_Article_KAB1();
        }

        private void btnKAB2_Checked(object sender, RoutedEventArgs e)
        {
            region_Article.Content = new Recipe_Article_KAB2();
        }

        private void btnTA_Checked(object sender, RoutedEventArgs e)
        {
            region_Article.Content = new Recipe_Article_TA();
        }



        private void BtnLTD_Loaded(object sender, RoutedEventArgs e)
        {
            Task obTask = Task.Run(() => 
            {
                Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    btnLTD.IsChecked = true;
                });
            });

        }
    }
}