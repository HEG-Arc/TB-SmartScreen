using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace POC_MultiUserIdentification
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal System.Windows.Controls.Image bifImage { get; set; }

        private List<KeyValuePair<string, string>> users = new Dictionary<String, String>
        {
            {"USER-MAXIME-BECK-457895", "Maxime Beck"},
            {"USER-MARC-ABRAHAM-789554", "Marc Abraham"},
        }.ToList();

        private KeyValuePair<string, string> user;

        public App()
        {

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
