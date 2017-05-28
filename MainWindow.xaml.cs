using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using System.Linq;

namespace POC_UserAwareness
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int PIXELS_PER_BYTE = 4;
        private const int COLOR_FRAME_SIZE_RATIO = 2;
        private const int HEAD_RECTANGLE_SIZE = 200;
        private const int HEAD_RECT_THICKNESS = 5;

        private const double AMBIANT_ZONE = 1.75;
        private const double IMPLICITE_ZONE = 1.25;
        private const double SUBTIL_ZONE = 0.75;

        private KinectSensor sensor;
        private ColorFrameReader cfReader;
        private byte[] cfDataConverted;
        private WriteableBitmap cfBitmap;

        private MultiSourceFrameReader msfr;
        private int bodyCount;
        private Body[] bodies;

        private FaceFrameSource[] faceFrameSources = null;
        private FaceFrameReader[] faceFrameReaders = null;
        private FaceFrameResult[] faceFrameResults = null;

        private int displayWidth;
        private int displayHeight;

        DetectionResult[] detectionResult;

        public MainWindow()
        {
            InitializeComponent();
            detectionResult = Enumerable.Repeat(DetectionResult.Unknown, 6).ToArray();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.GetDefault();
            cfReader = sensor.ColorFrameSource.OpenReader();
            FrameDescription fd = sensor.ColorFrameSource.FrameDescription;
            displayWidth = fd.Width;
            displayHeight = fd.Height;

            cfDataConverted = new byte[fd.LengthInPixels * PIXELS_PER_BYTE];
            cfBitmap = new WriteableBitmap(displayWidth, displayHeight, 96, 96, PixelFormats.Pbgra32, null);

            image.Source = cfBitmap;
            sensor.Open();

            bodyCount = sensor.BodyFrameSource.BodyCount;
            bodies = new Body[bodyCount];

            FaceFrameFeatures faceFrameFeatures = FaceFrameFeatures.FaceEngagement;
            faceFrameSources = new FaceFrameSource[bodyCount];
            faceFrameReaders = new FaceFrameReader[bodyCount];
            for (int i = 0; i < bodyCount; i++)
            {
                this.faceFrameSources[i] = new FaceFrameSource(sensor, 0, faceFrameFeatures);
                this.faceFrameReaders[i] = this.faceFrameSources[i].OpenReader();
                this.faceFrameReaders[i].FrameArrived += Reader_FaceFrameArrived;
            }

            this.faceFrameResults = new FaceFrameResult[this.bodyCount];

            msfr = sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.Color);
            msfr.MultiSourceFrameArrived += Msfr_MultiSourceFrameArrived;
        }

        private void Reader_FaceFrameArrived(object sender, FaceFrameArrivedEventArgs e)
        {
            using (FaceFrame faceFrame = e.FrameReference.AcquireFrame())
            {
                if (faceFrame != null)
                {
                    int index = this.GetFaceSourceIndex(faceFrame.FaceFrameSource);
                    this.faceFrameResults[index] = faceFrame.FaceFrameResult;
                }
            }
        }

        private int GetFaceSourceIndex(FaceFrameSource faceFrameSource)
        {
            int index = -1;
            for (int i = 0; i < this.bodyCount; i++)
            {
                if (this.faceFrameSources[i] == faceFrameSource)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private void Msfr_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrame msf;
            try
            {
                msf = e.FrameReference.AcquireFrame();
                if (msf != null)
                {
                    using (BodyFrame bodyFrame = msf.BodyFrameReference.AcquireFrame())
                    {
                        using (ColorFrame cfFrame = msf.ColorFrameReference.AcquireFrame())
                        {
                            if (bodyFrame != null && cfFrame != null)
                            {
                                cfFrame.CopyConvertedFrameDataToArray(cfDataConverted, ColorImageFormat.Bgra);
                                Int32Rect rect = new Int32Rect(0, 0, (int)cfBitmap.Width, (int)cfBitmap.Height);
                                int stride = (int)cfBitmap.Width * PIXELS_PER_BYTE;
                                cfBitmap.WritePixels(rect, cfDataConverted, stride, 0);

                                bodyFrame.GetAndRefreshBodyData(bodies);
                                bodyCanvas.Children.Clear();
                                for (int i = 0; i < bodyCount; i++)
                                {
                                    if (this.faceFrameSources[i].IsTrackingIdValid && this.faceFrameResults[i] != null)
                                    {
                                        this.faceFrameResults[i].FaceProperties.TryGetValue(FaceProperty.Engaged, out detectionResult[i]);
                                    }
                                    else
                                    {
                                        if (this.bodies[i].IsTracked)
                                            this.faceFrameSources[i].TrackingId = this.bodies[i].TrackingId;
                                    }

                                    if (bodies[i].IsTracked)
                                    {
                                        Joint headJoint = bodies[i].Joints[JointType.Head];
                                        if (headJoint.TrackingState == TrackingState.Tracked)
                                            this.DrawResults(headJoint, detectionResult[i]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            { }
        }

        private void DrawResults(Joint headJoint, DetectionResult detectionResult)
        {
            ColorSpacePoint csp = sensor.CoordinateMapper.MapCameraPointToColorSpace(headJoint.Position);
            Rectangle headRect;
            TextBlock headInfos = new TextBlock() { Foreground = Brushes.Red, FontSize = 25 };

            String strEngagement = "";
            String strZone = "";

            if (detectionResult == DetectionResult.Maybe)
                strEngagement = "No";
            else
                strEngagement = detectionResult.ToString();

            if (detectionResult != DetectionResult.Yes)
            {
                headInfos = new TextBlock() { Foreground = Brushes.Red, FontSize = 25 };
                headRect = new Rectangle() { Stroke = Brushes.Red, StrokeThickness = HEAD_RECT_THICKNESS };
            }
            else
            {
                headInfos = new TextBlock() { Foreground = Brushes.GreenYellow, FontSize = 25 };
                headRect = new Rectangle() { Stroke = Brushes.GreenYellow, StrokeThickness = HEAD_RECT_THICKNESS };
            }

            double distance = Math.Round(headJoint.Position.Z, 2);

            if (distance > 0)
                strZone = "Personal Zone";
            if (distance >= SUBTIL_ZONE)
                strZone = "Subtil Zone";
            if (distance >= IMPLICITE_ZONE)
                strZone = "Implicite Zone";
            if (distance >= AMBIANT_ZONE)
                strZone = "Ambiant Zone";

            headInfos.Text = "Distance : " + Math.Round(headJoint.Position.Z, 2) + "m\n" +
                             "Zone : " + strZone + "\n" +
                             "Engagement : " + strEngagement;

            headRect.Height = headRect.Width = HEAD_RECTANGLE_SIZE / headJoint.Position.Z;

            Canvas.SetLeft(headRect, csp.X / COLOR_FRAME_SIZE_RATIO - headRect.Width / 2);
            Canvas.SetTop(headRect, csp.Y / COLOR_FRAME_SIZE_RATIO - headRect.Height / 2);
            Canvas.SetLeft(headInfos, csp.X / COLOR_FRAME_SIZE_RATIO - headRect.Width / 2);
            Canvas.SetTop(headInfos, csp.Y / COLOR_FRAME_SIZE_RATIO + headRect.Height / 2);

            bodyCanvas.Children.Add(headRect);
            bodyCanvas.Children.Add(headInfos);
        }
    }
}