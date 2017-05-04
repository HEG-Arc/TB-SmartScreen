using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace POC_VoiceRecognition
{
    /// <summary>
    ///MainWindow.xaml - Interaction Logic
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor kinectSensor;
        private KinectAudioStream convertStream = null;
        private SpeechRecognitionEngine speechEngine = null;

        private SolidColorBrush defaultBrush = null;
        private SolidColorBrush yellowBrush = null;
        

        public MainWindow()
        {
            InitializeComponent();
            defaultBrush = new SolidColorBrush(Color.FromRgb(244, 244, 244));
            yellowBrush = new SolidColorBrush(Color.FromRgb(255, 255, 0));
        }

        /// <summary>
        /// Check if the Kinect Reconizer and a supported language pack are installed.
        /// </summary>
        /// <returns></returns>
        private static RecognizerInfo TryGetKinectRecognizer()
        {
            IEnumerable<RecognizerInfo> recognizers;

            // This is required to catch the case when an expected recognizer is not installed.
            // By default - the x86 Speech Runtime is always expected. 
            try
            {
                recognizers = SpeechRecognitionEngine.InstalledRecognizers();
            }
            catch (COMException)
            {
                return null;
            }

            foreach (RecognizerInfo recognizer in recognizers)
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        /// <summary>
        /// Initialize components.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.kinectSensor = KinectSensor.GetDefault();

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Open();
                
                IReadOnlyList<AudioBeam> audioBeamList = this.kinectSensor.AudioSource.AudioBeams;
                System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

                // create the convert stream
                this.convertStream = new KinectAudioStream(audioStream);
            }
            else
            {
                this.lblResult.Content = Properties.Resources.kinectNotReady;
                return;
            }

            RecognizerInfo ri = TryGetKinectRecognizer();

            if (null != ri)
            {
                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar)))
                {
                    var g = new Grammar(memoryStream);
                    this.speechEngine.LoadGrammar(g);
                }
                this.speechEngine.SpeechRecognized += SpeechEngine_SpeechRecognized;

                // let the convertStream know speech is going active
                this.convertStream.SpeechActive = true;

                // For long recognition sessions (a few hours or more), it may be beneficial to turn off adaptation of the acoustic model. 
                // This will prevent recognition accuracy from degrading over time.
                this.speechEngine.SetInputToAudioStream(
                    this.convertStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                this.speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                this.lblResult.Content = Properties.Resources.NoSpeechRecognizer;
            }
        }
  
        /// <summary>
        /// Handles SpeechReconized events trigger by the SpeechEngine.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpeechEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Speech utterance confidence below which we treat speech as if it hadn't been heard
            const double ConfidenceThreshold = 0.6;
            double confidenceLevel = e.Result.Confidence;

            this.ResetComponants();
            if (confidenceLevel >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "FILTER_BY":
                        filterBy.Fill = yellowBrush;
                        break;

                    case "ORDER_BY":
                        orderBy.Fill = yellowBrush;
                        break;

                    case "COUNTRY":
                        country.Fill = yellowBrush;
                        break;

                    case "CITY":
                        city.Fill = yellowBrush;
                        break;

                    case "CUSTOMER":
                        customer.Fill = yellowBrush;
                        break;
                }
            }

            this.lblResult.Content = Properties.Resources.ConfidenceLevel + " : " + Math.Round(confidenceLevel, 3);
        }

        /// <summary>
        /// Reset the color of all rectangles of the UI.
        /// </summary>
        private void ResetComponants()
        {
            filterBy.Fill = defaultBrush;
            orderBy.Fill = defaultBrush;

            customer.Fill = defaultBrush;
            city.Fill = defaultBrush;
            country.Fill = defaultBrush;
        }

        /// <summary>
        /// Clear components before closing the app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (null != this.convertStream)
            {
                this.convertStream.SpeechActive = false;
            }

            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= this.SpeechEngine_SpeechRecognized;
                this.speechEngine.RecognizeAsyncStop();
            }

            if (null != this.kinectSensor)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }
    }
}
