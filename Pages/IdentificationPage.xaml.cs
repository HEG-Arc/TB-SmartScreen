using Microsoft.Kinect;
using SCE_ProductionChain.Model;
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
using ZXing.Kinect;

namespace SCE_ProductionChain.Pages
{
    /// <summary>
    /// Logique d'interaction pour IdentificationPage.xaml
    /// </summary>
    public partial class IdentificationPage : Page
    {
        private const double COLOR_SCALE_RATIO = 2.16;
        private const int PIXELS_PER_BYTE = 4;
        private const int NB_FRAMES_BEFORE_DECODE = 30;
        private const double DISTANCE_FOR_SCANNING = 1.2;

        private App app;
        private KinectSensor sensor;
        private MultiSourceFrameReader msfr;
        private byte[] cfDataConverted;
        private WriteableBitmap cfBitmap;
        private CoordinateMapper coordinateMapper;

        private Body[] bodies;
        private List<ulong> collidedBodies;

        private int cptFrame = 0;
        private BarcodeReader reader;

        private Point barcodePosition;
        private string barcodeContent;

        private Ellipse collisionEllipse;

        public IdentificationPage()
        {
            InitializeComponent();
            app = (App)Application.Current;
            this.Loaded += IdentificationPage_Loaded;
            this.Unloaded += IdentificationPage_Unloaded;
        }

