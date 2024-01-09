using System.Windows;
using VisiWin.Controls;
using VisiWin.ApplicationFramework;
using System.ComponentModel;
using HMI.Views.MainRegion.Recipe;
using HMI.Views.MainRegion.Protocol;
using HMI.User;
using HMI.Diagnose;
using VisiWin.DataAccess;
using System.Windows.Media;
using System.Diagnostics;

namespace HMI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : BaseWindow
    {

        IVariableService VS;
        IVariable ERS;
        IVariable BFS;
        public MainWindow()
        {
            InitializeComponent();
            userlabel.Visibility = Visibility.Hidden;
            VS = ApplicationService.GetService<IVariableService>();
            ERS = VS.GetVariable("PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Sammelstörung Anlage");
            ERS.Change += ERS_Change;
            BFS = VS.GetVariable("PLC.PLC.Blocks.10 HMI.00 Allgemein.DB HMI Allgemein.Gerneral.Sammelbedinerführung Anlage");
            BFS.Change += BFS_Change;
        }

        private void BFS_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                diagnose.IsBlinkEnabled = true;
                var converter = new System.Windows.Media.BrushConverter();
                if ((bool)e.Value)
                {
                    if ((bool)ERS.Value)
                    {
                        diagnose.Background = (Brush)converter.ConvertFromString("#FFBE2828");  //red
                        diagnose.BlinkBrush = (Brush)converter.ConvertFromString("#FFBC49");  //yellow
                    }
                    else
                    {
                        diagnose.Background = (Brush)converter.ConvertFromString("#FF807F7F");//  gray
                        diagnose.BlinkBrush = (Brush)converter.ConvertFromString("#FFBC49");//  yellow
                    }
                }
                else
                {
                    if ((bool)ERS.Value)
                    {
                        diagnose.Background = (Brush)converter.ConvertFromString("#FF807F7F");//  gray
                        diagnose.BlinkBrush = (Brush)converter.ConvertFromString("#FFBE2828");  //red
                    }
                    else
                    {
                        diagnose.BlinkBrush = (Brush)converter.ConvertFromString("#FF807F7F");//  gray
                        diagnose.IsBlinkEnabled = false;
                    }                     
                }              
            }
        }

        private void ERS_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                var converter = new System.Windows.Media.BrushConverter();
                diagnose.IsBlinkEnabled = true;
                if ((bool)e.Value)
                {
                    if ((bool)BFS.Value)
                    {
                        diagnose.Background = (Brush)converter.ConvertFromString("#FFBE2828");  //red
                        diagnose.BlinkBrush = (Brush)converter.ConvertFromString("#FFBC49");  //yellow
                    }
                    else
                    {
                        diagnose.Background = (Brush)converter.ConvertFromString("#FF807F7F");  //gray
                        diagnose.BlinkBrush = (Brush)converter.ConvertFromString("#FFBE2828");//  red
                    }
                }
                else
                {
                    if ((bool)BFS.Value)
                    {
                        diagnose.Background = (Brush)converter.ConvertFromString("#FF807F7F");  //gray
                        diagnose.BlinkBrush = (Brush)converter.ConvertFromString("#FFBC49");//  yellow
                    }
                    else
                    {
                        diagnose.BlinkBrush = (Brush)converter.ConvertFromString("#FF807F7F");//  gray
                        diagnose.IsBlinkEnabled = false;
                    }
                   
                }

            }
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            dashboard.LocalizableText = "@Appbar.Text1";
            moverview.LocalizableText = "@Appbar.lblAnlageübersicht";
            handmode.LocalizableText = "@Appbar.lblHandmenü";
            recipe.LocalizableText = "@Appbar.lblRezeptverwaltung";
            parameter.LocalizableText = "@Appbar.lblParameter";
            protocol.LocalizableText = "@Appbar.lblAuftraege";
            betriebstunden.LocalizableText = "@Appbar.lblBetriebstunden";
            logs.LocalizableText = "@Appbar.lbllogs";
            usermanager.LocalizableText = "@Appbar.lblBenutzerübersicht";
            diagnose.LocalizableText = "@Appbar.lblMeldungen";
            userlabel.Visibility = Visibility.Visible;
            langlabel.Visibility = Visibility.Visible;
           

            turnoff.LocalizableText = "@Appbar.Exit";
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
            dashboard.LocalizableText = "@Appbar.Text3";
            moverview.LocalizableText = "@Appbar.Text3";
            handmode.LocalizableText = "@Appbar.Text3";
            recipe.LocalizableText = "@Appbar.Text3";
            parameter.LocalizableText = "@Appbar.Text3";
            protocol.LocalizableText = "@Appbar.Text3";
            logs.LocalizableText = "@Appbar.Text3";
            betriebstunden.LocalizableText = "@Appbar.Text3";
            usermanager.LocalizableText = "@Appbar.Text3";
            diagnose.LocalizableText = "@Appbar.Text3";
            
            langlabel.Visibility = Visibility.Hidden;
            userlabel.Visibility = Visibility.Hidden;
            turnoff.LocalizableText = "@Appbar.Text3";
        }

        private void Handmode_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MainRegion", "Handmenu_DB");
        }

        private void Recipe_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MainRegion", "Recipe_PN");
            IRegionService iRS = ApplicationService.GetService<IRegionService>();
            Recipe_PN R_PN = (Recipe_PN)iRS.GetView("Recipe_PN");
            switch (R_PN.pn_recipe.SelectedPanoramaRegionIndex)
            {
                case 0: R_PN.pn_recipe.ScrollNext(true); break;
                case 1: break;
                case 2: R_PN.pn_recipe.ScrollPrevious(true); break;
            }
         
          
        }

        private void Parameter_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MainRegion", "Parameter_DB");
        }

        private void Moverview_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MainRegion", "MO_MainView");
        }

        private void Protocol_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MainRegion", "Protocol_PN");
            IRegionService iRS = ApplicationService.GetService<IRegionService>();
            Protocol_PN P_PN = (Protocol_PN)iRS.GetView("Protocol_PN");
            P_PN.pn_protocol.ScrollPrevious(true); 
        }

        private void Usermanager_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MainRegion", "User_PN");
            IRegionService iRS = ApplicationService.GetService<IRegionService>();
            User_PN U_PN = (User_PN)iRS.GetView("User_PN");
            U_PN.pn_benutzerverwaltung.ScrollPrevious(true);
        }

        private void Diagnose_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("MainRegion", "Diagnose_PN");
            IRegionService iRS = ApplicationService.GetService<IRegionService>();
            Diagnose_PN D_PN = (Diagnose_PN)iRS.GetView("Diagnose_PN");
            D_PN.pn_diagnose.ScrollPrevious(true);
        }

        private void Turnoff_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().CloseMainWindow();
            System.Environment.Exit(-1);
        }
    }
}
