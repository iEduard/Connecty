using System;
using System.Runtime.Serialization;

namespace Connecty
{
    [Serializable()]
    public class tcpIpSettings : ISerializable
    {

        //  Define the Class Variables
        public int port { get; set; } // Port
        public string ip { get; set; } // IP-Address
        public string clientServerSelection { get; set; } // Client Server selection Client = 1 / Server = 2
        public bool restartTcpServer { get; set; }        // TRUE := Open up the Server again after a Disconnect // FALSE := Stay disconnected



        /// <summary>
        /// The Construktor
        /// </summary>
        public tcpIpSettings()
        {
            // Preset the Settings
            port = 3000;
            ip = "192.168.0.1";
            clientServerSelection = "Client";
            restartTcpServer = true;
        }


        /// <summary>
        /// Load the Binary data to the Variables of the Obkject...
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public tcpIpSettings(SerializationInfo info, StreamingContext ctxt)
        {
            try
            {
                this.port = (int)info.GetValue("tcpIpPort", typeof(int));
                this.ip = (string)info.GetValue("tcpIpIp", typeof(string));
                this.clientServerSelection = (string)info.GetValue("tcpIpServerClient", typeof(string));
                this.restartTcpServer = (bool)info.GetValue("restartTcpServer", typeof(bool));
            }
            catch(Exception Err)
            {
                this.port = 9004;
                this.ip = "127.0.0.1";
                this.clientServerSelection = "Client";
                this.restartTcpServer = true;
            }

        }

        /// <summary>
        /// Add the Values to the Binary Data Info...
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("tcpIpPort", this.port);
            info.AddValue("tcpIpIp", this.ip);
            info.AddValue("tcpIpServerClient", this.clientServerSelection);
            info.AddValue("restartTcpServer", this.restartTcpServer);

        }
    }
}
