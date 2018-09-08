using System;
using System.Collections.Generic;
using System.Linq;

namespace Connecty
{
    class MsgLogHandler
    {

        public ViewSettings viewSettings { get; set; }
        public RingBuffer<MsgData> msgLog; // the Stored in and Out  Messages in an Ringbuffer
        public List<MsgData> sendHistory;
        private int sendMsgHistoryCurrentIndex = -1;// The current Position of the Send Msg History Selection (Presetet to -1 for the First Occured Search to get the First Item at Position 0)

        /// <summary>
        ///  Construtor
        /// </summary>
        public MsgLogHandler(int ringBufferSize, int sendHistorySize)
        {
            // Create a new Ring Buffer with the given size
            msgLog = new RingBuffer<MsgData>(ringBufferSize);

            // Create a new List for the Send Data
            sendHistory = new List<MsgData>(sendHistorySize);
		}
        
        /// <summary>
        ///  Add a new Message Data
        /// </summary>
        public void Add(MsgData message)
        {
            // Add a new MSG Data to the Ringbuffer
            msgLog.Add(message);   
        }

        /// <summary>
        /// This function allows to convert the User Inut String before storing the data in the ringbuffer
        /// </summary>
        /// <param name="stringMessage"></param>
        public MsgData getRawMsgWithCurrentViewSettings(string stringMessage)
        {
            // Define the needed Variables
            MsgData message = new MsgData();
            message.setCurrentTimeStamp();

            // Variables for the Split
            string[] splitValues = new string[] { ",", " " };
            string[] splitedMsg = stringMessage.Split(splitValues, 1024, StringSplitOptions.RemoveEmptyEntries);
            byte[] rawData = new byte[splitedMsg.Length];

            // Convert the User Input to the Current Settings
            // Return the Msg with the Current View Configuration
            switch (viewSettings.dataPresentation)
            {
                default:// ASCII Encoded Strings

                    message.value = Converty.specialAsciiStringToMsgData(stringMessage);
                    break;

                case 1:// HEX Values of the Bytes recived

                    for(int i = 0; i <= splitedMsg.Length - 1; i++)
                    {
                        rawData[i] = Convert.ToByte(splitedMsg[i], 16);
                    }

                    message.value = rawData;

                    break;

                case 2:// DEC Values of the Bytes recived

                    for (int i = 0; i <= splitedMsg.Length - 1; i++)
                    {
                        rawData[i] = Convert.ToByte(splitedMsg[i], 10);
                    }

                    message.value = rawData;

                    break;

                case 3:// BIN Values of the Bytes recived

                    for (int i = 0; i <= splitedMsg.Length - 1; i++)
                    {
                        rawData[i] = Convert.ToByte(splitedMsg[i], 2);
                    }

                    message.value = rawData;

                    break;

            }

            return message;
        }
        
        /// <summary>
        ///  Get the Last Message from the Ringbuffer
        /// </summary>
        /// <returns>Returns the last MSG Data</returns>
        public MsgData getLastMsg()
        {
            // return the last Message as an Message Data Object
            return msgLog.Last();
        }

        /// <summary>
        ///  Get the Last Message from the Ringbuffer
        /// </summary>
        /// <returns></returns>
        public string getLastMsgWithCurrentViewSettings()
        {

            // Get the Last Data and convert it
            return getMsgWithCurrentViewSettings(msgLog.Last());            
        }

        /// <summary>
        ///  Convert the given Msg to the current view Setting as a String
        /// </summary>
        /// <returns></returns>
        public string getMsgWithCurrentViewSettings(MsgData msg)
        {

            string returnMsg;

            // Only change the view to the Sended and Recived Data
            if (msg.type == MsgData.messageType.recived || msg.type == MsgData.messageType.send)
            {
                // Return the Msg with the Current View Configuration
                switch (viewSettings.dataPresentation)
                {
                    case 1:
                        returnMsg = Converty.msgDataToHexData(msg);
                        break;
                    case 2:
                        returnMsg = Converty.msgDataToDecData(msg);
                        break;
                    case 3:
                        returnMsg = Converty.msgDataToBinData(msg);
                        break;
                    // Default View String with ASCII Chars
                    default:
                        returnMsg = Converty.msgDataToSpecialAsciiString(msg.value);
                        break;
                }
            }
            else
            {
                returnMsg = Converty.msgDataToAsciiChar(msg);
            }


            return returnMsg;

        }

