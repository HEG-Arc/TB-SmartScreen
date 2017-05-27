using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing.Kinect;
using System;
using System.Windows.Shapes;

namespace POC_MultiUserIdentification.Pages
{
    /// <summary>
    /// Logique d'interaction pour IdentificationPage.xaml
    /// </summary>
    public partial class IdentificationPage : Page
    {
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
                /*
                BodyFrame bodyFrame = null;
                BodyIndexFrame bodyIndexFrame = null;
                ColorFrame colorFrame = null;
                */
                bool oneBodyTracked = false;

                try
                {
                    msf = e.FrameReference.AcquireFrame();
                    if (msf != null)
                    {
                        /*
                        bodyFrame = msf.BodyFrameReference.AcquireFrame();
                        bodyIndexFrame = msf.BodyIndexFrameReference.AcquireFrame();
                        colorFrame = msf.ColorFrameReference.AcquireFrame();

                        if (bodyFrame == null || bodyIndexFrame == null || colorFrame == null)
                        {
                            return;
                        }
                        */

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
                                                this.getUser(barcodePosition);
                                                currentBarcode = null;
                                            }
                                        }                                        
                                    }
                                }
                            }                            
                        }                        
                        
                        /*
                        canvas.Children.Clear();
                        if (barcodePosition != null)
                        {
                            Ellipse ellipse = new Ellipse() { Height = 20, Width = 20, Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };
                            Canvas.SetLeft(ellipse, barcodePosition.X - ellipse.Width / 2);
                            Canvas.SetTop(ellipse, barcodePosition.Y - ellipse.Height / 2);
                            canvas.Children.Add(ellipse);
                        }
                        */
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
            FrameDescription fd = this.sensor.ColorFrameSource.FrameDescription;
            /*
            uint[] res = new uint[4];
            res[0] = bifDataConverted[getPixelPosition(fd, new Point(barcodePosition.X - PIXEL_CHECKER_RADIUS, barcodePosition.Y))]; // left
            res[1] = bifDataConverted[getPixelPosition(fd, new Point(barcodePosition.X + PIXEL_CHECKER_RADIUS, barcodePosition.Y))]; // right
            res[2] = bifDataConverted[getPixelPosition(fd, new Point(barcodePosition.X, barcodePosition.Y - PIXEL_CHECKER_RADIUS))]; // up            
            res[3] = bifDataConverted[getPixelPosition(fd, new Point(barcodePosition.X, barcodePosition.Y + PIXEL_CHECKER_RADIUS))]; // down
            */

            app.barcodePoint = new ColorSpacePoint() { X = barcodePosition.X, Y = barcodePosition.Y };

            debug.Content = "position X : " + barcodePosition.X + "\n" +
                             "position Y : " + barcodePosition.Y + "\n";

            canvas.Children.Clear();
            Ellipse e = new Ellipse() { Height = 2, Width = 2, Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };
            Canvas.SetLeft(e, barcodePosition.X - 1);
            Canvas.SetTop(e, barcodePosition.Y - 1);
            canvas.Children.Add(e);
            /*
            canvas.Children.Clear();
            Ellipse el = new Ellipse() { Height = 2, Width = 2, Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };
            Ellipse er = new Ellipse() { Height = 2, Width = 2, Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };
            Ellipse eu = new Ellipse() { Height = 2, Width = 2, Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };
            Ellipse ed = new Ellipse() { Height = 2, Width = 2, Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };
            Ellipse e = new Ellipse() { Height = 2, Width = 2, Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0)) };

            Canvas.SetLeft(el, (barcodePosition.X - PIXEL_CHECKER_RADIUS) / PIXELS_PER_BYTE);
            Canvas.SetTop(el, barcodePosition.Y / PIXELS_PER_BYTE);
            Canvas.SetLeft(er, (barcodePosition.X + PIXEL_CHECKER_RADIUS) / PIXELS_PER_BYTE);
            Canvas.SetTop(er, barcodePosition.Y / PIXELS_PER_BYTE);
            Canvas.SetLeft(eu, barcodePosition.X / PIXELS_PER_BYTE);
            Canvas.SetTop(eu, (barcodePosition.Y - PIXEL_CHECKER_RADIUS) / PIXELS_PER_BYTE);
            Canvas.SetLeft(ed, barcodePosition.X / PIXELS_PER_BYTE);
            Canvas.SetTop(ed, (barcodePosition.Y + PIXEL_CHECKER_RADIUS) / PIXELS_PER_BYTE);
            Canvas.SetLeft(e, 50);
            Canvas.SetTop(e, 50);
            canvas.Children.Add(el);
            canvas.Children.Add(er);
            canvas.Children.Add(eu);
            canvas.Children.Add(ed);
            canvas.Children.Add(e);
            */

            //bodyColorCanvas.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#" + bifDataConverted[app.res[0]].ToString("X8")));
            /*
            for (int i = 0; i < res.Length; i++)
            {
                debug.Content += res[i] + "#" + res[i].ToString("X8") + "\n";
                if (res[i] > 0)
                    bodyColorCanvas.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#" + res[i].ToString("X8")));
            }*/
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
                //this.scanning = false;
                //image.Visibility = Visibility.Hidden;
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
                //this.Login(result.ToString());
            }
        }

        private void Login(string userCode)
        {
            foreach (KeyValuePair<string, string> kv in app.Users)
            {
                if (kv.Key.Equals(userCode))
                {
                    app.User = kv;
                    continue;
                }
            }

            if (!app.User.Equals(default(KeyValuePair<string, string>)))
            {
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