        private void IdentificationPage_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.GetDefault();
            coordinateMapper = sensor.CoordinateMapper;
            bodies = new Body[sensor.BodyFrameSource.BodyCount];
            collidedBodies = new List<ulong>();
            collisionEllipse = new Ellipse() { Height = 200, Width = 200, Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0)), StrokeThickness = 5 };
            app.onIdentificationPage = true;

            initUI();
            initKinect();
        }

        private void initUI()
        {
            this.tbMessage.Text = Properties.Resources.IdentificationShowCard;
        }
        private void initKinect()
        {
            FrameDescription fd = sensor.ColorFrameSource.FrameDescription;
            cfDataConverted = new byte[fd.LengthInPixels * PIXELS_PER_BYTE];
            cfBitmap = new WriteableBitmap(fd.Width, fd.Height, 96, 96, PixelFormats.Pbgra32, null);
            reader = new BarcodeReader();

            multiSourceFrameImage.Source = cfBitmap;
            sensor.Open();

            msfr = app.msfr;
            if (app.identificationPage == null)
            {
                msfr.MultiSourceFrameArrived += Msfr_MultiSourceFrameArrived;
                app.identificationPage = this;
            }
        }

        private void navigateToHomePage()
        {
            if (app.calendarPage != null)
                this.NavigationService.Navigate(app.calendarPage);
            else
                this.NavigationService.Navigate(new CalendarPage());
        }

        private void Msfr_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            if (NavigationService != null)
            {
                MultiSourceFrame msf;
                Joint joint;
                ColorSpacePoint csp;
                Point point;

                User currentUser;
                bool isDistanceOK = false;

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
                                    multiSourceFrameCanvas.Children.Clear();

                                    if (barcodePosition != null && barcodeContent != null)
                                    {
                                        Canvas.SetLeft(collisionEllipse, barcodePosition.X - collisionEllipse.Width / 2);
                                        Canvas.SetTop(collisionEllipse, barcodePosition.Y - collisionEllipse.Height / 2);
                                        //multiSourceFrameCanvas.Children.Add(collisionEllipse);
                                    }

                                    foreach (Body body in bodies)
                                    {
                                        if (body.IsTracked)
                                        {
                                            bool didCollide = false;
                                            currentUser = app.getUser(body.TrackingId);

                                            if (barcodePosition != null && barcodeContent != null)
                                            {
                                                foreach (KeyValuePair<JointType, Joint> kvJoint in body.Joints)
                                                {
                                                    joint = kvJoint.Value;
                                                    csp = coordinateMapper.MapCameraPointToColorSpace(joint.Position);
                                                    point = new Point() { X = csp.X / COLOR_SCALE_RATIO, Y = csp.Y / COLOR_SCALE_RATIO };

                                                    if (Collider.doesCollide(this.collisionEllipse, point))
                                                        didCollide = true;
                                                }
                                            }

                                            body.Joints.TryGetValue(JointType.Head, out joint);
                                            csp = coordinateMapper.MapCameraPointToColorSpace(joint.Position);
                                            point = new Point() { X = csp.X / COLOR_SCALE_RATIO, Y = csp.Y / COLOR_SCALE_RATIO };

                                            isDistanceOK = (joint.Position.Z < DISTANCE_FOR_SCANNING);
                                            if(isDistanceOK)
                                            {
                                                if (cptFrame > NB_FRAMES_BEFORE_DECODE)
                                                {
                                                    this.DecodeFrame(colorFrame);
                                                    cptFrame = 0;
                                                }
                                                this.tbMessage.Text = Properties.Resources.IdentificationShowCard;
                                            }
                                            else
                                            {
                                                this.tbMessage.Text = Properties.Resources.IdentificationGetCloser;
                                            }
                                                
                                            Drawer.DrawHeadRectangle(joint, point, currentUser, app.users.IndexOf(currentUser), isDistanceOK,
                                                                 multiSourceFrameCanvas, COLOR_SCALE_RATIO);

                                            if (didCollide)
                                                collidedBodies.Add(body.TrackingId);
                                        }
                                    }

                                    // Affichage des images couleurs à l'écran
                                    colorFrame.CopyConvertedFrameDataToArray(cfDataConverted, ColorImageFormat.Bgra);
                                    Int32Rect rect = new Int32Rect(0, 0, (int)cfBitmap.Width, (int)cfBitmap.Height);
                                    int stride = (int)cfBitmap.Width * PIXELS_PER_BYTE;
                                    cfBitmap.WritePixels(rect, cfDataConverted, stride, 0);

                                    if (collidedBodies.Count > 0)
                                        this.TryLogin(collidedBodies, barcodeContent);

                                    // Scannage
                                    if (cptFrame > NB_FRAMES_BEFORE_DECODE)
                                    {
                                        this.DecodeFrame(colorFrame);
                                        cptFrame = 0;
                                    }
                                    cptFrame++;
                                    collidedBodies.Clear();
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private void DecodeFrame(ColorFrame colorFrame)
        {
            var result = reader.Decode(colorFrame);
            if (result != null)
            {
                barcodePosition = new Point() { X = result.ResultPoints[0].X / COLOR_SCALE_RATIO, Y = result.ResultPoints[0].Y / COLOR_SCALE_RATIO };
                barcodeContent = result.ToString();
            }
        }

        private void TryLogin(List<ulong> potentialUsers, string userCode)
        {
            if (NavigationService == null)
                return;
            if (userCode == null)
                return;
            if (potentialUsers.Count == 0)
                return;
            else if (potentialUsers.Count > 1)
            {
                this.lblError.Content = Properties.Resources.IdentificationErrorTooCloseToOtherPeople;
                return;
            }

            foreach (User user in app.users)
            {
                if (user.Code.Equals(userCode) || user.BodyId.Equals(potentialUsers[0]))
                {
                    this.lblError.Content = user.Username + Properties.Resources.IdentificationErrorAlreadyIdentified;
                    return;
                }
            }

            User currentUser = null;
            foreach (User user in app.availableUsers)
            {
                if (user.Code.Equals(userCode))
                {
                    currentUser = user;
                    currentUser.BodyId = potentialUsers[0];
                    continue;
                }
            }

            if (currentUser != null)
            {
                app.users.Add(currentUser);
                navigateToHomePage();
            }
            else
            {
                this.lblError.Content = Properties.Resources.IdentificationErrorAccessProhibited;
            }

            this.barcodeContent = null;
        }

        private void IdentificationPage_Unloaded(object sender, RoutedEventArgs e)
        {
            app.onIdentificationPage = false;
        }
    }
}
