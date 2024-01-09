using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VisiWin.ApplicationFramework;

namespace HMI.UserControls
{
    /// <summary>
    /// Interaction logic for UserControl2.xaml
    /// </summary>
    public partial class WorkMode2 : UserControl
    {
       
        int TouchClick_WM = 1;

        public VisiWin.Controls.Key BTN_START;
        public VisiWin.Controls.Key BTN_STOP;
        public string Variable_WorkingMode;

        public WorkMode2()
        {
            InitializeComponent();
            BTN_START = btnstart;
            BTN_STOP = btnstop;
        }


        private void WorkingMode_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DoWork();
        }

        private void WorkingMode_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            DoWork();
        }

        private void DoWork()
        {
            if (TouchClick_WM == 3)
            {
                TouchClick_WM = 1;
            }
            else
            {
                TouchClick_WM++;
            }

            ColorAnimation CA = new ColorAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };

            switch (TouchClick_WM)
            {
                
                case 1:
                    lbl.Content = "Automatic";
                
                    btnstart.Visibility = Visibility.Visible;
                    btnstop.Visibility = Visibility.Visible;

                    CA.To = (Color)ColorConverter.ConvertFromString("#FF025502");
                    ApplicationService.SetVariableValue(Variable_WorkingMode,1);
                    break;
                case 2:
                    lbl.Content = "Hand Mode";

                    btnstart.Visibility = Visibility.Hidden;
                    btnstop.Visibility = Visibility.Hidden;

                    CA.To = (Color)ColorConverter.ConvertFromString("#FF006AAD");
                    ApplicationService.SetVariableValue(Variable_WorkingMode, 2);
                    break;
                case 3:
                    lbl.Content = "SetUp Mode";

                    btnstart.Visibility = Visibility.Hidden;
                    btnstop.Visibility = Visibility.Hidden;
                    CA.To = (Color)ColorConverter.ConvertFromString("#FFBC49");
                    ApplicationService.SetVariableValue(Variable_WorkingMode, 3);
                    break;
            }
            BtnShadow1.Visibility = Visibility.Visible;
            this.WorkingMode.Fill.BeginAnimation(SolidColorBrush.ColorProperty, CA);
        }

        private void Btnstart_Click(object sender, RoutedEventArgs e)
        {
            btnstart.IsBlinkEnabled = true;
        }

        private void lbl_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            BtnShadow1.Visibility = Visibility.Hidden;
        }
    }
}
