﻿using Microsoft.Kinect;
using POC_MultiUserIndification_Collider.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace POC_MultiUserIndification_Collider
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int COLOR_SCALE_RATIO = 4;
        private const int PIXELS_PER_BYTE = 4;
        private const int NB_FRAMES_BEFORE_DECODE = 30;

        private const int HEAD_RECTANGLE_SIZE = 400 / COLOR_SCALE_RATIO;
        private const int HAED_RECTANGLE_THICKNESS = 2;

        private App app;

        private KinectSensor sensor;
        private MultiSourceFrameReader msfr;
        private byte[] cfDataConverted;
        private WriteableBitmap cfBitmap;
        private CoordinateMapper coordinateMapper;

        private Body[] bodies;

        public MainWindow()
        {
            InitializeComponent();
            app = (App)Application.Current;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            initKinect();
            //frame.Navigate(new IdentificationPage());
        }

        private void initKinect()
        {
            sensor = KinectSensor.GetDefault();
            coordinateMapper = sensor.CoordinateMapper;
            bodies = new Body[sensor.BodyFrameSource.BodyCount];

            FrameDescription fd = sensor.ColorFrameSource.FrameDescription;
            cfDataConverted = new byte[fd.LengthInPixels * PIXELS_PER_BYTE];
            cfBitmap = new WriteableBitmap(fd.Width, fd.Height, 96, 96, PixelFormats.Pbgra32, null);

            colorFrameIndicator.Source = cfBitmap;
            sensor.Open();

            msfr = sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body);
            msfr.MultiSourceFrameArrived += Msfr_MultiSourceFrameArrived;
        }

        private void Msfr_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            MultiSourceFrame msf;
            Joint headJoint = new Joint();

            try
            {
                msf = e.FrameReference.AcquireFrame();
                if (msf != null)
                {
                    using (BodyFrame bodyFrame = msf.BodyFrameReference.AcquireFrame())
                    {
                        using (ColorFrame colorFrame = msf.ColorFrameReference.AcquireFrame())
                        {
                            if (bodyFrame != null && colorFrame != null)
                            {
                                // Gestion des corps
                                bodyFrame.GetAndRefreshBodyData(bodies);
                                canvasIndicator.Children.Clear();
                                foreach (Body body in bodies)
                                {
                                    if (body.IsTracked)
                                    {
                                        app.nbBodyTracked++;
                                                           
                                        body.Joints.TryGetValue(JointType.Head, out headJoint);
                                        ColorSpacePoint csp = coordinateMapper.MapCameraPointToColorSpace(headJoint.Position);
                                        Point headPosition = new Point() { X = csp.X / COLOR_SCALE_RATIO, Y = csp.Y / COLOR_SCALE_RATIO };

                                        this.DrawHeadRectangle(headJoint, headPosition);
                                    }
                                }

                                // Affichage des images couleurs à l'écran
                                colorFrame.CopyConvertedFrameDataToArray(cfDataConverted, ColorImageFormat.Bgra);
                                Int32Rect rect = new Int32Rect(0, 0, (int)cfBitmap.Width, (int)cfBitmap.Height);
                                int stride = (int)cfBitmap.Width * PIXELS_PER_BYTE;
                                cfBitmap.WritePixels(rect, cfDataConverted, stride, 0);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private void DrawHeadRectangle(Joint headJoint, Point headPosition)
        {
            Rectangle headRect = new Rectangle() { Stroke = new SolidColorBrush(Color.FromRgb(255, 0, 0)), StrokeThickness = HAED_RECTANGLE_THICKNESS };
            headRect.Height = headRect.Width = HEAD_RECTANGLE_SIZE / headJoint.Position.Z;
            Canvas.SetLeft(headRect, headPosition.X - headRect.Width / 2);
            Canvas.SetTop(headRect, headPosition.Y - headRect.Height / 2);
            canvasIndicator.Children.Add(headRect);
        }
    }
}
