using Microsoft.Kinect;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace POC_UserAwareness
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int PIXELS_PER_BYTE = 4;

        private KinectSensor sensor;
        private ColorFrameReader cfReader;
        private byte[] cfDataConverted;
        private WriteableBitmap cfBitmap;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.GetDefault();
            cfReader = sensor.ColorFrameSource.OpenReader();
            FrameDescription fd = sensor.ColorFrameSource.FrameDescription;
            cfDataConverted = new byte[fd.LengthInPixels * PIXELS_PER_BYTE];
            cfBitmap = new WriteableBitmap(fd.Width, fd.Height, 96, 96, PixelFormats.Pbgra32, null);

            image.Source = cfBitmap;
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
                }
            }
        }
    }
}
