using System;
using System.Runtime.Serialization;
using System.Windows;

namespace Connecty
{
    [Serializable()]
    public class ApplicationSettings : ISerializable
    {

        /// <summary>
        ///  Define the Class Variables
        /// </summary>
        public int msgLogRingBufferSize { get; set; }        // Size of the Ringbuffer for the Msg Log
        public int sendHistorySize { get; set; }             // Count of stored Send Messages 
        public Point position { get; set; }            // Position of the Window when the Application was closed
        public double height { get; set; }    
        public double width { get; set; }    
             
        /// <summary>
        /// Constructor
        /// </summary>
        public ApplicationSettings()
        {
            msgLogRingBufferSize = 4096;
            sendHistorySize = 20;

            position = new Point(50, 50);
            height = 500;
            width = 600;

        }

        /// <summary>
        /// Load the Settings from the  inary file...
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public ApplicationSettings(SerializationInfo info, StreamingContext ctxt)
        {
            try
            {
                this.msgLogRingBufferSize = (int)info.GetValue("msgLogRingBufferSize", typeof(int));
            }
            catch
            {
                this.msgLogRingBufferSize = 255;

                Console.WriteLine("Das laden der Buffer Einstellungen hat nicht geklappt");
            }

            try
            {
                this.sendHistorySize = (int)info.GetValue("sendHistorySize", typeof(int));
            }
            catch
            {
                this.sendHistorySize = 20;
                Console.WriteLine("Das laden der Buffer Einstellungen hat nicht geklappt");

            }

            try
            {
                this.position = (Point)info.GetValue("position", typeof(Point));
                this.height = (double)info.GetValue("height", typeof(double));
                this.width = (double)info.GetValue("width", typeof(double));


            }
            catch
            {
                this.position = new Point(500, 500);
                this.height = 700;
                this.width = 600;
                Console.WriteLine("Das laden der Fenster Einstellungen hat nicht geklappt");

            }

        }

        /// <summary>
        /// Add the Settings 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("msgLogRingBufferSize", this.msgLogRingBufferSize);
            info.AddValue("sendHistorySize", this.sendHistorySize);

            info.AddValue("position", this.position);
            info.AddValue("height", this.height);
            info.AddValue("width", this.width);

        }


    }
}
