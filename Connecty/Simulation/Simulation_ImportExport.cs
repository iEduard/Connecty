using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace Connecty
{

    /// <summary>
    ///  Function to read / load the Simulatoin konfiguration
    /// </summary>
    public class Simulation_ImportExport
    {

        // Define some String Constants for the Syntax
        const string startSequence = "#StartAblauf";
        const string endSequence = "#EndeAblauf";
        const string waitForMsg = "WarteAuf";
        const string sendMsg = "Sende";
        const string delayMsg = "Warte";

        private static string simulationExtension = ".txt";// Experimental

        private LinkedList<Simulation_Job> sequenceJobs = new LinkedList<Simulation_Job>();


        #region Impoprt Simulation Data

        /// <summary>
        /// Read the Simulation Konfiguration and add the Jobs to the linked list
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public LinkedList<Simulation_Job> ReadSimulationKonfiguration()
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Simulation importieren";
            openFileDialog.DefaultExt = simulationExtension;
            openFileDialog.Filter = "Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                sequenceJobs = ReadSimulationFile(openFileDialog.FileName);
            }

            // Return the Jobs in a Linked List
            return sequenceJobs;

        }


        /// <summary>
        /// Read the Simulation Konfiguration and add the Jobs to the linked list
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public LinkedList<Simulation_Job> ReadSimulationKonfiguration(string filePath)
        {
            // Read the Konfiguraton
            sequenceJobs = ReadSimulationFile(filePath);
            // Return the Jobs in a Linked List
            return sequenceJobs;
        }


        /// <summary>
        /// Function to check the Plausebility
        /// </summary>
        /// <returns></returns>
        public bool checkPlauseability(StreamReader file)
        {

            bool dataIsValid = true;

            // Read the complete Data to a String
            string SimulationData = file.ReadToEnd();

            int startPoint = SimulationData.IndexOf(startSequence);
            int endPoint = SimulationData.IndexOf(startSequence);


            if ( (SimulationData.IndexOf(startSequence) != SimulationData.LastIndexOf(startSequence))
                || SimulationData.IndexOf(startSequence) == -1)
            {
                dataIsValid = false;

            }

            if ((SimulationData.IndexOf(endSequence) != SimulationData.LastIndexOf(endSequence))
                || SimulationData.IndexOf(endSequence) == -1)
            {
                dataIsValid = false;
            }

            return dataIsValid;

        }

        /// <summary>
        /// Read the Simulation Konfiguration and add the Jobs to the linked list
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private LinkedList<Simulation_Job> ReadSimulationFile(string filePath)
        {
            // Create and Init some Variables
            int counter = 0;// Counter Variable
            string line;// Current Line

            bool startFound = false;// True = we found the Start Value of the Simulation File
            bool endFound = false;// True = we found the End Value of the Simulation File

            try
            {

                using (StreamReader file = new StreamReader(filePath))
                {
                    //Read the Data Line by Line 
                    while ((line = file.ReadLine()) != null)
                    {

                        if (line.Contains(startSequence))
                        {
                            startFound = true;
                        }

                        if (line.Contains(endSequence))
                        {
                            endFound = true;
                        }

                        // If we are in the Sequence we check for the Requested Data
                        if (startFound == true && endFound == false)
                        {
                            checkForSimulationWork(line);
                        }

                        //Console.WriteLine(line);
                        counter++;
                    }
                }


            }
            catch
            {
                sequenceJobs = null;
            }


            // Return the Jobs in a Linked List
            return sequenceJobs;

        }

        /// <summary>
        /// Check for the Jobs in the Textfile
        /// </summary>
        /// <param name="inputData"></param>
        private void checkForSimulationWork(string inputData)
        {

            if (inputData.Contains(waitForMsg))
            {
                sequenceJobs.AddLast(new Simulation_Job(Smimulation_SequenceType.WaitFor, getMsgData(inputData)));
            }
            else if (inputData.Contains(sendMsg))
            {
                sequenceJobs.AddLast(new Simulation_Job(Smimulation_SequenceType.Send, getMsgData(inputData)));
            }
            else if (inputData.Contains(delayMsg))
            {
                sequenceJobs.AddLast(new Simulation_Job(Smimulation_SequenceType.Delay, getMsgData(inputData)));
            }

        }

        /// <summary>
        /// Get the Substring of the Jobs
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        private string getMsgData(string inputData)
        {

            int startIndex = inputData.IndexOf("(");
            int endIndex = inputData.IndexOf(")");

            return inputData.Substring(startIndex + 1, endIndex - startIndex - 1);

        }
            
        #endregion


        #region Export Simulation Data
           
        /// <summary>
        /// Export Data to a File
        /// </summary>
        public bool ExportSimulationDataToFileSystem(LinkedList<Simulation_Job> sequenceJobs)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Connecty Simulation speichern";
            saveFileDialog.DefaultExt = simulationExtension;
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "Textdateien (*.txt)|*.txt|Alle Dateien (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                save(saveFileDialog.FileName, sequenceJobs);
            }

            return false;
        }


        /// <summary>
        /// Function where the user chooses the Name and the Path of the Settings File
        /// </summary>
        /// <param name="settings"></param>
        private void save(string path, LinkedList<Simulation_Job> sequenceJobs)
        {

            // Write the string to a file.
            using (StreamWriter file = new StreamWriter(path))
            {
                file.WriteLine(startSequence);
                file.WriteLine("");

                foreach (Simulation_Job job in sequenceJobs)
                {


                    switch (job.Type)
                    {

                        case Smimulation_SequenceType.WaitFor:
                            file.Write("\t" + waitForMsg);
                            break;

                        case Smimulation_SequenceType.Send:
                            file.Write("\t" + sendMsg);
                            break;

                        case Smimulation_SequenceType.Delay:
                            file.Write("\t" + delayMsg);
                            break;

                    }

                    file.Write("(");
                    file.Write(job.Value);
                    file.WriteLine(")");
                }

                file.WriteLine("");
                file.Write(endSequence);
            }
        }
        
        #endregion

    }
}