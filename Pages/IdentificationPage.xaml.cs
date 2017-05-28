using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing.Kinect;
using System;
using System.Windows.Shapes;
using POC_MultiUserIdentification.Model;

namespace POC_MultiUserIdentification.Pages
{
    /// <summary>
    /// Logique d'interaction pour IdentificationPage.xaml
    /// </summary>
    public partial class IdentificationPage : Page
    {
        private const int COLOR_SCALE_RATIO = 4;
        private const int NB_FRAMES_BEFORE_DECODE = 30;
        private const int PIXELS_PER_BYTE = 4;

        private App app;
        private MultiSourceFrameReader msfr;
        private KinectSensor sensor;
        private byte[] cfDataConverted;
        private WriteableBitmap cfBitmap;
        private BarcodeReader reader;
        private Body[] bodies;

        private DepthSpacePoint[] colorMappedToDepthPoints = null;
        private CoordinateMapper coordinateMapper = null;

        private int cptFrame = 0;
        private bool scanning = true;

        private String currentBarcode;
        private ZXing.ResultPoint barcodePosition = null;

        private uint[] bifDataConverted;

        public IdentificationPage()
        {
            InitializeComponent();

            app = (App)Application.Current;
            sensor = app.sensor;
            msfr = app.msfr;
            bifDataConverted = app.bodyIndexFrameDataConverted;            

            app.timer.Tick += Timer_Tick;
            this.Loaded += IdentificationPage_Loaded;
            this.Unloaded += IdentificationPage_Unloaded;
        }

        private void IdentificationPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.coordinateMapper = sensor.CoordinateMapper;

            FrameDescription fd = sensor.ColorFrameSource.FrameDescription;
            app.colorMappedToDepthPoints = new DepthSpacePoint[fd.Width * fd.Height];
            colorMappedToDepthPoints = app.colorMappedToDepthPoints;

            cfDataConverted = new byte[fd.LengthInPixels * PIXELS_PER_BYTE];
            cfBitmap = new WriteableBitmap(fd.Width, fd.Height, 96, 96, PixelFormats.Pbgra32, null);
            reader = new BarcodeReader();

            image.Source = cfBitmap;
            DisableScanning();

            bodies = new Body[6];

            if (app.cptMsfrE == 1)
            {
                msfr.MultiSourceFrameArrived += MsfReader_MultiSourceFrameArrived;
                app.cptMsfrE++;
            }
        }

        private void MsfReader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                MultiSourceFrame msf;
                bool oneBodyTracked = false;

                try
                {
                    msf = e.FrameReference.AcquireFrame();
                    if (msf != null)
                    {                    
                        using (ColorFrame colorFrame = msf.ColorFrameReference.AcquireFrame())
                        {
                            if(colorFrame != null)
                            {
                                colorFrame.CopyConvertedFrameDataToArray(cfDataConverted, ColorImageFormat.Bgra);
                                Int32Rect rect = new Int32Rect(0, 0, (int)cfBitmap.Width, (int)cfBitmap.Height);
                                int stride = (int)cfBitmap.Width * PIXELS_PER_BYTE;
                                cfBitmap.WritePixels(rect, cfDataConverted, stride, 0);

                                // Wait a number of frame before decoding to avoid
                                // a jerky image output resulting from this slow
                                // process.
                                if (scanning)
                                {
                                    if (cptFrame > NB_FRAMES_BEFORE_DECODE)
                                    {
                                        this.DecodeFrame(colorFrame);
                                        cptFrame = 0;
                                    }
                                    cptFrame++;
                                }

                                using (BodyFrame bodyFrame = msf.BodyFrameReference.AcquireFrame())
                                {
                                    using(DepthFrame bodyIndexFrame = msf.DepthFrameReference.AcquireFrame())
                                    {
                                        if(bodyFrame != null && bodyIndexFrame != null)
                                        {
                                            using (KinectBuffer depthFrameData = bodyIndexFrame.LockImageBuffer())
                                            {
                                                this.coordinateMapper.MapColorFrameToDepthSpaceUsingIntPtr(
                                                    depthFrameData.UnderlyingBuffer,
                                                    depthFrameData.Size,
                                                    this.colorMappedToDepthPoints);
                                            }

                                            bodyFrame.GetAndRefreshBodyData(bodies);
                                            foreach (Body body in bodies)
                                            {
                                                if (body.IsTracked)
                                                    oneBodyTracked = true;
                                            }

                                            if (currentBarcode != null && barcodePosition != null)
                                            {
                                                if (app.currentPerson != 0)
                                                {
                                                    this.Login(currentBarcode, app.currentPerson);
                                                    currentBarcode = null;
                                                    app.currentPerson = 0;
                                                }                                                    
                                                else
                                                    this.getUser(barcodePosition);
                                            }
                                        }                                        
                                    }
                                }
                            }                            
                        }                                                                       
                    }
                }
                catch
                { }

                if (oneBodyTracked)
                {
                    if (app.timer.IsEnabled)
                        app.timer.Stop();
                    EnableScanning();
                }
                else
                {
                    app.timer.Start();
                }
            }
        }

        private void getUser(ZXing.ResultPoint barcodePosition)
        {
            app.barcodePoint = new ColorSpacePoint() { X = barcodePosition.X, Y = barcodePosition.Y };

            // Debug
            debug.Content = "position X : " + barcodePosition.X + "\n" +
                            "position Y : " + barcodePosition.Y + "\n";
            DrawResultPoint(barcodePosition);
        }

        private void DrawResultPoint(ZXing.ResultPoint rp)
        {
            canvas.Children.Clear();
            Ellipse e = new Ellipse() { Height = 6, Width = 6, Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };
            Canvas.SetLeft(e, rp.X / COLOR_SCALE_RATIO - (e.Width / 2));
            Canvas.SetTop(e, rp.Y / COLOR_SCALE_RATIO - (e.Height / 2));
            canvas.Children.Add(e);
        }

        private DepthSpacePoint getPixelPosition(FrameDescription fd, Point point)
        {
            DepthSpacePoint p = colorMappedToDepthPoints[fd.Width * ((int)point.Y - 1) + (int)point.X];
            return p;
        }

        private void EnableScanning()
        {
            if (!scanning)
            {
                this.scanning = true;
                image.Visibility = Visibility.Visible;
            }
        }

        private void DisableScanning()
        {
            if (scanning)
            {
                this.scanning = false;
                image.Visibility = Visibility.Hidden;
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DisableScanning();
        }

        private void DecodeFrame(ColorFrame colorFrame)
        {
            var result = reader.Decode(colorFrame);
            if (result != null)
            {
                barcodePosition = new ZXing.ResultPoint(result.ResultPoints[0].X,
                                                        result.ResultPoints[0].Y);
                currentBarcode = result.ToString();
            }
        }

        private void Login(string userCode, uint color)
        {
            String username = null;
            foreach (KeyValuePair<string, string> kv in app.Users)
            {
                if (kv.Key.Equals(userCode))
                {
                    username = kv.Value;
                    continue;
                }
            }

            //if (!app.User.Equals(default(KeyValuePair<string, string>)))
            if(username != null)
            {
                app.users.Add(new User(username, color));
                if (this.NavigationService.CanGoForward)
                    this.NavigationService.GoForward();
                else
                    this.NavigationService.Navigate(new MainPage());
            }
        }

        private void IdentificationPage_Unloaded(object sender, RoutedEventArgs e)
        {
            DisableScanning();
        }
    }
}