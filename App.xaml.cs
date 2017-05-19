using Microsoft.Kinect;
using POC_MultiUserIdentification.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace POC_MultiUserIdentification
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const int NB_SECOND_BEFORE_LOGOUT = 3;
        internal MultiSourceFrameReader msfr { get; set; }
        internal KinectSensor sensor { get; set; }

        internal int cptMsfrE = 0;

        internal DispatcherTimer timer;

        private List<KeyValuePair<string, string>> users = new Dictionary<String, String>
        {
            {"USER-MAXIME-BECK-457895", "Maxime Beck"},
            {"USER-MARC-ABRAHAM-789554", "Marc Abraham"},
        }.ToList();

        private KeyValuePair<string, string> user;

        public App()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, NB_SECOND_BEFORE_LOGOUT);
        }

        public List<KeyValuePair<string, string>> Users
        {
            get
            { return this.users; }
            set
            { this.users = value; }
        }

        public KeyValuePair<string, string> User
        {
            get
            { return this.user; }
            set
            { this.user = value; }
        }
    }
}
