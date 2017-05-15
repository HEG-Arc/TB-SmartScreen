using Microsoft.Kinect;
using Microsoft.Kinect.Input;
using Microsoft.Kinect.Wpf.Controls;
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

namespace POC_GestureNavigation.Pages
{
    /// <summary>
    /// Logique d'interaction pour ObjectPage.xaml
    /// </summary>
    public partial class ObjectPage : Page
    {
        private const int PIXELS_PER_BYTE = 4;

        private KinectSensor sensor;
        private BodyFrameReader bfr;
        private Body[] bodies;
        private bool grabbing = false;
        private bool grabbingImage = false;

        private KinectPointerPoint kinectPointerPoint;
        private Point kinectPointerPosition;
        private Point imagePosition;

        private Image image;


        public ObjectPage()
        {
            InitializeComponent();
            this.Loaded += ObjectPage_Loaded;
        }

        private void ObjectPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Kinect init
            sensor = KinectSensor.GetDefault();
            bfr = sensor.BodyFrameSource.OpenReader();

            sensor.Open();
            bodies = new Body[6];

            bfr.FrameArrived += Bfr_FrameArrived;
            KinectCoreWindow kinectCoreWindow = KinectCoreWindow.GetForCurrentThread();
            kinectCoreWindow.PointerMoved += KinectCoreWindow_PointerMoved;

            // UI init
            image = new Image();
            image.Source = new BitmapImage(new Uri("/Images/Components.png", UriKind.Relative));
            image.Height = 150;
            image.Width = 150;
            image.HorizontalAlignment = HorizontalAlignment.Left;
            image.VerticalAlignment = VerticalAlignment.Top;
            Canvas.SetLeft(image, 50);
            Canvas.SetTop(image, 50);
            canvas.Children.Add(image);

        }

        private void KinectCoreWindow_PointerMoved(object sender, KinectPointerEventArgs e)
        {
            kinectPointerPoint = e.CurrentPoint;
            kinectPointerPosition.X = kinectPointerPoint.Position.X * canvas.ActualWidth;
            kinectPointerPosition.Y = kinectPointerPoint.Position.Y * canvas.ActualHeight;

            if (grabbingImage)
            {
                this.canvas.Children.Clear();
                Canvas.SetLeft(image, kinectPointerPosition.X - image.Width / 2);
                Canvas.SetTop(image, kinectPointerPosition.Y - image.Height / 2);
                /// TODO set image position to kinectPointerPosition
                this.canvas.Children.Add(image);
            }
        }

        private void Bfr_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    bodyFrame.GetAndRefreshBodyData(bodies);
                    foreach (Body b in bodies)
                    {
                        if (b.IsTracked)
                            Grabbing = (b.HandRightState == HandState.Closed || b.HandLeftState == HandState.Closed);
                    }
                }
            }
        }

        private void Image_onGrabbing()
        {
            double imgY = (double)image.GetValue(Canvas.TopProperty);
            double imgX = (double)image.GetValue(Canvas.LeftProperty);
            double kppX = kinectPointerPosition.X;
            double kppY = kinectPointerPosition.Y;

            //Debbugging
              string dbg = "imgX: " + imgX + "\n" +
                           "imgY:" + imgY + "\n" +
                           "kppX: " + kppX + "\n" +
                           "kppY:" + kppY + "\n" +
                           "farX:" + (imgX + image.Width) + "\n" +
                           "farY:" + (imgY + image.Height) + "\n";
            debug.Content = dbg + ((kppX > imgX && kppX < imgX + image.Width) &&
              (kppY > imgY && kppY < imgY + image.Height));

            grabbingImage = ((kppX > imgX && kppX < imgX + image.Width) &&
               (kppY > imgY && kppY < imgY + image.Height));

        }

        private void Image_onRelease()
        {
            grabbingImage = false;
        }

        bool Grabbing
        {
            get
            {
                return grabbing;
            }
            set
            {
                if (grabbing != value)
                {
                    grabbing = value;
                    if (grabbing)
                        Image_onGrabbing();
                    else
                        Image_onRelease();
                }
            }
        }
    }
}
