using Microsoft.Kinect;
using POC_MultiUserIdification_Collider.Pages;
using POC_MultiUserIndification_Collider.Model;
using POC_MultiUserIndification_Collider.Util;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZXing.Kinect;

namespace POC_MultiUserIndification_Collider.Pages
{
    /// <summary>
    /// Logique d'interaction pour IdentificationPage.xaml
    /// </summary>
    public partial class IdentificationPage : Page
    {
        private const int COLOR_SCALE_RATIO = 2;
        private const int PIXELS_PER_BYTE = 4;
        private const int NB_FRAMES_BEFORE_DECODE = 30;

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
        }

        private void IdentificationPage_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.GetDefault();
            coordinateMapper = sensor.CoordinateMapper;
            bodies = new Body[sensor.BodyFrameSource.BodyCount];
            collidedBodies = new List<ulong>();

            collisionEllipse = new Ellipse() { Height = 200, Width = 200, Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0)), StrokeThickness = 5 };
            initKinect();
        }

        private void drawCollisionEllipse()
        {
            Canvas.SetLeft(collisionEllipse, 250);
            Canvas.SetTop(collisionEllipse, 250);
            colorFrameCanvas.Children.Add(collisionEllipse);
        }

        private void initKinect()
        {
            FrameDescription fd = sensor.ColorFrameSource.FrameDescription;
            cfDataConverted = new byte[fd.LengthInPixels * PIXELS_PER_BYTE];
            cfBitmap = new WriteableBitmap(fd.Width, fd.Height, 96, 96, PixelFormats.Pbgra32, null);
            reader = new BarcodeReader();

            colorFrameImage.Source = cfBitmap;
            sensor.Open();

            msfr = app.msfr;
            if (app.identificationPage == null)
            {
                msfr.MultiSourceFrameArrived += Msfr_MultiSourceFrameArrived;
                app.identificationPage = this;
            }
        }


        private void Msfr_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            if (NavigationService != null)
            {
                MultiSourceFrame msf;
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
                                    lblDebug.Content = "no collisions";
                                    foreach (Body body in bodies)
                                    {
                                        if (body.IsTracked)
                                        {
                                            bool didCollide = false;

                                            colorFrameCanvas.Children.Clear();
                                            if (barcodePosition != null && barcodeContent != null)
                                            {
                                                Canvas.SetLeft(collisionEllipse, barcodePosition.X - collisionEllipse.Width / 2);
                                                Canvas.SetTop(collisionEllipse, barcodePosition.Y - collisionEllipse.Height / 2);
                                                colorFrameCanvas.Children.Add(collisionEllipse);

                                                foreach (KeyValuePair<JointType, Joint> kvJoint in body.Joints)
                                                {
                                                    Joint joint = kvJoint.Value;
                                                    ColorSpacePoint csp = coordinateMapper.MapCameraPointToColorSpace(joint.Position);
                                                    Point point = new Point() { X = csp.X / COLOR_SCALE_RATIO, Y = csp.Y / COLOR_SCALE_RATIO };

                                                    if (Collider.doesCollide(this.collisionEllipse, point))
                                                    {
                                                        lblDebug.Content = joint.JointType;
                                                        didCollide = true;
                                                    }
                                                }
                                            }

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

                /*
                if(collidedBodies.Count == 1)
                {
                    User user = new User(collidedBodies[0].TrackingId, result.ToString());
                    lblUser.Content = "ID : " + user.BodyId + " Username : " + user.Username;
                } 
                */
            }
        }

        private void TryLogin(List<ulong> potentialUsers, string username)
        {
            if (NavigationService == null)
                return;
            if (username == null)
                return;
            if (potentialUsers.Count == 0)
                return;
            else if (potentialUsers.Count > 1)
                return;

            User user = new User(potentialUsers[0], username);

            if (app.users.Contains(user))
                return;

            app.users.Add(user);

            this.barcodeContent = null;

            if (app.mainPage != null)
                this.NavigationService.Navigate(app.mainPage);
            else
                this.NavigationService.Navigate(new MainPage());
            //lblDebug.Content = "id : " + user.BodyId + " name : " + username;
        }
    }
}
