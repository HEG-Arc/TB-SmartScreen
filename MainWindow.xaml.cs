﻿using Microsoft.Kinect;
using POC_MultiUserIdentification.Pages;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace POC_MultiUserIdentification
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int PIXELS_PER_BYTE = 4;

        private KinectSensor sensor;
        private BodyIndexFrameReader bifReader;
        private FrameDescription bifFrameDescription;
        private uint[] bifDataConverted;
        private WriteableBitmap bifBitmap;

        private static readonly uint[] BodyColor =
        {
            0x0000FF00,
            0x00FF0000,
            0xFFFF4000,
            0x40FFFF00,
            0xFF40FF00,
            0xFF808000,
        };

        public MainWindow()
        {
            InitializeComponent();
            this.frame.Navigate(new IdentificationPage());
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.GetDefault();
            bifReader = sensor.BodyIndexFrameSource.OpenReader();
            bifFrameDescription = sensor.BodyIndexFrameSource.FrameDescription;
            bifDataConverted = new uint[bifFrameDescription.Width * bifFrameDescription.Height];
            bifBitmap = new WriteableBitmap(bifFrameDescription.Width, bifFrameDescription.Height, 96, 96, PixelFormats.Bgr32, null);

            this.imgBodyIndex.Source = bifBitmap;
            ((App)Application.Current).bifImage = imgBodyIndex;

            sensor.Open();

            bifReader.FrameArrived += BifReader_FrameArrived;
        }

        private void BifReader_FrameArrived(object sender, BodyIndexFrameArrivedEventArgs e)
        {
            bool bodyIndexFrameProcessed = false;
            using (BodyIndexFrame bifFrame = e.FrameReference.AcquireFrame())
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
                    }
                }
            }

            if (bodyIndexFrameProcessed)
            {
                this.RenderBodyIndexPixels();
            }
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
                    this.bifDataConverted[i] = 0x00000000;
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
