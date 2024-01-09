﻿using HMI.Views.MessageBoxRegion;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.MachineOverview
{

	[ExportView("MO_Status_BasketF")]
	public partial class MO_Status_BasketF : VisiWin.Controls.View
	{

        public MO_Status_BasketF()
		{
			this.InitializeComponent();
        }

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@StatusView.Text38", "@Tasten.Entfernen", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Exclamation) == MessageBoxResult.Yes)
            {
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC Status Platz.Daten von BP.Material.Daten loeschen", 1);
                new SP_Logging(1);
            }
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@StatusView.Text28", "@Tasten.speichern", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC Status Platz.Daten von BP.Material.Daten übernehmen", true);
                new SP_Logging(0);
            }
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
                paintTYP.StateList = Temp_SC;
                paintTYP2.StateList = Temp_SC;
            }
        }

        private void mr_ValueChanged(object sender, VisiWin.DataAccess.VariableEventArgs e)
        {
            IRecipeClass T = ApplicationService.GetService<IRecipeService>().GetRecipeClass("MachineRecipe");
            if (T.IsExistingRecipeFile(mr.Value))
            {
                rd.Value = T.GetRecipeFile(mr.Value).Description;
            }
            else { rd.Value = ""; }
        }

        private void Btn_Deleteb1_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@StatusView.Text28", "@Tasten.Entfernen", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC Status Platz.Daten von BP.Material.Daten Korb 1 löschen", 1);
            }
        }

        private void Btn_Deleteb2_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@StatusView.Text28", "@Tasten.Entfernen", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC Status Platz.Daten von BP.Material.Daten Korb 2 löschen", 1);
            }
        }

        private void Mr_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            ApplicationService.SetView("MessageBoxRegion", "MO_Status_Recipe");
        }

        private void Mr_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ApplicationService.SetView("MessageBoxRegion", "MO_Status_Recipe");
        }
    }
}