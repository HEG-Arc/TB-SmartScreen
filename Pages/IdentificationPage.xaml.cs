using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing.Kinect;
using System;

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
        private ColorFrameReader cfReader;
        private byte[] cfDataConverted;
        private WriteableBitmap cfBitmap;
        private BarcodeReader reader;
        private Body[] bodies;
        private int cptFrame = 0;
        private bool scanning = true;

        public IdentificationPage()
        {
            InitializeComponent();

            app = (App)Application.Current;
            sensor = app.sensor;
            msfr = app.msfr;
            
            this.Loaded += IdentificationPage_Loaded;
            this.Unloaded += IdentificationPage_Unloaded;                        
        }

        private void IdentificationPage_Loaded(object sender, RoutedEventArgs e)
        {
            cfReader = sensor.ColorFrameSource.OpenReader();
            FrameDescription fd = sensor.ColorFrameSource.FrameDescription;
            cfDataConverted = new byte[fd.LengthInPixels * PIXELS_PER_BYTE];
            cfBitmap = new WriteableBitmap(fd.Width, fd.Height, 96, 96, PixelFormats.Pbgra32, null);
            reader = new BarcodeReader();

            image.Source = cfBitmap;
            DisableScanning();

            bodies = new Body[6];

            if (app.cptMsfrE == 1)
            {
                msfr.MultiSourceFrameArrived += MsfReader_MultiSourceFrameArrived;
                cfReader.FrameArrived += CfReader_FrameArrived;
                app.cptMsfrE++;
            }            
        }

        private void MsfReader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            if(this.NavigationService != null)
            {
                Console.WriteLine("Identification Page");
                MultiSourceFrame msf;
                bool oneBodyTracked = false;
                try
                {
                    msf = e.FrameReference.AcquireFrame();
                    if (msf != null)
                    {
                        using (BodyFrame bodyFrame = msf.BodyFrameReference.AcquireFrame())
                        {
                            if (bodyFrame != null)
                            {
                                bodyFrame.GetAndRefreshBodyData(bodies);
                                foreach (Body body in bodies)
                                {
                                    if (body.IsTracked)
                                        oneBodyTracked = true;
                                }
                            }
                        }
                    }
                }
                catch
                { }

                if (oneBodyTracked)
                    ActivateScanning();
                else
                    DisableScanning();
            }            
        }

        private void ActivateScanning()
        {
            if(!scanning)
            {
                this.scanning = true;
                image.Visibility = Visibility.Visible;
            }            
        }

        private void DisableScanning()
        {
            if(scanning)
            {
                this.scanning = false;
                image.Visibility = Visibility.Hidden;
            }            
        }

        private void CfReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            if (this.NavigationService != null)
            {
                using (ColorFrame cfFrame = e.FrameReference.AcquireFrame())
                {
                    if (cfFrame != null)
                    {
                        cfFrame.CopyConvertedFrameDataToArray(cfDataConverted, ColorImageFormat.Bgra);
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
                                this.DecodeFrame(cfFrame);
                                cptFrame = 0;
                            }
                            cptFrame++;
                        }
                    }
                }
            }            
        }

        private void DecodeFrame(ColorFrame colorFrame)
        {
            var result = reader.Decode(colorFrame);
            if (result != null)
                this.Login(result.ToString());
        }

        private void Login(string userCode)
        {
            App app = (App)Application.Current;
            debug.Content = userCode;

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
            //msfr.MultiSourceFrameArrived -= MsfReader_MultiSourceFrameArrived;
            //cfReader.FrameArrived -= CfReader_FrameArrived;
        }
    }
}