        /// <summary>
        ///  Convert the given Msg to the current view Setting as a String
        /// </summary>
        /// <returns></returns>
        public List<MsgHighlightStruct> getMsgHighlightetListWithCurrentViewSettings(MsgData msg)
        {

            List<MsgHighlightStruct> _list = new List<MsgHighlightStruct>();
            MsgHighlightStruct returnStruct;

            // Only change the view to the Sended and Recived Data
            if (msg.type == MsgData.messageType.recived || msg.type == MsgData.messageType.send)
            {
                // Return the Msg with the Current View Configuration
                switch (viewSettings.dataPresentation)
                {
                    case 1:
                        returnStruct.msgAsString = Converty.msgDataToHexData(msg);
                        returnStruct.msgStringType = 0;
                        _list.Add(returnStruct);
                        break;
                    case 2:
                        returnStruct.msgAsString = Converty.msgDataToDecData(msg);
                        returnStruct.msgStringType = 0;
                        _list.Add(returnStruct);
                        break;
                    case 3:
                        returnStruct.msgAsString = Converty.msgDataToBinData(msg);
                        returnStruct.msgStringType = 0;
                        _list.Add(returnStruct);
                        break;
                    // Default View String with ASCII Chars
                    default:
                        //returnMsg = Converty.msgDataToSpecialAsciiString(msg.value);
                        _list = Converty.getMsgDataAsStringWithHighlightetView(msg.value);

                        break;
                }
            }
            else
            {
                returnStruct.msgAsString = Converty.msgDataToAsciiChar(msg);
                returnStruct.msgStringType = 0;
                _list.Add(returnStruct);

            }


            return _list;

        }


        #region Send History

        /// <summary>
        /// Returns the previous MSG in the Send History
        /// </summary>
        /// <returns></returns>
        public string getPriviousSendedMsg()
        {
            // Check if the History allready exist
            if(sendHistory.Count == 0)
            {
                return "";
            }


            MsgData sendMsgHistoryItem = new MsgData();

            // Increment the Index of the History List
            if (sendMsgHistoryCurrentIndex >= sendHistory.Count-1)
            {
                sendMsgHistoryCurrentIndex = 0;
            }
            else
            {
                sendMsgHistoryCurrentIndex++;
            }

            sendMsgHistoryItem = sendHistory.ElementAt(sendMsgHistoryCurrentIndex);


            return getMsgWithCurrentViewSettings(sendMsgHistoryItem);
        }

        /// <summary>
        /// Returns the Data from the Next MSG in the Send History
        /// </summary>
        /// <returns></returns>
        public string getNextSendedMsg()
        {
            // Check if the History allready exist
            if (sendHistory.Count == 0)
            {
                return "";
            }

            MsgData sendMsgHistoryItem = new MsgData();

            // Decrement the Index of the History List
            if (sendMsgHistoryCurrentIndex <= 0)
            {
                sendMsgHistoryCurrentIndex = sendHistory.Count-1;// Keep in mind that the list is 0 Based!!!
            }
            else
            {
                sendMsgHistoryCurrentIndex--;
            }

            sendMsgHistoryItem = sendHistory.ElementAt(sendMsgHistoryCurrentIndex);

            return getMsgWithCurrentViewSettings(sendMsgHistoryItem);
        }

        /// <summary>
        /// By sending a new Message this function is called
        /// And we check if the entered Msg is already Stored in the List then remove it and add it to the First Place
        /// </summary>
        /// <param name="sendMessage"></param>
        public void messageWasSend(MsgData sendMessage)
        {

            // Try to find the Send Msg in the History Log
            sendHistory.Remove(sendMessage); // This is not working!

	        // Create the needed Variables
	        List<int> ListToRemove = new List<int>();
	
	        // Finde the Duplicates in the send History
	        for(int i = 0 ; i <= sendHistory.Count -1 ; i++)
	        {
                MsgData item = sendHistory.ElementAt<MsgData>(i);

		        if(item.value.SequenceEqual(sendMessage.value))
		        {
			        //Store the Index wie have to remove
			        ListToRemove.Add(i);
		        }
	        }
	
	        // Remove the found Duplicates from the History List
	        for(int i = 0 ; i <= ListToRemove.Count -1 ; i++ )
	        {
		        sendHistory.RemoveAt(ListToRemove.ElementAt<int>(i));
            }


            // Insert the new Message to the First Position of the Insert History
            sendHistory.Insert(0, sendMessage);

            // Remove the overlapping Data
            if (sendHistory.Count > 10)
            {
                sendHistory.RemoveAt(sendHistory.Count - 1);
            }


            // Init the Current viewed Histoy Message Index
            sendMsgHistoryCurrentIndex = -1;
        }

        #endregion


    }
}
