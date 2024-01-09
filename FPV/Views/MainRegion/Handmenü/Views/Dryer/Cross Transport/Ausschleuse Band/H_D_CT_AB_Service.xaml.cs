﻿using HMI.Views.MessageBoxRegion;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.Helper;
using VisiWin.Language;
using VisiWin.Logging;

namespace HMI.Handmenu.Drayer
{
    /// <summary>
    /// Interaction logic for KeyAndSwitchView.xaml
    /// </summary>
    [ExportView("H_D_CT_AB_Service")]

    public partial class H_D_CT_AB_Service : View
    {
        private readonly VisiWin.Logging.ILoggingService loggingService;

        public H_D_CT_AB_Service()
        {
            this.InitializeComponent();
            this.loggingService = ApplicationService.GetService<ILoggingService>();
        }

        private void Key_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@HandMenu.Text1", "@HandMenu.Text2", MessageBoxButton.YesNo, icon: MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.5 Modul 3 Trockner.03 Ausschleussen.03 Ausschleusse Band / Hub / Box / Vereinzelung.Ausschleusse Band.DB Ausschleusse Band HMI.PC.Antrieb.Vorwahl Referenz",1);
                ILanguageService textService = ApplicationService.GetService<ILanguageService>();

                string txt = textService.GetText("@Logging.Service.Text17");
                this.loggingService.Log("Service", "NewReference", txt, FastDateTime.Now);
            }
        }
    }
}