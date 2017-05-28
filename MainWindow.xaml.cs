using Microsoft.Kinect;
using POC_MultiUserIdentification.Model;
using POC_MultiUserIdentification.Pages;
using System;
using System.Collections.Generic;
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
        private const int INFRARED_SCALE_RATIO = 2;
        private const int PIXELS_PER_BYTE = 4;
        private const int PIXEL_CHECKER_RADIUS = 20;

        private App app;
        private KinectSensor sensor;
        private BodyIndexFrameReader bifReader;
        private MultiSourceFrameReader msfr;
        private FrameDescription bifFrameDescription;
        private uint[] bifDataConverted;
        private WriteableBitmap bifBitmap;

        private List<User> users;
        private List<uint> currentPeople;

        private static readonly uint[] BodyColor =
        {
            0x0000FF,
            0x00FF00,
            0xFFFF40,
            0x40FFFF,
            0xFF40FF,
            0xFF8080,
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

            users = new List<User>();
            currentPeople = new List<uint>();

            app.users = this.users;
            app.currentPeople = this.currentPeople;

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
                                    DepthSpacePoint[] dsp = new DepthSpacePoint[4];
                                    uint[] colors = new uint[4];

                                    colors[0] = ColorSpacePointToBodyIndexColor((ColorSpacePoint)app.barcodePoint, -1, 0, out dsp[0]); // Left
                                    colors[1] = ColorSpacePointToBodyIndexColor((ColorSpacePoint)app.barcodePoint, 1, 0, out dsp[1]); // Right
                                    colors[2] = ColorSpacePointToBodyIndexColor((ColorSpacePoint)app.barcodePoint, 0, -1, out dsp[2]); // Up
                                    colors[3] = ColorSpacePointToBodyIndexColor((ColorSpacePoint)app.barcodePoint, 0, 1, out dsp[3]); // Down                                    

                                    DrawDepthSpacePoints(dsp);

                                    uint userColor = GetUserColor(colors);
                                    if (userColor != 0 && userColor != 1) // Color found
                                        app.currentPerson = userColor;
                                    else if (userColor == 1) // There is equals
                                        Console.WriteLine("There is equals");
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

        private uint GetUserColor(uint[] colors)
        {
            Dictionary<uint, int> kvpd = new Dictionary<uint, int>();
            for (int i = 0; i < colors.Length; i++)
            {
                if (kvpd.ContainsKey(colors[i]))
                    kvpd[colors[i]]++;
                else if (colors[i] != bodyColorBlack)
                    kvpd.Add(colors[i], 1);
            }

            // If colors contains nothing but black
            if (kvpd.Count == 0)
                return 0;

            uint userColor = 0;
            int highestValue = 0;
            bool thereIsEquals = false;
            foreach (KeyValuePair<uint, int> entry in kvpd)
            {
                if (entry.Value > highestValue)
                {
                    highestValue = entry.Value;
                    userColor = entry.Key;
                    thereIsEquals = false;
                }
                else if (entry.Value == highestValue)
                    thereIsEquals = true;
            }

            if (thereIsEquals)
                return 1;
            return userColor;
        }

        private void DrawDepthSpacePoints(DepthSpacePoint[] dsp)
        {
            Ellipse[] ellipses = new Ellipse[dsp.Length];

            bodyIndexCanvas.Children.Clear();
            for (int i = 0; i < dsp.Length; i++)
            {
                ellipses[i] = new Ellipse() { Height = 2, Width = 2, Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255)) };
                if (!Double.IsInfinity(dsp[i].X) && !Double.IsInfinity(dsp[i].Y))
                {
                    Canvas.SetLeft(ellipses[i], dsp[i].X / INFRARED_SCALE_RATIO);
                    Canvas.SetTop(ellipses[i], dsp[i].Y / INFRARED_SCALE_RATIO);
                    bodyIndexCanvas.Children.Add(ellipses[i]);
                }
            }
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

            try
            {
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
                        if (depthSpaceIndex >= 0)
                        {
                            color = this.bifDataConverted[depthSpaceIndex];
                            if (color != bodyColorBlack)
                                continue;
                        }
                    }
                }
            }
            catch { }

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

            this.currentPeople = new List<uint>();
            app.currentPeople = currentPeople;

            // convert body index to a visual representation
            for (int i = 0; i < (int)bodyIndexFrameDataSize; ++i)
            {
                // the BodyColor array has been sized to match
                // BodyFrameSource.BodyCount
                if (frameData[i] < BodyColor.Length)
                {
                    if (!this.currentPeople.Contains(frameData[i]))
                        this.currentPeople.Add(frameData[i]);

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

            // Determines the unidentified users
            bool isUnidentifiedUser = true;
            app.unidentifiedPeople = new List<uint>();
            for (int i = 0; i < this.currentPeople.Count; i++)
            {
                this.currentPeople[i] = BodyColor[this.currentPeople[i]];
                for (int j = 0; j < this.users.Count; j++)
                {
                    if(this.currentPeople[i].Equals(users[j].Color))
                    {
                        isUnidentifiedUser = false;
                        continue;
                    }
                }

                if (isUnidentifiedUser)
                    app.unidentifiedPeople.Add(this.currentPeople[i]);
                else
                    isUnidentifiedUser = true;
            }

            // Determines if idententified users are still there (in front of the screen)
            bool isUserStillThere = false;
            for (int i = 0; i < this.users.Count; i++)
            {
                foreach (uint color in this.currentPeople)
                {
                    if (users[i].Color.Equals(color))
                        isUserStillThere = true;
                }
                if (!isUserStillThere)
                    this.users.Remove(users[i]);
                else
                    isUserStillThere = false;
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
