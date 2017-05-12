using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing.Kinect;

namespace POC_MultiUserIdentification.Pages
{
    /// <summary>
    /// Logique d'interaction pour IdentificationPage.xaml
    /// </summary>
    public partial class IdentificationPage : Page
    {
        private const int NB_FRAMES_BEFORE_DECODE = 30;
        private const int PIXELS_PER_BYTE = 4;

        private KinectSensor sensor;
        private ColorFrameReader cfReader;
        private byte[] cfDataConverted;
        private WriteableBitmap cfBitmap;
        private BarcodeReader reader;
        private int cptFrame = 0;
        private bool scanning = true;

        public IdentificationPage()
        {
            InitializeComponent();
            this.Loaded += IdentificationPage_Loaded;
        }

        private void IdentificationPage_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.GetDefault();
            cfReader = sensor.ColorFrameSource.OpenReader();
            FrameDescription fd = sensor.ColorFrameSource.FrameDescription;
            cfDataConverted = new byte[fd.LengthInPixels * PIXELS_PER_BYTE];
            cfBitmap = new WriteableBitmap(fd.Width, fd.Height, 96, 96, PixelFormats.Pbgra32, null);
            reader = new BarcodeReader();

            image.Source = cfBitmap;
            //image.Visibility = Visibility.Hidden;

            sensor.Open();

            cfReader.FrameArrived += CfReader_FrameArrived;
        }

        private void CfReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
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
                    if(scanning)
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

        private void DecodeFrame(ColorFrame colorFrame)
        {
            var result = reader.Decode(colorFrame);
            if (result != null)
                this.Login(result.ToString());
        }

        public void Login(string userCode)
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
                this.sensor.Close();
                this.NavigationService.Navigate(new MainPage());
            }
        }
    }
}
