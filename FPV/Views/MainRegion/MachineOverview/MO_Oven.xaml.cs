using HMI.Views.DialogRegion;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using VisiWin.ApplicationFramework;
using VisiWin.Controls;
using VisiWin.DataAccess;
using VisiWin.Recipe;
using HMI.UserControls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Threading;

namespace HMI.Views.MainRegion.MachineOverview
{

    [ExportView("MO_Oven")]
    public partial class MO_Oven : VisiWin.Controls.View
    {
        IVariableService VS = ApplicationService.GetService<IVariableService>();
        IVariable WM1;
        IVariable WM2;
        IVariable WM3;
        IVariable HVTO_Takt;
        IVariable HNTO_Takt;
        IVariable HVTiO;
        IVariable HNTiO;

        private bool active = false;
        public MO_Oven()
        {
            InitializeComponent();
        }

        private void View_Initialized(object sender, System.EventArgs e)
        {
            WM1 = VS.GetVariable("PLC.PLC.Blocks.5 Modul 3 Trockner.02 HVT.Fahrwagen.02 Fahrantrieb.DB HVT Fahrwagen Status.In Position.Ofen");
            WM1.Change += WM_Change;

            WM2 = VS.GetVariable("PLC.PLC.Blocks.5 Modul 3 Trockner.02 HVT.Fahrwagen.00 Allgemein.DB HVT Fahrwagen Tablett PD.Status.Charge.Material vorhanden");
            WM2.Change += WM2_Change;

            WM3 = VS.GetVariable("PLC.PLC.Blocks.5 Modul 3 Trockner.04 HNT.02 Fahrawagen.02 Fahrantrieb.DB HNT Fahrwagen Status.In Position.Ofen");
            WM3.Change += WM3_Change;

            HVTO_Takt = VS.GetVariable("PLC.PLC.Blocks.5 Modul 3 Trockner.02 HVT.Paletten Schieber.DB HVT Palettenschieber Status.Allgemein.Flanke Transport in Ofen erfolgreich");
            HVTO_Takt.Change += O_Takt_Change;

            HNTO_Takt = VS.GetVariable("PLC.PLC.Blocks.5 Modul 3 Trockner.04 HNT.03 Paletten Schieber.DB HNT Palettenschieber Status.Allgemein.Flanke Transport in Kühlzone erfolgreich");
            HNTO_Takt.Change += O_Takt_Change;

            HVTiO = VS.GetVariable("PLC.PLC.Blocks.5 Modul 3 Trockner.02 HVT.Fahrwagen.02 Fahrantrieb.DB HVT Fahrwagen Status.In Position.Ofen");
            HVTiO.Change += O_Takt_Change;

            HNTiO = VS.GetVariable("PLC.PLC.Blocks.5 Modul 3 Trockner.04 HNT.02 Fahrawagen.02 Fahrantrieb.DB HNT Fahrwagen Status.In Position.Ofen");
            HNTiO.Change += O_Takt_Change;
        }

  

        private void WM3_Change(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                HNT_T.Margin = new Thickness(1490, 376, 0, 0);
                btnhnt.Margin = new Thickness(1506, 338, 0, 0);
            }
            else
            {
                HNT_T.Margin = new Thickness(1427, 575, 0, 0);
                btnhnt.Margin = new Thickness(1443, 552, 0, 0);
            }
        }

        private void WM_Change(object sender, VariableEventArgs e)
        {
            if (WM2 != null && (bool)WM2.Value)
            {
                if ((bool)e.Value)
                {
                    btnhvt.Margin = new Thickness(275, 292, 0, 0);
                    HVT_T2.Visibility = Visibility.Hidden;
                    HVT_T1.Visibility = Visibility.Visible;
                }
                else
                {
                    btnhvt.Margin = new Thickness(223, 485, 0, 0);
                    HVT_T2.Visibility = Visibility.Visible;
                    HVT_T1.Visibility = Visibility.Hidden; 
                }
            }
            else
            {
                HVT_T2.Visibility = Visibility.Hidden;
                HVT_T1.Visibility = Visibility.Hidden;
                if ((bool)e.Value)
                {
                    btnhvt.Margin = new Thickness(275, 292, 0, 0);
                }
                else
                {
                    btnhvt.Margin = new Thickness(223, 485, 0, 0);
                }
            }
        }

