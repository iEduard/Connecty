using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;

namespace Connecty
{
    /// <summary>
    /// Interaktionslogik für QuickButtonsControl.xaml
    /// </summary>
    public partial class CollorPickerControl : UserControl
    {

        
        #region local private variables

        #endregion

        #region public variables

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        public CollorPickerControl()
        {

            InitializeComponent();

        }


        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="telegramColor">Colot to be set for the Coresponding Telegram Type</param>
        /// <param name="sendReciveSelect">1 = Send // 2 = Recived</param>
        public void setForegroundColor(Color telegramColor, int sendReciveSelect)
        {

            if (sendReciveSelect == 1)
            {
                btSendColor.Foreground =  new SolidColorBrush(telegramColor);
            }
            else if (sendReciveSelect == 2)
            {
                btReciveColor.Foreground = new SolidColorBrush(telegramColor);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="telegramColor">Colot to be set for the Coresponding Telegram Type</param>
        /// <param name="sendReciveSelect">1 = Send // 2 = Recived</param>
        public void setForegroundColor(Color sendColor, Color reciveColor)
        {

            btSendColor.Foreground = new SolidColorBrush(sendColor);
            btReciveColor.Foreground = new SolidColorBrush(reciveColor);

        }

        #endregion

        #region Local Private Methods

        /// <summary>
        /// Collor Select Click 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorSelect_Click(object sender, RoutedEventArgs e)
        {
            // get the textBlock Object
            Button _button = sender as Button;
            int _sendReciveSelection = (Int32)_button.Resources["SendReciveSelection"];

            // Get the Visual Element
            ContentControl _visualElement = sender as ContentControl;
            
            // Create an Settings View
            //ColorPicker _colorPicker = new ColorPicker(PointToScreen(Mouse.GetPosition(this)));
            ColorPicker _colorPicker = new ColorPicker(_visualElement.PointToScreen(new Point(0,0)));

            if (_colorPicker.ShowDialog() == true)
            {

                // Create the Event Arguments
                TelegrammColorEventArgs _args = new TelegrammColorEventArgs();
                _args.Color = _colorPicker.getColor;
                _args.SendReciveSelection =_sendReciveSelection;

                // Update the UI
                setForegroundColor(_args.Color, _args.SendReciveSelection);

                // Set the Event that the User Changed an Input
                telegrammColorChange(_args);

            }
        }


        #endregion

        #region Event Handler

        // An event that clients can use to be notified whenever the
        // Color of the Send and recived Telegramms has to be changed
        public event TelegrammColorChangedEventHandler TelegrammColorChange;

        // Invoke the Changed event; called whenever list New Message should be send
        protected virtual void telegrammColorChange(TelegrammColorEventArgs e)
        {
            if (TelegrammColorChange != null)
                TelegrammColorChange(this, e);
        }


        #endregion

    }
}
