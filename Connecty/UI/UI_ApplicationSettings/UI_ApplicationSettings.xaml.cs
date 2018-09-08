using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Connecty
{
    /// <summary>
    /// Interaktionslogik für UI_ApplicationSettings.xaml
    /// </summary>
    public partial class UI_ApplicationSettings : Window
    {

        #region local Variables
        private ApplicationSettings applicationSettings;
        private ApplicationSettings oldApplicationSettings;

        private const int msgLogUpperLimit = 999999;
        private const int msgLogLowerLimit = 10;

        private const int sendHistoryUpperLimit = 100;
        private const int sendHistoryLowerLimit = 1;

        /// <summary>
        /// Start Position of the Window
        /// </summary>
        private Point windowStartPosistion;
        #endregion

        #region Public Variables
        /// <summary>
        /// Funtion to return the User entered Settings
        /// </summary>
        public ApplicationSettings getUserParams
        {
            // Return the User Settings
            get { return applicationSettings; }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="themeNameList"></param>
        /// <param name="position"></param>
        public UI_ApplicationSettings(ApplicationSettings settings, List<string> themeNameList, Point position)
        {
            windowStartPosistion = position;
            // Copy the object to the Local variable. So the Settings will not take effekt bevore the user accepts the Entered Values
            applicationSettings = (ApplicationSettings)ObjectCopier.Clone(settings);
            oldApplicationSettings = (ApplicationSettings)ObjectCopier.Clone(settings);


            InitializeComponent();

            PresetTheUiWithCurrentSettinsg();

            // Theme Selector
            // cbThemeSelection
            // Set the itemsource to the list of theme names.
            cbThemeSelection.ItemsSource = themeNameList; // manager.ThemeNameList;
           

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
        }


        #region Private local Methods

        /// <summary>
        /// Helping function to preset the UI with the current Settings
        /// </summary>
        private void PresetTheUiWithCurrentSettinsg()
        {
            // Set the Current Settings to the UI
            tbMsgLogBufferSize.Text = applicationSettings.msgLogRingBufferSize.ToString();
            tbSendHistoryBufferSize.Text = applicationSettings.sendHistorySize.ToString();

            cbDebugModeState.IsChecked = applicationSettings.debugModeIsActive;
            cbExpertModeState.IsChecked = applicationSettings.expertModeIsActive;

            cbThemeSelection.SelectedItem = applicationSettings.uiTheme;

            cbSpecialCharBold.IsChecked = applicationSettings.uiSpecialCharSetBold;
            cbSpecialCharItalic.IsChecked = applicationSettings.uiSpecialCharSetItalic;
            cbSpecialCharChangeColor.IsChecked = applicationSettings.uiSpecialCharSetColorChange;
        }

        /// <summary>
        /// Get the Data From the UI Elements
        /// </summary>
        private void getDataFromUi()
        {

            int value = 0;

            int.TryParse(tbMsgLogBufferSize.Text, out value);
            applicationSettings.msgLogRingBufferSize = value;

            int.TryParse(tbSendHistoryBufferSize.Text, out value);
            applicationSettings.sendHistorySize = value;

            applicationSettings.debugModeIsActive = (bool)cbDebugModeState.IsChecked;
            applicationSettings.expertModeIsActive = (bool)cbExpertModeState.IsChecked;

            applicationSettings.uiTheme = cbThemeSelection.SelectedValue.ToString();

        }

        /// <summary>
        /// Check the Param State of the Pages
        /// </summary>
        /// <returns>Returns True if all Data inserted are valid. False if their is an Value that is not Valid</returns>
        private bool checkPlauseability()
        {

            bool dataIsPlausible = true;


            if (!(CheckIntInput(tbSendHistoryBufferSize.Text, sendHistoryLowerLimit, sendHistoryUpperLimit)))
            {
                tbSendHistoryBufferSize.BorderBrush = Brushes.Red;
                dataIsPlausible = false;
            }
            else
            {
                tbSendHistoryBufferSize.BorderBrush = (SolidColorBrush)TryFindResource("TextBoxBorderBrush"); // Brushes.Black;
                applicationSettings.sendHistorySize = int.Parse(tbSendHistoryBufferSize.Text);
            }


            if (!(CheckIntInput(tbMsgLogBufferSize.Text, msgLogLowerLimit, msgLogUpperLimit)))
            {
                tbMsgLogBufferSize.BorderBrush = Brushes.Red;
                dataIsPlausible = false;
            }
            else
            {
                tbMsgLogBufferSize.BorderBrush = (SolidColorBrush)TryFindResource("TextBoxBorderBrush"); //Brushes.Black;
                applicationSettings.msgLogRingBufferSize = int.Parse(tbMsgLogBufferSize.Text);
            }


            // Check if the Input has changed
            if (dataIsPlausible)
            {
                CheckIfSettingsHaveChanged();
            }

            return dataIsPlausible;

        }

        /// <summary>
        ///  Returns True if the Port Select is Plausible and Fals if not
        /// </summary>
        /// <returns></returns>
        private bool CheckIntInput(string value, int lowerLimit, int upperLimit)
        {
            bool dataValid = true;
            int valueAsInt;

            bool isNumeric = int.TryParse(value, out valueAsInt);

            if (!(valueAsInt >= lowerLimit) && (valueAsInt <= upperLimit))
            {
                dataValid = false;
            }


            return dataValid;

        }

        /// <summary>
        /// Check if the Application has do do a restart after the Settings have been used..
        /// </summary>
        private bool CheckIfSettingsHaveChanged()
        {

            bool valueHaveChanged = false;

            if (!(oldApplicationSettings.sendHistorySize == applicationSettings.sendHistorySize)
                || !(oldApplicationSettings.msgLogRingBufferSize == applicationSettings.msgLogRingBufferSize)
                || !(oldApplicationSettings.debugModeIsActive == applicationSettings.debugModeIsActive))
            {
                valueHaveChanged = true;
            }
            else
            {
                valueHaveChanged = false;
            }


            return valueHaveChanged;
        }


        /// <summary>
        /// Use the entered User input
        /// </summary>
        private void useUserInput()
        {
            // To be Done Chek the Plauseability....
            if (checkPlauseability())
            {

                getDataFromUi();

                if (CheckIfSettingsHaveChanged())
                {
                    // Configure the message box to be displayed
                    string caption = "Übernehmen der Einstellungen";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Information;

                    // Display message box
                    MessageBox.Show("Die Einstellungsänderungen werden erst nach einem Neustart der Anwendung übernommen", caption, button, icon);

                }


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

        #region User Input

        #region Input Changed

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

            checkPlauseability();

        }

        /// <summary>
        /// Checked Satate changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbDebugModeState_Click(object sender, RoutedEventArgs e)
        {
            CheckBox _checkBox = sender as CheckBox;
            applicationSettings.debugModeIsActive = (bool)_checkBox.IsChecked;
            checkPlauseability();
        }

        /// <summary>
        /// Expert Mode Checked On/Off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbExpertMode_Click(object sender, RoutedEventArgs e)
        {
            CheckBox _checkBox = sender as CheckBox;
            applicationSettings.expertModeIsActive = (bool)_checkBox.IsChecked;
            checkPlauseability();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbThemeSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When they change the selection, just set the theme to the selected value.
            applicationSettings.uiTheme = cbThemeSelection.SelectedValue.ToString();
            checkPlauseability();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSpecialCharItalic_Click(object sender, RoutedEventArgs e)
        {
            CheckBox _checkBox = sender as CheckBox;
            applicationSettings.uiSpecialCharSetItalic = (bool)_checkBox.IsChecked;
            checkPlauseability();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSpecialCharBold_Click(object sender, RoutedEventArgs e)
        {
            CheckBox _checkBox = sender as CheckBox;
            applicationSettings.uiSpecialCharSetBold = (bool)_checkBox.IsChecked;
            checkPlauseability();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSpecialCharChangeColor_Click(object sender, RoutedEventArgs e)
        {
            CheckBox _checkBox = sender as CheckBox;
            applicationSettings.uiSpecialCharSetColorChange = (bool)_checkBox.IsChecked;
            checkPlauseability();
        }

        #endregion

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

        #region Public Methods

        #endregion

    }
}
