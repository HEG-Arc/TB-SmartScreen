using Microsoft.Kinect;
using Microsoft.Kinect.Wpf.Controls;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using SCE_ProductionChain.Model;
using SCE_ProductionChain.Pages;
using SCE_ProductionChain.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SCE_ProductionChain
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double COLOR_SCALE_RATIO = 4.5;
        private const int PIXELS_PER_BYTE = 4;

        private const double SPEECH_CONFIDENCE_THRESHOLD = 0.6;

        private App app;
        private KinectSensor sensor;
        private MultiSourceFrameReader msfr;
        private byte[] cfDataConverted;
        private WriteableBitmap cfBitmap;

        private CoordinateMapper coordinateMapper;
        private Body[] bodies;

        // Speech
        private KinectAudioStream convertStream = null;
        private SpeechRecognitionEngine speechEngine = null;

        private bool debug = false;
        private bool acceptSpeechForMultiuser;
        private bool acceptSpeechForExchangeHours;

        public MainWindow()
        {
            InitializeComponent();
            app = (App)Application.Current;
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            initKinect();
            initVoiceRecognizer();
            //this.frame.Navigate(new IdentificationPage());

            /* Simule deux utilisateurs connectés */
            debug = true;
            app.availableUsers[0].Color = Drawer.BodyColors[2];
            app.availableUsers[1].Color = Drawer.BodyColors[0];
            app.users.Add(app.availableUsers[0]);
            app.users.Add(app.availableUsers[1]);
            this.frame.Navigate(new CalendarPage());
            /* */
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

            //KinectCoreWindow kinectCoreWindow = KinectCoreWindow.GetForCurrentThread();
            //kinectCoreWindow.PointerMoved += KinectCoreWindow_PointerMoved; ;

            msfr = sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body);
            app.msfr = msfr;
            msfr.MultiSourceFrameArrived += Msfr_MultiSourceFrameArrived;
        }

        private static RecognizerInfo TryGetKinectRecognizer()
        {
            IEnumerable<RecognizerInfo> recognizers;

            // This is required to catch the case when an expected recognizer is not installed.
            // By default - the x86 Speech Runtime is always expected. 
            try
            {
                recognizers = SpeechRecognitionEngine.InstalledRecognizers();
            }
            catch (COMException)
            {
                return null;
            }

            foreach (RecognizerInfo recognizer in recognizers)
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "fr-FR".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }
            return null;
        }

        private void initVoiceRecognizer()
        {
            IReadOnlyList<AudioBeam> audioBeamList = this.sensor.AudioSource.AudioBeams;
            System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

            // create the convert stream
            this.convertStream = new KinectAudioStream(audioStream);

            RecognizerInfo ri = TryGetKinectRecognizer();

            if (ri != null)
            {
                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar)))
                {
                    var g = new Grammar(memoryStream);
                    this.speechEngine.LoadGrammar(g);
                }
                this.speechEngine.SpeechRecognized += SpeechEngine_SpeechRecognized; ;

                // let the convertStream know speech is going active
                this.convertStream.SpeechActive = true;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                this.speechEngine.SetInputToAudioStream(
                    this.convertStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                // NoSpeechRecognizer
            }
        }

        private void SpeechEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            double confidenceLevel = e.Result.Confidence;
            if (confidenceLevel >= SPEECH_CONFIDENCE_THRESHOLD)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "EXCHANGE":
                        if (this.acceptSpeechForExchangeHours)
                            app.navigateToConfirmExchangePage(this.frame.NavigationService);                            
                        break;

                    case "ACCEPT":
                        if (app.onConfirmExchangePage)
                        {

                        }
                        else if (this.acceptSpeechForMultiuser)
                            app.navigateToIdentificationPage(this.frame.NavigationService);
                        break;

                    case "REFUSE":
                        if (app.onConfirmExchangePage)
                        {
                            app.timeSlotsToTransact.Clear();
                            app.navigateToCalendarPage(this.frame.NavigationService);
                        }
                        else if (this.acceptSpeechForMultiuser)
                        {
                            if (app.onConfirmMultiuserPage)
                                app.navigateToPageBeforeExit(this.frame.NavigationService);                                
                        }
                        break;

                    case "RECONNECT":
                        if (app.onConfirUserExitPage)
                            app.navigateToIdentificationPage(this.frame.NavigationService);
                        break;

                    case "EXIT":
                        if (app.onConfirUserExitPage)
                            app.navigateToPageBeforeExit(this.frame.NavigationService);
                        break;
                }
            }
        }

        //private void KinectCoreWindow_PointerMoved(object sender, KinectPointerEventArgs e)
        //{
        //    KinectPointerPoint kinectPointerPoint = e.CurrentPoint;
        //    Point kinectPointerPosition = new Point();
        //    Ellipse handPointer = new Ellipse() { Height = 20, Width = 20, Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };

        //    this.handGestureCanvas.Children.Clear();
        //    kinectPointerPosition.X = kinectPointerPoint.Position.X * handGestureCanvas.ActualWidth;
        //    kinectPointerPosition.Y = kinectPointerPoint.Position.Y * handGestureCanvas.ActualHeight;
        //    Canvas.SetLeft(handPointer, kinectPointerPosition.X);
        //    Canvas.SetTop(handPointer, kinectPointerPosition.Y);
        //    this.handGestureCanvas.Children.Add(handPointer);
        //}

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

                                if (!debug)
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
                                if (app.userTwoLoggedOut)
                                {
                                    app.navigateToConfirmUserExitPage(this.frame.NavigationService);
                                    app.userTwoLoggedOut = false;
                                }

                                // Lorsqu'il n'y a plus personne devant l'écran
                                if (app.users.Count <= 0)
                                    app.navigateToIdentificationPage(this.frame.NavigationService);

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
            {
                this.btnMultiUser.Visibility = Visibility.Visible;
                this.acceptSpeechForMultiuser = true;
            }
        }
        private void disableMultiuserButton()
        {
            if (this.btnMultiUser.Visibility != Visibility.Hidden)
            {
                this.btnMultiUser.Visibility = Visibility.Hidden;
                this.acceptSpeechForMultiuser = false;
            }
        }
        private void enableExchangeHoursButton()
        {
            if (this.btnExchangeHours.Visibility != Visibility.Visible)
            {
                this.btnExchangeHours.Visibility = Visibility.Visible;
                this.acceptSpeechForExchangeHours = true;
            }
        }
        private void disableExchangeHoursButton()
        {
            if (this.btnExchangeHours.Visibility != Visibility.Hidden)
            {
                this.btnExchangeHours.Visibility = Visibility.Hidden;
                this.acceptSpeechForExchangeHours = false;
            }
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
            app.navigateToConfirmMultiuserPage(this.frame.NavigationService);
        }

        private void btnExchangeHours_Click(object sender, RoutedEventArgs e)
        {
            app.navigateToConfirmExchangePage(this.frame.NavigationService);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (null != this.convertStream)
            {
                this.convertStream.SpeechActive = false;
            }

            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= this.SpeechEngine_SpeechRecognized;
                this.speechEngine.RecognizeAsyncStop();
            }

            if (null != this.sensor)
            {
                this.sensor.Close();
                this.sensor = null;
            }
        }
    }
}
