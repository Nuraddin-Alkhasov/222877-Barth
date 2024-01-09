﻿using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Handmenu.Drayer
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("H_D_HNT_PS_PN")]
    public partial class H_D_HNT_PS_PN : View
    {
		
        public H_D_HNT_PS_PN()
        {
            this.InitializeComponent();
            
        }

        private void service_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            pn_H_D_HNT_PS.ScrollNext();
        }

        private void pn_arm_SelectedPanoramaRegionChanged(object sender, SelectedPanoramaRegionChangedEventArgs e)
        {
            if (pn_H_D_HNT_PS.SelectedPanoramaRegionIndex == 0)
            {
                service.Visibility = System.Windows.Visibility.Visible;
                handmenu.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                service.Visibility = System.Windows.Visibility.Hidden;
                handmenu.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void handmenu_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            pn_H_D_HNT_PS.ScrollPrevious();
        }
    }
}