using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Net;

namespace Connecty
{
    /// <summary>
    /// Interaktionslogik für Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private SingleConnection userSettings = new SingleConnection();
        private string errorMessage;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="param"></param>
        public Settings(SingleConnection param)
        {
            InitializeComponent();

            userSettings = param;

            // Set the UI to the Current Settings
            updateUiWithCurrentSettings();
        }

        /// <summary>
        /// Preset the UI with the current Settings
        /// </summary>
        private void updateUiWithCurrentSettings()
        {

            // Set the Combox Params for the ClientServer Selection
            cbFunctionSelection.ItemsSource = new string[] { "Single RS232", "Single TCP", "Multi" };
            cbClientServerSelect.ItemsSource = new string[] { "Client", "Server" };


            System.Windows.Controls.ItemCollection selectedItem = tcTabControl.Items;

            foreach (System.Windows.Controls.TabItem item in tcTabControl.Items)
            {
                if (item.Header.Equals("TCP IP") && userSettings.currentConnectionSetting == 1)
                {
                    tcTabControl.SelectedItem = item;
                }
                else if (item.Header.Equals("RS232") && userSettings.currentConnectionSetting == 2)
                {
                    tcTabControl.SelectedItem = item;
                }
            }

            setUiRs232Settings();
            setUiTcpIpSettings();
        }

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

        }

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
        /// Accept the User entered Settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bAccept_Click(object sender, RoutedEventArgs e)
        {
            //
            if (checkPlausebility())
            {
                System.Windows.Controls.TabItem selectedItem = (System.Windows.Controls.TabItem)tcTabControl.SelectedItem;

                if (selectedItem.Header.Equals("TCP IP"))
                {
                    userSettings.currentConnectionSetting = 1;
                }
                else if (selectedItem.Header.Equals("RS232"))
                {
                    userSettings.currentConnectionSetting = 2;
                }

                // ComboBoxItem typeItem = (ComboBoxItem)cbPort.SelectedItem;
                userSettings.serialSettings.port = cbPort.SelectionBoxItem.ToString();
                userSettings.serialSettings.baud = Convert.ToInt32(cbBaudrate.SelectionBoxItem.ToString());
                userSettings.serialSettings.setParityWithString(cbParity.SelectionBoxItem.ToString());
                userSettings.serialSettings.dataBits = Convert.ToInt32(tbDataBits.Text);
                userSettings.serialSettings.setStopBitsWithString(cbStoppbits.SelectionBoxItem.ToString());

                userSettings.tcpSettings.clientServerSelection = cbClientServerSelect.SelectionBoxItem.ToString();
                userSettings.tcpSettings.port = Convert.ToInt32(tbTcpIpPort.Text);
                userSettings.tcpSettings.restartTcpServer = (bool)cbTcpAutomaticReconnect.IsChecked;

                // Only Store the IP Address if the Client is aktive
                if (cbClientServerSelect.SelectedItem.ToString().Equals("Client"))
                {
                    userSettings.tcpSettings.ip = tbTcpIpAddress.Text;
                }
                

                this.DialogResult = true;
                this.Close();
            }
            else
            {
                // Configure the message box to be displayed
                string messageBoxText = errorMessage;
                string caption = "Ungültige Einstellungen";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;

                // Display message box
                MessageBox.Show(messageBoxText, caption, button, icon);
            }


        }

        /// <summary>
        /// Abbort the Settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
        
        /// <summary>
        /// Funtion to return the User entered Settings
        /// </summary>
        public SingleConnection getUserParams
        {
            // Return the User Settings
            get { return userSettings; }
        }

        /// <summary>
        /// Evemthandler Client Server Selection Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbClientServerSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Update the Textbox for the IP Address
            setIpTextBoxData();
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
        /// Check the Settings if they are valid
        /// </summary>
        /// <returns></returns>
        private bool checkPlausebility()
        {
            // Define and preset the Boolean Flag for the Valid Check
            bool settingsAreValid = true;

            // Check the Amount of DataBits
            if(Convert.ToInt32(tbDataBits.Text) >10 || Convert.ToInt32(tbDataBits.Text) <5)
            {
                errorMessage = "Ungültige anzahl an Datenbits für die Serielle Kommunikation";
                settingsAreValid = false;
            }


            // Check the IP if we have a Client Connection
            if (cbClientServerSelect.SelectedItem.ToString().Equals("Client"))
            {
                try
                {
                    IPAddress.Parse(tbTcpIpAddress.Text);
                }
                catch (Exception)
                {
                    errorMessage = "Ungültige IP Adresse. Bitte eine IP Adresse im Format X.X.X.X [X = 0..255] eintragen";
                    settingsAreValid = false;
                }
            }

            return settingsAreValid;
        }

        private void cbFunctionSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbConnection1Selection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbConnection2Selection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
