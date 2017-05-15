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
using System.Windows.Threading;

namespace POC_GestureNavigation.Pages
{
    /// <summary>
    /// Logique d'interaction pour ObjectPage.xaml
    /// </summary>
    public partial class ObjectPage : Page
    {
        private const int PIXELS_PER_BYTE = 4;
        private const int NB_IMG_DISPLAYED = 10;

        private App app;

        private KinectSensor sensor;
        private BodyFrameReader bfr;
        private Body[] bodies;

        private bool grabbing = false;
        private CustomImage grabbedImage = null;

        private KinectCoreWindow kinectCoreWindow;
        private KinectPointerPoint kinectPointerPoint;
        private Point kinectPointerPosition;

        private List<CustomImage> images;
        private CustomImage bucket;

        private Label lblTime;
        private DispatcherTimer timer;

        public ObjectPage()
        {
            InitializeComponent();
            app = (App)Application.Current;
            lblTime = app.lblTime;
            timer = app.timer;
            this.Loaded += ObjectPage_Loaded;
        }        

        private void ObjectPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Kinect init
            sensor = KinectSensor.GetDefault();
            bfr = sensor.BodyFrameSource.OpenReader();
            kinectCoreWindow = KinectCoreWindow.GetForCurrentThread();
            sensor.Open();
            bodies = new Body[6];            
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStart.Visibility = Visibility.Hidden;
            bfr.FrameArrived += Bfr_FrameArrived;
            app.ResetTimer();
            timer.Start();
            initUI();
        }
        
        private void initUI()
        {            
            lblTime.Visibility = Visibility.Visible;

            bucket = new CustomImage();
            bucket.Source = new BitmapImage(new Uri("/Images/Bucket.png", UriKind.Relative));
            bucket.Height = 200;
            bucket.Width = 200;
            bucket.Position = new Point(grid.RenderSize.Width - bucket.Width - 20, grid.RenderSize.Height / 2 - bucket.Height / 2);

            CustomImage template = new CustomImage();
            template.Source = new BitmapImage(new Uri("/Images/Components.png", UriKind.Relative));
            template.Height = 150;
            template.Width = 150;

            Random rand = new Random();
            images = new List<CustomImage>();
            for (int i = 0; i < NB_IMG_DISPLAYED; i++)
            {
                CustomImage mi = CustomImage.Clone(template);
                mi.Position = new Point(rand.Next(0, (int)(grid.RenderSize.Width - mi.Width - bucket.Width - 20)),
                                        rand.Next(0, (int)(grid.RenderSize.Height - mi.Height)));
                images.Add(mi);
            }
            DisplayImages();            
                    
            kinectCoreWindow.PointerMoved += KinectCoreWindow_PointerMoved;            
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
            foreach(CustomImage im in images)
            {
                if (im.DoesCollide(kinectPointerPosition))
                {
                    grabbedImage = im;
                    continue;
                }
            }            
        }

        private void onRelease()
        {
            if(grabbedImage != null && bucket.DoesCollide(grabbedImage))
            {
                images.Remove(grabbedImage);
                if (images.Count == 0)
                    GameOver();
                DisplayImages();
            }
            grabbedImage = null;
        }

        private void DisplayImages(CustomImage except = null)
        {
            canvas.Children.Clear();
            foreach (CustomImage mi in images)
            {
                if (!mi.Equals(except))
                {
                    Canvas.SetLeft(mi, mi.Position.X);
                    Canvas.SetTop(mi, mi.Position.Y);
                    canvas.Children.Add(mi);
                }
            }

            Canvas.SetLeft(bucket, bucket.Position.X);
            Canvas.SetTop(bucket, bucket.Position.Y);
            canvas.Children.Add(bucket);
        }

        private void GameOver()
        {
            timer.Stop();
            app.objectScore = lblTime.Content.ToString();
            
            kinectCoreWindow.PointerMoved -= KinectCoreWindow_PointerMoved;
            bfr.FrameArrived -= Bfr_FrameArrived;

            bucket.Visibility = Visibility.Hidden;
            btnStart.Visibility = Visibility.Visible;
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
