using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Connecty
{
    /// <summary>
    /// Interaktionslogik für ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : Window
    {

        /// <summary>
        /// Selected Color of the Palat
        /// </summary>
        private Color selectedColor;

        /// <summary>
        /// Start Position of the Window
        /// </summary>
        private Point windowStartPosistion;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public ColorPicker(Point position)
        {
            windowStartPosistion = position;
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColorSelect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle selectedRect = sender as Rectangle;

            //Get the Color of the Selected Rectangle
            selectedColor = ((SolidColorBrush)selectedRect.Fill).Color;

            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Geter for the selected Color
        /// </summary>
        public Color getColor
        {
            get { return selectedColor; }

        }

        /// <summary>
        /// Key Logger. Detect if the Escape Key was pressed...
        /// If So we close the Color Picker and set the Dialog Result to false!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Escape))
            {
                this.DialogResult = false;
                this.Close();
            }
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
    }
}
