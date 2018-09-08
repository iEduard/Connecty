using System;
using System.Net;
using System.Windows.Controls;
using System.Windows.Media;

namespace Connecty
{

    // A delegate type for hooking up change notifications.
    public delegate void TcpIpSettingsChangedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Interaktionslogik für Page2.xaml
    /// </summary>
    public partial class settingsPageTcpIp : Page
    {

        #region Local Variables
        private SingleConnectionSettings userSettings = new SingleConnectionSettings();
        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings"></param>
        public settingsPageTcpIp(SingleConnectionSettings settings)
        {
            userSettings = settings;
            InitializeComponent();
            setUiTcpIpSettings();

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings"></param>
        public settingsPageTcpIp()
        {
            InitializeComponent();
            setUiTcpIpSettings();

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
            userSettings.tcpSettings.clientServerSelection = cbClientServerSelect.SelectedItem.ToString();

            if (IsDataValid())
            {
                userSettings.tcpSettings.port = Convert.ToInt32(tbTcpIpPort.Text);
            }
            else
            {
                userSettings.tcpSettings.port = 0;
            }

            userSettings.tcpSettings.restartTcpServer = (bool)cbTcpAutomaticReconnect.IsChecked;

            // Only Store the IP Address if the Client is aktive
            if (cbClientServerSelect.SelectedItem.ToString().Equals("Client"))
            {
                if (IsDataValid())
                {
                    userSettings.tcpSettings.ip = tbTcpIpAddress.Text;
                }
                else
                {
                    userSettings.tcpSettings.ip = "";
                }

            }

            // Return the User Settings
            return userSettings;
        }

        /// <summary>
        /// Returns True if the Data is Valid and Fals if the Data is Invalid
        /// </summary>
        /// <returns></returns>
        public bool IsDataValid()
        {

            bool dataIsValid = true;
            if (!(CheckPortInput() && CheckIpInput()))
            {
                dataIsValid = false;
            }
            return dataIsValid;

        }

        #endregion

        #region Set UI wit current Configuration

        /// <summary>
        /// Set the ComboBox Values and set the Chosen Index From the Current Settings
        /// </summary>
        /// <param name="param"></param>
        private void setUiTcpIpSettings()
        {
            // Init the Comboboxes

            // Set the Combox Params for the ClientServer Selection
            cbClientServerSelect.ItemsSource = new string[] { "Client", "Server" };
            cbClientServerSelect.SelectedIndex = cbClientServerSelect.Items.IndexOf(userSettings.tcpSettings.clientServerSelection);

            cbTcpAutomaticReconnect.IsChecked = userSettings.tcpSettings.restartTcpServer;

            // Set the IP Address TextBox
            setIpTextBoxData();

            // Set the Port TextBox
            tbTcpIpPort.Text = userSettings.tcpSettings.port.ToString();

        }

        /// <summary>
        /// Update the IP TextBox
        /// </summary>
        private void setIpTextBoxData()
        {
            if (cbClientServerSelect.SelectedItem.ToString().Equals("Server"))
            {
                tbTcpIpAddress.Text = "Alle verfügbaren";
                tbTcpIpAddress.IsEnabled = false;
            }
            else
            {
                tbTcpIpAddress.Text = userSettings.tcpSettings.ip;
                tbTcpIpAddress.IsEnabled = true;
            }
        }

        /// <summary>
        /// Function to disable the ui elements
        /// </summary>
        public void DisableEditFunctions()
        {

            // Disable all the Editable UI Elements
            cbClientServerSelect.IsEnabled = false;
            tbTcpIpPort.IsEnabled = false;
            tbTcpIpAddress.IsEnabled = false;
            cbTcpAutomaticReconnect.IsEnabled = false;

        }

        #endregion

        #region Check User Input on Input Change Events

        /// <summary>
        /// Client Server Selection Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbClientServerSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update the UI
            setIpTextBoxData();

            // Set the Event that the User Changed an Input
            FireUpOnChangedEvent();
        }

        /// <summary>
        /// IP or Port Input was Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void userInputTextChanged(object sender, TextChangedEventArgs e)
        {
            // Get the object that calls the Function
            TextBox userInputBox = sender as TextBox;
            Brush wrongInputBackground = Brushes.Red;

            // Check the IP Address
            if (userInputBox.Name == "tbTcpIpAddress" && cbClientServerSelect.SelectedItem.ToString().Equals("Client"))
            {
                if (CheckIpInput())
                {
                    userInputBox.Background = Brushes.White;
                }
                else
                {
                    userInputBox.Background = wrongInputBackground;
                }

            }
            else if (userInputBox.Name == "tbTcpIpPort")
            {
                if (CheckPortInput())
                {
                    userInputBox.Background = Brushes.White;
                }
                else
                {
                    userInputBox.Background = wrongInputBackground;
                }

            }

            // Set the Event for the User Input Changed Evend
            FireUpOnChangedEvent();

        }

        /// <summary>
        /// Check the User Input for the Port Data
        /// </summary>
        /// <returns></returns>
        private bool CheckPortInput()
        {

            bool dataIsValid = true;
            int portValue =0;


            // Check the Port

            bool isNumeric = int.TryParse(tbTcpIpPort.Text, out portValue);


            if (!((portValue <= 65534) && (portValue > 0)))
            {
                dataIsValid = false;
            }

            return dataIsValid;
        }

        /// <summary>
        /// Check the User Input for the IP Data
        /// </summary>
        /// <returns></returns>
        private bool CheckIpInput()
        {
            bool dataIsValid = true;
            IPAddress myAddress;

            // Only check the IP if we selected the Client
            if (cbClientServerSelect.SelectedItem.ToString().Equals("Client"))
            {
                // Check the IP Address
                if ((tbTcpIpAddress.Text.Split('.').Length) == 4)
                {
                    if(IPAddress.TryParse(tbTcpIpAddress.Text, out myAddress))
                    {
                        dataIsValid = true;
                    }
                    else
                    {
                        dataIsValid = false;
                    }
                }
                else
                {
                    dataIsValid = false;
                }
            }

            return dataIsValid;
        }

        #endregion

        #region On Change Eventhandler

        /// <summary>
        /// Private Function to Start the Event only if the Data Is OK
        /// </summary>
        private void FireUpOnChangedEvent()
        {
            if (CheckPortInput() && CheckIpInput())
            {
                // Set the Event that the User Changed an Input
                OnChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// An event that clients can use to be notified whenever the
        /// elements of the list change. 
        /// </summary>
        public event TcpIpSettingsChangedEventHandler Changed;

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
