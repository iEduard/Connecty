using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Connecty
{
    /// <summary>
    /// Interaction logic for NewQuickButtonDialog.xaml
    /// </summary>
    public partial class QuickButtonEditor : Window
    {

        #region local private Variables

        /// <summary>
        /// object of the Created Quick Button
        /// </summary>
        private Button quickButton;

        /// <summary>
        /// Store the information if the Name has changed
        /// </summary>
        private bool namehasChanged;

        /// <summary>
        /// Start Position of the Window
        /// </summary>
        private Point windowStartPosistion;

        #endregion


        #region Constructors


        /// <summary>
        /// open the Editor with default data
        /// </summary>
        public QuickButtonEditor(Point startPosition)
        {
            windowStartPosistion = startPosition;
            InitializeComponent();
            namehasChanged = false;
            quickButton = new Button();

            // No Function Key allowed
            cbFunktionKeyLabel.Visibility = Visibility.Collapsed;
            cbFunktionKey.Visibility = Visibility.Collapsed;

        }

        /// <summary>
        /// open the Editor with the given Button Data
        /// </summary>
        /// <param name="btName"></param>
        /// <param name="btData"></param>
        public QuickButtonEditor(string btName, string btData, Point startPosition)
        {
            windowStartPosistion = startPosition;
            InitializeComponent();
            namehasChanged = false;
            quickButton = new Button();

            tbData.Text = btData;
            tbName.Text = btName;

            // No Function Key allowed
            cbFunktionKeyLabel.Visibility = Visibility.Collapsed;
            cbFunktionKey.Visibility = Visibility.Collapsed;

        }

        /// <summary>
        /// Open the Editor and handle over the available function Keys
        /// </summary>
        /// <param name="availableFunctionKeys">List of available Function Keys</param>
        public QuickButtonEditor(string[] availableFunctionKeys, Point startPosition)
        {
            windowStartPosistion = startPosition;
            InitializeComponent();
            namehasChanged = false;
            quickButton = new Button();

            // No Function Key allowed
            cbFunktionKey.ItemsSource = availableFunctionKeys;
            cbFunktionKey.SelectedIndex = 0;
        }

        /// <summary>
        /// Open the Editor with a given Button
        /// </summary>
        /// <param name="_button"></param>
        public QuickButtonEditor(Button _button, Point startPosition)
        {
            windowStartPosistion = startPosition;
            InitializeComponent();

            // Create a new Button
            quickButton = _button;

            tbData.Text = (string)_button.Resources["data"];
            tbName.Text = _button.Content.ToString();

            namehasChanged = true;

            // No Function Key allowed
            cbFunktionKeyLabel.Visibility = Visibility.Collapsed;
            cbFunktionKey.Visibility = Visibility.Collapsed;

        }

        /// <summary>
        /// Open the Editor with a given Button
        /// </summary>
        /// <param name="_button"></param>
        public QuickButtonEditor(Button _button, string[] availableFunctionKeys, Point startPosition)
        {
            windowStartPosistion = startPosition;
            InitializeComponent();

            // Create a new Button
            quickButton = _button;

            tbData.Text = (string)_button.Resources["data"];
            tbName.Text = _button.Content.ToString();

            namehasChanged = true;

        }

        #endregion


        #region Ui Events

        /// <summary>
        /// Accept Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAccept_Click(object sender, RoutedEventArgs e)
        {

            //---------------------------------------------------------
            // Set the Data Resource
            //---------------------------------------------------------
            // Check if we allready got the Resource added
            if (quickButton.Resources["data"] != null)
            {
                // Remove the Existing Resource bevore adding a new one
                quickButton.Resources.Remove("data");

            }

            quickButton.Resources.Add("data", tbData.Text);

            //---------------------------------------------------------
            // Set the Function Key Resource
            //---------------------------------------------------------
            // Check if we allready got the Resource added
            if (quickButton.Resources["shortCut"] != null)
            {
                // Remove the Existing Resource bevore adding a new one
                quickButton.Resources.Remove("shortCut");

            }

            if (cbFunktionKey.Visibility == Visibility.Visible)
            {
                quickButton.Resources.Add("shortCut", getFunctionKeyFromString(cbFunktionKey.SelectedItem.ToString()));
            }



            //---------------------------------------------------------
            // Set the Name of the Button
            //---------------------------------------------------------
            if (!namehasChanged)
            {
                setNameFromData();
            }

            quickButton.Content = tbName.Text;



            this.DialogResult = true;
            this.Close();



        }

        /// <summary>
        /// Abbort Clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btAbbort_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
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

        /// <summary>
        /// Button Name has Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbName_TextChanged(object sender, TextChangedEventArgs e)
        {
            namehasChanged = true;
        }


        #endregion


        #region Private Methods

        /// <summary>
        /// Returns the F-Key of the given Quick Button
        /// </summary>
        /// <param name="keyAsString"></param>
        /// <returns></returns>
        private Key getFunctionKeyFromString(string keyAsString)
        {

            Key _key = new Key();

            switch (keyAsString)
            {
                case "F2":
                    _key = Key.F2;
                    break;
                case "F3":
                    _key = Key.F3;
                    break;
                default:
                    _key = Key.F1;
                    break;
            }

            return _key;

        }

        /// <summary>
        /// Set the Name of the Button from the given Data
        /// </summary>
        private void setNameFromData()
        {
            tbName.Text = tbData.Text.Substring(0, 5) + "...";
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Return the Button
        /// </summary>
        /// <returns></returns>
        public Button getButton()
        {
            return this.quickButton;
        }

        #endregion

    }
}
