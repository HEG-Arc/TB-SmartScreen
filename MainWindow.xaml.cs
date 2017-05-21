using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace POC_UserAwareness
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int PIXELS_PER_BYTE = 4;
        private const int COLOR_FRAME_SIZE_RATIO = 3; 

        private KinectSensor sensor;
        private ColorFrameReader cfReader;
        private byte[] cfDataConverted;
        private WriteableBitmap cfBitmap;

        private MultiSourceFrameReader msfr;
        private Body[] bodies;

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

            bodies = new Body[6];
            msfr = sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.Color);
            msfr.MultiSourceFrameArrived += Msfr_MultiSourceFrameArrived;
            
            //cfReader.FrameArrived += CfReader_FrameArrived;

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
                                foreach (Body body in bodies)
                                {
                                    if (body.IsTracked)
                                    {
                                        lblResult.Text = "";
                                        foreach (KeyValuePair<JointType, JointOrientation> joint in body.JointOrientations)
                                        {
                                            lblResult.Text += joint.Key + " :\n" +
                                                                    "W : " + Math.Round(joint.Value.Orientation.W, 0) + "\n" +
                                                                    "X : " + Math.Round(joint.Value.Orientation.X, 0) + "\n" +
                                                                    "Y : " + Math.Round(joint.Value.Orientation.Y, 0) + "\n" +
                                                                    "Z : " + Math.Round(joint.Value.Orientation.Z, 0) + "\n\n";
                                        }

                                        Joint headJoint = body.Joints[JointType.Head];                                        
                                        if (headJoint.TrackingState == TrackingState.Tracked)
                                        {
                                            //JointOrientation orientation;
                                            //body.JointOrientations.TryGetValue(JointType.Head, out orientation);
                                            ColorSpacePoint dsp = sensor.CoordinateMapper.MapCameraPointToColorSpace(headJoint.Position);
                                            Rectangle headRect = new Rectangle() { Width = 100, Height = 100, Stroke = Brushes.Red };
                                            TextBlock headInfos = new TextBlock() { Foreground = Brushes.Red };

                                            headInfos.Text = "X Position : " + headJoint.Position.X + "\n" +
                                                             "Y Position : " + headJoint.Position.Y + "\n" +
                                                             "Z Position : " + headJoint.Position.Z + "\n";

                                            bodyCanvas.Children.Add(headRect);
                                            bodyCanvas.Children.Add(headInfos);

                                            Canvas.SetLeft(headRect, dsp.X / COLOR_FRAME_SIZE_RATIO - 50);
                                            Canvas.SetTop(headRect, dsp.Y / COLOR_FRAME_SIZE_RATIO - 50);
                                            Canvas.SetLeft(headInfos, dsp.X / COLOR_FRAME_SIZE_RATIO - 50);
                                            Canvas.SetTop(headInfos, dsp.Y / COLOR_FRAME_SIZE_RATIO + 50);
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
