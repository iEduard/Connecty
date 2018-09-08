using System;
using System.Runtime.Serialization;


namespace Connecty
{
    [Serializable()]
    public class SingleConnectionSettings : ISerializable
    {
        /// <summary>
        /// 1 = TCP IP / 2 = RS232
        /// </summary>
        public int currentConnectionSetting { get; set; }
        public String connectionName { get; set; }
        public rs232Settings serialSettings { get; set; } // Com Port Settings
        public tcpIpSettings tcpSettings { get; set; } // TCP Settings


        /// <summary>
        /// Construtor
        /// </summary>
        public SingleConnectionSettings()
        {
            currentConnectionSetting = 1;
            serialSettings = new rs232Settings();
            tcpSettings = new tcpIpSettings();

        }

        /// <summary>
        /// Function that returns a Human readable Text of the Current Settungs
        /// This Text can be displayed to the user
        /// </summary>
        /// <returns></returns>
        public string getSettingsInfo()
        {
            string sSettingsAsText;

            if (currentConnectionSetting == 1)
            {
                sSettingsAsText = "TCP: " + tcpSettings.clientServerSelection + " @ ";

                if (tcpSettings.clientServerSelection == "Client")
                {
                    sSettingsAsText += "IP:" + tcpSettings.ip;
                }

                else if (tcpSettings.clientServerSelection == "Server")
                {
                    sSettingsAsText += "Alle vorhandenen IP's";
                }

                sSettingsAsText += (" || Port: " + tcpSettings.port.ToString());

            }
            else if (currentConnectionSetting == 2)
            {
                sSettingsAsText = "Seriell: @ " + serialSettings.port;
            }
            else
            {
                sSettingsAsText = "";
            }

            return sSettingsAsText;
        }

        /// <summary>
        /// Serialize helper function to get the saved data from the File
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public SingleConnectionSettings(SerializationInfo info, StreamingContext ctxt)
        {
            try
            {
                this.currentConnectionSetting = (int)info.GetValue("connection", typeof(int));

            }
            catch
            {
                this.currentConnectionSetting = 1;
                Console.WriteLine("Das laden der verbindungseinstellung hat nicht geklappt");
            }


            try
            {
                this.serialSettings = (rs232Settings)info.GetValue("rs232Settings", typeof(rs232Settings));
            }
            catch
            {
                this.serialSettings = new rs232Settings();
                Console.WriteLine("Das laden der verbindungseinstellung hat nicht geklappt");
            }


            try
            {
                this.tcpSettings = (tcpIpSettings)info.GetValue("tcpIpSettings", typeof(tcpIpSettings));
            }
            catch
            {
                this.tcpSettings = new tcpIpSettings();
                Console.WriteLine("Das laden der verbindungseinstellung hat nicht geklappt");
            }

            try
            {
                this.connectionName = (String)info.GetValue("connectionName", typeof(String));
            }
            catch
            {
                this.connectionName = "Verbindung 1";
                Console.WriteLine("Das laden des connectionName hat nicht geklappt");
            }


        }

        /// <summary>
        /// Serialize helper function to write the data to the File
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("connection", this.currentConnectionSetting);
            info.AddValue("connectionName", this.connectionName);
            info.AddValue("rs232Settings", this.serialSettings);
            info.AddValue("tcpIpSettings", this.tcpSettings);
        }


    }
}
