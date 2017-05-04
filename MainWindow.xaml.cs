using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.Drawing;
using System.IO;
using ZXing;

namespace ColorFrameTest
{
    /// <summary>
    /// MainWindow.xaml - Interaction Logic
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor sensor;
        ColorFrameReader cfReader;
        byte[] cfDataConverted;
        WriteableBitmap cfBitmap;
        IBarcodeReader reader;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
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
            cfDataConverted = new byte[fd.LengthInPixels * 4];
            cfBitmap = new WriteableBitmap(fd.Width, fd.Height, 96, 96, PixelFormats.Pbgra32, null);            
            reader = new BarcodeReader();

            image.Source = cfBitmap;
            sensor.Open();

            cfReader.FrameArrived += CfReader_FrameArrived;
        }

        /// <summary>
        /// ColorFrameReader's FrameArrived event handler
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
                    int stride = (int)cfBitmap.Width * 4;
                    cfBitmap.WritePixels(rect, cfDataConverted, stride, 0);
                }
            }
        }

        /// <summary>
        /// Scan button's Click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnScan_Click(object sender, RoutedEventArgs e)
        {
            WriteableBitmap wbmp = this.cfBitmap;
            if (this.cfBitmap != null)
            {
                BitmapImage bmpImg = ConvertWriteableBitmapToBitmapImage(this.cfBitmap);
                Bitmap bmp = this.BitmapImage2Bitmap(bmpImg);

                this.Capture.Source = bmpImg;
                ScanBitmap(bmp);
            }
        }

        /// <summary>
        /// Decode a Bitmap image and show the resulting barcode if found
        /// </summary>
        /// <param name="bitmap"></param>
        private void ScanBitmap(Bitmap bitmap)
        {
            var result = reader.Decode(bitmap);
            if (result != null)
            {
                MessageBox.Show("Barcode is :" + result.Text);
            }
        }

        /// <summary>
        /// Convert WriteableBitmap to BitmapImage
        /// </summary>
        /// <param name="wbm">WriteableBitmap object</param>
        /// <returns>Converted BitmapImage object</returns>
        public BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
        }

        /// <summary>
        /// Convert BitmapImage to Bitmap
        /// </summary>
        /// <param name="wbm">BitmapImage object</param>
        /// <returns>Converted Bitmap object</returns>
        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
    }
}