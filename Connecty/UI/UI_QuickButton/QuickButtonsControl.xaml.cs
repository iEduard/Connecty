using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Connecty
{
    /// <summary>
    /// Interaktionslogik für QuickButtonsControl.xaml
    /// </summary>
    public partial class QuickButtonsControl : UserControl
    {

        
        #region local private variables

        private LinkedList<Button> buttonList;

        #endregion

        #region public variables

        public string lastQuickButtonsPath;

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        public QuickButtonsControl()
        {

            InitializeComponent();

            // Create a new  Button List
            buttonList = new LinkedList<Button>();


        }



        #region Public Methods

        /// <summary>
        /// Load QuickButtons from existing XML File
        /// </summary>
        public void LoadQuickButtons()
        {


            quickButtonXmlParserReturnVal _parserReturnValue = QuickButtonXmlParser.loadQuickButtons();

            if (_parserReturnValue.retVal == 1)
            {
                // At first remove all the Quick Buttons already Created
                RemoveAllQuickButtons();

                // Load the List of Buttons to the local List
                buttonList = _parserReturnValue.quickButtonsList;
                lastQuickButtonsPath = _parserReturnValue.pathAndName;

                AddAllQuickButtonsToTheUI();
            }


        }

        /// <summary>
        /// Load QuickButtons from existing XML File
        /// </summary>
        /// <param name="pathAndFileName">The Path and the Filename of the XML File</param>
        public void LoadQuickButtons(string pathAndFileName)
        {

            // Load the List of Buttons to the local List
            quickButtonXmlParserReturnVal _parserReturnValue = QuickButtonXmlParser.loadQuickButtons(pathAndFileName);

            if (_parserReturnValue.retVal == 1)
            {
                // At first remove all the Quick Buttons already Created
                RemoveAllQuickButtons();

                // Load the List of Buttons to the local List
                buttonList = _parserReturnValue.quickButtonsList;
                lastQuickButtonsPath = _parserReturnValue.pathAndName;

                AddAllQuickButtonsToTheUI();
            }


        }

        /// <summary>
        /// Safe the Current Quickbuttons to an XML File
        /// </summary>
        public void SafeQuickButtonsToXml()
        {
            
            if ( File.Exists(lastQuickButtonsPath) )
            {
                QuickButtonXmlParser.saveQuickButtons(buttonList, lastQuickButtonsPath);
            }
            else
            {
                lastQuickButtonsPath = QuickButtonXmlParser.saveQuickButtons(buttonList);
            }



        }

        /// <summary>
        /// Safe the Current Quickbuttons to an XML File
        /// </summary>
        public void SafeAsQuickButtonsToXml()
        {

            lastQuickButtonsPath = QuickButtonXmlParser.saveQuickButtons(buttonList);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputKey"></param>
        public void KeyInpuEvent(Key inputKey)
        {
            foreach (Button _button in buttonList)
            {
                if ((Key)_button.Resources["shortCut"] == inputKey)
                {
                    // Set the User Hint to the TextBox
                    MsgData logMessage = new MsgData();
                    logMessage.value = Converty.specialAsciiStringToMsgData((String)_button.Resources["data"]);
                    logMessage.type = MsgData.messageType.send;

                    // Set the Event that the User Changed an Input
                    MsgSendRecivedEventArgs msgLogEventArgs = new MsgSendRecivedEventArgs();
                    msgLogEventArgs.msgData = logMessage;
                    msgSendRecived(msgLogEventArgs);
                }
            }
        }

        /// <summary>
        /// Create a new Quickbutton with Dialog and add it to the UI
        /// </summary>
        public void AddQuickButton()
        {
            CrateQuickButton();
        }

        /// <summary>
        /// Debug Funktion not working yet..
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FunctionKey_EventHandler(Key functionKey)
        {
            // To be done...s
        }

        #endregion

        #region Local Private Methods


        /// <summary>
        /// Event for the Add Quick Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddQuickButton_Click(object sender, RoutedEventArgs e)
        {
            CrateQuickButton();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CrateQuickButton()
        {

            // Debug String list
            string[] funktionKeyList = { "F1", "F2", "F3" };

            // Create an Settings View
            QuickButtonEditor _quickButtonDialog = new QuickButtonEditor("Button" + (buttonList.Count + 1).ToString(), "", PointToScreen(Mouse.GetPosition(this)));

            if (_quickButtonDialog.ShowDialog() == true)
            {
                AddQuickButton(_quickButtonDialog.getButton());
            }

        }

        /// <summary>
        /// Add all Quick Buttons to the UI from the local Button List
        /// </summary>
        private void AddAllQuickButtonsToTheUI()
        {

            foreach (Button _button in buttonList)
            {
                _button.Click += QuickButton_Click;
                _button.MouseRightButtonDown += QuickButton_RightClick;
                /*_button.BorderBrush = (Brush)FindResource("MainBackground"); // Brushes.DarkGray;
                _button.Background = (Brush)FindResource("MainBackground"); // Brushes.DarkGray;
                _button.Foreground = (Brush)FindResource("MainTextColor");*/
                _button.MinWidth = 50;

                theMightyStackPanel.Children.Add(_button);
            }
        }

        /// <summary>
        /// Click of the Quick Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuickButton_Click(object sender, RoutedEventArgs e)
        {
            Button _quickButton = sender as Button;

            // Set the User Hint to the TextBox
            MsgData logMessage = new MsgData();
            logMessage.value = Converty.specialAsciiStringToMsgData((String)_quickButton.Resources["data"]);
            logMessage.type = MsgData.messageType.send;

            // Set the Event that the User Changed an Input
            MsgSendRecivedEventArgs msgLogEventArgs = new MsgSendRecivedEventArgs();
            msgLogEventArgs.msgData = logMessage;
            msgSendRecived(msgLogEventArgs);

        }

        /// <summary>
        /// Right Click event for the Quick Buttons
        /// Create the Contextmenu for the Referenced Quick Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuickButton_RightClick(object sender, MouseButtonEventArgs e)
        {
            Button button = sender as Button;
            ContextMenu menu = new ContextMenu();

            //menu.Template = (ControlTemplate)FindResource("SubMenuItemControlTemplateTheme");


            MenuItem _menuItem = new MenuItem();
            _menuItem.Header = "Löschen";
            _menuItem.Click += QuickButtonContext_Click;
            menu.Items.Add(_menuItem);


            _menuItem = new MenuItem();
            _menuItem.Header = "Bearbeiten";
            _menuItem.Click += QuickButtonContext_Click;
            menu.Items.Add(_menuItem);


            button.ContextMenu = menu;

        }

        /// <summary>
        /// Contextklick event for the Quick Buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuickButtonContext_Click(object sender, RoutedEventArgs e)
        {

            // Create the local Variables
            MenuItem menuItem = (MenuItem)e.Source;
            ContextMenu menu = (ContextMenu)menuItem.Parent;
            Button _button = (Button)menu.PlacementTarget;


            if ((string)menuItem.Header == "Löschen")
            {
                // Remove the Item
                RemoveQuickButton(_button);
            }
            else if ((string)menuItem.Header == "Bearbeiten")
            {
                // Edit the Button
                EditQuickButtons(_button);
            }

        }

        /// <summary>
        /// Define the Style and add the Events 
        /// Add the Button to the UI
        /// </summary>
        /// <param name="_button">Butto to </param>
        private void AddQuickButton(Button _button)
        {
            // Add the Eventhandlers of the Button
            _button.Click += QuickButton_Click;
            _button.MouseRightButtonDown += QuickButton_RightClick;
            _button.MinWidth = 50;

            // Add the Button to the linked List and the UI!
            buttonList.AddLast(_button);
            theMightyStackPanel.Children.Add(_button);

        }

        /// <summary>
        /// Remove all Quick Buttons
        /// </summary>
        private void RemoveAllQuickButtons()
        {

            

            theMightyStackPanel.Children.Clear();
            buttonList.Clear();


        }

        /// <summary>
        /// Remove the Selected Quick Button
        /// </summary>
        /// <param name="buttonToErase">Button Reference to delete</param>
        private void RemoveQuickButton(Button buttonToErase)
        {
            // Remove the Item from the UI
            theMightyStackPanel.Children.Remove(buttonToErase);

            // Remove the Item from the lnked List
            buttonList.Remove(buttonToErase);
        }

        /// <summary>
        /// Edit the Quickbutton by opening the Edit Dialog
        /// </summary>
        /// <param name="quickButton">Reference to the Button to edit</param>
        private void EditQuickButtons(Button quickButton)
        {
            // Create an Editor view
            QuickButtonEditor quickButtonDialog = new QuickButtonEditor(quickButton, PointToScreen(Mouse.GetPosition(this)));

            if (quickButtonDialog.ShowDialog() == true)
            {
                Button myQuickButton = quickButtonDialog.getButton();
                quickButton.Content = myQuickButton.Content;
                quickButton.Resources = myQuickButton.Resources;

            }
        }

        

        #endregion

        #region Class Events

        // An event that clients can use to be notified whenever the
        // A new Message has to be send
        public event MsgSendRecivedEventHandler MsgSendRecived;

        // Invoke the Changed event; called whenever list New Message should be send
        protected virtual void msgSendRecived(MsgSendRecivedEventArgs e)
        {
            if (MsgSendRecived != null)
                MsgSendRecived(this, e);
        }

        #endregion

    }
}
