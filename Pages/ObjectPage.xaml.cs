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
        private Image bucket;


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
            ((App)Application.Current).btnBack.Visibility = Visibility.Visible;
            ((App)Application.Current).lblTitle.Visibility = Visibility.Visible;
            ((App)Application.Current).tbSubTitle.Text = "Glisser/déposer tous les composants dans la corbeille.";

            bucket = new Image();
            bucket.Source = new BitmapImage(new Uri("/Images/Bucket.png", UriKind.Relative));
            bucket.Height = 200;
            bucket.Width = 200;
            bucket.HorizontalAlignment = HorizontalAlignment.Right;
            bucket.VerticalAlignment = VerticalAlignment.Center;

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
                mi.Position = new Point(rand.Next(0, (int)(grid.RenderSize.Width - mi.Width - bucket.Width - 20)),
                                        rand.Next(0, (int)(grid.RenderSize.Height - mi.Height)));
                images.Add(mi);
            }
            DisplayImages();
        }

        private void KinectCoreWindow_PointerMoved(object sender, KinectPointerEventArgs e)
        {
            kinectPointerPoint = e.CurrentPoint;
            kinectPointerPosition.X = kinectPointerPoint.Position.X * canvas.ActualWidth;
            kinectPointerPosition.Y = kinectPointerPoint.Position.Y * canvas.ActualHeight;

            if (grabbedImage != null)
            {
                DisplayImages(grabbedImage);

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
            foreach (MovableImage im in images)
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
            if (grabbedImage != null && isInBucket(grabbedImage))
            {
                images.Remove(grabbedImage);
                if (images.Count == 0)
                    debug.Content = "game over !";
                DisplayImages();
            }
            grabbedImage = null;
        }

        private void DisplayImages(MovableImage except = null)
        {
            canvas.Children.Clear();
            foreach (MovableImage mi in images)
            {
                if (!mi.Equals(except))
                {
                    Canvas.SetLeft(mi, mi.Position.X);
                    Canvas.SetTop(mi, mi.Position.Y);
                    canvas.Children.Add(mi);
                }
            }

            Canvas.SetLeft(bucket, grid.RenderSize.Width - bucket.Width - 20);
            Canvas.SetTop(bucket, grid.RenderSize.Height / 2 - bucket.Height / 2);
            canvas.Children.Add(bucket);
        }

        private bool isInBucket(MovableImage img)
        {
            double bucketX = Canvas.GetLeft(bucket);
            double bucketY = Canvas.GetTop(bucket);

            return (
                        (img.Position.X + img.Width > bucketX && img.Position.X < bucketX + bucket.Width) &&
                        (img.Position.Y + img.Height > bucketY && img.Position.Y < bucketY + bucket.Height)
                    );
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