using HMI.Views.MessageBoxRegion;
using System.Windows;
using VisiWin.ApplicationFramework;
using VisiWin.DataAccess;
using VisiWin.Helper;
using VisiWin.Language;
using VisiWin.Logging;
using VisiWin.Recipe;

namespace HMI.Views.MainRegion.MachineOverview
{

	[ExportView("MO_DataOverwrite")]
	public partial class MO_DataOverwrite : VisiWin.Controls.View
	{
        IVariableService VS;
        IVariable inPause;
        private readonly ILoggingService loggingService;
        ILanguageService textService;
        public MO_DataOverwrite()
		{
           
            this.InitializeComponent();
            textService = ApplicationService.GetService<ILanguageService>();
            this.loggingService = ApplicationService.GetService<ILoggingService>();
            VS = ApplicationService.GetService<IVariableService>();
            inPause = VS.GetVariable("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Fahre Anlage in Pause");
            inPause.Change += InPause_Change;

        }

        private void InPause_Change(object sender, VariableEventArgs e)
        {
            if (e.Value != e.PreviousValue)
            {
                if ((bool)e.Value)
                {
                    pn_dataoverwrite.ScrollNext();
                }
                else
                {
                    pn_dataoverwrite.ScrollPrevious();
                }
                
            }
        }

        private void Pn_dataoverwrite_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (pn_dataoverwrite.SelectedPanoramaRegionIndex == 0)
            {
                pn_dataoverwrite.ScrollNext();
            }
            else
            {
                pn_dataoverwrite.ScrollPrevious();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetView("DialogRegion", "EmptyView");
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@StatusView.Text28", "@Tasten.Text12", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                
                IRegionService iRS = ApplicationService.GetService<IRegionService>();
                MO_DO_Data MODOData = (MO_DO_Data)iRS.GetView("MO_DO_Data");
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Header.Maschinenprogramm#STRING40", MODOData.mr.Value);
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.MR.Header.User#STRING40", MODOData.user.Value);
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Fahre Anlage in Pause", true);
                string txt = textService.GetText("@Logging.Service.Text22");
                this.loggingService.Log("Service", "RecipeOverwrite", txt, FastDateTime.Now);
            }
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxView.Show("@StatusView.Text37", "@Tasten.Text13", MessageBoxButton.YesNo, MessageBoxResult.No, MessageBoxIcon.Question) == MessageBoxResult.Yes)
            {
                
                ApplicationService.SetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Fahre Anlage in Pause abbruch", true);
                string txt = textService.GetText("@Logging.Service.Text23");
                this.loggingService.Log("Service", "RecipeOverwrite", txt, FastDateTime.Now);
            }
        }

        private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                if (ApplicationService.GetVariableValue("PLC.PLC.Blocks.10 HMI.01 PC.DB PC.Data from PC.Fahre Anlage in Pause").ToString() == "true")
                {
                    pn_dataoverwrite.SelectedPanoramaRegionIndex = 1;
                }
            }           
        }
    }
}