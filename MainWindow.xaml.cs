using Microsoft.Kinect;
using Microsoft.Kinect.Input;
using Microsoft.Kinect.Wpf.Controls;
using SCE_ProductionChain.Model;
using SCE_ProductionChain.Pages;
using SCE_ProductionChain.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SCE_ProductionChain
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double COLOR_SCALE_RATIO = 4.5;
        private const int PIXELS_PER_BYTE = 4;

        private App app;
        private KinectSensor sensor;
        private MultiSourceFrameReader msfr;
        private byte[] cfDataConverted;
        private WriteableBitmap cfBitmap;

        private CoordinateMapper coordinateMapper;
        private Body[] bodies;

        public MainWindow()
        {
            InitializeComponent();
            app = (App)Application.Current;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            initKinect();
            this.frame.Navigate(new IdentificationPage());
            //this.frame.Navigate(new CalendarPage());
        }

        private void initKinect()
        {
            sensor = KinectSensor.GetDefault();
            kinectRegion.KinectSensor = KinectSensor.GetDefault();
            KinectRegion.SetKinectRegion(this, kinectRegion);

            coordinateMapper = sensor.CoordinateMapper;
            bodies = new Body[sensor.BodyFrameSource.BodyCount];

            FrameDescription fd = sensor.ColorFrameSource.FrameDescription;
            cfDataConverted = new byte[fd.LengthInPixels * PIXELS_PER_BYTE];
            cfBitmap = new WriteableBitmap(fd.Width, fd.Height, 96, 96, PixelFormats.Pbgra32, null);

            multiSourceFrameIndicator.Source = cfBitmap;
            sensor.Open();

            KinectCoreWindow kinectCoreWindow = KinectCoreWindow.GetForCurrentThread();
            kinectCoreWindow.PointerMoved += KinectCoreWindow_PointerMoved; ;

            msfr = sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body);
            app.msfr = msfr;
            msfr.MultiSourceFrameArrived += Msfr_MultiSourceFrameArrived;
        }

        private void KinectCoreWindow_PointerMoved(object sender, KinectPointerEventArgs e)
        {
            KinectPointerPoint kinectPointerPoint = e.CurrentPoint;
            Point kinectPointerPosition = new Point();            
            Ellipse handPointer = new Ellipse() { Height = 20, Width = 20, Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };

            this.handGestureCanvas.Children.Clear();
            kinectPointerPosition.X = kinectPointerPoint.Position.X * handGestureCanvas.ActualWidth;
            kinectPointerPosition.Y = kinectPointerPoint.Position.Y * handGestureCanvas.ActualHeight;
            Canvas.SetLeft(handPointer, kinectPointerPosition.X);
            Canvas.SetTop(handPointer, kinectPointerPosition.Y);
            this.handGestureCanvas.Children.Add(handPointer);
        }

        private void Msfr_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrame msf;
            Joint headJoint = new Joint();
            User currentUser = null;

            try
            {
                msf = e.FrameReference.AcquireFrame();
                if (msf != null)
                {
                    using (BodyFrame bodyFrame = msf.BodyFrameReference.AcquireFrame())
                    {
                        using (ColorFrame colorFrame = msf.ColorFrameReference.AcquireFrame())
                        {
                            if (bodyFrame != null && colorFrame != null)
                            {
                                // Gestion des corps
                                bodyFrame.GetAndRefreshBodyData(bodies);
                                app.trackedBodies.Clear();
                                multiSourceFrameIndicatorCanvas.Children.Clear();
                                foreach (Body body in bodies)
                                {
                                    if (body.IsTracked)
                                    {
                                        app.trackedBodies.Add(body.TrackingId);

                                        body.Joints.TryGetValue(JointType.Head, out headJoint);
                                        ColorSpacePoint csp = coordinateMapper.MapCameraPointToColorSpace(headJoint.Position);
                                        Point headPosition = new Point() { X = csp.X / COLOR_SCALE_RATIO, Y = csp.Y / COLOR_SCALE_RATIO };

                                        currentUser = app.getUser(body.TrackingId);
                                        //DrawHeadRectangle(headJoint, headPosition, currentUser, userIndex);
                                        Drawer.DrawHeadRectangle(headJoint, headPosition, currentUser, app.users.IndexOf(currentUser), null,
                                                                 multiSourceFrameIndicatorCanvas, COLOR_SCALE_RATIO);
                                    }
                                }

                                app.UpdateUsers();
                                app.UpdateUnidentified();

                                // Lorqu'une personne non identifiée est présente devant l'écran
                                if (app.unidentifiedBodies.Count > 0 && app.unidentifiedBodies.Count <= app.LIMIT_USERS && 
                                    (app.onCalendarPage || app.onStatisticsPage))
                                    enableMultiuserButton();
                                else
                                    disableMultiuserButton();

                                // Lorsqu'une ou plusieurs plage horaires sont sélectionnées ET qu'on se 
                                // trouve sur la page du calendrier
                                if (app.timeSlotsToTransact.Count > 0 && app.onCalendarPage)
                                    enableExchangeHoursButton();
                                else
                                    disableExchangeHoursButton();

                                // Si un des deux utilisateurs se délogue
                                if(app.userTwoLoggedOut)
                                {
                                    app.navigateToConfirmUserExitPage(this.frame);
                                    app.userTwoLoggedOut = false;
                                }

                                // Lorsqu'il n'y a plus personne devant l'écran
                                if (app.users.Count <= 0)
                                    app.navigateToIdentificationPage(this.frame);

                                // Affichage des images couleurs à l'écran
                                colorFrame.CopyConvertedFrameDataToArray(cfDataConverted, ColorImageFormat.Bgra);
                                Int32Rect rect = new Int32Rect(0, 0, (int)cfBitmap.Width, (int)cfBitmap.Height);
                                int stride = (int)cfBitmap.Width * PIXELS_PER_BYTE;
                                cfBitmap.WritePixels(rect, cfDataConverted, stride, 0);
                            }
                        }
                    }
                }
            }
            catch { }

            this.updateUI();
        }

        private void enableMultiuserButton()
        {
            if (this.btnMultiUser.Visibility != Visibility.Visible)
                this.btnMultiUser.Visibility = Visibility.Visible;
        }
        private void disableMultiuserButton()
        {
            if (this.btnMultiUser.Visibility != Visibility.Hidden)
                this.btnMultiUser.Visibility = Visibility.Hidden;
        }
        private void enableExchangeHoursButton()
        {
            if (this.btnExchangeHours.Visibility != Visibility.Visible)
                this.btnExchangeHours.Visibility = Visibility.Visible;
        }
        private void disableExchangeHoursButton()
        {
            if (this.btnExchangeHours.Visibility != Visibility.Hidden)
                this.btnExchangeHours.Visibility = Visibility.Hidden;
        }
        private void updateUI()
        {
            if (app.onIdentificationPage)
                initForIdentificationPage();
            else if (app.onConfirmationPage)
                initForConfirmationPages();
            else
                initForOtherPages();
        }
        private void initForIdentificationPage()
        {
            if (this.multiSourceFrameIndicator.Visibility != Visibility.Hidden)
            {
                this.multiSourceFrameIndicator.Visibility = Visibility.Hidden;
                this.multiSourceFrameIndicatorCanvas.Visibility = Visibility.Hidden;
                this.btnCalendar.Visibility = Visibility.Hidden;
                this.btnStatistics.Visibility = Visibility.Hidden;
            }
        }

        private void initForConfirmationPages()
        {
            if (this.btnCalendar.Visibility != Visibility.Hidden)
            {
                this.btnExchangeHours.Visibility = Visibility.Hidden;
                this.btnCalendar.Visibility = Visibility.Hidden;
                this.btnStatistics.Visibility = Visibility.Hidden;
            }
        }

        private void initForOtherPages()
        {
            if (this.btnCalendar.Visibility != Visibility.Visible)
            {
                this.multiSourceFrameIndicator.Visibility = Visibility.Visible;
                this.multiSourceFrameIndicatorCanvas.Visibility = Visibility.Visible;
                this.btnCalendar.Visibility = Visibility.Visible;
                this.btnStatistics.Visibility = Visibility.Visible;
            }
        }

        private void btnCalendar_Click(object sender, RoutedEventArgs e)
        {
            if (app.calendarPage != null)
                this.frame.Navigate(app.calendarPage);
            else
                this.frame.Navigate(new CalendarPage());
        }

        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            if (app.statisticsPage != null)
                this.frame.Navigate(app.statisticsPage);
            else
                this.frame.Navigate(new StatisticsPage());
        }

        private void btnMultiUser_Click(object sender, RoutedEventArgs e)
        {
            app.navigateToConfirmMultiuserPage(this.frame);
        }

        private void btnExchangeHours_Click(object sender, RoutedEventArgs e)
        {
            app.navigateToConfirmExchangePage(this.frame);
        }    
    }
}
