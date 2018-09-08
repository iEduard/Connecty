using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Xml;

namespace Connecty
{

    #region Eventhandler and Event Argument Definitions


    /// <summary>
    /// Event Arguments for the Update-UInformation Available Event
    /// </summary>
    public class UpdateInformationAvailableEventArgs : EventArgs
    {
        public Version onlineReleaseVersion { get; set; }
        public Version onlineDebugVersion { get; set; }
        public Version localCurrentVersion { get; set; }
    }

    /// <summary>
    /// Event for the Updated Version Information from the Local, Online Debug and the Online Release App
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void UpdateInformationAvailableEventHandler(object sender, UpdateInformationAvailableEventArgs e);

    #endregion


    class UpdateHandler : INotifyPropertyChanged
    {
        #region Private Variables

        private static string releaseUrl = "https://www.dropbox.com/s/xqj5r5wm53ts2qh/Connecty.exe?dl=1";// Download URL for the Release version
        private static string debugUrl = "https://www.dropbox.com/s/8oxj9iwsn9no6md/Connecty.exe?dl=1";// Download URL for the Debug Version

        private static string releaseReleaseNotesUrl = "https://www.dropbox.com/s/feuzec7gxv1t6xw/VersionHistory.txt?dl=0";// Download URL for the Release version
        private static string debugReleaseNotesUrl = "https://www.dropbox.com/s/xm9f7kiiem329hv/VersionHistory.txt?dl=0";// Download URL for the Debug Version

        private Thread getVersionThread;
        private Thread downloadReleaseThread;
        private Thread downloadDebugThread;


        private Version onlineReleaseVersion;
        private Version onlineDebugVersion;
        private Version localCurrentVersion;

        private Visibility newStateReleaseVersion = Visibility.Collapsed;
        private Visibility newStateDebugVersion = Visibility.Collapsed;

        private Visibility readStateReleaseVersion = Visibility.Collapsed;
        private Visibility readStateDebugVersion = Visibility.Collapsed;

        public int downloadStateReleaseVersion = 0;
        public int downloadStateDebugVersion = 0;

        #endregion

        #region Public Variables

        public Version OnlineReleaseVersion
        {
            get { return this.onlineReleaseVersion; }
            set
            {
                if (this.onlineReleaseVersion != value)
                    {
                        this.onlineReleaseVersion = value;
                        this.NotifyPropertyChanged("OnlineReleaseVersion");
                    }
            }
        }        
        public Version OnlineDebugVersion
        {
            get { return this.onlineDebugVersion; }
            set
            {
                if (this.onlineDebugVersion != value)
                {
                    this.onlineDebugVersion = value;
                    this.NotifyPropertyChanged("OnlineDebugVersion");
                }
            }
        }
        public Version LocalCurrentVersion
        {
            get { return this.localCurrentVersion; }
            set
            {
                if (this.localCurrentVersion != value)
                {
                    this.localCurrentVersion = value;
                    this.NotifyPropertyChanged("LocalCurrentVersion");
                }
            }
        }

        public Visibility NewStateReleaseVersion
        {
            get { return this.newStateReleaseVersion; }
            set
            {
                if (this.newStateReleaseVersion != value)
                {
                    this.newStateReleaseVersion = value;
                    this.NotifyPropertyChanged("NewStateReleaseVersion");
                }
            }
        }
        public Visibility NewStateDebugVersion
        {
            get { return this.newStateDebugVersion; }
            set
            {
                if (this.newStateDebugVersion != value)
                {
                    this.newStateDebugVersion = value;
                    this.NotifyPropertyChanged("NewStateDebugVersion");
                }
            }
        }

        public Visibility ReadStateReleaseVersion
        {
            get { return this.readStateReleaseVersion; }
            set
            {
                if (this.readStateReleaseVersion != value)
                {
                    this.readStateReleaseVersion = value;
                    this.NotifyPropertyChanged("ReadStateReleaseVersion");
                }
            }
        }
        public Visibility ReadStateDebugVersion
        {
            get { return this.readStateDebugVersion; }
            set
            {
                if (this.readStateDebugVersion != value)
                {
                    this.readStateDebugVersion = value;
                    this.NotifyPropertyChanged("ReadStateDebugVersion");
                }
            }
        }

