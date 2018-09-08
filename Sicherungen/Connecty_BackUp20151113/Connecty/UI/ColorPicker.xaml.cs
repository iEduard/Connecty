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

        private Color selectedColor;

        public ColorPicker()
        {
            InitializeComponent();
        }

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
    }
}
