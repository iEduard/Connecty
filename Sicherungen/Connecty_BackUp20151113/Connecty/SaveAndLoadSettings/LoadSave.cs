using Microsoft.Win32;
using System;
using System.Windows;

namespace Connecty
{
    public static class LoadSave
    {

        private static string defaultSettingsName = "DefaultSettings.cs";
        private static string settingsExtension = ".cs";// Experimental


        /// <summary>
        /// Function where the user chooses the Name and the Path of the Settings File
        /// </summary>
        /// <param name="settings"></param>
        public static void saveSettings(ConnectySetings settings)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Connecty Einstellungen speichern";
            saveFileDialog.DefaultExt = settingsExtension;
            saveFileDialog.AddExtension = true;
            // saveFileDialog.Filter = "Settings (*.cs)|";

            if(saveFileDialog.ShowDialog() == true)
            {
                save(saveFileDialog.FileName, settings);

            }


        }


        /// <summary>
        /// Save the Settings with a specified Name
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="settings"></param>
        public static void saveSettings(string filePath, string fileName, ConnectySetings settings)
        {
            save(filePath, fileName, settings);
        }


        /// <summary>
        /// Save the Settings with the Default Name
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="settings"></param>
        public static void saveSettings(string filePath, ConnectySetings settings)
        {
            save(filePath, defaultSettingsName, settings);
        }

        /// <summary>
        /// Save the given Data to a path withe a given Name
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="settings"></param>
        private static void save(string filePath, string fileName, ConnectySetings settings)
        {
            save(filePath + fileName, settings);
        }


        /// <summary>
        /// The Actual Save Settings funktion where all the magic happens
        /// </summary>
        /// <param name="fileNameAndPath"></param>
        /// <param name="settings"></param>
        private static void save(string fileNameAndPath, ConnectySetings settings)
        {
            try
            {
                //save the car list to a file
                ObjectToSerialize objectToSerialize = new ObjectToSerialize();
                objectToSerialize.ConnectionSettings = settings.connectionSettings;

                objectToSerialize.ApplicationSettings = settings.applicationSettings;
                objectToSerialize.ViewSettings = settings.viewSettings;

                Serializer serializer = new Serializer();
                serializer.SerializeObject(fileNameAndPath, objectToSerialize);

            }
            catch (Exception)
            {

                // Configure the message box to be displayed
                string messageBoxText = "Die Einstellungen konnten nicht gespeichert werden. Bitte prüfen ob die notwendigen rechte für das speichern vorhanden sind.";
                string caption = "Datei speichern nicht erfolgreich";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;

                // Display message box
                MessageBox.Show(messageBoxText, caption, button, icon);
            }
        }


        /// <summary>
        /// Open up the File Load Dialog and let the user Choose a Settings that he like
        /// </summary>
        /// <returns></returns>
        public static ConnectySetings openSettings()
        {
            ConnectySetings settings;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Connecty Einstellungen laden";
            openFileDialog.DefaultExt = settingsExtension;
            // openFileDialog.Filter = "Settings (*.cs)|";

            if (openFileDialog.ShowDialog() == true)
            {
                return settings = load(openFileDialog.FileName);
            }
            else
            {
                return null;
            }

        }


        /// <summary>
        /// Load Specific Settings
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static ConnectySetings loadSettings(string filePath, string fileName)
        {
            return load(filePath, fileName);
        }

        /// <summary>
        /// Load the Default Settings
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static ConnectySetings loadSettings(string filePath)
        {
            return load(filePath, defaultSettingsName);
        }


        /// <summary>
        /// Safe the Settings
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        private static ConnectySetings load(string filePath, string fileName)
        {
            return load(filePath + fileName);
        }


        /// <summary>
        /// The actual Load Settings function that does all the magic
        /// </summary>
        /// <param name="fileNameAndPath"></param>
        /// <returns></returns>
        private static ConnectySetings load(string fileNameAndPath)
        {
            ConnectySetings loadedSettings = new ConnectySetings();

            try
            {
                ObjectToSerialize objectToSerialize = new ObjectToSerialize();
                Serializer serializer = new Serializer();
                objectToSerialize = serializer.DeSerializeObject(fileNameAndPath);

                loadedSettings.connectionSettings = objectToSerialize.ConnectionSettings;
                loadedSettings.applicationSettings = objectToSerialize.ApplicationSettings;
                loadedSettings.viewSettings = objectToSerialize.ViewSettings;

            }
            catch (Exception)
            {
                // Configure the message box to be displayed
                string messageBoxText = "Die Einstellungen konnten nicht geladen werden.";
                string caption = "Ungültige Settings Datei";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;

                // Display message box
                MessageBox.Show(messageBoxText, caption, button, icon);
            }

            // Return the loaded Settings
            return loadedSettings;
        

        }

     


    }
}
