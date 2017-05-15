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
        private bool canMove = false;
        private CustomImage grabbedImage = null;

        private KinectPointerPoint kinectPointerPoint;
        private Point kinectPointerPosition;

        private CustomImage image;


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
            image = new CustomImage();
            image.Source = new BitmapImage(new Uri("/Images/Components.png", UriKind.Relative));
            image.Height = 150;
            image.Width = 150;
            image.HorizontalAlignment = HorizontalAlignment.Left;
            image.VerticalAlignment = VerticalAlignment.Top;
            image.Position = new Point(50, 50);

            Canvas.SetLeft(image, image.Position.X);
            Canvas.SetTop(image, image.Position.Y);
            canvas.Children.Add(image);
        }

        private void KinectCoreWindow_PointerMoved(object sender, KinectPointerEventArgs e)
        {
            kinectPointerPoint = e.CurrentPoint;
            kinectPointerPosition.X = kinectPointerPoint.Position.X * canvas.ActualWidth;
            kinectPointerPosition.Y = kinectPointerPoint.Position.Y * canvas.ActualHeight;

            if (canMove)
            {
                canvas.Children.Clear();

                grabbedImage.Position = new Point(kinectPointerPosition.X - grabbedImage.Width / 2, kinectPointerPosition.Y - grabbedImage.Height / 2);
                Canvas.SetLeft(grabbedImage, grabbedImage.Position.X);
                Canvas.SetTop(grabbedImage, grabbedImage.Position.Y);

                canvas.Children.Add(grabbedImage);
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

        private void onGrab()
        {
            if(image.isGrabbed(kinectPointerPosition))
            {
                canMove = true;
                grabbedImage = image;
            }
        }

        private void onRelease()
        {
            canMove = false;
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
                        onGrab();
                    else
                        onRelease();
                }
            }
        }
    }
}
