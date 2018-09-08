using System.Windows;
using System.Reflection;
using System.IO;

namespace Connecty
{
    /// <summary>
    /// Interaktionslogik für InfoDialog.xaml
    /// </summary>
    public partial class InfoDialog : Window
    {

        /// <summary>
        /// Info Dialog Constructor
        /// </summary>
        public InfoDialog()
        {
            InitializeComponent();

            // Init the labels
            lVersion.Content = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // Get the Version History Text from the Embedded Resource File
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Connecty.Resources.VersionHistory.txt"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                tbVersionHistory.AppendText(result);
            }

            // Set the Textbox to read only
            tbVersionHistory.IsReadOnly = true;

        }


        /// <summary>
        /// Close the Info Dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Open up the Default Email Client to send a Bug Report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void emailLink_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "mailto:ing.eduard.schmidt@gmail.com?subject=Connecty Support&body=Connecty Version=" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
            proc.Start();
        }
    }
}
