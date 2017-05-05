using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using ZXing.Kinect;

namespace POC_BarcodeIdentification
{
    /// <summary>
    /// MainWindow.xaml - Interaction Logic
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int NB_FRAMES_BEFORE_DECODE = 30;
        private const int PIXELS_PER_BYTE = 4;

        private KinectSensor sensor;
        private ColorFrameReader cfReader;
        private byte[] cfDataConverted;
        private WriteableBitmap cfBitmap;
        private BarcodeReader reader;
        private int cptFrame = 0;
        private bool scanning = false;

        public MainWindow()
        {
            InitializeComponent();
            initUI();  
            this.Loaded += MainWindow_Loaded;
        }        

        /// <summary>
        /// User Interface Initialisation
        /// </summary>
        private void initUI()
        {
            this.lblScan.Content = Properties.Resources.NotScanning;
            this.lblResult.Content = "";
            this.btnScan.Content = Properties.Resources.StartScan;
        }

        /// <summary>
        /// Components initialisation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.GetDefault();
            cfReader = sensor.ColorFrameSource.OpenReader();
            FrameDescription fd = sensor.ColorFrameSource.FrameDescription;
            cfDataConverted = new byte[fd.LengthInPixels * PIXELS_PER_BYTE];
            cfBitmap = new WriteableBitmap(fd.Width, fd.Height, 96, 96, PixelFormats.Pbgra32, null);            
            reader = new BarcodeReader();

            image.Source = cfBitmap;
            sensor.Open();

            cfReader.FrameArrived += CfReader_FrameArrived;
        }

        /// <summary>
        /// Convert the current frame to "Bgra" image format and link it to our ColorFrame bitmap.
        /// If user if scanning, decode the frame.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                    if(scanning)
                    {
                        // Wait a number of frame before decoding to avoid
                        // a jerky image output resulting from this slow
                        // process.
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
        
        /// <summary>
        /// Toggle scanning
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            this.scanning = !this.scanning;
            this.btnScan.Content = this.scanning ? Properties.Resources.StopScan : Properties.Resources.StartScan;
            this.lblScan.Content = this.scanning ? Properties.Resources.Scanning : Properties.Resources.NotScanning;
        }

        /// <summary>
        /// Decodes a ColorFrame and shows the resulting barcode if found
        /// </summary>
        /// <param name="colorFrame"></param>
        private void DecodeFrame(ColorFrame colorFrame)
        {
            var result = reader.Decode(colorFrame);
            if (result != null)
                lblResult.Content = Properties.Resources.BarcodeIs + " : " + result.Text;
        }
    }
}