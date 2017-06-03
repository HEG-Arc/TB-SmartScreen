using Microsoft.Kinect;
using POC_MultiUserIndification_Collider.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace POC_MultiUserIndification_Collider
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal List<User> users { get; set; }

        internal List<ulong> trackedBodies { get; set; }

        internal Page identificationPage { get; set; }

        internal Page mainPage { get; set; }

        internal MultiSourceFrameReader msfr { get; set; }

        public App()
        {
            this.users = new List<User>();
            this.trackedBodies = new List<ulong>();
        }

    }
}
