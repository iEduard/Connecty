using System;
using System.Runtime.Serialization;


namespace Connecty
{
    [Serializable()]
    public class ConnectionSettings : ISerializable
    {

        public SingleConnectionSettings connection1 { get; set; }
        public SingleConnectionSettings connection2 { get; set; }


        /// <summary>
        /// 0 = Sinle Connection (Simply Connecty)
        /// 1 = Pass Through mode (Connecty in the Middle)
        /// 2 = Simulation mode (Simple Connecty with an Simulated endPoint)
        /// </summary>
        public int functionSelect { get; set; }              // Functionality Selection...

        /// <summary>
        /// Constructor
        /// </summary>
        public ConnectionSettings()
        {
            connection1 = new SingleConnectionSettings();
            connection1.connectionName = "Verbindung 1";
            connection2 = new SingleConnectionSettings();
            connection2.connectionName = "Verbindung 2";
        }

        /// <summary>
        /// Function that returns a Human readable Text of the Current Settungs
        /// This Text can be displayed to the user
        /// </summary>
        /// <returns></returns>
        public string getSettingsInfo()
        {
            string sSettingsAsText;

            if (functionSelect == 0)
            {
                sSettingsAsText = connection1.getSettingsInfo();
            }
            else 
            {
                sSettingsAsText = "Connecty in the Middle";
            }

            return sSettingsAsText;
        }

        /// <summary>
        /// Serialize helper function to get the saved data from the File
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public ConnectionSettings(SerializationInfo info, StreamingContext ctxt)
        {
            try
            {
                this.connection1 = (SingleConnectionSettings)info.GetValue("connection1", typeof(SingleConnectionSettings));
            }
            catch
            {
                this.connection1 = new SingleConnectionSettings();
                this.connection1.connectionName = "Verbindung 1";
            }

            try
            {
                this.connection2 = (SingleConnectionSettings)info.GetValue("connection2", typeof(SingleConnectionSettings));
            }
            catch
            {
                this.connection2 = new SingleConnectionSettings();
                this.connection2.connectionName = "Verbindung 2";
            }

            try
            {
                this.functionSelect = (int)info.GetValue("functionSelect", typeof(int));
            }
            catch
            {
                this.functionSelect = 0;
                Console.WriteLine("Das laden der Funktions Einstellungen hat nicht geklappt");

            }

        }

        /// <summary>
        /// Serialize helper function to write the data to the File
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {

            info.AddValue("connection1", this.connection1);
            info.AddValue("connection2", this.connection2);
            info.AddValue("functionSelect", this.functionSelect);

        }


    }
}
