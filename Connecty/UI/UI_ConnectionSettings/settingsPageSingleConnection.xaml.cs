using System;
using System.Windows.Controls;
using System.Windows;

namespace Connecty
{

    // A delegate type for hooking up change notifications.
    public delegate void SingleConnectionSettingsChangedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Interaktionslogik für Page1.xaml
    /// </summary>
    public partial class settingsPageSingleConnection : Page
    {
        
        private SingleConnectionSettings currentConnectionSettings;
        private settingsPageTcpIp tcpIpPage;
        private settingsPageRs232 rs232Page;
        private bool expertModeIsEnabled = false;

        #region Constructors

        /// <summary>
        /// Constructor with Settings
        /// </summary>
        /// <param name="settings"></param>
        public settingsPageSingleConnection(SingleConnectionSettings settings, bool enableExpertMode)
        {
            // Init the View
            InitializeComponent();

            // Set the Data to local variable
            currentConnectionSettings = settings;
            expertModeIsEnabled = enableExpertMode;


            connectionSelection.ItemsSource = new string[] { "RS232", "TCP" };

            // Set up the UI to corresponding to the current Settings
            updateUiWithCurrentSettings();

        }

        /// <summary>
        /// Constructor with now params (This Constructor is setting the default settings)
        /// </summary>
        public settingsPageSingleConnection()
        {
            // Init the View
            InitializeComponent();

            // Set the Data to local variable
            currentConnectionSettings = new SingleConnectionSettings();

            connectionSelection.ItemsSource = new string[] { "RS232", "TCP" };

            // Set up the UI to corresponding to the current Settings
            updateUiWithCurrentSettings();

        }

        #endregion


        /// <summary>
        /// Read the current settings an show the user the right UI with the Current Settings
        /// </summary>
        public void updateUiWithCurrentSettings()
        {

            if (currentConnectionSettings.currentConnectionSetting == 1)
            {
                connectionSelection.SelectedIndex = connectionSelection.Items.IndexOf("TCP");
                tcpIpPage = new settingsPageTcpIp(currentConnectionSettings, expertModeIsEnabled);
                tcpIpPage.Changed += new TcpIpSettingsChangedEventHandler(connectionSettingsChanged);

                if (settingsContentFrame.IsLoaded)
                {
                    settingsContentFrame.Content = tcpIpPage;
                }
                else
                {
                    settingsContentFrame.Loaded += new RoutedEventHandler(tcpipContentFrameLoaded);
                }

            }
            else
            {
                connectionSelection.SelectedIndex = connectionSelection.Items.IndexOf("RS232");
                rs232Page = new settingsPageRs232(currentConnectionSettings, expertModeIsEnabled);
                rs232Page.Changed += new Rs232SettingsChangedEventHandler(connectionSettingsChanged);

                if (settingsContentFrame.IsLoaded)
                {
                    settingsContentFrame.Content = rs232Page;
                }
                else
                {
                    settingsContentFrame.Loaded += new RoutedEventHandler(rs232ContentFrameLoaded);
                }
            }
        }

        /// <summary>
        /// Selection of the Connection has Changed. Lets show the Data Corresponding to the Requested Connection Ínterface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectionSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the Sender Object as an ComboBox Object 
            ComboBox conSelect = sender as ComboBox;

            if (conSelect.SelectedItem.ToString() == "RS232")
            {
                currentConnectionSettings.currentConnectionSetting = 2;//Save the current selected Connection Interface
                rs232Page = new settingsPageRs232(currentConnectionSettings, expertModeIsEnabled);// Create a new Page with the current Interface Settings
                rs232Page.Changed += new Rs232SettingsChangedEventHandler(connectionSettingsChanged);

                if (settingsContentFrame.IsLoaded)
                {
                    settingsContentFrame.Content = rs232Page;
                }
                else
                {
                    settingsContentFrame.Loaded += new RoutedEventHandler(rs232ContentFrameLoaded);
                }

            }
            else
            {
                currentConnectionSettings.currentConnectionSetting = 1;//Save the current selected Connection Interface
                tcpIpPage = new settingsPageTcpIp(currentConnectionSettings, expertModeIsEnabled);// Create a new Page with the current Interface Settings
                tcpIpPage.Changed += new TcpIpSettingsChangedEventHandler(connectionSettingsChanged);


                if (settingsContentFrame.IsLoaded)
                {
                    settingsContentFrame.Content = tcpIpPage;
                }
                else
                {
                    settingsContentFrame.Loaded += new RoutedEventHandler(tcpipContentFrameLoaded);
                }

            }

        }

        /// <summary>
        /// Content Fram RS232 Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rs232ContentFrameLoaded(object sender, RoutedEventArgs e)
        {
            settingsContentFrame.Content = rs232Page;
        }

        /// <summary>
        /// Content Fram TCP/IP Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tcpipContentFrameLoaded(object sender, RoutedEventArgs e)
        {
            settingsContentFrame.Content = tcpIpPage;
        }


        /// <summary>
        /// Get the Valid State from the current Page
        /// </summary>
        /// <returns></returns>
        public bool checkisPlausible()
        {

            bool yesItIsValid;

            if (currentConnectionSettings.currentConnectionSetting == 2)
            {
                if (rs232Page != null)
                {
                    yesItIsValid = rs232Page.IsDataValid();
                }
                else
                {
                    yesItIsValid = false;
                }

            }
            else
            {
                if (tcpIpPage != null)
                {
                    yesItIsValid = tcpIpPage.IsDataValid();
                }
                else
                {
                    yesItIsValid = false;
                }

            }
            
            return yesItIsValid;
        }

        /// <summary>
        /// Funtion to return the User entered Settings. This Function is called when the Accept button was Pressed
        /// </summary>
        public SingleConnectionSettings getUserParams()
        {
            if (currentConnectionSettings.currentConnectionSetting == 1)
            {
                if (tcpIpPage != null)
                {
                    currentConnectionSettings = tcpIpPage.getUserParams();
                }

            }
            else
            {
                if (rs232Page != null)
                {
                    currentConnectionSettings = rs232Page.getUserParams();
                }

            }

            // Return the User Settings
            return currentConnectionSettings;
        }

        /// <summary>
        /// Function to disable the ui elements
        /// </summary>
        public void DisableEditFunctions()
        {

            // Disable all the Editable UI Elements
            connectionSelection.IsEnabled = false;
            if (tcpIpPage != null)
            {
                tcpIpPage.DisableEditFunctions();

            }
            if (rs232Page != null)
            {

                rs232Page.DisableEditFunctions();
            }

        }

        // This will be called whenever the list changes.
        private void connectionSettingsChanged(object sender, EventArgs e)
        {
            // Set the Event that the User Changed an Input
            OnChanged(EventArgs.Empty);
        }


        #region On Change Eventhandler

        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event SingleConnectionSettingsChangedEventHandler Changed;

        // Invoke the Changed event; called whenever list changes
        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }



        #endregion

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Set up the UI to corresponding to the current Settings
            // setUiFromPreviousSettings();
        }
    }
}