        private void WM2_Change(object sender, VariableEventArgs e)
        {
            if ((bool)e.Value)
            {
                if ((bool)WM1.Value)
                {
                    btnhvt.Margin = new Thickness(223, 485, 0, 0);
                    HVT_T2.Visibility = Visibility.Visible;
                    HVT_T1.Visibility = Visibility.Hidden;
                }
                else
                {
                    btnhvt.Margin = new Thickness(275, 292, 0, 0);
                    HVT_T2.Visibility = Visibility.Hidden;
                    HVT_T1.Visibility = Visibility.Visible;
                }
            }
            else
            {
                HVT_T2.Visibility = Visibility.Hidden;
                HVT_T1.Visibility = Visibility.Hidden;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ApplicationService.SetVariableValue("Trend.ID", ((System.Windows.Controls.Button)sender).Tag);
            ApplicationService.SetView("MainRegion", "TrendView");
        }

        private void PictureBox_PreviewTouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            if (sender.GetType().Name == "Tracking")
            {
                Track m = (Track)((Tracking)sender).Tag;
                SP temp = new SP(m.type, m.modul, m.part, m.DK);
            }
            else
            {
                string[] a = ((PictureBox)sender).Tag.ToString().Split('*');
                SP temp = new SP(a[0], a[1], a[2], a[3]);
            }
        }

