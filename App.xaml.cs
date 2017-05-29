using Microsoft.Kinect;
using POC_MultiUserIdentification.Model;
using POC_MultiUserIdentification.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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

        internal List<User> users { get; set; }
        internal List<uint> currentPeople { get; set; }
        internal List<uint> unidentifiedPeople { get; set; }

        internal uint currentPerson { get; set; }

        internal uint[] bodyIndexFrameDataConverted { get; set; }

        internal StackPanel spUnidentifiedPeople { get; set; }

        internal StackPanel spIdentifiedPeople { get; set; }

        internal IdentificationPage idPage { get; set; }

        internal MainPage mPage { get; set; }

        internal Nullable<ColorSpacePoint> barcodePoint = null;

        internal DepthSpacePoint[] colorMappedToDepthPoints = null;

        internal bool identicationPage = false;

        internal bool mainPage = false;

        internal DispatcherTimer timer;        

        private List<KeyValuePair<string, string>> availableUsers = new Dictionary<String, String>
        {
            {"USER-MAXIME-BECK-457895", "Maxime Beck"},
            {"USER-MARC-ABRAHAM-789554", "Marc Abraham"},
        }.ToList();

        public App()
        {
            currentPerson = 0;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, NB_SECOND_BEFORE_LOGOUT);
            users = new List<User>();
            currentPeople = new List<uint>();
            unidentifiedPeople = new List<uint>();
        }

        public List<KeyValuePair<string, string>> Users
        {
            get
            { return this.availableUsers; }
            set
            { this.availableUsers = value; }
        }
    }
}
