using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;

namespace Connecty
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {




        #region Local Variables
        
        // Create the object of the Settings
        private ConnectySetings settings;


        private FunctionInterface connection;

        private MsgLogHandler msgLog;// the Message Log Data 
        private MsgData currentSendData = new MsgData(); // This is a helper for the connversion of the Text in the SendBox to the new requested View Settings
        private int connectionSelectionSendData = 1; // Selection for the Connection where we want to send the Data to

        // Heare are some Routed Commands for the Hot Keys. Yes they are realy hot
        public static RoutedCommand connectRoutedCommand = new RoutedCommand();
        public static RoutedCommand disconnectRoutedCommand = new RoutedCommand();
        public static RoutedCommand changeViewUpRoutedCommand = new RoutedCommand();
        public static RoutedCommand changeViewDownRoutedCommand = new RoutedCommand();
        public static RoutedCommand settingsRoutedCommand = new RoutedCommand();
        public static RoutedCommand resetMsgLogRoutedCommand = new RoutedCommand();
        public static RoutedCommand openNewSimulationWindowRoutedCommand = new RoutedCommand();
        public static RoutedCommand openApplicationSettngsRoutedCommand = new RoutedCommand();
        public static RoutedCommand functionKeyForQuickbuttonsRoutedCommand = new RoutedCommand();
        public static RoutedCommand increaseZoomFactorRoutedCommand = new RoutedCommand();
        public static RoutedCommand decreaseZoomFactorRoutedCommand = new RoutedCommand();

        // Simulation Data
        List<Simulation_UI> simUi = new List<Simulation_UI>();
        LinkedList<MenuItem> simMenuBarItems = new LinkedList<MenuItem>();

        private bool disposed = false; // to detect redundant calls

        // Theme Handling
        // Create an instance of the ThemeManager.
        // NOTE: In a real world solution, I would make this a Singleton class.  You will never need more than one instance of it.
        ThemeManager themeManager = new ThemeManager();

        /// <summary>
        /// Update Handling
        /// </summary>
        UpdateHandler updateHandler = new UpdateHandler();

        #endregion

        /// <summary>
        /// Main Window Constructor
        /// </summary>
        public MainWindow()
        {

            // Load the Default Settings
            settings = LoadSave.loadSettings(System.AppDomain.CurrentDomain.BaseDirectory);

            // Create a new Connection Interface Object
            connection = new FunctionInterface();
            connection.StatusbarUpdate += new ConnectionStateUpdateEventHandler(UpdateConnectionState_Event);
            connection.SendRecived += new MsgSendRecivedEventHandler(MsgSendRecived_Event);



            msgLog = new MsgLogHandler(settings.applicationSettings.msgLogRingBufferSize, settings.applicationSettings.sendHistorySize);
            msgLog.viewSettings = settings.viewSettings;


            // Init the UI
            InitializeComponent();

            // Add some Hot Keys and bind the Eventhandlers to the UI
            AddHotKeys();

            // Init the UI
            tbSendData.Text = "";// Erase the Textbox Send Data
            updateMainWindowTitle();// Update the Main Window Title


            // Try to set the Position , the Height and the Width
            this.Left = settings.applicationSettings.position.X;
            this.Top = settings.applicationSettings.position.Y;
            this.Width = settings.applicationSettings.width;
            this.Height = settings.applicationSettings.height;

            // Set the Scaling Factor of the MSG Log
            sScaleSlider.Value = settings.applicationSettings.msgLogZoomFactor;
            lScaleValue.Content = (settings.applicationSettings.msgLogZoomFactor * 100).ToString() + "%";
            
            // Update the UI with the Current View Settings
            UpdateViewStateDependence(msgLog.viewSettings);

            // Update the Statusbar
            update_StatusBarSendTo(connectionSelectionSendData);

            // Load the Last QuickButtons
            QuickButtonsControl.LoadQuickButtons(settings.applicationSettings.lastQuickButtonsPath);

            // Debug Stuff
            debugStuff();

            // When they change the selection, just set the theme to the selected value.
            themeManager.SetTheme(settings.applicationSettings.uiTheme);

            // Set the Focus to the Send Textbox
            tbSendData.Focus();

        }


        #region Taskbar Interface

        /// <summary>
        /// Taskbar Button was Pressed Connect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbConnect_Click(object sender, EventArgs e)
        {
            ConnectionInterface_Connect();
        }

        /// <summary>
        /// Taskbar Button was Pressed Siconnect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbDisconnect_Click(object sender, EventArgs e)
        {
            ConnectionInterface_Disconnect();
        }


        #endregion


        #region Menu Items

        /// <summary>
        /// Menu Item About
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            // Open the About Dialog
            InfoDialog AboutDialog = new InfoDialog(this.PointToScreen(new Point(0, 0)));
            AboutDialog.ShowDialog();

        }

        /// <summary>
        /// View Binary Values of the In and Outgoing Data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_ViewRepresentationChange_Click(object sender, RoutedEventArgs e)
        {

            MenuItem menuItemSender = sender as MenuItem;

            // Save the current SendBox Data with the Current View Settings
            try
            {
                currentSendData = msgLog.getRawMsgWithCurrentViewSettings(tbSendData.Text);
            }
            catch
            {
                currentSendData = msgLog.getRawMsgWithCurrentViewSettings("");
            }

            // Set the Current View Configuration
            try
            {
                settings.viewSettings.dataPresentation = (Int32)menuItemSender.Resources["ViewRepresentation"];
            }
            catch
            {
                settings.viewSettings.dataPresentation = 0;
            }


            msgLog.viewSettings = settings.viewSettings;

            // Update the StatusBar
            UpdateViewStateDependence(msgLog.viewSettings);
        }

        /// <summary>
        /// View the MsgLog with the TimeStamp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_TimeStamp_Click(object sender, RoutedEventArgs e)
        {

            if (settings.viewSettings.showTimeStamp)
            {
                settings.viewSettings.showTimeStamp = false;
                menuItem_TimeStamp.IsChecked = false;
            }
            else
            {
                settings.viewSettings.showTimeStamp = true;
                menuItem_TimeStamp.IsChecked = true;
            }


            // Update the Msg Log Settings
            msgLog.viewSettings = settings.viewSettings;


            // Update the Msg Log
            reloadRtb();
        }


        /// <summary>
        /// Call the Updatewindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemCheckForUpdates_Click(object sender, RoutedEventArgs e)
        {
            UpdateCheckWindow _updateWindow = new UpdateCheckWindow(this.PointToScreen(new Point(0, 0)));
            _updateWindow.Left = this.Left;
            _updateWindow.Top = this.Top;
            _updateWindow.Show();
        }


        /// <summary>
        /// Zoom Slider Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sScaleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                lScaleValue.Content = (e.NewValue * 100).ToString() + "%";
            }
            catch
            {
                //throw new Exception();
            }

        }


        /// <summary>
        /// Log Fenster Speichern
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemSaveLog_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog safeDialog = new SaveFileDialog();
            safeDialog.Title = "Connecty Log Speichenr";
            safeDialog.DefaultExt = ".txt";
            safeDialog.AddExtension = true;
            safeDialog.Filter = "Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*";

            if (safeDialog.ShowDialog() == true)
            {
                using (StreamWriter sr = new StreamWriter(safeDialog.FileName))
                {
                    TextRange _textRange = new TextRange(rtbInOutData.Document.ContentStart, rtbInOutData.Document.ContentEnd);

                    sr.Write(_textRange.Text);


                }
            }
        }

        #endregion


        #region MSG Log Update Functions


        /// <summary>
        /// Update the Rich Text Box from the UI Thread
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Color"></param>
        private void UpdateRTB(MsgData msg)
        {
            // Define some Helper Variables
            var sb = new StringBuilder();
            TextRange range = new TextRange(rtbInOutData.Document.ContentEnd, rtbInOutData.Document.ContentEnd);


            // Return the Msg with the Current View Configuration
            switch (msg.type)
            {

                #region Recived Data

                case MsgData.messageType.recived:// MSG Type Recived
                    if ( msgLog.viewSettings.showTimeStamp )
                    {
                        range.Text = "\n";
                        range.Text += msg.timeStamp.ToString("dd.MM.yyyy HH:mm:ss.fff") + ": ";


                        if (settings.connectionSettings.functionSelect == 1)
                        {

                            if (msg.connectionNumber == 1)
                            {
                                range.Text += " ";
                                range.Text += connection.connectionSettings.connection1.connectionName;
                                range.Text += " => ";
                                range.Text += connection.connectionSettings.connection2.connectionName;
                                range.Text += " :";
                            }
                            else
                            {
                                range.Text += " ";
                                range.Text += connection.connectionSettings.connection2.connectionName;
                                range.Text += " => ";
                                range.Text += connection.connectionSettings.connection1.connectionName;
                                range.Text += " :";
                            }

                        }
                        else
                        {
                            range.Text += " In: ";
                        }

                        range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(settings.viewSettings.receiveColor));
                        range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                        range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                        WriteMsgDataToRtb(msg, new SolidColorBrush(settings.viewSettings.receiveColor));
                    }
                    else
                    {
                        range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(settings.viewSettings.receiveColor));
                        range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                        range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                        WriteMsgDataToRtb(msg, new SolidColorBrush(settings.viewSettings.receiveColor));
                    }

                    break;

                #endregion

                #region Send Data
                case MsgData.messageType.send:// MSG Type Sended

                    if (msgLog.viewSettings.showTimeStamp)
                    {
                        range.Text = "\n";
                        range.Text += msg.timeStamp.ToString("dd.MM.yyyy HH:mm:ss.fff") + ": ";



                        if (settings.connectionSettings.functionSelect == 1)
                        {

                            if (msg.connectionNumber == 1)
                            {
                                range.Text += " ";
                                range.Text += "To";
                                range.Text += " => ";
                                range.Text += connection.connectionSettings.connection1.connectionName;
                                range.Text += " :";
                            }
                            else
                            {
                                range.Text += " ";
                                range.Text += "To";
                                range.Text += " => ";
                                range.Text += connection.connectionSettings.connection2.connectionName;
                                range.Text += " :";
                            }

                        }
                        else
                        {
                            range.Text += " Out: ";
                        }

                        range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(settings.viewSettings.sendColor));
                        range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                        range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                        WriteMsgDataToRtb(msg, new SolidColorBrush(settings.viewSettings.sendColor));
                    }
                    else
                    {
                        range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(settings.viewSettings.sendColor));
                        range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                        range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                        WriteMsgDataToRtb(msg, new SolidColorBrush(settings.viewSettings.sendColor));
                    }

                    break;

                #endregion

                #region Info Positive

                case MsgData.messageType.infoPositive:// MSG Type Info Positive

                    sb.Append("\n-----------------------------------------------------------\n");
                    sb.Append(msgLog.getMsgWithCurrentViewSettings(msg));
                    sb.Append("\n-----------------------------------------------------------\n ");
                    range.Text = sb.ToString();
                    range.ApplyPropertyValue(TextElement.ForegroundProperty, System.Windows.Media.Brushes.Green);
                    range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                    range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                    break;

                #endregion

                #region Info Negativ

                case MsgData.messageType.infoNegative:// MSG Type Info Negative

                    sb.Append("\n-----------------------------------------------------------\n");
                    sb.Append(msgLog.getMsgWithCurrentViewSettings(msg));
                    sb.Append("\n-----------------------------------------------------------\n ");
                    range.Text = sb.ToString();

                    range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
                    range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                    range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                    break;

                #endregion

                // Default View
                default:
                    range.Text = msgLog.getMsgWithCurrentViewSettings(msg);
                    range.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Black);
                    range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                    range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                    break;
            }

            rtbInOutData.ScrollToEnd();// Scroll to the end of teh RichTextBox
            svInOutData.ScrollToEnd();// Scroll to the end of the ScrollView
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="asciiCharColor"></param>
        /// <param name="specialASciiChar"></param>
        private void WriteMsgDataToRtb(MsgData msg, SolidColorBrush asciiCharColor)
        {
            // Create some local Variables
            TextRange range;
            List<MsgHighlightStruct> _msgList = msgLog.getMsgHighlightetListWithCurrentViewSettings(msg);

            // Create an Solid Color Brush for the Special ASCII Chars
            SolidColorBrush _specialAscciSolidColoBrush;

            // Set the Brightness for the Special ASCII Signs
            double _brightnessFactor;
            if (settings.applicationSettings.uiTheme == "dark")
            {
                _brightnessFactor = 1.4;
            }
            else
            {
                _brightnessFactor = 0.6;
            }


            // Get all the List entrys and 
            foreach (MsgHighlightStruct _data in _msgList)
            {
                range = new TextRange(rtbInOutData.Document.ContentEnd, rtbInOutData.Document.ContentEnd);

                switch (_data.msgStringType)
                {

                    //Standard ASCII Chars
                    case 0:
                        range.Text = _data.msgAsString;
                        range.ApplyPropertyValue(TextElement.ForegroundProperty, asciiCharColor);
                        range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                        range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Normal);
                        break;

                    // Special ASCII Chars
                    case 1:


                        range.Text = _data.msgAsString;

                        if (settings.applicationSettings.uiSpecialCharSetColorChange)
                        {
                            _specialAscciSolidColoBrush = new SolidColorBrush(Color.FromArgb(asciiCharColor.Color.A,
                                                                                 (byte)(asciiCharColor.Color.R * _brightnessFactor),
                                                                                 (byte)(asciiCharColor.Color.G * _brightnessFactor),
                                                                                 (byte)(asciiCharColor.Color.B * _brightnessFactor)));
                            range.ApplyPropertyValue(TextElement.ForegroundProperty, _specialAscciSolidColoBrush);
                        } 


                        if (settings.applicationSettings.uiSpecialCharSetBold)
                        {
                            range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold); ;
                        }

                        if (settings.applicationSettings.uiSpecialCharSetItalic)
                        {
                            range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
                        }
                        break;

                    // Non ASCII Chars
                    case 2:
                        range.Text = _data.msgAsString;

                        if (settings.applicationSettings.uiSpecialCharSetColorChange)
                        {
                            _specialAscciSolidColoBrush = new SolidColorBrush(Color.FromArgb(asciiCharColor.Color.A,
                                                                                 (byte)(asciiCharColor.Color.R * _brightnessFactor),
                                                                                 (byte)(asciiCharColor.Color.G * _brightnessFactor),
                                                                                 (byte)(asciiCharColor.Color.B * _brightnessFactor)));
                            range.ApplyPropertyValue(TextElement.ForegroundProperty, _specialAscciSolidColoBrush);
                        }

                        if (settings.applicationSettings.uiSpecialCharSetBold)
                        {
                            range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);                            ;
                        }

                        if (settings.applicationSettings.uiSpecialCharSetItalic)
                        {
                            range.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
                        }
                        break;

                }
            }
        }


        /// <summary>
        /// Update the Rich Text Box from an other Thread
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageColor"></param>
        public void MsgSendRecived_Event(object sender, MsgSendRecivedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Threading.DispatcherOperationCallback(delegate
            {
                // Store the Data to the msg Log Handler
                msgLog.Add(e.msgData);

                // Update the RichtextBox
                UpdateRTB(e.msgData);


                if (simUi.Count > 0)
                {


                    foreach (Simulation_UI ui in simUi)
                    {
                        ui.InputUpdate(e.msgData);
                    }

                }

                return null;
            }), null);
        }

        /// <summary>
        /// Reload the RichTextBox with the New View Settings
        /// </summary>
        private void reloadRtb()
        {
            // First clear the complete RichTextBox
            rtbInOutData.Document.Blocks.Clear();

            foreach (MsgData msg in msgLog.msgLog)
            {

                string msgValue = msgLog.getMsgWithCurrentViewSettings(msg);
                UpdateRTB(msg);

            }

            // Try to convert the Send Data to the new View Settings
            try
            {
                if (currentSendData.value != null)
                {
                    tbSendData.Text = msgLog.getMsgWithCurrentViewSettings(currentSendData);
                }
                else
                {
                    tbSendData.Text = "";
                }
            }
            catch 
            {
                // We are not able to change the View Corresponding to the new View Setting so we will erase the Input
                tbSendData.Text = "";
            }

        }

        #endregion

        /// <summary>
        /// TextBox "Send Data" Layout has changed 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSendData_LayoutUpdated(object sender, EventArgs e)
        {
            ScrollViewer sv = FindVisualChild<ScrollViewer>(tbSendData);

            if (sv != null)
            {
                if (sv.ComputedHorizontalScrollBarVisibility == System.Windows.Visibility.Visible)
                {
                    tbSendData.Height = 40;
                    bSend.Height = 40;
                }
                else
                {
                    tbSendData.Height = 25;
                    bSend.Height = 25;
                }
            }
        }

        /// <summary>
        /// Find the Visual Child in the current UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        /// <summary>
        /// Update the Main Window Title
        /// </summary>
        private void updateMainWindowTitle()
        {
            // Set the Title of the Window to show the Current selected Connection
            this.Title = settings.connectionSettings.getSettingsInfo();
        }


        #region Statusbar Update Functions

        /// <summary>
        /// Event for the Update of the Eventhandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void UpdateConnectionState_Event(object sender, ConnectionStateUpdateEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Threading.DispatcherOperationCallback(delegate
            {
                // Update the RichtextBox
                UpdateStatusBarConnectionState(e.connection1State, e.connection2State);

                return null;
            }), null);
        }

        /// <summary>
        /// Update the Rich Text Box from the UI Thread
        /// </summary>
        /// <param name="connectionState1"></param>
        /// <param name="connectionState2"></param>
        private void UpdateStatusBarConnectionState(int connectionState1, int connectionState2)
        {
            // Single Connection
            if (settings.connectionSettings.functionSelect == 0)
            {
                SetTextBlockView("", tbConnection1State, connectionState1);
                SetTextBlockView("", tbConnection2State, connectionState2);
            }// Connecty in the Middle
            else if (settings.connectionSettings.functionSelect == 1)
            {
                SetTextBlockView(connection.connectionSettings.connection1.connectionName + " ", tbConnection1State, connectionState1);
                SetTextBlockView(connection.connectionSettings.connection2.connectionName + " ", tbConnection2State, connectionState2);
            }

            // Also update the Send to State
            update_StatusBarSendTo(connectionSelectionSendData);
        }

        /// <summary>
        /// Change the View Of the Textblock
        /// </summary>
        /// <param name="additionalInfo"> Additional String to enter a Pre Info </param>
        /// <param name="textBlockObject"> Textblock object to change </param>
        /// <param name="connectionState"> Status of the Connection </param>
        private void SetTextBlockView(string additionalInfo, TextBlock textBlockObject, int connectionState)
        {

            // Return the Msg with the Current View Configuration
            switch (connectionState)
            {

                case -1:// Disabled
                    textBlockObject.Visibility = Visibility.Collapsed;
                    break;

                case 0:// Disconnected
                    textBlockObject.Visibility = Visibility.Visible;
                    textBlockObject.Text = additionalInfo + " Getrennt ";
                    textBlockObject.Background = Brushes.Red;
                    break;

                case 1:// Connecting
                    textBlockObject.Visibility = Visibility.Visible;
                    textBlockObject.Text = additionalInfo + " Verbindungsaufbau... ";
                    textBlockObject.Background = Brushes.DarkOrange; 
                    break;
                case 2:// Disconnecting
                    textBlockObject.Visibility = Visibility.Visible;
                    textBlockObject.Text = additionalInfo + " Verbindungsabbau... ";
                    textBlockObject.Background = Brushes.PaleVioletRed;
                    break;
                case 3:// Connected
                    textBlockObject.Visibility = Visibility.Visible;
                    textBlockObject.Text = additionalInfo + " Verbunden ";
                    textBlockObject.Background = Brushes.Green;
                    break;

                // Default View
                default:
                    textBlockObject.Visibility = Visibility.Visible;
                    textBlockObject.Text = additionalInfo + "...";
                    textBlockObject.Background = Brushes.DimGray;
                    break;
            }
        }

        /// <summary>
        /// on Click event of the Status Bar to change the Send to State
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSendTo_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            // Change the Selection of the the Data
            if (connectionSelectionSendData == 1)
            {
                connectionSelectionSendData = 2;
            }
            else
            {
                connectionSelectionSendData = 1;
            }

            update_StatusBarSendTo(connectionSelectionSendData);

        }

        /// <summary>
        /// Update the Statusbar with the to send Data
        /// </summary>
        /// <param name="selection"></param>
        private void update_StatusBarSendTo(int selection)
        {
            try
            {
                if (connection.connectionSettings == null)
                {
                    StatusBar_SendToTextBlock.Visibility = Visibility.Collapsed;
                    StatusBar_SendToSeperator.Visibility = Visibility.Collapsed;
                    return;
                }

                if (connection.connectionSettings.functionSelect == 1)
                {

                    StatusBar_SendToTextBlock.Visibility = Visibility.Visible;
                    StatusBar_SendToSeperator.Visibility = Visibility.Visible;

                    if (selection == 1)
                    {
                        StatusBar_SendToTextBlock.Text = "Senden an: " + connection.connectionSettings.connection1.connectionName;
                    }
                    else
                    {
                        StatusBar_SendToTextBlock.Text = "Senden an: " + connection.connectionSettings.connection2.connectionName;
                    }

                }
                else
                {
                    StatusBar_SendToTextBlock.Visibility = Visibility.Collapsed;
                    StatusBar_SendToSeperator.Visibility = Visibility.Collapsed;
                }

            }
            catch
            {
                StatusBar_SendToTextBlock.Visibility = Visibility.Collapsed;
                StatusBar_SendToSeperator.Visibility = Visibility.Collapsed;
            }

        }

        #endregion


        /// <summary>
        /// Update the StatusBar View State
        /// </summary>
        /// <param name="connectionState"></param>
        private void UpdateViewStateDependence(ViewSettings viewState)
        {
            // Update the View Status
            switch (viewState.dataPresentation)
            {

                case 0:// ASCII
                    tbViewState.Text = " ASCII ";

                    menuItem_BIN.IsChecked = false;
                    menuItem_DEC.IsChecked = false;
                    menuItem_HEX.IsChecked = false;
                    menuItem_ASCII_Zeichen.IsChecked = true;
                    break;

                case 1:// HEX
                    tbViewState.Text = " HEX ";

                    menuItem_BIN.IsChecked = false;
                    menuItem_DEC.IsChecked = false;
                    menuItem_ASCII_Zeichen.IsChecked = false;
                    menuItem_HEX.IsChecked = true;
                    break;

                case 2:// DEC
                    tbViewState.Text = " DEC ";

                    menuItem_BIN.IsChecked = false;
                    menuItem_HEX.IsChecked = false;
                    menuItem_ASCII_Zeichen.IsChecked = false;
                    menuItem_DEC.IsChecked = true;

                    break;

                case 3:// BIN
                    tbViewState.Text = " BIN ";

                    menuItem_HEX.IsChecked = false;
                    menuItem_ASCII_Zeichen.IsChecked = false;
                    menuItem_DEC.IsChecked = false;
                    menuItem_BIN.IsChecked = true;
                    break;

            }


            // Set the TimeStamo View
            if (viewState.showTimeStamp)
            {
                menuItem_TimeStamp.IsChecked = true;
            }
            else
            {
                menuItem_TimeStamp.IsChecked = false;
            }


            // Update the Toolbar Color for the Send and Receive View
            SendRecieColorControl.setForegroundColor(settings.viewSettings.sendColor, settings.viewSettings.receiveColor);

            // Reloasd the Msg Viewer
            reloadRtb();

        }

        /// <summary>
        /// Change the View State 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeViewUp_EventHandler(object sender, RoutedEventArgs e)
        {
            // Save the current SendBox Data with the Current View Settings
            try
            {
                currentSendData = msgLog.getRawMsgWithCurrentViewSettings(tbSendData.Text);
            }
            catch
            {
                currentSendData = msgLog.getRawMsgWithCurrentViewSettings("");
            }

            if (settings.viewSettings.dataPresentation >= 3)
            {
                settings.viewSettings.dataPresentation = 0;
            }
            else
            {
                settings.viewSettings.dataPresentation++;
            }

            msgLog.viewSettings = settings.viewSettings;
            UpdateViewStateDependence(settings.viewSettings);
        }

        /// <summary>
        /// Change the View State
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeViewDown_EventHandler(object sender, RoutedEventArgs e)
        {
            // Save the current SendBox Data with the Current View Settings
            try
            {
                currentSendData = msgLog.getRawMsgWithCurrentViewSettings(tbSendData.Text);
            }
            catch
            {
                currentSendData = msgLog.getRawMsgWithCurrentViewSettings("");
            }

            if (settings.viewSettings.dataPresentation <= 0)
            {
                settings.viewSettings.dataPresentation = 3;
            }
            else
            {
                settings.viewSettings.dataPresentation--;
            }

            msgLog.viewSettings = settings.viewSettings;
            UpdateViewStateDependence(settings.viewSettings);
        }

        /// <summary>
        /// Function is called after load of the Toolbar
        /// In this Function we will hide the Toolbar DropDown Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            /*            
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness();
            }*/
        }

        /// <summary>
        /// Button Send was Triggerd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Send_Data(object sender, RoutedEventArgs e)
        {
            ConnectionInterface_Send(tbSendData.Text);
        }

        /// <summary>
        /// Enter was Pressed on the Textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendData_KeyDown(object sender, KeyEventArgs e)
        {

            // User wants to send the Data
            if (e.Key == Key.Enter)
            {
                ConnectionInterface_Send(tbSendData.Text);
            }

            // User wants to Browse The Send History
            if (e.Key == Key.Up)
            {
                tbSendData.Text = msgLog.getPriviousSendedMsg();
            }

            // User wants to Browse The Send History
            if (e.Key == Key.Down)
            {
                tbSendData.Text = msgLog.getNextSendedMsg();
            }

        }

        /// <summary>
        /// Window Closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Save the Window Position.
            settings.applicationSettings.position = new Point( this.Left, this.Top);

            // Save the Height and the Width of the Window.
            settings.applicationSettings.width = this.Width;
            settings.applicationSettings.height = this.Height;

            // Save the Last Quick Button Path
            settings.applicationSettings.lastQuickButtonsPath = QuickButtonsControl.lastQuickButtonsPath;

            // Save the last Zoom Factor
            settings.applicationSettings.msgLogZoomFactor = sScaleSlider.Value;


            // Try to save the Settings
            LoadSave.saveSettings(System.AppDomain.CurrentDomain.BaseDirectory, settings);

            // Disconnect the Connections
            ConnectionInterface_Disconnect();

            CloseAllSimulationUi();

        }

        /// <summary>
        /// Help was Called
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                UserHelper.showUserHelp();
            }
            catch
            {
                // Configure the message box to be displayed
                string messageBoxText = "Die Hilfedatei konnte nicht erstellt oder geöffnet werden. Bitte die Anwendug auf einem Lokalen Verzeichniss ausführen.";
                string caption = "Fehler in der Hilfedatei";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;

                // Display message box
                MessageBox.Show(messageBoxText, caption, button, icon);
            }
        }


        #region Command Bindings

        /// <summary>
        /// Add the Hot Keys and Key Bindings to the UI
        /// </summary>
        private void AddHotKeys()
        {
            try
            {
                // ----------------------------------------------------------------------------------------------
                // Add the Connect Commands
                CommandBinding cb = new CommandBinding(connectRoutedCommand, Connect_EventHandler);
                this.CommandBindings.Add(cb);
                menuItemVerbinden.Command = connectRoutedCommand;
                toolBarConnect.Command = connectRoutedCommand;

                KeyGesture kg = new KeyGesture(Key.F11, ModifierKeys.None);
                InputBinding ib = new InputBinding(connectRoutedCommand, kg);
                this.InputBindings.Add(ib);

                // ----------------------------------------------------------------------------------------------
                // Add the Disconnect Command
                cb = new CommandBinding(disconnectRoutedCommand, Disconnect_EventHandler);
                this.CommandBindings.Add(cb);
                menuItemTrennen.Command = disconnectRoutedCommand;
                toolBarDisconnect.Command = disconnectRoutedCommand;

                kg = new KeyGesture(Key.F12, ModifierKeys.None);
                ib = new InputBinding(disconnectRoutedCommand, kg);
                this.InputBindings.Add(ib);

                // ----------------------------------------------------------------------------------------------
                // Add the Change View Command UP
                cb = new CommandBinding(changeViewUpRoutedCommand, ChangeViewUp_EventHandler);
                this.CommandBindings.Add(cb);

                kg = new KeyGesture(Key.Left, ModifierKeys.Alt);
                ib = new InputBinding(changeViewUpRoutedCommand, kg);
                this.InputBindings.Add(ib);

                // ----------------------------------------------------------------------------------------------
                // Add the Change View Command Down
                cb = new CommandBinding(changeViewDownRoutedCommand, ChangeViewDown_EventHandler);
                this.CommandBindings.Add(cb);

                kg = new KeyGesture(Key.Right, ModifierKeys.Alt);
                ib = new InputBinding(changeViewDownRoutedCommand, kg);
                this.InputBindings.Add(ib);

                // ----------------------------------------------------------------------------------------------
                // Add the Settings Command
                cb = new CommandBinding(settingsRoutedCommand, Settings_EventHandler);
                this.CommandBindings.Add(cb);
                menuItemEinstellungen.Command = settingsRoutedCommand;
                toolBarConnectionSettings.Command = settingsRoutedCommand;

                kg = new KeyGesture(Key.P, ModifierKeys.Control);
                ib = new InputBinding(settingsRoutedCommand, kg);
                this.InputBindings.Add(ib);

                // ----------------------------------------------------------------------------------------------
                // Add the Erase MsgLog Command
                
                //Stop the Richtext Box for captioring the CTR and L Gesture 
                KeyBinding keyBinding = new KeyBinding(ApplicationCommands.NotACommand, Key.L, ModifierKeys.Control);
                rtbInOutData.InputBindings.Add(keyBinding);

                cb = new CommandBinding(resetMsgLogRoutedCommand, MsgLogReset_EventHandler);
                this.CommandBindings.Add(cb);
                menuItem_MsgLogReset.Command = resetMsgLogRoutedCommand;

                kg = new KeyGesture(Key.L, ModifierKeys.Control);
                ib = new InputBinding(resetMsgLogRoutedCommand, kg);
                this.InputBindings.Add(ib);


                // ----------------------------------------------------------------------------------------------
                // Add the New Simulation Window Command
                cb = new CommandBinding(openNewSimulationWindowRoutedCommand, OpenNewSimulationWindow_EventHandler);
                this.CommandBindings.Add(cb);
                menuItemShowSimulationWindow.Command = openNewSimulationWindowRoutedCommand;

                kg = new KeyGesture(Key.N, ModifierKeys.Control);
                ib = new InputBinding(openNewSimulationWindowRoutedCommand, kg);
                this.InputBindings.Add(ib);


                // ----------------------------------------------------------------------------------------------
                // Add the open Application Settings Commands
                cb = new CommandBinding(openApplicationSettngsRoutedCommand, OpenApplicationSettings_EventHandler);
                this.CommandBindings.Add(cb);
                menuItem_ApplicationSettings.Command = openApplicationSettngsRoutedCommand;

                kg = new KeyGesture(Key.OemComma, ModifierKeys.Control);
                ib = new InputBinding(openApplicationSettngsRoutedCommand, kg);
                this.InputBindings.Add(ib);

                // ----------------------------------------------------------------------------------------------
                // Add the Increase Zoom Factor Command

                //Stop the Richtext Box for captioring the CTR and + Gesture 
                keyBinding = new KeyBinding(ApplicationCommands.NotACommand, Key.OemPlus, ModifierKeys.Control);
                rtbInOutData.InputBindings.Add(keyBinding);

                cb = new CommandBinding(increaseZoomFactorRoutedCommand, IncreaseZoomFactor_Event);
                this.CommandBindings.Add(cb);

                kg = new KeyGesture(Key.OemPlus, ModifierKeys.Control);
                ib = new InputBinding(increaseZoomFactorRoutedCommand, kg);
                this.InputBindings.Add(ib);

                // ----------------------------------------------------------------------------------------------
                // Add the Decrease Zoom Factor Command

                //Stop the Richtext Box for captioring the CTR and - Gesture 
                keyBinding = new KeyBinding(ApplicationCommands.NotACommand, Key.OemMinus, ModifierKeys.Control);
                rtbInOutData.InputBindings.Add(keyBinding);

                cb = new CommandBinding(decreaseZoomFactorRoutedCommand, DecreaseZoomFactor_Event);
                this.CommandBindings.Add(cb);

                kg = new KeyGesture(Key.OemMinus, ModifierKeys.Control);
                ib = new InputBinding(decreaseZoomFactorRoutedCommand, kg);
                this.InputBindings.Add(ib);

            }
            catch
            {
                //handle exception error
            }
        }

        /// <summary>
        /// Open Settings was triggerd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ConnectySetings loadedSettings = LoadSave.openSettings();

            if (loadedSettings != null)
            {
                settings = loadedSettings;
                updateMainWindowTitle();
                UpdateViewStateDependence(settings.viewSettings);

                // In case we got a different Connection Settings we will disconnect the Current Connection
                ConnectionInterface_Disconnect();

            }

        }

        /// <summary>
        /// Safe Settings was triggerd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            LoadSave.saveSettings(settings);
        }

        /// <summary>
        /// Application Close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Increase the Msg Log Zoom Factor by CTR and + Gesture 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IncreaseZoomFactor_Event(object sender, ExecutedRoutedEventArgs e)
        {
            sScaleSlider.Value += 0.1;
        }

        /// <summary>
        /// Decrease the Msg Log Zoom Factor by CTR and + Gesture
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecreaseZoomFactor_Event(object sender, ExecutedRoutedEventArgs e)
        {
            sScaleSlider.Value += -0.1;
        }

        /// <summary>
        /// Mouse Wheel Turned
        /// </summary>
        /// <param name="args"></param>
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs args)
        {

            base.OnPreviewMouseWheel(args);

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                sScaleSlider.Value += (args.Delta > 0) ? 0.1 : -0.1;
            }

        }


        /// <summary>
        /// Mouse Wheel Clicked
        /// </summary>
        /// <param name="args"></param>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs args)
        {

            base.OnPreviewMouseDown(args);

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (args.MiddleButton == MouseButtonState.Pressed)
                {
                    sScaleSlider.Value = 1;
                }
            }

        }





        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect_EventHandler(object sender, ExecutedRoutedEventArgs e)
        {
            ConnectionInterface_Connect();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Disconnect_EventHandler(object sender, RoutedEventArgs e)
        {
            ConnectionInterface_Disconnect();
        }

        /// <summary>
        /// The MenuItem Cleare the Message Log was Klicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgLogReset_EventHandler(object sender, RoutedEventArgs e)
        {

            rtbInOutData.Document.Blocks.Clear();
            msgLog.msgLog.Clear();
        }

        /// <summary>
        /// Menu Item "Parameter"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_EventHandler(object sender, RoutedEventArgs e)
        {

            // Disable the Editing functions if we are currently connected
            bool connectionState = true;                
            if (connection.connectionStateCommuniation1 >0 || connection.connectionStateCommuniation2 > 0)
            {
                connectionState = false;
            }

            // Create an Settings View
            ConnectionSettings_UI paramWindow = new ConnectionSettings_UI(settings.connectionSettings, connectionState, settings.applicationSettings, this.PointToScreen(new Point(0, 0)));

            if (paramWindow.ShowDialog() == true)
            {
                // Get the User entered Data from the Settings Window
                settings.connectionSettings = paramWindow.getUserParams;

                // Update the Main Window Title
                updateMainWindowTitle();

                // Save the Data to the File System
                LoadSave.saveSettings(System.AppDomain.CurrentDomain.BaseDirectory, settings);
            }

        }

        /// <summary>
        /// Menu Item "Einstellungen"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenApplicationSettings_EventHandler(object sender, RoutedEventArgs e)
        {

            // Create an Settings View
            UI_ApplicationSettings applicationSettingsWindow = new UI_ApplicationSettings(settings.applicationSettings, themeManager.ThemeNameList, this.PointToScreen(new Point(0, 0)));

            if (applicationSettingsWindow.ShowDialog() == true)
            {
                // Get the User entered Data from the Settings Window
                settings.applicationSettings = applicationSettingsWindow.getUserParams;

                // Save the Data to the File System
                LoadSave.saveSettings(System.AppDomain.CurrentDomain.BaseDirectory, settings);

                // Take the UI Theme
                themeManager.SetTheme(settings.applicationSettings.uiTheme);
            }

        }

        /// <summary>
        /// Search Event was triggerd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchTextBox_Search(object sender, RoutedEventArgs e)
        {

            // First of all reload the Textbox in case we gote some Style we do not want
            UpdateViewStateDependence(settings.viewSettings);

            // Get the Sender as a Search Box
            UIControls.SearchTextBox searchBox = (UIControls.SearchTextBox)sender;

            string mySearchString = searchBox.Text;

            TextRange range = new TextRange(rtbInOutData.Document.ContentStart, rtbInOutData.Document.ContentEnd);
            //range.Text = @"TOP a multiline text or file END";
            Regex reg = new Regex("("+ mySearchString + ")", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var start = rtbInOutData.Document.ContentStart;
            while (start != null && start.CompareTo(rtbInOutData.Document.ContentEnd) < 0)
            {
                if (start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    var match = reg.Match(start.GetTextInRun(LogicalDirection.Forward));

                    var textrange = new TextRange(start.GetPositionAtOffset(match.Index, LogicalDirection.Forward), start.GetPositionAtOffset(match.Index + match.Length, LogicalDirection.Backward));
                    textrange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(Colors.Yellow));
                    textrange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                    start = textrange.End; // I'm not sure if this is correct or skips ahead too far, try it out!!!
                }
                start = start.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

        #region Connection Functions

        /// <summary>
        /// Try to Connect the Connection
        /// </summary>
        private void ConnectionInterface_Connect()
        {
            // Start the Connection
            connection.Connect(settings.connectionSettings);
        }

        /// <summary>
        /// Try to Disconnect the current Connection
        /// </summary>
        private void ConnectionInterface_Disconnect()
        {
            // Start the Connection
            connection.Disconnect();
        }

        /// <summary>
        /// Send Data to the Connected Serial Port
        /// </summary>
        /// <param name="message"></param>
        private void ConnectionInterface_Send(string message)
        {
            // If the Message is empty we do not send anything
            if (message == "")
            {
                return;
            }

            // Send the Data
            try
            {
                MsgData newSendMsg = msgLog.getRawMsgWithCurrentViewSettings(message);
                newSendMsg.connectionNumber = connectionSelectionSendData;
                if (connection.Send(newSendMsg) == 1)
                {
                    msgLog.messageWasSend(msgLog.getRawMsgWithCurrentViewSettings(message));
                    tbSendData.Text = "";
                }

            }
            catch
            {
                // Configure the message box to be displayed
                string messageBoxText = "Die Eingaben passen nicht zu der aktuellen Darstellung. Bitte Eingaben überprüfen.";
                string caption = "Ungültige Eingabe für das Senden";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;

                // Display message box
                MessageBox.Show(messageBoxText, caption, button, icon);
            }
        }

        /// <summary>
        /// Send Data to the Connected Serial Port
        /// </summary>
        /// <param name="message"></param>
        private void ConnectionInterface_SendMsgData(MsgData msg)
        {
            // If the Message is empty we do not send anything
            if (msg.value == null)
            {
                return;
            }

            // Send the Data
            try
            {
                msg.connectionNumber = connectionSelectionSendData;
                if (connection.Send(msg) == 1)
                {
                    // Everything is fine
                }

            }
            catch
            {
                // Configure the message box to be displayed
                string messageBoxText = "Der Senden Job konnte nicht bearbeitet werden. Das Telegramm ist ungültig!";
                string caption = "Ungültige parameter für das Senden";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;

                // Display message box
                MessageBox.Show(messageBoxText, caption, button, icon);
            }
        }

        #endregion

        #region Simulation Functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenNewSimulationWindow_EventHandler(object sender, RoutedEventArgs e)
        {
            // Create a new Simulation Object and add it to the List
            simUi.Add(new Simulation_UI());

            // Add the Eventhandler of the Simulation Object
            simUi[simUi.Count - 1].MsgSendRecived += new MsgSendRecivedEventHandler(simulatioSendMsg);
            simUi[simUi.Count - 1].Closed += SimulationWindowClosedEvent;

            // Start the UI of the Simulation object
            simUi[simUi.Count - 1].Show();

            // Add a new Entry to the MenuBar for the Currently insetred Simulation Window
            addsimulationWindowToMenuBar(simUi.Count);

        }


        /// <summary>
        /// Open a Simulation Window with an Existing Simulation File...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemOpenExistingSimulationWindow_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Simulation importieren";
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                // Create a new Simulation Object and add it to the List
                simUi.Add(new Simulation_UI(openFileDialog.FileName));

                // Add the Eventhandler of the Simulation Object
                simUi[simUi.Count - 1].MsgSendRecived += new MsgSendRecivedEventHandler(simulatioSendMsg);
                simUi[simUi.Count - 1].Closed += SimulationWindowClosedEvent;

                // Start the UI of the Simulation object
                simUi[simUi.Count - 1].Show();

                // Add a new Entry to the MenuBar for the Currently insetred Simulation Window
                addsimulationWindowToMenuBar(simUi.Count);
            }

        }


        /// <summary>
        /// SImulation Object Close Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SimulationWindowClosedEvent(object sender, EventArgs e)
        {
            // Get the Simulation Object from the Sender Object
            Simulation_UI simulationUiObject = sender as Simulation_UI;

            // Remove the Object from the List of Simulations
            simUi.Remove(simulationUiObject);

            // Remove the Item in the MenuBar
            menuSimulation.Items.Remove(simMenuBarItems.Last.Value);

            // Remove the Menu Item from the local List of Menuitems
            simMenuBarItems.RemoveLast();

            // If the last Simulation UI is closed disable the MenuItem for Closing all Simu UI's....
            if (simUi.Count <= 0)
            {
                this.menuItemCloseAllSimulations.IsEnabled = false;
            }

            // set the Title to the Simulation Window
            UpdateSimulationWindowTitle();
        }


        /// <summary>
        /// Add nex MenuItem to the MenuBar for the Simulation Window
        /// </summary>
        /// <param name="itemNumber"></param>
        private void addsimulationWindowToMenuBar(int itemNumber)
        {
            //Add MenuItem to the local list of MenuItems
            MenuItem newSimulationWindowMenuItem = new MenuItem();
            simMenuBarItems.AddLast(newSimulationWindowMenuItem);

            simMenuBarItems.Last.Value.Header = "Simulations Fenster " + itemNumber.ToString();
            simMenuBarItems.Last.Value.Click += MenutemSimulationWindow_Click;

            // Add the last entry from the List of MenuItems to the Menubar
            this.menuSimulation.Items.Add(simMenuBarItems.Last.Value);
            this.menuItemCloseAllSimulations.IsEnabled = true;

            // set the Title to the Simulation Window
            UpdateSimulationWindowTitle();
        }


        /// <summary>
        /// Calling Simulation Window Evemt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenutemSimulationWindow_Click(object sender, RoutedEventArgs e)
        {
            // Get the Sender Object as an MenuItem
            MenuItem myMenuItem = sender as MenuItem;

            // Get the Header of teh MenuItem and extract the Integer
            int currentWindowNumber = Convert.ToInt32(Regex.Match((string)myMenuItem.Header, @"\d+").Value);

            // Call the Sim UI by the given Index
            simUi[currentWindowNumber - 1].Activate();

        }


        /// <summary>
        /// Close the complete Simulation UI's
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemCloseAllSimulations_Click(object sender, RoutedEventArgs e)
        {
            CloseAllSimulationUi();
        }

        /// <summary>
        /// msgSend Event from the Simulation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simulatioSendMsg(object sender, MsgSendRecivedEventArgs e)
        {
            ConnectionInterface_SendMsgData(e.msgData);
            
        }


        /// <summary>
        /// Close all Opend Simulation Windows
        /// </summary>
        private void CloseAllSimulationUi()
        {
            // Stop the Simulation
            if (simUi.Count > 0)
            {
                // Copy the objects to an Array. Otherwise we will erase the Objects during the ForEach Loop
                Simulation_UI[] currentUiObjects = new Simulation_UI[simUi.Count];
                simUi.CopyTo(currentUiObjects);

                // Try to close all the Simulation Objects 
                try
                {
                    // Try to Close all the Simulation UI Objects
                    for (int i = 0; i < currentUiObjects.Length; i++)
                    {
                        currentUiObjects[i].Close();
                    }
                }
                catch
                {
                    // OK Not all of the Objects could be closed.... we are going on anyway..
                }

            }

            // Disable the MenuItem for cloasing all SimUi's...
            this.menuItemCloseAllSimulations.IsEnabled = false;

        }

        /// <summary>
        /// Update the Simulation Window Titles
        /// </summary>
        private void UpdateSimulationWindowTitle()
        {
            // Stop the Simulation
            if (simUi.Count > 0)
            {
                int counter = 1;

                foreach (Simulation_UI simulationWindow in simUi)
                {
                    
                    simulationWindow.SetWindowTitle("Simulations Fenster " + counter.ToString());
                    counter++;

                }

            }

        }


        #endregion

        #region Quick Buttons

        /// <summary>
        /// Load a new QuickButton Setting from XML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_LoadQuickButtons_Click(object sender, RoutedEventArgs e)
        {
            QuickButtonsControl.LoadQuickButtons();
        }

        /// <summary>
        /// Save the Current QuickButton Setting to XML
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_SafeQuickButtons_Click(object sender, RoutedEventArgs e)
        {
            QuickButtonsControl.SafeQuickButtonsToXml();
        }

        /// <summary>
        /// Safe the Current QuickButtons Settings to XML as...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_SafeAsQuickButtons_Click(object sender, RoutedEventArgs e)
        {
            QuickButtonsControl.SafeAsQuickButtonsToXml();
        }

        /// <summary>
        /// Add a New Quick Button with the Menubar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItem_AddQuickButtons_Click(object sender, RoutedEventArgs e)
        {
            QuickButtonsControl.AddQuickButton();
        }



        /// <summary>
        /// msgSend Event from the Simulation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void quickButtonSendMsg(object sender, MsgSendRecivedEventArgs e)
        {
            ConnectionInterface_SendMsgData(e.msgData);

        }


        #endregion

        #region SendRecive Color

        /// <summary>
        /// Eventhandler for the Color Change Event of teh Send and Recive Telegramms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendReciveColorChanged_Event(object sender, TelegrammColorEventArgs e)
        {
            if (e.SendReciveSelection == 1)
            {
                settings.viewSettings.sendColor = e.Color;
            }
            else
            {
                settings.viewSettings.receiveColor = e.Color;
            }

            // Set the Current View Configuration
            msgLog.viewSettings = settings.viewSettings;

            // Update the UI
            UpdateViewStateDependence(msgLog.viewSettings);
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Clean up the required Objects
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (connection != null)
                    {
                        connection.Dispose();
                    }
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Public Dispose Method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        #region Debug that Bug

        private void debugStuff()
        { 
            if (settings.applicationSettings.debugModeIsActive)
            {
                ;
            }
            else
            {

            }
        }



        #endregion


        /// <summary>
        /// On Window Loaded Event. Is Called after the Window is completle loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            checkForNewReleaseVersion();
        }



        
        #region CheckForNewRelease

        /// <summary>
        /// Check for Release Updates
        /// </summary>
        private void checkForNewReleaseVersion()
        {
            miNewVersionAivailable.Visibility = Visibility.Collapsed;
            updateHandler.PropertyChanged += newReleaseVersionCheckupdated;
            updateHandler.DownloadCompleted += DownloadCompleted;
            updateHandler.getVersionInformation();
        }

        /// <summary>
        /// Download the Relese Version to check 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void miNewVersionAivailable_Click(object sender, RoutedEventArgs e)
        {
            updateHandler.downloadReleaseVersion();
        }


        /// <summary>
        /// Event called if the UpdateHandler has recived new Data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newReleaseVersionCheckupdated(object sender, PropertyChangedEventArgs e)
        {
             if (e.PropertyName.Equals("NewStateReleaseVersion"))
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Threading.DispatcherOperationCallback(delegate
                {
                    // Update the RichtextBox
                    miNewVersionAivailable.Visibility = Visibility.Visible;

                    return null;
                }), null);
            }
        }


        /// <summary>
        /// New Version check changed Event
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

        #endregion


    }
}
