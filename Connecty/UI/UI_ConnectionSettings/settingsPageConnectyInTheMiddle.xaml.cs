using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Connecty
{
    /// <summary>
    /// Interaktionslogik für settingsPageConnectyInTheMiddle.xaml
    /// </summary>
    public partial class settingsPageConnectyInTheMiddle : Page
    {

        #region Local Variables
        private ConnectionSettings currentConnectionSettings;
        private settingsPageSingleConnection connection_1;
        private settingsPageSingleConnection connection_2;
        private bool expertModeIsEnabled = false;
        #endregion

        #region Public Data
        public bool dataIsValid
        {
            get { return checkisPlausible(); }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings"></param>
        public settingsPageConnectyInTheMiddle(ConnectionSettings settings, bool enableExpertMode)
        {

            currentConnectionSettings = settings;
            expertModeIsEnabled = enableExpertMode;
            // Initialize the UI
            InitializeComponent();

            // Update the UI
            updateUiWithCurrentSettings();

        }

        /// <summary>
        /// Constroctor for the UI
        /// </summary>
        public settingsPageConnectyInTheMiddle()
        {
            // Initialize the UI
            InitializeComponent();

        }
        #endregion

        #region Update the UI

        /// <summary>
        /// This function sets the Data from the Current Settings
        /// </summary>
        public void updateUiWithCurrentSettings()
        {

            // Set the Data to the Left Frame for the Connection 1
            Connection1_Name.Text = currentConnectionSettings.connection1.connectionName;
            connection_1 = new settingsPageSingleConnection(currentConnectionSettings.connection1, expertModeIsEnabled);
            connection_1.Changed += new SingleConnectionSettingsChangedEventHandler(connectionSettingsChanged);

            if (settingsContentFrameConnection1.IsLoaded)
            {
                settingsContentFrameConnection1.Content = connection_1;
            }
            else
            {
                settingsContentFrameConnection1.Loaded += new RoutedEventHandler(contentFrame1Loaded);
            }


            // Set the Data to the Left Frame for the Connection 2
            Connection2_Name.Text = currentConnectionSettings.connection2.connectionName;
            connection_2 = new settingsPageSingleConnection(currentConnectionSettings.connection2, expertModeIsEnabled);
            connection_2.Changed += new SingleConnectionSettingsChangedEventHandler(connectionSettingsChanged);

            if (settingsContentFrameConnection2.IsLoaded)
            {
                settingsContentFrameConnection2.Content = connection_2;
            }
            else
            {
                settingsContentFrameConnection2.Loaded += new RoutedEventHandler(contentFrame2Loaded);
            }



            // Check the Plausability. To Show the User Wrong Inputs
            checkisPlausible();
        }

        /// <summary>
        /// Content Frame 1 Loaded
        /// So we can set the UI Settings to the current Parameters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contentFrame1Loaded(object sender, RoutedEventArgs e)
        {
            settingsContentFrameConnection1.Content = connection_1;
        }

        /// <summary>
        /// Content Frame 2 Loaded
        /// So we can set the UI Settings to the current Parameters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contentFrame2Loaded(object sender, RoutedEventArgs e)
        {
            settingsContentFrameConnection2.Content = connection_2;
        }

        /// <summary>
        /// This will be called whenever the list changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectionSettingsChanged(object sender, EventArgs e)
        {
            // CHeck the Plausability after every user change
            checkisPlausible();
        }

        /// <summary>
        /// Disable the User Input
        /// </summary>
        public void DisableEditFunctions()
        {
            connection_1.DisableEditFunctions();
            connection_2.DisableEditFunctions();

        }


        #endregion

        #region Public Methods for the Controler
        /// <summary>
        /// Funtion to return the User entered Settings. This Function is called when the Accept button was Pressed
        /// </summary>
        public ConnectionSettings getUserParams()
        {

            // Get the Data from the two Pages
            currentConnectionSettings.connection1 = connection_1.getUserParams();
            currentConnectionSettings.connection1.connectionName = Connection1_Name.Text;

            currentConnectionSettings.connection2 = connection_2.getUserParams();
            currentConnectionSettings.connection2.connectionName = Connection2_Name.Text;

            // Return the User Settings
            return currentConnectionSettings;
        }

        #endregion

        #region Local Methods

        /// <summary>
        /// Get the Valid State from the current Page
        /// </summary>
        /// <returns></returns>
        public bool checkisPlausible()
        {
            bool yesDataIsValid = false;

            SingleConnectionSettings con1 = connection_1.getUserParams();
            SingleConnectionSettings con2 = connection_2.getUserParams();


            if ((connection_1.checkisPlausible() && connection_2.checkisPlausible()))
            {
                // Bevor we can compare those Variables we have to figure out wich Interface is used!
                if (con1.currentConnectionSetting == con2.currentConnectionSetting)
                {
                    if (con1.currentConnectionSetting == 1)
                    {
                        // It is not Possible that two Servers listen to the Same Port...
                        if (con1.tcpSettings.clientServerSelection == "Server"
                            && con2.tcpSettings.clientServerSelection == "Server"
                            && con1.tcpSettings.port == con2.tcpSettings.port)
                        {
                            yesDataIsValid = false;
                        }
                        else
                        {
                            yesDataIsValid = true;
                        }
                    }
                    else
                    {
                        if (con1.serialSettings.port == con2.serialSettings.port)
                        {

                            yesDataIsValid = false;
                        }
                        else
                        {
                            yesDataIsValid = true;
                        }
                    }
                }
                else
                {
                    yesDataIsValid = true;
                }


                // Color the UI for the User to show wich setting is not ok..
                if (!yesDataIsValid)
                {
                    // Set the Boarder Thicknes. To show the user te wrong Input
                    BoarderSettingsContenFrame2.BorderBrush = Brushes.Red;

                }
                else
                {
                    // Hide the Boarder to let the User know that the input is valid
                    BoarderSettingsContenFrame2.BorderBrush = Brushes.LightGray;
                }
            }


            // Check if both Connections are OK
            return yesDataIsValid;
        }

        #endregion

    }
}
