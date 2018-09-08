using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connecty
{
    public class MsgData
    {
        /// <summary>
        ///  Define the Class Variables
        /// </summary>
        public DateTime timeStamp { get; set; } // Time Stamp of the Message


        /// <summary>
        /// Type Definition of the MSG / recived / send / redirect / infoPositive / infoNegative
        /// </summary>
        public enum messageType { recived, send, redirect, infoPositive, infoNegative };
        public messageType type { get; set; } // Type Definition of the MSG / 0 = Recived / 1 = Send / 2 = Info Positive / 3 = Info Negativ
        public byte[] value { get; set; } // The Message it self 
        public int connectionNumber { get; set; } // The Number of the Connection for this Message

        /// <summary>
        /// Empty Constructor
        /// </summary>
        public MsgData()
        {
        }


        /// <summary>
        ///  Construtor
        /// </summary>
        public MsgData(byte[] MessageValue, messageType MessageType)
        {
            value = MessageValue;
            type = MessageType;
            setCurrentTimeStamp();
        }
        
        
        /// <summary>
        /// Save the current TimeStamp
        /// </summary>
        public void setCurrentTimeStamp()
        {
            // Store the Current Time Stamp
            timeStamp = DateTime.Now;
        }
        

    }
}
