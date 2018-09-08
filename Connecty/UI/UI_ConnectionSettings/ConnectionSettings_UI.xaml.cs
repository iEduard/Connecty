using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Connecty
{
    /// <summary>
    /// Interaktionslogik für ConnectionSettings_UI.xaml
    /// </summary>
    public partial class ConnectionSettings_UI : Window
    {

        #region Local Variables
        private settingsPageSingleConnection singleConnectionPage;
        private settingsPageConnectyInTheMiddle connectyInTheMiddlepage;
        private ConnectionSettings userSettings = new ConnectionSettings();
        private ApplicationSettings applicationSettings = new ApplicationSettings();

        /// <summary>
        /// Start Position of the Window
        /// </summary>
        private Point windowStartPosistion;

        #endregion

        #region Public Variables
        /// <summary>
        /// Funtion to return the User entered Settings
        /// </summary>
        public ConnectionSettings getUserParams
        {
            // Return the User Settings
            get { return userSettings; }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings"></param>
        public ConnectionSettings_UI(ConnectionSettings settings, bool setUiEnable, ApplicationSettings appSettings, Point position)
        {
            // Store the Starting Position to a local Variable
            windowStartPosistion = position;

            // Clone the Settings Object. becouse We are gone to change the Settings withe the interfacing of the users..
            userSettings = (ConnectionSettings)ObjectCopier.Clone(settings);
            applicationSettings = (ApplicationSettings)ObjectCopier.Clone(appSettings);

            InitializeComponent();

            singleConnectionPage = new settingsPageSingleConnection(userSettings.connection1, applicationSettings.expertModeIsActive);
            connectyInTheMiddlepage = new settingsPageConnectyInTheMiddle(userSettings, applicationSettings.expertModeIsActive);

            //Check the Current User Settings
            updateUiWithCurrentSettings();

            SettingsModeFrame.Loaded += new RoutedEventHandler(ConnectionSettingsFrameLoaded);


            if (!setUiEnable)
            {
                DisableEditFunctions();
            }
        }


        #region UI Control 

        /// <summary>
        /// Frame loaded event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectionSettingsFrameLoaded(object sender, RoutedEventArgs e)
        {
            SetTheFrameContent();
        }

        /// <summary>
        /// Set the Content to the Frame
        /// </summary>
        private void SetTheFrameContent()
        {
            if (!SettingsModeFrame.IsLoaded)
            {
                return;
            }

            if (userSettings.functionSelect == 0)
            {
                SettingsModeFrame.Content = singleConnectionPage;
                singleConnectionPage.updateUiWithCurrentSettings();
            }
            else
            {
                SettingsModeFrame.Content = connectyInTheMiddlepage;
                connectyInTheMiddlepage.updateUiWithCurrentSettings();

            }
        }

        /// <summary>
        /// Disable the UI in Case the User is not allowed to make any changes
        /// </summary>
        private void DisableEditFunctions()
        {
            bAccept.IsEnabled = false;

            // Disable the Single Connection
            singleConnectionPage.DisableEditFunctions();

            // Disable the Multi Connection
            connectyInTheMiddlepage.DisableEditFunctions();

        }

        /// <summary>
        /// Update the UI with the Given Settings
        /// </summary>
        private void updateUiWithCurrentSettings()
        {

            // Set the Combobox Data
            cbModeSelect.ItemsSource = new string[] { "Einfache Verbindung", "Verbindungsbrücke" };
            cbModeSelect.SelectedIndex = userSettings.functionSelect;

        }

        /// <summary>
        /// 
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
        }

        #endregion

        #region User Interaction

        /// <summary>
        /// Accept the User entered Settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bAccept_Click(object sender, RoutedEventArgs e)
        {
            useUserInput();
        }

        /// <summary>
        /// Abbort the Settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            discardUserInput();
        }

        /// <summary>
        /// Check the input key from the User
        /// With Enter we will Accept the Entries
        /// With Escape we will Abbort the Settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Escape))
            {
                discardUserInput();
            }
            else if ((e.Key == Key.Enter))
            {
                useUserInput();
            }
        }

        /// <summary>
        /// The FunctionSelection was changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbModeSelect_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Get the Combobox Item
            ComboBox myCombobox = sender as ComboBox;

            // Set the Setting to the new Value vrom the User selceted Function Mode
            userSettings.functionSelect = myCombobox.SelectedIndex;

            // Set the requested Page to the Frame
            SetTheFrameContent();

        }

        #endregion

        #region Local Private Methods

        /// <summary>
        /// Get the Data From the Pages
        /// </summary>
        private void getDataFromPages()
        {

            if (userSettings.functionSelect == 1)
            {
                userSettings = connectyInTheMiddlepage.getUserParams();
            }
            else
            {
                userSettings.connection1 = singleConnectionPage.getUserParams();
            }
        }

        /// <summary>
        /// Check the Param State of the Pages
        /// </summary>
        /// <returns>Returns True if all Data inserted are valid. False if their is an Value that is not Valid</returns>
        private bool checkPlauseability()
        {
            
            bool dataIsPlausible = false;


            if (userSettings.functionSelect == 1)
            {
                dataIsPlausible = connectyInTheMiddlepage.checkisPlausible(); // .dataIsValid;
            }
            else
            {
                dataIsPlausible = singleConnectionPage.checkisPlausible();
            }



            return dataIsPlausible;

        }

        /// <summary>
        /// Use the entered User input
        /// </summary>
        private void useUserInput()
        {
            // To be Done Chek the Plauseability....
            if (checkPlauseability())
            {

                getDataFromPages();

                this.DialogResult = true;
                //this.Close();
            }
            else
            {

                // Configure the message box to be displayed
                string caption = "Ungültige Einstellungen";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;

                // Display message box
                MessageBox.Show("Eingegebene Daten sind ungültig. Bitte die Roten Felder mit gültigen Daten füllen", caption, button, icon);
            }
        }

        /// <summary>
        /// Do not use the User input
        /// </summary>
        private void discardUserInput()
        {
            this.DialogResult = false;
        }

        #endregion


    }
}