        public int DownloadStateReleaseVersion
        {
            get { return this.downloadStateReleaseVersion; }
            set
            {
                if (this.downloadStateReleaseVersion != value)
                {
                    this.downloadStateReleaseVersion = value;
                    this.NotifyPropertyChanged("DownloadStateReleaseVersion");
                }
            }
        }
        public int DownloadStateDebugVersion
        {
            get { return this.downloadStateDebugVersion; }
            set
            {
                if (this.downloadStateDebugVersion != value)
                {
                    this.downloadStateDebugVersion = value;
                    this.NotifyPropertyChanged("DownloadStateDebugVersion");
                }
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public UpdateHandler()
        {
            OnlineReleaseVersion = new Version();
            OnlineDebugVersion = new Version();
            LocalCurrentVersion = new Version();
        }

        #region Public Methods

        /// <summary>
        /// Start the get Version Information
        /// </summary>
        public void getVersionInformation()
        {
            // Check if an Thread is already Running
            if (getVersionThread != null)
            {
                if (!getVersionThread.IsAlive)
                {
                    getVersionThread = new Thread(new ThreadStart(getVersions));
                    getVersionThread.Start();
                }
            }
            else
            {
                getVersionThread = new Thread(new ThreadStart(getVersions));
                getVersionThread.Start();
            }


        }

        /// <summary>
        /// Start the Thread and Download the Release Version
        /// </summary>
        public void downloadReleaseVersion()
        {

            if (downloadReleaseThread != null)
            {
                if (!downloadReleaseThread.IsAlive)
                {
                    downloadReleaseThread = new Thread(new ThreadStart(() => downloadConnecty(releaseUrl, new DownloadProgressChangedEventHandler(releaseDownLoadChanged), ("Connecty" + onlineReleaseVersion.ToString()))));
                    downloadReleaseThread.Start();
                }
            }
            else
            {
                downloadReleaseThread = new Thread(new ThreadStart(() => downloadConnecty(releaseUrl, new DownloadProgressChangedEventHandler(releaseDownLoadChanged), ("Connecty" + onlineReleaseVersion.ToString()))));
                downloadReleaseThread.Start();
            }


        }

        /// <summary>
        /// Start the Thread and Download the Debug Version
        /// </summary>
        public void downloadDebugVersion()
        {
            if (downloadDebugThread != null)
            {
                if (!downloadDebugThread.IsAlive)
                {
                    downloadDebugThread = new Thread(new ThreadStart(() => downloadConnecty(debugUrl, new DownloadProgressChangedEventHandler(debugDownLoadChanged), ("Connecty" + onlineDebugVersion.ToString()))));
                    downloadDebugThread.Start();
                }
            }
            else
            {
                downloadDebugThread = new Thread(new ThreadStart(() => downloadConnecty(debugUrl, new DownloadProgressChangedEventHandler(debugDownLoadChanged), ("Connecty" + onlineDebugVersion.ToString()))));
                downloadDebugThread.Start();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void openDebugReleaseInfo()
        {
            Process.Start(debugReleaseNotesUrl);
        }

        /// <summary>
        /// 
        /// </summary>
        public void openReleaseReleaseInfo()
        {
            Process.Start(releaseReleaseNotesUrl);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileUrl"></param>
        /// <param name="downloadProgressChangedEvent"></param>
        private void downloadConnecty(string fileUrl, DownloadProgressChangedEventHandler downloadProgressChangedEvent, string fileName)
        {
            // Check for Internet Connection....
            if (CheckForInternetConnection())
            {
                // Get the Path to Download the File To
                SaveFileDialog _saveFileDialog = new SaveFileDialog();
                _saveFileDialog.Title = "Connecty speichern unter:";
                _saveFileDialog.DefaultExt = ".exe";
                _saveFileDialog.AddExtension = true;
                _saveFileDialog.FileName = fileName;
                _saveFileDialog.InitialDirectory = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);

                if (_saveFileDialog.ShowDialog() == true)
                {
                    // saveFileDialog.FileName;
                    using (WebClient _webClient = new WebClient())
                    {
                        _webClient.DownloadFileAsync(new System.Uri(fileUrl), _saveFileDialog.FileName);
                        _webClient.DownloadProgressChanged += downloadProgressChangedEvent;
                        _webClient.DownloadFileCompleted += downloadCompletedEvent;
                    }
                }
            }
        }

        #region get Version Info

        /// <summary>
        /// The Working Thread to get the Online and Local Verion Number
        /// </summary>
        private void getVersions()
        {
            ReadStateDebugVersion = Visibility.Visible;
            ReadStateReleaseVersion = Visibility.Visible;


            // Set the Event to notify with the Version Information
            UpdateInformationAvailableEventArgs _argument = new UpdateInformationAvailableEventArgs();

            // Load the Current Version
            getCurrentVerison();
            //_argument.localCurrentVersion = localCurrentVersion;
            //versionInformationChanged(_argument);


            if (CheckForInternetConnection())
            {
                // Load the Online Version
                getOnlineVersions();
            }


            if(LocalCurrentVersion < OnlineReleaseVersion)
            {
                NewStateReleaseVersion = Visibility.Visible;
            }
            else
            {
                NewStateReleaseVersion = Visibility.Collapsed;
            }


            if (LocalCurrentVersion < OnlineDebugVersion)
            {
                NewStateDebugVersion = Visibility.Visible;
            }
            else
            {
                NewStateDebugVersion = Visibility.Collapsed;
            }

            ReadStateDebugVersion = Visibility.Collapsed;
            ReadStateReleaseVersion = Visibility.Collapsed;
        }

        /// <summary>
        /// Get the Current Assembly Information from the actual Connecty Instance
        /// </summary>
        private void getCurrentVerison()
        {
            LocalCurrentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <summary>
        /// Get the Online Version Infos
        /// </summary>
        private void getOnlineVersions()
        {
            // Get XML File with the Version Description from: But we have to change the dl=0 to dl=1 to direkt Download the File...
            // https://www.dropbox.com/s/6uf6d3w4cews3rh/VersionInfo.xml?dl=0

            string _filePath = "https://www.dropbox.com/s/6uf6d3w4cews3rh/VersionInfo.xml?dl=1";
            //string _filePath = "C:\\Users\\Eduard Schmidt\\Dropbox\\VisualStudio Projects\\Connecty\\VersionInfo.xml";
            XmlDocument _xmlDocument = new XmlDocument();

            ReadStateReleaseVersion = Visibility.Visible;
            ReadStateDebugVersion = Visibility.Visible;

            try
            {
                // Load the XML Doc from the given URL
                _xmlDocument.Load(_filePath);
                
                // Read the Konfigurations XML
                XmlNodeList _nodeList = _xmlDocument.DocumentElement.SelectNodes("Binary");

                // Get the Online File Versions from the XML
                for (int _i = 0; _i <= (_nodeList.Count - 1); _i++)
                {
                    if ("Release" == _nodeList.Item(_i).Attributes.Item(0).InnerText)
                    {

                        OnlineReleaseVersion = new Version(Convert.ToInt32(_nodeList.Item(_i).SelectSingleNode("Mayor").InnerText),
                                                                        Convert.ToInt32(_nodeList.Item(_i).SelectSingleNode("Minor").InnerText),
                                                                        Convert.ToInt32(_nodeList.Item(_i).SelectSingleNode("Revision").InnerText),
                                                                        Convert.ToInt32(_nodeList.Item(_i).SelectSingleNode("Build").InnerText));
                    }
                    if ("Debug" == _nodeList.Item(_i).Attributes.Item(0).InnerText)
                    {

                        OnlineDebugVersion = new Version(Convert.ToInt32(_nodeList.Item(_i).SelectSingleNode("Mayor").InnerText),
                                                                        Convert.ToInt32(_nodeList.Item(_i).SelectSingleNode("Minor").InnerText),
                                                                        Convert.ToInt32(_nodeList.Item(_i).SelectSingleNode("Revision").InnerText),
                                                                        Convert.ToInt32(_nodeList.Item(_i).SelectSingleNode("Build").InnerText));
                    }
                }
            }
            catch
            {

            }

            ReadStateReleaseVersion = Visibility.Collapsed;
            ReadStateDebugVersion = Visibility.Collapsed;
        }

        #endregion


        /// <summary>
        /// Checks if an Internetconnection is available
        /// </summary>
        /// <returns>Returns True if an Internetconnection is available</returns>
        private static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }


        #endregion

        #region Events

        #region Public Events

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="propName"></param>
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        #endregion

        #region Local Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downloadCompletedEvent(object sender, AsyncCompletedEventArgs e)
        {
            downloadCompleted(e);
        }

        /// <summary>
        /// 
        /// </summary>
        public event AsyncCompletedEventHandler DownloadCompleted;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void downloadCompleted(AsyncCompletedEventArgs e)
        {
            if (DownloadCompleted != null)
                DownloadCompleted(this, e);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void releaseDownLoadChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.DownloadStateReleaseVersion = e.ProgressPercentage;
            }
         }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void debugDownLoadChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e != null)
            {
                this.DownloadStateDebugVersion = e.ProgressPercentage;
            }
                
        }
        #endregion

        #endregion
    }
}
