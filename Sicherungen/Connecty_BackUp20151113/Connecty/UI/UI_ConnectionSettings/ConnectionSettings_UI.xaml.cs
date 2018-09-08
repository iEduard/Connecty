using System.Windows;
using System.Windows.Input;

namespace Connecty
{
    /// <summary>
    /// Interaktionslogik für ConnectionSettings_UI.xaml
    /// </summary>
    public partial class ConnectionSettings_UI : Window
    {

        #region Local Variables
        private settingsPageConnection singleConnectionPage;
        private settingsPageConnectyInTheMiddle connectyInTheMiddlepage;
        private ConnectionSettings userSettings = new ConnectionSettings();
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
        public ConnectionSettings_UI(ConnectionSettings settings, bool setUiEnable)
        {

            // Clone the Settings Object. becouse We are gone to change the Settings withe the interfacing of the users..
            userSettings = (ConnectionSettings)ObjectCopier.Clone(settings);

            InitializeComponent();

            //Check the Current User Settings
            updateUiWithCurrentSettings();

            singleConnectionPage = new settingsPageConnection(userSettings.connection1);
            SingleConnectionContentFrame.Loaded += new RoutedEventHandler(singleConnectionPageLoaded);

            connectyInTheMiddlepage = new settingsPageConnectyInTheMiddle(userSettings);
            ConnectyInTheMiddleContentFrame.Loaded += new RoutedEventHandler(multiConnectionPageLoaded);


            if (!setUiEnable)
            {
                DisableEditFunctions();
            }
        }


        private void singleConnectionPageLoaded(object sender, RoutedEventArgs e)
        {
            SingleConnectionContentFrame.Content = singleConnectionPage;
            singleConnectionPage.updateUiWithCurrentSettings();
        }

        private void multiConnectionPageLoaded(object sender, RoutedEventArgs e)
        {
            ConnectyInTheMiddleContentFrame.Content = connectyInTheMiddlepage;
            connectyInTheMiddlepage.updateUiWithCurrentSettings();
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
            // Set the Selecetion of the Function
            ConectionSettingTabControl.SelectedIndex = userSettings.functionSelect;
        }


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

        #endregion

        /// <summary>
        /// Get the Data From the Pages
        /// </summary>
        private void getDataFromPages()
        {
            if (ConectionSettingTabControl.SelectedIndex == 0)
            {
                userSettings.functionSelect = 0;
                userSettings.connection1 = singleConnectionPage.getUserParams();
            }
            else if (ConectionSettingTabControl.SelectedIndex == 1)
            {
                userSettings.functionSelect = 1;
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

            if (ConectionSettingTabControl.SelectedIndex == 0)
            {
                dataIsPlausible = singleConnectionPage.checkisPlausible();
            }
            else if (ConectionSettingTabControl.SelectedIndex == 1)
            {
                dataIsPlausible = connectyInTheMiddlepage.dataIsValid;
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




    }
}
