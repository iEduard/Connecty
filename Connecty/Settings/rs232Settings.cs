using System;
using System.IO.Ports;
using System.Runtime.Serialization;

namespace Connecty
{
    [Serializable()]
    public class rs232Settings : ISerializable
    {

        /// <summary>
        ///  Define the Class Variables
        /// </summary>
        public string port { get; set; }              // Com Port
        public int baud { get; set; }              // Baut Rate of the ComPort
        public int dataBits { get; set; }          // Amount of Data Bits
        public StopBits stopBits { get; set; }     // Amount of Stop Bits
        public Parity parity { get; set; }         // Parity
        public int readTimeOut { get; set; }       // TimeOut for the Read on the RS232
 

        /// <summary>
        /// Constructor
        /// </summary>
        public rs232Settings()
        {
            // set the Defaults
            port = "COM1";
            baud = 19200;        // Set the Baudrate to 9600
            dataBits = 8;       // Set the DataBits to 8
            stopBits = System.IO.Ports.StopBits.One;      // Set the StopBits to 1
            parity = System.IO.Ports.Parity.Odd;         // Set the Default Parity to None
            readTimeOut  = 200;   // Set the Default Read Timeout to 200ms
            
        }


        public rs232Settings(SerializationInfo info, StreamingContext ctxt)
        {
            this.port = (string)info.GetValue("serialPort", typeof(string));
            this.baud = (int)info.GetValue("serialBaud", typeof(int));
            this.dataBits = (int)info.GetValue("serialDataBits", typeof(int));
            this.stopBits = (StopBits)info.GetValue("serialStopBits", typeof(StopBits));
            this.parity = (Parity)info.GetValue("serialParity", typeof(Parity));
            this.readTimeOut = (int)info.GetValue("readTimeOut", typeof(int));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("serialPort", this.port);
            info.AddValue("serialBaud", this.baud);
            info.AddValue("serialDataBits", this.dataBits);
            info.AddValue("serialStopBits", this.stopBits);
            info.AddValue("serialParity", this.parity);
            info.AddValue("readTimeOut", this.readTimeOut);
        }

        /// <summary>
        /// Returns teh German Name for the Current Parity Setting
        /// {"Ohne", "Gerade", "Ungerade", "Fest 0", "Fest 1" };
        /// </summary>
        /// <returns></returns>
        public string getParityAsString()
        {
            string currentParityName;

            switch (parity)
            {
                case System.IO.Ports.Parity.Even:
                    currentParityName = "Gerade";
                    break;
                case System.IO.Ports.Parity.Mark:
                    currentParityName = "Fest 1";
                    break;
                case System.IO.Ports.Parity.None:
                    currentParityName = "Ohne";
                    break;
                case System.IO.Ports.Parity.Odd:
                    currentParityName = "Ungerade";
                    break;
                case System.IO.Ports.Parity.Space:
                    currentParityName = "Fest 0";
                    break;
                default:
                    parity = System.IO.Ports.Parity.None; // No valid Parity found! We will set it to None!
                    currentParityName = "Ohne";
                    break;
            }


            // Return the found Value
            return currentParityName;
        }

        /// <summary>
        /// Set the Parity with a String!
        /// {"Ohne", "Gerade", "Ungerade", "Fest 0", "Fest 1" };
        /// </summary>
        /// <returns></returns>
        public void setParityWithString(string currentParityName)
        {
            switch (currentParityName)
            {
                case "Gerade":
                    parity = System.IO.Ports.Parity.Even;
                    break;
                case "Fest 1":
                    parity = System.IO.Ports.Parity.Mark;
                    break;
                case "Ohne":
                    parity = System.IO.Ports.Parity.None;
                    break;
                case "Ungerade":
                    parity = System.IO.Ports.Parity.Odd;
                    break;
                case "Fest 0":
                    parity = System.IO.Ports.Parity.Space;
                    break;
                default:
                    parity = System.IO.Ports.Parity.None; // No valid Parity found! We will set it to None!
                    currentParityName = "Ohne";
                    break;
            }
        }


        /// <summary>
        /// Returns teh German Name for the Current StopBits Setting
        /// { "Ohne", "1", "1,5", "2"};
        /// </summary>
        /// <returns></returns>
        public string getStopBitsAsString()
        {
            string currentStopBitsName;

            switch (stopBits)
            {
                case System.IO.Ports.StopBits.None:
                    currentStopBitsName = "Ohne";
                    break;
                case System.IO.Ports.StopBits.One:
                    currentStopBitsName = "1";
                    break;
                case System.IO.Ports.StopBits.OnePointFive:
                    currentStopBitsName = "1,5";
                    break;
                case System.IO.Ports.StopBits.Two:
                    currentStopBitsName = "2";
                    break;
                default:
                    stopBits = System.IO.Ports.StopBits.One; // No valid StopBit found! We will set it to None!
                    currentStopBitsName = "1";
                    break;
            }


            // Return the found Value
            return currentStopBitsName;
        }

        /// <summary>
        /// Set the Stopbits with a String!
        /// { "Ohne", "1", "1,5", "2"};
        /// </summary>
        /// <returns></returns>
        public void setStopBitsWithString(string currentStopBitName)
        {
            switch (currentStopBitName)
            {
                case "Ohne":
                    stopBits = System.IO.Ports.StopBits.None;
                    break;
                case "1":
                    stopBits = System.IO.Ports.StopBits.One;
                    break;
                case "1,5":
                    stopBits = System.IO.Ports.StopBits.OnePointFive;
                    break;
                case "2":
                    stopBits = System.IO.Ports.StopBits.Two;
                    break;
                default:
                    stopBits = System.IO.Ports.StopBits.One; // No valid Parity found! We will set it to None!
                    currentStopBitName = "1";
                    break;
            }
        }
    }
}
