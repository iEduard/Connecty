using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Connecty
{
    /// <summary>
    /// Interaktionslogik für UpdateCheckWindow.xaml
    /// </summary>
    public partial class UpdateCheckWindow : Window
    {

        /// <summary>
        /// Update handler object
        /// </summary>
        private UpdateHandler updateHandler;

        /// <summary>
        /// Start Position of the Window
        /// </summary>
        private Point windowStartPosistion;


        public static RoutedCommand debugViewRoutedCommand = new RoutedCommand();

        /// <summary>
        /// Constructor of the Update Window
        /// </summary>
        public UpdateCheckWindow(Point position)
        {
            windowStartPosistion = position;

            // Initalize the UI
            InitializeComponent();

            updateHandler = new UpdateHandler();
            updateHandler.getVersionInformation();
            updateHandler.DownloadCompleted += DownloadCompleted;

            this.DataContext = updateHandler;
            this.SizeToContent = SizeToContent.Height;
        }



        #region User Interactioin

        /// <summary>
        /// DownLoad the DebugVersion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btGetDebugVersion_Click(object sender, RoutedEventArgs e)
        {
            //updateHandler.DebugDownLoadChanged += new DownloadProgressChangedEventHandler(onDownloadProgressDebugVersionChanged);
            updateHandler.downloadDebugVersion();
        }

        /// <summary>
        /// Open the Release Notes from the Debug Version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btGetDebugVersionInfo_Click(object sender, RoutedEventArgs e)
        {
            updateHandler.openDebugReleaseInfo();
        }

        /// <summary>
        /// Download the Release Version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btGetReleaseVersion_Click(object sender, RoutedEventArgs e)
        {
            //updateHandler.ReleaseDownLoadChanged += new DownloadProgressChangedEventHandler(onDownloadProgressReleaseVersionChanged);
            updateHandler.downloadReleaseVersion();
        }

        /// <summary>
        /// Open the Release Notes from the Release Version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btGetReleaseVersionInfo_Click(object sender, RoutedEventArgs e)
        {
            updateHandler.openReleaseReleaseInfo();
        }

        /// <summary>
        /// Read the Version Information from the local Application and the Online Version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btCheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            updateHandler.getVersionInformation();
        }

        /// <summary>
        /// Close the Updater Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// Add the Hot Keys and Key Bindings to the UI
        /// </summary>
        private void AddHotKeys()
        {
            try
            {
                // ----------------------------------------------------------------------------------------------
                // Add the Show / Hide Debug Download Commands
                CommandBinding cb = new CommandBinding(debugViewRoutedCommand, DebugView_EventHandler);
                this.CommandBindings.Add(cb);

                KeyGesture kg = new KeyGesture(Key.D, ModifierKeys.Control);
                InputBinding ib = new InputBinding(debugViewRoutedCommand, kg);
                this.InputBindings.Add(ib);

            }
            catch
            {
                //handle exception error
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DebugView_EventHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (spDebugVersion.Visibility == Visibility.Visible)
            {
                spDebugVersion.Visibility = Visibility.Collapsed;
            }
            else
            {
                spDebugVersion.Visibility = Visibility.Visible;
            }
        }



        #endregion

        #region Eventhandler

        /// <summary>
        /// Eventhandler for the Version Information is Available
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onVersionInformationChanged(object sender, UpdateInformationAvailableEventArgs e)
        {
            /*
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Threading.DispatcherOperationCallback(delegate
            {

                // Update the Labels wit the version Informations
                // lbCurrentVersion.Content = e.localCurrentVersion;
                // lbOnlineDebugVersion.Content = e.onlineDebugVersion;
                // lbOnlineReleaseVersion.Content = e.onlineReleaseVersion;

                int _versionDifferent = e.localCurrentVersion.CompareTo(e.onlineReleaseVersion) ;
                if (_versionDifferent < 0)
                {
                    lbNewStateRelease.Visibility = Visibility.Visible;
                }
                else
                {
                    lbNewStateRelease.Visibility = Visibility.Hidden;
                }

                _versionDifferent = e.localCurrentVersion.CompareTo(e.onlineDebugVersion);
                if (_versionDifferent < 0)
                {
                    lbNewStateDebug.Visibility = Visibility.Visible;
                }
                else
                {
                    lbNewStateDebug.Visibility = Visibility.Hidden;
                }

                return null;
            }), null);*/
        }

        /// <summary>
        /// Eventhandler to update the Progressbar for the Release Version Download
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onDownloadProgressReleaseVersionChanged(object sender, DownloadProgressChangedEventArgs e)
        {

            this.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Threading.DispatcherOperationCallback(delegate
            {
                // Update the Progress Bar of the Release Download
                pbReleaseDownloadState.Value = e.ProgressPercentage;

                if (e.ProgressPercentage >= 100)
                {
                    lbNewStateRelease.Content = "Heruntergeladen";
                    lbNewStateRelease.Foreground = Brushes.Green;
                }

                return null;
            }), null);

        }

        /// <summary>
        /// Eventhandler to update the Progressbar for the Debug Version Download
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onDownloadProgressDebugVersionChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Threading.DispatcherOperationCallback(delegate
            {
                // Update the Progress Bar of the Release Download
                pbDebugDownloadState.Value = e.ProgressPercentage;
                return null;
            }), null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Da ist was schiefgelaufen.. Die Datei konnte nicht gespeichert werden. Bitte erneut versuchen.", "Speichern der Datei nicht möglich", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }

        /// <summary>
        /// Window loaded Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // In order to avoid divverences in the behaivior of the UI with an different Scalling Factor set in Windows we have to get 
            // an Point with the Scalling Factor erased
            windowStartPosistion = UiHelper.PointWithScalingDependencies(PresentationSource.FromVisual(this), windowStartPosistion);

            this.Left = windowStartPosistion.X;
            this.Top = windowStartPosistion.Y;

            // Add the Hotkeys to the View
            AddHotKeys();
        }

        #endregion

    }
}
