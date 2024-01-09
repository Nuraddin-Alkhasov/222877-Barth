using HMI.Views.DialogRegion;
using HMI.Views.MainRegion.Recipe.Custom_Objects;
using HMI.Views.MessageBoxRegion;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.Recipe
{

    [ExportView("Recipe_Machine")]
    public partial class Recipe_Machine : VisiWin.Controls.View
    {

        private int selectedBS = 1;
        public bool saveActive = true;

        public Recipe_Machine()
        {
            

            this.InitializeComponent();
          
            dgv_recipeMR.DataContext = new RecipeAdapter_Article();
            dgv_recipeC.DataContext = new RecipeAdapter_CoatingSteps();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dgv_recipeMR.SelectedIndex > -1)
            {
                RecipeFileInfo T = (RecipeFileInfo)dgv_recipeMR.SelectedItem;
                Ax1_name.Value = T.FileName;
                Ax1_descr.Value = T.Description;
                Ax1.IsChecked = true;
            }

            if (dgv_recipeC.SelectedIndex > -1)
            {
                RecipeFileInfo T = (RecipeFileInfo)dgv_recipeC.SelectedItem;
                switch (selectedBS)
                {
                    case 1:
                        BSx2.IsChecked = true;
                        BSx1_name.Value = T.FileName;
                        BSx1_descr.Value = T.Description;
                        break;
                    case 2:
                        BSx3.IsChecked = true;
                        BSx2_name.Value = T.FileName;
                        BSx2_descr.Value = T.Description;
                        break;
                    case 3:
                        BSx4.IsChecked = true;
                        BSx3_name.Value = T.FileName;
                        BSx3_descr.Value = T.Description;
                        break;
                    case 4:
                        BSx4.IsChecked = true;
                        BSx4_name.Value = T.FileName;
                        BSx4_descr.Value = T.Description;
                        break;
                }
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("DialogRegion", "Recipe_Browser", 6);
        }

        private RecipeFileInfo GetFileInfo(DataGrid dg, string name)
        {
            for (int i = 0; i < dg.Items.Count; i++)
            {
                if (((RecipeFileInfo)dg.Items[i]).FileName == name)
                {
                    return (RecipeFileInfo)dg.Items[i];
                }
            }
            return (new RecipeFileInfo());
        }

        public void dgvUpdate()
        {
            ((RecipeAdapter_Article)dgv_recipeMR.DataContext).UpdateFileList();
            ((RecipeAdapter_CoatingSteps)dgv_recipeC.DataContext).UpdateFileList();
        }

        private void article_del_Click(object sender, RoutedEventArgs e)
        {
            Ax1_name.Value = "";
            Ax1_descr.Value = "";
        }

        private void c1_del_Click(object sender, RoutedEventArgs e)
        {

            BSx1_name.Value = "";
            BSx1_descr.Value = "";
        }

        private void c2_del_Click(object sender, RoutedEventArgs e)
        {
            BSx2_name.Value = "";
            BSx2_descr.Value = "";
        }

        private void c3_del_Click(object sender, RoutedEventArgs e)
        {
            BSx3_name.Value = "";
            BSx3_descr.Value = "";
        }

        private void c4_del_Click(object sender, RoutedEventArgs e)
        {
            BSx4_name.Value = "";
            BSx4_descr.Value = "";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            switch (selectedBS)
            {
                case 1: break;
                case 2:
                    string TempN = BSx1_name.Value;
                    string TempD = BSx1_descr.Value;
                    BSx1_name.Value = BSx2_name.Value;
                    BSx1_descr.Value = BSx2_descr.Value;
                    BSx2_name.Value = TempN;
                    BSx2_descr.Value = TempD;
                    BSx1.IsChecked = true;
                   
                    break;
                case 3:

                    TempN = BSx2_name.Value;
                    TempD = BSx2_descr.Value;
                    BSx2_name.Value = BSx3_name.Value;
                    BSx2_descr.Value = BSx3_descr.Value;
                    BSx3_name.Value = TempN;
                    BSx3_descr.Value = TempD;
                    BSx2.IsChecked = true;
                   
                    break;
                case 4:
                    TempN = BSx3_name.Value;
                    TempD = BSx3_descr.Value;
                    BSx3_name.Value = BSx4_name.Value;
                    BSx3_descr.Value = BSx4_descr.Value;
                    BSx4_name.Value = TempN;
                    BSx4_descr.Value = TempD;
                    BSx3.IsChecked = true;
                  
                    break;
                default: break;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            switch (selectedBS)
            {
                case 1:
                    string TempN = BSx2_name.Value;
                    string TempD = BSx2_descr.Value;
                    BSx2_name.Value = BSx1_name.Value;
                    BSx2_descr.Value = BSx1_descr.Value;
                    BSx1_name.Value = TempN;
                    BSx1_descr.Value = TempD;
                    BSx2.IsChecked = true;
                 
                    break;
                case 2:
                    TempN = BSx3_name.Value;
                    TempD = BSx3_descr.Value;
                    BSx3_name.Value = BSx2_name.Value;
                    BSx3_descr.Value = BSx2_descr.Value;
                    BSx2_name.Value = TempN;
                    BSx2_descr.Value = TempD;
                    BSx3.IsChecked = true;
                   
                    break;
                case 3:
                    TempN = BSx4_name.Value;
                    TempD = BSx4_descr.Value;
                    BSx4_name.Value = BSx3_name.Value;
                    BSx4_descr.Value = BSx3_descr.Value;
                    BSx3_name.Value = TempN;
                    BSx3_descr.Value = TempD;
                    BSx4.IsChecked = true;
                   
                    break;
                case 4: break;
                default: break;
            }
        }

        private void Ax1_Checked(object sender, RoutedEventArgs e)
        {
        ///    selectedBS = 0;
        }

        private void BS1_Checked(object sender, RoutedEventArgs e)
        {
            selectedBS = 1;
        }

        private void BS2_Checked(object sender, RoutedEventArgs e)
        {
            selectedBS = 2;
        }

        private void BS3_Checked(object sender, RoutedEventArgs e)
        {
            selectedBS = 3;
        }

        private void BS4_Checked(object sender, RoutedEventArgs e)
        {
            selectedBS = 4;
        }

        private void name_ValueChanged(object sender, VisiWin.DataAccess.VariableEventArgs e)
        {
            System.Threading.Tasks.Task.Factory.StartNew(() => BlinkONOFF()); 
        }

        private void BlinkONOFF()
        {
            Application.Current.Dispatcher.InvokeAsync((Action)delegate {
            saveActive = true;
            Ax1.IsBlinkEnabled = false;
            BSx1.IsBlinkEnabled = false;
            BSx2.IsBlinkEnabled = false;
            BSx3.IsBlinkEnabled = false;

            bool[] a = new bool[5];
            a[0] = (Ax1_name.Value != "");
            a[1] = (BSx1_name.Value != "");
            a[2] = (BSx2_name.Value != "");
            a[3] = (BSx3_name.Value != "");
            a[4] = (BSx4_name.Value != "");
            int b = 0;
            for (int i = 4; i >= 0; i--)
            {
                if (a[i])
                {
                    b = i;
                    break;
                }
            }

            switch (b)
            {
                case 1:
                        Blinking(Ax1_name.Value, Ax1);
                    break;
                case 2:
                        Blinking(Ax1_name.Value, Ax1);
                        Blinking(BSx1_name.Value, BSx1);
                        break;
                case 3:
                        Blinking(Ax1_name.Value, Ax1);
                        Blinking(BSx1_name.Value, BSx1);
                        Blinking(BSx2_name.Value, BSx2);
                        break;
                case 4:
                        Blinking(Ax1_name.Value, Ax1);
                        Blinking(BSx1_name.Value, BSx1);
                        Blinking(BSx2_name.Value, BSx2);
                        Blinking(BSx3_name.Value, BSx3);
                        break;
            }
            });
        }
        private void Blinking(string RN, VisiWin.Controls.RadioButton RX)
        {
            if (RN != "")
            {
                RX.IsBlinkEnabled = false;
               
            }
            else
            {
                RX.IsBlinkEnabled = true;
                RX.IsChecked = true;
                saveActive = false;
            }
        }

        private void dgv_recipeMR_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgv_recipeMR.SelectedIndex != -1)
            {
                dgv_recipeC.SelectedIndex = -1;
                BSList.IsChecked = false;
                ArticleList.IsChecked = true;
            }

        }

        private void dgv_recipeC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgv_recipeC.SelectedIndex != -1)
            {
                dgv_recipeMR.SelectedIndex = -1;
                BSList.IsChecked = true;
                ArticleList.IsChecked = false;
            }
        }

        private void ArticleList_DoubleTap(object sender, VisiWin.Controls.Gestures.GestureEventArgs e)
        {
            IRegionService iRS = ApplicationService.GetService<IRegionService>();
            Recipe_PN R_PN = (Recipe_PN)iRS.GetView("Recipe_PN");
            R_PN.pn_recipe.ScrollPrevious();
        }

        private void BSList_DoubleTap(object sender, VisiWin.Controls.Gestures.GestureEventArgs e)
        {
            IRegionService iRS = ApplicationService.GetService<IRegionService>();
            Recipe_PN R_PN = (Recipe_PN)iRS.GetView("Recipe_PN");
            R_PN.pn_recipe.ScrollNext();
        }

		private void ArticleList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			IRegionService iRS = ApplicationService.GetService<IRegionService>();
            Recipe_PN R_PN = (Recipe_PN)iRS.GetView("Recipe_PN");
            R_PN.pn_recipe.ScrollPrevious();
		}

		private void BSList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{ 
			IRegionService iRS = ApplicationService.GetService<IRegionService>();
            Recipe_PN R_PN = (Recipe_PN)iRS.GetView("Recipe_PN");
            R_PN.pn_recipe.ScrollNext();
		}

        private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                string rn = ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Header.Maschinenprogramm#STRING40").ToString();
                if (rn != null || rn != "")
                {
                    (new AutoRecipeLoad(rn)).LoadStackAsync();
                }
            }
           
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("DialogRegion", "Recipe_Binding");
            
        }
        private void _1PreviewTouchDown(object sender, TouchEventArgs e)
        {
            dgv_recipeMR.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
        }
        private void _2PreviewTouchDown(object sender, TouchEventArgs e)
        {
            dgv_recipeC.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
        }
    }
}