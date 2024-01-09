using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.Recipe
{
    /// <summary>
    /// Interaction logic for BSStepEdit.xaml
    /// </summary>
    [ExportView("Recipe_Coating_PR")]
    public partial class Recipe_Coating_PR : VisiWin.Controls.View
    {
        IRecipeClass MainR = ApplicationService.GetService<IRecipeService>().GetRecipeClass("MainR");
        private int selectedStep = 0;
        private int StepID = 0;
        RecipeFileInfo T;
        public bool saveActive = true;

        public Recipe_Coating_PR()
        {
            this.InitializeComponent();
            dgv_recipeD.DataContext = new RecipeAdapter_D();
            dgv_recipeS.DataContext = new RecipeAdapter_S();
            dgv_recipeR.DataContext = new RecipeAdapter_R();
        }

        public void SelectionChanged(int sel)
        {
            for (int i = 0; i < 8; i++)
            {
                if (i != sel)
                {
                    ((Recipe_Coating_StepsName)tStack.Items[i]).RBName.IsChecked = false;
                }
            }
            selectedStep = sel;
        }

        private void addStep_Click(object sender, RoutedEventArgs e)
        {
            if (T != null)
            { 
                tStack.Items[selectedStep] = new Recipe_Coating_StepsName(T.FileName, T.Description, StepID, selectedStep);
                if (selectedStep == 7)
                {
                    ((Recipe_Coating_StepsName)tStack.Items[0]).RBName.IsChecked = true;
              
                }
                else
                {
                    ((Recipe_Coating_StepsName)tStack.Items[selectedStep + 1]).RBName.IsChecked = true;
                }
                scrollTO();
            }
        }

        private void scrollTO()
        {
            switch (selectedStep)
            {
                case 0: sv_stack.ScrollToVerticalOffset(0); break;
                case 1: sv_stack.ScrollToVerticalOffset(0); break;
                case 2: sv_stack.ScrollToVerticalOffset(0); break;
                case 3: sv_stack.ScrollToVerticalOffset(0); break;
                case 4: sv_stack.ScrollToVerticalOffset(603); break;
                case 5: sv_stack.ScrollToVerticalOffset(603); break;
                case 6: sv_stack.ScrollToVerticalOffset(603); break;
                case 7: sv_stack.ScrollToVerticalOffset(603); break;
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("DialogRegion", "Recipe_Browser", 5);
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

        private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                StateCollection Temp_SC = new StateCollection();
                for (int i = 1; i <= 9; i++)
                {
                    string temp = ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.00 Allgemein.Lacktyp Namen.Lacktyp Name[" + i.ToString() + "]").ToString();
                    if (temp != "")
                    {
                        Temp_SC.Add(new State()
                        {
                            Text = temp,
                            Value = i.ToString()
                        });
                    }
                }

                cb1.StateList = Temp_SC;
            }
        }

        private void ButtonOben_Click(object sender, RoutedEventArgs e)
        {
            if (selectedStep!=0)
            {
                ((Recipe_Coating_StepsName)tStack.Items[selectedStep]).Step_Switcher(selectedStep - 1);
                ((Recipe_Coating_StepsName)tStack.Items[selectedStep - 1]).RBName.IsChecked = true;
            }       
        }

        private void ButtonUnten_Click(object sender, RoutedEventArgs e)
        {
            if (selectedStep != 7)
            {
                ((Recipe_Coating_StepsName)tStack.Items[selectedStep]).Step_Switcher(selectedStep + 1);
                ((Recipe_Coating_StepsName)tStack.Items[selectedStep + 1]).RBName.IsChecked = true;
            }
        }

        public void BlinkONOFF()
        {
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() => BlinkONOFTH());
        }

        private void BlinkONOFTH()
        {
            Application.Current.Dispatcher.InvokeAsync((Action)delegate {
                saveActive = true;
            for (int i = 0; i < 8; i++)
            {
                ((Recipe_Coating_StepsName)tStack.Items[i]).BlinkOFF();
            }

            for (int i = 7; i >= 0; i--)
            {
                if ((((Recipe_Coating_StepsName)tStack.Items[i]).RName.Value != ""))
                {
                    for (int x = 0; x <= i; x++)
                    {
                        if (((Recipe_Coating_StepsName)tStack.Items[x]).RName.Value != "")
                        {
                            ((Recipe_Coating_StepsName)tStack.Items[x]).BlinkOFF();
                        }
                        else
                        {
                            saveActive = false;

                            ((Recipe_Coating_StepsName)tStack.Items[x]).BlinkON();
                                scrollTO();
                            }
                    }
                    break;
                }
            }
            });
        }

        private void Bg_DoWork()
        {
            Application.Current.Dispatcher.InvokeAsync((Action)delegate {
                for (int i = 0; i < 8; i++)
                {
                    tStack.Items.Add(new Region());
                    tStack.Items[i] = new Recipe_Coating_StepsName(i);
                }
            ((Recipe_Coating_StepsName)tStack.Items[0]).RBName.IsChecked = true;
            });
        }

        private void dgv_recipeD_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgv_recipeD.SelectedIndex != -1)
            {
                dgv_recipeS.SelectedIndex = -1;
                dgv_recipeR.SelectedIndex = -1;
                DList.IsChecked = true;
                SList.IsChecked = false;
                RList.IsChecked = false;
                StepID = 1;
                T = (RecipeFileInfo)dgv_recipeD.SelectedItem;
            }
        }

        private void dgv_recipeS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgv_recipeS.SelectedIndex != -1)
            {
                dgv_recipeD.SelectedIndex = -1;
                dgv_recipeR.SelectedIndex = -1;
                DList.IsChecked = false;
                SList.IsChecked = true;
                RList.IsChecked = false;
                StepID = 2;
                T = (RecipeFileInfo)dgv_recipeS.SelectedItem;
            }
        }

        private void dgv_recipeR_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgv_recipeR.SelectedIndex != -1)
            {
                dgv_recipeD.SelectedIndex = -1;
                dgv_recipeS.SelectedIndex = -1;
                DList.IsChecked = false;
                SList.IsChecked = false;
                RList.IsChecked = true;
                StepID = 3;
                T = (RecipeFileInfo)dgv_recipeR.SelectedItem;
            }

        }

        private void View_Initialized(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => Bg_DoWork());
        }

        public void dgvUpdate()
        {
            ((RecipeAdapter_D)dgv_recipeD.DataContext).UpdateFileList();
            ((RecipeAdapter_S)dgv_recipeS.DataContext).UpdateFileList();
            ((RecipeAdapter_R)dgv_recipeR.DataContext).UpdateFileList();
        }

        private void _1PreviewTouchDown(object sender, TouchEventArgs e)
        {
            dgv_recipeD.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
        }
        private void _2PreviewTouchDown(object sender, TouchEventArgs e)
        {
            dgv_recipeS.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
        }
        private void _3PreviewTouchDown(object sender, TouchEventArgs e)
        {
            dgv_recipeR.UnselectAllCells();
            ((DataGridRow)sender).IsSelected = true;
        }
    }
}