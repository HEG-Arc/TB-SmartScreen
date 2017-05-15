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
        private const int NB_IMG_DISPLAYED = 10;

        private KinectSensor sensor;
        private BodyFrameReader bfr;
        private Body[] bodies;

        private bool grabbing = false;
        private MovableImage grabbedImage = null;

        private KinectPointerPoint kinectPointerPoint;
        private Point kinectPointerPosition;

        private List<MovableImage> images;


        public ObjectPage()
        {
            InitializeComponent();
            images = new List<MovableImage>();
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
            MovableImage template = new MovableImage();
            template.Source = new BitmapImage(new Uri("/Images/Components.png", UriKind.Relative));
            template.Height = 150;
            template.Width = 150;
            template.HorizontalAlignment = HorizontalAlignment.Left;
            template.VerticalAlignment = VerticalAlignment.Top;

            Random rand = new Random();
            for (int i = 0; i < NB_IMG_DISPLAYED; i++)
            {
                MovableImage mi = MovableImage.Clone(template);
                mi.Position = new Point(rand.Next(0, (int)(grid.RenderSize.Width - mi.Width)),
                                        rand.Next(0, (int)(grid.RenderSize.Height - mi.Height)));

                Canvas.SetLeft(mi, mi.Position.X);
                Canvas.SetTop(mi, mi.Position.Y);
                canvas.Children.Add(mi);

                images.Add(mi);
            }            
        }

        private void KinectCoreWindow_PointerMoved(object sender, KinectPointerEventArgs e)
        {
            kinectPointerPoint = e.CurrentPoint;
            kinectPointerPosition.X = kinectPointerPoint.Position.X * canvas.ActualWidth;
            kinectPointerPosition.Y = kinectPointerPoint.Position.Y * canvas.ActualHeight;

            if (grabbedImage != null)
            {
                canvas.Children.Clear();

                foreach(MovableImage mi in images)
                {
                    if(!mi.Equals(grabbedImage))
                    {
                        Canvas.SetLeft(mi, mi.Position.X);
                        Canvas.SetTop(mi, mi.Position.Y);
                        canvas.Children.Add(mi);
                    }
                }

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
            foreach(MovableImage im in images)
            {
                if (im.IsGrabbed(kinectPointerPosition))
                {
                    grabbedImage = im;
                    continue;
                }
            }            
        }

        private void onRelease()
        {
            grabbedImage = null;
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
