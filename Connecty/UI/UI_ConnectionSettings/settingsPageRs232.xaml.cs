using System;
using System.IO.Ports;
using System.Windows.Controls;
using System.Windows.Media;


namespace Connecty
{

    // A delegate type for hooking up change notifications.
    public delegate void Rs232SettingsChangedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Interaktionslogik für Page3.xaml
    /// </summary>
    public partial class settingsPageRs232 : Page
    {

        #region Local Variables
        private SingleConnectionSettings userSettings = new SingleConnectionSettings();
        private bool expertModeIsEnabled = false;
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings"></param>
        public settingsPageRs232(SingleConnectionSettings settings, bool enableExpertMode)
        {
            userSettings = settings;
            expertModeIsEnabled = enableExpertMode;
            InitializeComponent();
            setUiRs232Settings();


            // Check the State of the PortSelection
            if (!CheckPortSelection())
            {
                cbPort.Background = Brushes.Red;
            }
            
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings"></param>
        public settingsPageRs232()
        {
            InitializeComponent();
            setUiRs232Settings();
        }

        #endregion

        #region Public Methods for the Control 

        /// <summary>
        /// Funtion to return the User entered Settings
        /// </summary>
        public SingleConnectionSettings getUserParams()
        {
            /*
            For the Comboboxes we have to use SelectedItem otherwise we will discover an old input within the on change event
            SelectionBoxItem is for now on Deprecated in this Project            
            */
            if (CheckPortSelection())
            {
                userSettings.serialSettings.port = cbPort.SelectedItem.ToString();
            }
            else
            {
                userSettings.serialSettings.port = "";
            }

            userSettings.serialSettings.baud = Convert.ToInt32(cbBaudrate.SelectedItem.ToString());
            userSettings.serialSettings.setParityWithString(cbParity.SelectedItem.ToString());

            if(CheckDataBitsInput())
            {
                userSettings.serialSettings.dataBits = Convert.ToInt32(tbDataBits.Text);
            }
            else
            {
                userSettings.serialSettings.dataBits = 0;
            }

            userSettings.serialSettings.setStopBitsWithString(cbStoppbits.SelectedItem.ToString());

            // Return the User Settings
            return userSettings;
        }

        /// <summary>
        /// Function to disable the ui elements
        /// </summary>
        public void DisableEditFunctions()
        {
            // Disable all the Editable UI Elements
            cbPort.IsEnabled = false;
            cbBaudrate.IsEnabled = false;
            cbParity.IsEnabled = false;
            tbDataBits.IsEnabled = false;
            cbStoppbits.IsEnabled = false;
        }

        /// <summary>
        /// Returns True if the Data is Valid and Fals if the Data is Invalid
        /// </summary>
        /// <returns></returns>
        public bool IsDataValid()
        {

            bool dataIsValid = true;
            if (!(CheckPortSelection() && CheckDataBitsInput()))
            {
                dataIsValid = false;
            }
            return dataIsValid;

        }

        #endregion

        #region Private Methods for Internal Controls
        /// <summary>
        /// Set the ComboBox Values and set the Chosen Index From the Current Settings
        /// </summary>
        /// <param name="param"></param>
        private void setUiRs232Settings()
        {
            // Init the Comboboxes

            // Set the Combobox Params for the Com Port
            cbPort.ItemsSource = SerialPort.GetPortNames();//Get the available ComPorts on the System an drop them to the Combobox

            if (cbPort.Items.IndexOf(userSettings.serialSettings.port) > -1)
            {
                cbPort.SelectedIndex = cbPort.Items.IndexOf(userSettings.serialSettings.port);// Set the ComPort to the Selected Port
            }
            else
            {
                cbPort.SelectedIndex = -1;// Set the Comport to the First Available Port
            }

            // Set the Combox Params for the Baud Rate
            cbBaudrate.ItemsSource = new string[] { "2400", "4800", "9600", "19200", "57600", "115200" };
            cbBaudrate.SelectedIndex = cbBaudrate.Items.IndexOf(userSettings.serialSettings.baud.ToString());

            // Set the Parity Combobox
            cbParity.ItemsSource = new string[] { "Ohne", "Gerade", "Ungerade", "Fest 0", "Fest 1" };
            cbParity.SelectedIndex = cbParity.Items.IndexOf(userSettings.serialSettings.getParityAsString());

            // Set the DataBits TextBox
            tbDataBits.Text = userSettings.serialSettings.dataBits.ToString();

            // Set the StopBits Combobox
            cbStoppbits.ItemsSource = new string[] { "Ohne", "1", "1,5", "2" };
            cbStoppbits.SelectedIndex = cbStoppbits.Items.IndexOf(userSettings.serialSettings.getStopBitsAsString());

            if (expertModeIsEnabled && false)
            {
                expertUI.Visibility = System.Windows.Visibility.Visible;
                tbReciveTimeOut.Text = userSettings.serialSettings.readTimeOut.ToString();
            }
            else
            {
                expertUI.Visibility = System.Windows.Visibility.Collapsed;
            }


        }

        #endregion

        #region Check User Input

        /// <summary>
        /// User entered an input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbDataBits_TextChanged(object sender, TextChangedEventArgs e)
        {

            Brush wrongInputBackground = Brushes.Red;
            TextBox currentTextBox = sender as TextBox;


            if (CheckDataBitsInput())
            {
                currentTextBox.Background = (SolidColorBrush)TryFindResource("TextBoxBackground"); //Brushes.White;
            }
            else
            {
                currentTextBox.Background = wrongInputBackground;
            }


        }

        /// <summary>
        ///  Returns True if the Port Select is Plausible and Fals if not
        /// </summary>
        /// <returns></returns>
        private bool CheckDataBitsInput()
        {
            bool dataValid = true;
            int dataBitsValue;

            bool isNumeric = int.TryParse(tbDataBits.Text, out dataBitsValue);

            if (!(dataBitsValue > 0) && (dataBitsValue <= 255))
            {
                dataValid = false;
            }


            return dataValid;

        }

        /// <summary>
        /// After Changing the Selected Port check the Data and set the Background
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboboxSender = sender as ComboBox;

            if (CheckPortSelection())
            {
                comboboxSender.Background = (SolidColorBrush)TryFindResource("TextBoxBackground"); //Brushes.White;
            }
            else
            {
                comboboxSender.Background = Brushes.Red;
            }

            // Set the Event that the User Changed an Input
            OnChanged(EventArgs.Empty);
        }

        /// <summary>
        ///  Returns True if the Port Select is Plausible and Fals if not
        /// </summary>
        /// <returns></returns>
        private bool CheckPortSelection()
        {
            bool dataValid = true;

            if (cbPort.SelectedItem != null)
            {
                if (cbPort.SelectedItem.ToString() == "")
                {
                    dataValid = false;
                }
            }
            else
            {
                dataValid = false;
            }

            return dataValid;
        }

        /// <summary>
        /// Feature for the Future
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbReciveTimeOut_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        #endregion

        #region On Change Eventhandler


        /// <summary>
        /// Starts the On Change Event if the User input of this Connection is OK
        /// </summary>
        private void FireUpOnChangeEvent()
        {

            if (CheckDataBitsInput() && CheckPortSelection())
            {
                // Only Call the OnChange Event when the Input Is OK
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event that clients can use to be notified whenever the
        /// elements of the list change.  
        /// </summary>
        public event Rs232SettingsChangedEventHandler Changed;

        /// <summary>
        /// Invoke the Changed event; called whenever list changes 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }


        #endregion



    }
}
