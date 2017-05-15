﻿using Microsoft.Kinect;
using Microsoft.Kinect.Wpf.Controls;
using POC_GestureNavigation.Pages;
using System.Windows;

namespace POC_GestureNavigation
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.kinectRegion.KinectSensor = KinectSensor.GetDefault();
            KinectRegion.SetKinectRegion(this, kinectRegion);
            ((App)Application.Current).kinectRegion = kinectRegion;
            ((App)Application.Current).lblTime = lblTime;

            this.frame.Navigate(new MainPage());
        }
    }
}
