using Microsoft.Kinect;
using POC_MultiUserIdentification.Pages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POC_MultiUserIdentification
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int PIXELS_PER_BYTE = 4;
        private const int PIXEL_CHECKER_RADIUS = 20;

        private App app;
        private KinectSensor sensor;
        private BodyIndexFrameReader bifReader;
        private MultiSourceFrameReader msfr;
        private FrameDescription bifFrameDescription;
        private uint[] bifDataConverted;
        private WriteableBitmap bifBitmap;

        private static readonly uint[] BodyColor =
        {
            0x0000FFFF,
            0x00FF00FF,
            0xFFFF40FF,
            0x40FFFFFF,
            0xFF40FFFF,
            0xFF8080FF,
        };

        private static readonly uint bodyColorBlack = 0x00000000;

        public MainWindow()
        {
            InitializeComponent();

            app = ((App)Application.Current);
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.GetDefault();
            app.sensor = sensor;

            bifReader = sensor.BodyIndexFrameSource.OpenReader();
            bifFrameDescription = sensor.BodyIndexFrameSource.FrameDescription;

            bifDataConverted = new uint[bifFrameDescription.Width * bifFrameDescription.Height];
            app.bodyIndexFrameDataConverted = bifDataConverted;

            bifBitmap = new WriteableBitmap(bifFrameDescription.Width, bifFrameDescription.Height, 96, 96, PixelFormats.Bgr32, null);

            this.imgBodyIndex.Source = bifBitmap;
            sensor.Open();

            msfr = sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.BodyIndex | FrameSourceTypes.Color | FrameSourceTypes.Depth);
            app.msfr = msfr;

            msfr.MultiSourceFrameArrived += Msfr_MultiSourceFrameArrived;
            app.cptMsfrE++;

            this.frame.Navigate(new IdentificationPage());
        }

        private void Msfr_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrame msf;
            bool bodyIndexFrameProcessed = false;
            try
            {
                msf = e.FrameReference.AcquireFrame();
                if (msf != null)
                {
                    using (BodyIndexFrame bifFrame = msf.BodyIndexFrameReference.AcquireFrame())
                    {
                        if (bifFrame != null)
                        {
                            using (KinectBuffer bodyIndexBuffer = bifFrame.LockImageBuffer())
                            {
                                if (((this.bifFrameDescription.Width * this.bifFrameDescription.Height) == bodyIndexBuffer.Size) &&
                                    (this.bifFrameDescription.Width == this.bifBitmap.PixelWidth) && (this.bifFrameDescription.Height == this.bifBitmap.PixelHeight))
                                {
                                    this.ProcessBodyIndexFrameData(bodyIndexBuffer.UnderlyingBuffer, bodyIndexBuffer.Size);
                                    bodyIndexFrameProcessed = true;
                                }

                                if (app.barcodePoint != null && app.colorMappedToDepthPoints != null)
                                {
                                    Ellipse[] ellipe = new Ellipse[4];
                                    DepthSpacePoint[] dsp = new DepthSpacePoint[4];
                                    uint[] color = new uint[4];
                                    int nbColor = 0;

                                    color[0] = ColorSpacePointToBodyIndexColor((ColorSpacePoint)app.barcodePoint, -1, 0, out dsp[0]);
                                    color[1] = ColorSpacePointToBodyIndexColor((ColorSpacePoint)app.barcodePoint, 1, 0, out dsp[1]);
                                    color[2] = ColorSpacePointToBodyIndexColor((ColorSpacePoint)app.barcodePoint, 0, 1, out dsp[2]);
                                    color[3] = ColorSpacePointToBodyIndexColor((ColorSpacePoint)app.barcodePoint, 0, -1, out dsp[3]);

                                    bodyIndexCanvas.Children.Clear();
                                    for(int i = 0; i < 4; i++)
                                    {
                                        if (color[i] != bodyColorBlack)
                                            nbColor++;

                                        ellipe[i] = new Ellipse() { Height = 2, Width = 2, Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
                                        Canvas.SetLeft(ellipe[i], dsp[i].X);
                                        Canvas.SetTop(ellipe[i], dsp[i].Y);
                                        bodyIndexCanvas.Children.Add(ellipe[i]);
                                    }

                                    if (nbColor > 0)
                                        Console.WriteLine("User is : " + color);
                                    else
                                        Console.WriteLine("Cannot detect user");
                                }
                            }
                        }
                    }
                }
            }
            catch
            { }

            if (bodyIndexFrameProcessed)
                this.RenderBodyIndexPixels();
        }

        // ColorSpacePointToBodyIndexColor(point, -1, 0); --> LEFT
        // ColorSpacePointToBodyIndexColor(point, 1, 0); --> RIGHT
        // ColorSpacePointToBodyIndexColor(point, 0, -1); --> UP
        // ColorSpacePointToBodyIndexColor(point, 0, 1); --> DOWN
        private uint ColorSpacePointToBodyIndexColor(ColorSpacePoint colorSpacePoint, int moveX, int moveY, out DepthSpacePoint depthSpacePoint)
        {
            int colorSpaceIndex = -1;
            int depthSpaceIndex = -1;
            depthSpacePoint = new DepthSpacePoint();
            uint color = bodyColorBlack;

            for (int i = 0; i < PIXEL_CHECKER_RADIUS; i++)
            {
                colorSpacePoint.X += i * moveX;
                colorSpacePoint.Y += i * moveY;
                colorSpaceIndex = PointToIndex(sensor.ColorFrameSource.FrameDescription,
                                               new Point() { X = colorSpacePoint.X, Y = colorSpacePoint.Y });

                depthSpacePoint = app.colorMappedToDepthPoints[colorSpaceIndex];
                depthSpaceIndex = PointToIndex(sensor.DepthFrameSource.FrameDescription,
                                               new Point() { X = depthSpacePoint.X, Y = depthSpacePoint.Y });

                if (depthSpaceIndex != -1)
                {
                    color = this.bifDataConverted[depthSpaceIndex];
                    if (color != bodyColorBlack)
                        continue;
                }
            }

            return color;
        }

        private int PointToIndex(FrameDescription fd, Point point)
        {
            int index = -1;
            if (!Double.IsInfinity(point.X) || !Double.IsInfinity(point.Y))
                index = fd.Width * ((int)point.Y - 1) + (int)point.X;
            return index;
        }

        private unsafe void ProcessBodyIndexFrameData(IntPtr bodyIndexFrameData, uint bodyIndexFrameDataSize)
        {
            byte* frameData = (byte*)bodyIndexFrameData;

            // convert body index to a visual representation
            for (int i = 0; i < (int)bodyIndexFrameDataSize; ++i)
            {
                // the BodyColor array has been sized to match
                // BodyFrameSource.BodyCount
                if (frameData[i] < BodyColor.Length)
                {
                    // this pixel is part of a player,
                    // display the appropriate color
                    this.bifDataConverted[i] = BodyColor[frameData[i]];
                }
                else
                {
                    // this pixel is not part of a player
                    // display black
                    this.bifDataConverted[i] = bodyColorBlack;
                }
            }
        }

        private void RenderBodyIndexPixels()
        {
            Int32Rect rect = new Int32Rect(0, 0, (int)bifBitmap.PixelWidth, (int)bifBitmap.PixelHeight);
            int stride = (int)bifBitmap.Width * PIXELS_PER_BYTE;
            bifBitmap.WritePixels(rect, bifDataConverted, stride, 0);
        }
    }
}
