﻿using VisiWin.ApplicationFramework;
using VisiWin.Controls;
namespace HMI.Parameter.Manipulator
{
    /// <summary>
    /// Interaction logic for DigitalIOView.xaml
    /// </summary>
    [ExportView("Parameter_Manipulator_PN")]
    public partial class Parameter_Manipulator_PN : View
    {
		
        public Parameter_Manipulator_PN()
        {
            this.InitializeComponent();
            
        }

        private void View_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible && pn_parameter_manipulator.SelectedPanoramaRegionIndex != 0)
            {
                pn_parameter_manipulator.NavigateToStart();
            }
        }
    }
}