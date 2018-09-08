using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Windows.Controls;

namespace Connecty
{

    public struct quickButtonXmlParserReturnVal
    {
        /// <summary>
        /// List with all the QuickButtons
        /// </summary>
        public LinkedList<Button> quickButtonsList;

        /// <summary>
        /// Returnvalue 
        /// 0 = No Success with the Load of the Buttons
        /// 1 = Buttons Loaded
        /// </summary>
        public int retVal;

        /// <summary>
        /// Path of the Loaded Buttons
        /// </summary>
        public string pathAndName;
    }


    static class QuickButtonXmlParser
    {



        /// <summary>
        /// Load the Quick buttons with the open File Dialog from Windows
        /// </summary>
        /// <returns>Returns the Linked List of Quick Buttons</returns>
        public static quickButtonXmlParserReturnVal loadQuickButtons()
        {

            quickButtonXmlParserReturnVal _retStruct = new quickButtonXmlParserReturnVal();
            _retStruct.quickButtonsList = new LinkedList<Button>();
            _retStruct.retVal = 0;

            OpenFileDialog _openFileDialog = new OpenFileDialog();
            _openFileDialog.Title = "Quick Buttons öffnen";
            _openFileDialog.DefaultExt = ".xml";
            _openFileDialog.AddExtension = true;

            // If the File Open Dialog was Abborted then Return with an Empty List of QuickButtons
            if (!_openFileDialog.ShowDialog() == true)
            {
                return _retStruct;
            }

            // The File Open Dialog was Closed with an Selection.
            _retStruct.quickButtonsList = _loadQuickButtons(_openFileDialog.FileName);
            _retStruct.pathAndName = _openFileDialog.FileName;
            _retStruct.retVal = 1;
            return _retStruct;



        }

        /// <summary>
        /// Load the Quick buttons with the open File Dialog from Windows
        /// </summary>
        /// <returns>Returns the Linked List of Quick Buttons</returns>
        public static quickButtonXmlParserReturnVal loadQuickButtons(string pathAndFileName)
        {

            quickButtonXmlParserReturnVal _retStruct = new quickButtonXmlParserReturnVal();
            _retStruct.quickButtonsList = new LinkedList<Button>();
            _retStruct.retVal = 0;

            if (pathAndFileName != null)
            {
                _retStruct.pathAndName = pathAndFileName;
                _retStruct.quickButtonsList = _loadQuickButtons(pathAndFileName);
                _retStruct.retVal = 1;
            }

            

            return _retStruct;
        }


        /// <summary>
        /// Local private Method for Loading the Quick buttons with the given path and Name
        /// </summary>
        /// <returns>Returns the Linked List of Quick Buttons</returns>
        private static LinkedList<Button> _loadQuickButtons(string pathAndName)
        {

            // Create the linked List for the Buttons
            LinkedList<Button> _quickys = new LinkedList<Button>();

            // Try to open the Configurations of the Adaptors
            try
            {

                // Get the Current Settings File and read it
                XmlDocument _doc = new XmlDocument();// Create a new XmlDocumentObject
                _doc.Load(pathAndName);//Load the XML Data
                

                // -------------------------------------------------------------------------------
                // Sequenz List
                // -------------------------------------------------------------------------------
                XmlNodeList _nodeList = _doc.DocumentElement.SelectNodes("Button");

                // Read all Sequences 
                for (int i = 0; i < _nodeList.Count; i++)
                {

                    Button _quickbutton = new Button();

                    // Get the Name
                    XmlAttributeCollection _nodeAttribute = _nodeList[i].Attributes;

                    _quickbutton.Content = _nodeList[i].SelectSingleNode("Name").InnerText;
                    _quickbutton.Resources.Add("data", _nodeList[i].SelectSingleNode("Daten").InnerText);

                    _quickys.AddLast(_quickbutton);
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine("ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss");
                Console.WriteLine(ex);
                Console.WriteLine("Tester: Adaptor Settings (" + pathAndName + ") colud not be loaded");
                Console.WriteLine("eeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee");
            }

            return _quickys;
        }


        /// <summary>
        /// Save the Current Quickbuttons via Windows save File Dialog
        /// </summary>
        /// <param name="quickButtonsList">List of Quickbuttons to Save</param>
        public static string saveQuickButtons(LinkedList<Button> quickButtonsList)
        {


            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Connecty Simulation speichern";
            saveFileDialog.DefaultExt = ".xml";
            saveFileDialog.AddExtension = true;
            // saveFileDialog.Filter = "Settings (*.cs)|";

            if (!saveFileDialog.ShowDialog() == true)
            {
                return "";
            }


            _saveQuickButtons(quickButtonsList, saveFileDialog.FileName);
            return saveFileDialog.FileName;
        }


        /// <summary>
        /// Save the Current Quickbuttons via Windows save File Dialog
        /// </summary>
        /// <param name="quickButtonsList">List of Quickbuttons to Save</param>
        public static void saveQuickButtons(LinkedList<Button> quickButtonsList, string pathAndName)
        {

            // Call the private method
            _saveQuickButtons(quickButtonsList, pathAndName);

        }


        /// <summary>
        /// Save the Current Quickbuttons via Windows save File Dialog
        /// </summary>
        /// <param name="quickButtonsList">List of Quickbuttons to Save</param>
        private static void _saveQuickButtons(LinkedList<Button> quickButtonsList, string pathAndName)
        {

            if (pathAndName == null)
            {
                return;
            }


            XmlDocument doc = new XmlDocument();

            //(1) the xml declaration is recommended, but not mandatory
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            //(2) string.Empty makes cleaner code
            XmlElement _xmlBody = doc.CreateElement(string.Empty, "Quickys", string.Empty);
            doc.AppendChild(_xmlBody);


            foreach (Button _button in quickButtonsList)
            {

                XmlElement _xmlButton = doc.CreateElement(string.Empty, "Button", string.Empty);
                _xmlBody.AppendChild(_xmlButton);

                XmlElement _xmlName = doc.CreateElement(string.Empty, "Name", string.Empty);
                XmlText text1 = doc.CreateTextNode(_button.Content.ToString());
                _xmlName.AppendChild(text1);
                _xmlButton.AppendChild(_xmlName);

                XmlElement _xmlDaten = doc.CreateElement(string.Empty, "Daten", string.Empty);
                XmlText text2 = doc.CreateTextNode((string)_button.Resources["data"]);
                _xmlDaten.AppendChild(text2);
                _xmlButton.AppendChild(_xmlDaten);

                XmlElement _xmlShortCut = doc.CreateElement(string.Empty, "Daten", string.Empty);
                XmlText text3 = doc.CreateTextNode("F2");
                _xmlShortCut.AppendChild(text3);
                _xmlButton.AppendChild(_xmlShortCut);


            }

            doc.Save(pathAndName);

        }


    }
}
