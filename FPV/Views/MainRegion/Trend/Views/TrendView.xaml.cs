using HMI.Views.DialogRegion;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisiWin.ApplicationFramework;
using VisiWin.UserManagement;

namespace HMI
{
    /// <summary>
    /// Interaction logic for TrendView.xaml
    /// </summary>
    [ExportView("TrendView")]
    public partial class TrendView : VisiWin.Controls.View
    {
        private readonly Stopwatch _doubleTapStopwatch = new Stopwatch();
        private Point _lastTapLocation;
        public TrendView()
        {
            InitializeComponent();
           
        }

        private void resolutionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = this.resolutionComboBox.SelectedItem as ComboBoxItem;
            TimeSpan newResolution;

            if (selectedItem != null && TimeSpan.TryParse(selectedItem.Content.ToString(), out newResolution))
                this.trendCurveContainer.Resolution = newResolution;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IUserManagementService userService = ApplicationService.GetService<IUserManagementService>();
            if (userService.CurrentUser != null && userService.CurrentUser.RightNames.Contains("Trend"))
            {
                DialogView.Show("ExportView", "@TrendSystemBenutzerText.Views.ExportView.ExportData", DialogButton.OK);
            }
         
        }

        private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                ((TrendAdapter)this.DataContext).ScaleMinValue = 0;
                ((TrendAdapter)this.DataContext).ScaleMaxValue = 400;
                int T_ID = Convert.ToInt16(ApplicationService.GetVariableValue("Trend.ID"));
                switch (T_ID)
                {
                    case 0:
                        curve1.ArchiveName = curve2.ArchiveName = "Vorzone";
                        curve1.TrendName = "Istwert";
                        curve2.TrendName = "Sollwert";
                        head.LocalizableText = "@TrendSystem.Text9"; break;
                    case 1:
                        curve1.ArchiveName = curve2.ArchiveName = "Zwischenzone";
                        curve1.TrendName = "Istwert";
                        curve2.TrendName = "";
                        head.LocalizableText = "@TrendSystem.Text8"; break;
                    case 2:
                        curve1.ArchiveName = curve2.ArchiveName = "Trockner";
                        curve1.TrendName = "Istwert";
                        curve2.TrendName = "Sollwert";
                        head.LocalizableText = "@TrendSystem.Text6"; break; ;
                    case 3:
                        curve1.ArchiveName = curve2.ArchiveName = "Kühlzone";
                        curve1.TrendName = "Istwert";
                        curve2.TrendName = "Sollwert";
                        head.LocalizableText = "@TrendSystem.Text7"; break;
                    case 4:
                        curve1.ArchiveName = curve2.ArchiveName = "Beschichtung";
                        curve1.TrendName = "Istwert";
                        curve2.TrendName = "Sollwert";
                        head.LocalizableText = "@TrendSystem.Text10"; break; 
                }
            }
           
        }
        public static double GetDistanceBetweenPoints(Point p, Point q)
        {
            double a = p.X - q.X;
            double b = p.Y - q.Y;
            double distance = Math.Sqrt(a * a + b * b);
            return distance;
        }
        private bool IsDoubleTap(TouchEventArgs e)
        {
            Point currentTapPosition = e.GetTouchPoint(this).Position;
            bool tapsAreCloseInDistance = GetDistanceBetweenPoints(currentTapPosition, _lastTapLocation) < 40;
            _lastTapLocation = currentTapPosition;

            TimeSpan elapsed = _doubleTapStopwatch.Elapsed;
            _doubleTapStopwatch.Restart();
            bool tapsAreCloseInTime = (elapsed != TimeSpan.Zero && elapsed < TimeSpan.FromSeconds(0.7));

            return tapsAreCloseInDistance && tapsAreCloseInTime;
        }

        private void Trendlist_TouchDown(object sender, TouchEventArgs e)
        {
            if (IsDoubleTap(e))
                ((TrendAdapter)this.DataContext).OnShowTrendCurveConfigurationCommandExecuted(null);
        }
    }
}
