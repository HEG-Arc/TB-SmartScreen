using Microsoft.Kinect.Wpf.Controls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace POC_GestureNavigation
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal KinectRegion kinectRegion { get; set; }

        internal DispatcherTimer timer { get; set; }

        internal Label lblTime { get; set; }

        internal String objectScore { get; set; }

        private int nbSecondsSinceStart = 0;

        public App()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            nbSecondsSinceStart++;
            TimeSpan time = TimeSpan.FromSeconds(nbSecondsSinceStart);
            lblTime.Content = time.ToString(@"hh\:mm\:ss");
        }

        public void ResetTimer()
        {
            lblTime.Content = "00:00:00";
            nbSecondsSinceStart = 0;
        }
    }
}