        private void PictureBox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender.GetType().Name == "Tracking")
            {
                Track m = (Track)((Tracking)sender).Tag;
                SP temp = new SP(m.type, m.modul, m.part, m.DK);
            }
            else
            {
                string [] a = ((PictureBox)sender).Tag.ToString().Split('*');
                SP temp = new SP(a[0], a[1], a[2], a[3]);
            }
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogView.Show("MO_Status_Transport", "Status Transporte", DialogButton.OK, DialogResult.Cancel);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string[] m = (((System.Windows.Controls.Button)sender).Tag.ToString()).Split('*');
            SP temp = new SP(m[0], m[1], m[2], m[3]);
        }

        private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible && !active)
            { 
                DoWork();
            }
        }

        private void O_Takt_Change(object sender, VariableEventArgs e)
        {
            if (!active)
            {
                DoWork();
            }
        }

        private void DoWork()
        {
            active = true;
            Task obTask = Task.Run(async () =>
            {
                await ClearTrack();
                await AddTrack(GetDataStack());
                active = false;
            });
        }

        private async Task<bool> ClearTrack()
        {
            List<Tracking> _track = new List<Tracking>();
            await Application.Current.Dispatcher.InvokeAsync((Action) delegate
            {
                foreach (object c in LayoutRoot.Children)
                {
                    if (c.GetType().Name == "Tracking")
                    {
                        _track.Add((Tracking)c);
                    }
                }
            });

            foreach (Tracking t in _track)
            {
                await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    DoubleAnimation animation = new DoubleAnimation();
                    animation.To = 0;
                    Storyboard.SetTarget(animation, t);
                    Storyboard.SetTargetProperty(animation, new PropertyPath(UserControl.OpacityProperty));
                    animation.Completed += (ss, ee) => { LayoutRoot.Children.Remove(t); };
                    Storyboard sb = new Storyboard();
                    sb.Children.Add(animation);
                    sb.Begin();
                });
            }

            return true;
        }

        private List<Track> GetDataStack()
        {
            List<string> l = new List<string>();

            l.Add(ApplicationService.GetVariableValue("PLC.PLC.Blocks.5 Modul 3 Trockner.02 HVT.Fahrwagen.00 Allgemein.DB HVT Fahrwagen Tablett PD.Header.Produktionsnummer#STRING40").ToString());
            l.Add(ApplicationService.GetVariableValue("PLC.PLC.Blocks.5 Modul 3 Trockner.02 HVT.Ofen Band 2.DB Ofen Band 2 PD.Header.Produktionsnummer#STRING40").ToString());

            for (int i = 6; i <= 31; i++)
            {
                l.Add(ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Ofen.Tablett[" + i.ToString() + "].Header.Produktionsnummer#STRING40").ToString());
            }

            l.Add(ApplicationService.GetVariableValue("PLC.PLC.Blocks.5 Modul 3 Trockner.04 HNT.02 Fahrawagen.00 Allgemein.DB HNT Fahrwagen PD.Header.Produktionsnummer#STRING40").ToString());


            for (int i = 1; i <= 31; i++)
            {
                l.Add(ApplicationService.GetVariableValue("PLC.PLC.Blocks.07 Tracking / Kommunikation.Tracking Kühlzone.Tablett[" + i.ToString() + "].Header.Produktionsnummer#STRING40").ToString());
            }

            l.Add(ApplicationService.GetVariableValue("PLC.PLC.Blocks.5 Modul 3 Trockner.03 Ausschleussen.02 Tablett Auskippen.Allgemein.DB Tablett Auskippen PD.Header.Produktionsnummer#STRING40").ToString());

            List <Track> lx = new List<Track>();
            string x = "";
            for (int i = l.Count - 1; i >= 0; i--)
            {
                if (l[i] != x)
                {
                    if (l[i] != "")
                    {
                        lx.Add(new Track(l[i], i));
                    }
                    x = l[i];
                }
            }
            return lx;
        }

        private async Task<bool> AddTrack(List<Track> _track)
        {
            foreach (Track t in _track)
            {
                Thread.Sleep(200);
                await Application.Current.Dispatcher.InvokeAsync((Action)delegate
                {
                    Tracking a = new Tracking();
                    a.Tag = t;
                    a.Margin = t.T;
                    a.RenderTransformOrigin = new Point(0.5, 0.5);

                    RotateTransform myRotateTransform = new RotateTransform();
                    myRotateTransform.Angle = 17.445;
                    TransformGroup myTransformGroup = new TransformGroup();
                    myTransformGroup.Children.Add(myRotateTransform);
                    a.RenderTransform = myTransformGroup;

                    a.PreviewTouchDown += PictureBox_PreviewTouchDown;
                    a.MouseLeftButtonDown += PictureBox_MouseLeftButtonDown;
                       
                    Task obTasks = Task.Run(() =>
                    {
                        Application.Current.Dispatcher.InvokeAsync((Action)delegate
                        {
                            LayoutRoot.Children.Add(a);
                        });
                    });
                });
            }
            return true;
        }
    }

    public class Track
    {
        public string pon;
        public int id;
        public string type;
        public string modul;
        public string part;
        public string DK;
        public Thickness T;

        public Track(string _pon, int _id)
        {
            pon = _pon;
            id = _id;
            T = GetCoords(_id);
            setData();
        }

        private Thickness GetCoords(int id)
        {
            switch (id)
            {
                case 0:
                    bool HVTinPosOven = (bool)ApplicationService.GetVariableValue("PLC.PLC.Blocks.5 Modul 3 Trockner.02 HVT.Fahrwagen.02 Fahrantrieb.DB HVT Fahrwagen Status.In Position.Ofen");
                    if (HVTinPosOven) { return new Thickness(321, 225, 1494, 629); }
                    else { return new Thickness(273, 429, 1542, 425); }
                case 1: return new Thickness(523, 223, 1292, 631);
                case 2: return new Thickness(560, 246, 1255, 608);
                case 3: return new Thickness(596, 249, 1219, 605);
                case 4: return new Thickness(633, 251, 1182, 603);
                case 5: return new Thickness(669, 254, 1146, 600);
                case 6: return new Thickness(706, 256, 1109, 598);
                case 7: return new Thickness(744, 258, 1071, 596);
                case 8: return new Thickness(780, 260, 1035, 594);
                case 9: return new Thickness(817, 262, 998, 592);
                case 10: return new Thickness(854, 265, 961, 589);
                case 11: return new Thickness(890, 267, 925, 587);
                case 12: return new Thickness(926, 269, 889, 585);
                case 13: return new Thickness(963, 272, 852, 582);
                case 14: return new Thickness(999, 275, 816, 579);
                case 15: return new Thickness(1036, 277, 779, 577);
                case 16: return new Thickness(1073, 280, 742, 574);
                case 17: return new Thickness(1110, 282, 705, 572);
                case 18: return new Thickness(1147, 284, 668, 570);
                case 19: return new Thickness(1184, 286, 631, 568);
                case 20: return new Thickness(1219, 289, 596, 565);
                case 21: return new Thickness(1257, 291, 558, 563);
                case 22: return new Thickness(1293, 293, 522, 561);
                case 23: return new Thickness(1330, 296, 485, 558);
                case 24: return new Thickness(1367, 298, 448, 556);
                case 25: return new Thickness(1404, 301, 411, 553);
                case 26: return new Thickness(1440, 303, 375, 551);
                case 27: return new Thickness(1478, 305, 337, 549);
                case 28:
                    bool HNTinPosOven = (bool)ApplicationService.GetVariableValue("PLC.PLC.Blocks.5 Modul 3 Trockner.04 HNT.02 Fahrawagen.02 Fahrantrieb.DB HNT Fahrwagen Status.In Position.Ofen");
                    if (HNTinPosOven) { return new Thickness(1515, 308, 300, 546); }
                    else { return new Thickness(1453, 506, 362, 348); }
                case 29: return new Thickness(1415, 503, 400, 351);
                case 30: return new Thickness(1378, 501, 437, 353);
                case 31: return new Thickness(1341, 497, 474, 357);
                case 32: return new Thickness(1304, 496, 511, 358);
                case 33: return new Thickness(1267, 494, 548, 360);
                case 34: return new Thickness(1230, 492, 585, 362);
                case 35: return new Thickness(1194, 488, 621, 366);
                case 36: return new Thickness(1157, 487, 658, 367);
                case 37: return new Thickness(1121, 484, 694, 370);
                case 38: return new Thickness(1084, 481, 731, 373);
                case 39: return new Thickness(1048, 479, 767, 375);
                case 40: return new Thickness(1011, 477, 804, 377);
                case 41: return new Thickness(974, 474, 841, 380);
                case 42: return new Thickness(937, 472, 878, 382);
                case 43: return new Thickness(900, 470, 915, 384);
                case 44: return new Thickness(864, 467, 951, 387);
                case 45: return new Thickness(828, 464, 987, 390);
                case 46: return new Thickness(790, 463, 1025, 391);
                case 47: return new Thickness(754, 460, 1061, 394);
                case 48: return new Thickness(717, 458, 1098, 396);
                case 49: return new Thickness(680, 455, 1135, 399);
                case 50: return new Thickness(644, 452, 1171, 402);
                case 51: return new Thickness(607, 451, 1208, 403);
                case 52: return new Thickness(570, 449, 1245, 405);
                case 53: return new Thickness(533, 446, 1282, 408);
                case 54: return new Thickness(497, 444, 1318, 410);
                case 55: return new Thickness(460, 441, 1355, 413);
                case 56: return new Thickness(423, 439, 1392, 415);
                case 57: return new Thickness(386, 436, 1429, 418);
                case 58: return new Thickness(348, 435, 1467, 419);
                case 59: return new Thickness(311, 430, 1504, 424);
                case 60: return new Thickness(311, 430, 1504, 424);
                default: return new Thickness(0, 0, 0, 0);
            }
        }

        private void setData()
        {
            switch (id)
            {
                case 0: type = "Tablet"; modul = "3"; part = "2"; DK = "0"; break;
                case 1: type = "Belt"; modul = "3"; part = "3"; DK = "0"; break;
                case 2: type = "Tablet"; modul = "3"; part = "8"; DK = "6"; break;
                case 3: type = "Tablet"; modul = "3"; part = "8"; DK = "7"; break;
                case 4: type = "Tablet"; modul = "3"; part = "8"; DK = "8"; break;
                case 5: type = "Tablet"; modul = "3"; part = "8"; DK = "9"; break;
                case 6: type = "Tablet"; modul = "3"; part = "8"; DK = "10"; break;
                case 7: type = "Tablet"; modul = "3"; part = "8"; DK = "11"; break;
                case 8: type = "Tablet"; modul = "3"; part = "8"; DK = "12"; break;
                case 9: type = "Tablet"; modul = "3"; part = "8"; DK = "13"; break;
                case 10: type = "Tablet"; modul = "3"; part = "8"; DK = "14"; break;
                case 11: type = "Tablet"; modul = "3"; part = "8"; DK = "15"; break;
                case 12: type = "Tablet"; modul = "3"; part = "8"; DK = "16"; break;
                case 13: type = "Tablet"; modul = "3"; part = "8"; DK = "17"; break;
                case 14: type = "Tablet"; modul = "3"; part = "8"; DK = "18"; break;
                case 15: type = "Tablet"; modul = "3"; part = "8"; DK = "19"; break;
                case 16: type = "Tablet"; modul = "3"; part = "8"; DK = "20"; break;
                case 17: type = "Tablet"; modul = "3"; part = "8"; DK = "21"; break;
                case 18: type = "Tablet"; modul = "3"; part = "8"; DK = "22"; break;
                case 19: type = "Tablet"; modul = "3"; part = "8"; DK = "23"; break;
                case 20: type = "Tablet"; modul = "3"; part = "8"; DK = "24"; break;
                case 21: type = "Tablet"; modul = "3"; part = "8"; DK = "25"; break;
                case 22: type = "Tablet"; modul = "3"; part = "8"; DK = "26"; break;
                case 23: type = "Tablet"; modul = "3"; part = "8"; DK = "27"; break;
                case 24: type = "Tablet"; modul = "3"; part = "8"; DK = "28"; break;
                case 25: type = "Tablet"; modul = "3"; part = "8"; DK = "29"; break;
                case 26: type = "Tablet"; modul = "3"; part = "8"; DK = "30"; break;
                case 27: type = "Tablet"; modul = "3"; part = "8"; DK = "31"; break;
                case 28: type = "Tablet"; modul = "3"; part = "7"; DK = "0"; break;
                case 29: type = "Tablet"; modul = "3"; part = "9"; DK = "1"; break;
                case 30: type = "Tablet"; modul = "3"; part = "9"; DK = "2"; break;
                case 31: type = "Tablet"; modul = "3"; part = "9"; DK = "3"; break;
                case 32: type = "Tablet"; modul = "3"; part = "9"; DK = "4"; break;
                case 33: type = "Tablet"; modul = "3"; part = "9"; DK = "5"; break;
                case 34: type = "Tablet"; modul = "3"; part = "9"; DK = "6"; break;
                case 35: type = "Tablet"; modul = "3"; part = "9"; DK = "7"; break;
                case 36: type = "Tablet"; modul = "3"; part = "9"; DK = "8"; break;
                case 37: type = "Tablet"; modul = "3"; part = "9"; DK = "9"; break;
                case 38: type = "Tablet"; modul = "3"; part = "9"; DK = "10"; break;
                case 39: type = "Tablet"; modul = "3"; part = "9"; DK = "11"; break;
                case 40: type = "Tablet"; modul = "3"; part = "9"; DK = "12"; break;
                case 41: type = "Tablet"; modul = "3"; part = "9"; DK = "13"; break;
                case 42: type = "Tablet"; modul = "3"; part = "9"; DK = "14"; break;
                case 43: type = "Tablet"; modul = "3"; part = "9"; DK = "15"; break;
                case 44: type = "Tablet"; modul = "3"; part = "9"; DK = "16"; break;
                case 45: type = "Tablet"; modul = "3"; part = "9"; DK = "17"; break;
                case 46: type = "Tablet"; modul = "3"; part = "9"; DK = "18"; break;
                case 47: type = "Tablet"; modul = "3"; part = "9"; DK = "19"; break;
                case 48: type = "Tablet"; modul = "3"; part = "9"; DK = "20"; break;
                case 49: type = "Tablet"; modul = "3"; part = "9"; DK = "21"; break;
                case 50: type = "Tablet"; modul = "3"; part = "9"; DK = "22"; break;
                case 51: type = "Tablet"; modul = "3"; part = "9"; DK = "23"; break;
                case 52: type = "Tablet"; modul = "3"; part = "9"; DK = "24"; break;
                case 53: type = "Tablet"; modul = "3"; part = "9"; DK = "25"; break;
                case 54: type = "Tablet"; modul = "3"; part = "9"; DK = "26"; break;
                case 55: type = "Tablet"; modul = "3"; part = "9"; DK = "27"; break;
                case 56: type = "Tablet"; modul = "3"; part = "9"; DK = "28"; break;
                case 57: type = "Tablet"; modul = "3"; part = "9"; DK = "29"; break;
                case 58: type = "Tablet"; modul = "3"; part = "9"; DK = "30"; break;
                case 59: type = "Tablet"; modul = "3"; part = "9"; DK = "31"; break;
                case 60: type = "Tablet"; modul = "3"; part = "4"; DK = "0"; break;
            }
        }

    }
}



