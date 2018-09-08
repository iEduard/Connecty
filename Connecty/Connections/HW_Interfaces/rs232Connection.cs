using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace Connecty
{
    class rs232Connection : IDisposable
    {
        private SerialPort pcSerialPort = new SerialPort();
        public rs232Settings pcSerialParam { get; set; }
        public int interfaceNumber { get; set; }// Number of the Connection Interface

        private bool disposed = false; // to detect redundant calls

        /// <summary>
        /// Constructor
        /// </summary>
        public rs232Connection()
        {
            // Init the local Variables
            pcSerialParam = new rs232Settings();
        }

        /// <summary>
        /// Connect the Serial Port
        /// </summary>
        public void connect()
        {
            MsgData logMessage = new MsgData();

            // Check if we have to close the Serial Port!
            if (pcSerialPort != null)
            {
                if (pcSerialPort.IsOpen)
                {
                    pcSerialPort.Close();
                }
            }


            // Open the Serial Port
            try
            {

                // Set the Serial Paramter
                pcSerialPort.PortName = pcSerialParam.port;
                pcSerialPort.BaudRate = pcSerialParam.baud;
                pcSerialPort.DataBits = pcSerialParam.dataBits;
                pcSerialPort.Parity = pcSerialParam.parity;
                pcSerialPort.StopBits = pcSerialParam.stopBits;

                // Add a new ComPort on Recive Handler
                pcSerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                pcSerialPort.Open();

                // Set the User Hint
                logMessage.value = Encoding.ASCII.GetBytes("Verbindung: " + pcSerialPort.PortName + " Erfolgreich aufgebaut");
                logMessage.type = MsgData.messageType.infoPositive;

                // Set the Event that the User Changed an Input
                MsgSendRecivedEventArgs msgLogEventArgs = new MsgSendRecivedEventArgs();
                msgLogEventArgs.msgData = logMessage;
                msgSendRecived(msgLogEventArgs);

            }
            catch (Exception)
            {
                // Set a User Hint
                logMessage.value = Encoding.ASCII.GetBytes("Verbindung: " + pcSerialPort.PortName + " Konnte nicht aufgebaut werden");
                logMessage.type = MsgData.messageType.infoNegative;


                // Set the Event that the User Changed an Input
                MsgSendRecivedEventArgs msgLogEventArgs = new MsgSendRecivedEventArgs();
                msgLogEventArgs.msgData = logMessage;
                msgSendRecived(msgLogEventArgs);
            }
        }

        /// <summary>
        /// Disconnect the Serial Port
        /// </summary>
        public void disconnect()
        {

            // Check if we have to close the Serial Port!
            if (pcSerialPort != null)
            {
                if (pcSerialPort.IsOpen)
                {
                    pcSerialPort.Close();

                    // Set the User hint
                    MsgData logMessage = new MsgData();
                    logMessage.value = Encoding.ASCII.GetBytes("Verbindung: " + pcSerialPort.PortName + " Erfolgreich abgebaut");
                    logMessage.type = MsgData.messageType.infoNegative;

                    // Set the Event that the User Changed an Input
                    MsgSendRecivedEventArgs msgLogEventArgs = new MsgSendRecivedEventArgs();
                    msgLogEventArgs.msgData = logMessage;
                    msgSendRecived(msgLogEventArgs);

                }
            }
        }

        /// <summary>
        /// Send Data over Serial Interface
        /// </summary>
        /// <param name="data"></param>
        public void send(MsgData message)
        {

            if (pcSerialPort.IsOpen)
            {
                try
                {
                    // Send the Data over Serial and to the UI
                    pcSerialPort.Write(message.value, 0, message.value.Length);// Send the Data after replacing the Human Readable ASCII Chars
                    

                    message.setCurrentTimeStamp();// Set the Time Stamp
                    message.connectionNumber = interfaceNumber;// Set the Interface Reference

                    // Set the Event that the User Changed an Input
                    MsgSendRecivedEventArgs msgLogEventArgs = new MsgSendRecivedEventArgs();
                    msgLogEventArgs.msgData = message;
                    msgSendRecived(msgLogEventArgs);

                }
                catch (Exception ex)
                {
                    MsgData logMessage = new MsgData();

                    // Set an error Message to the UI
                    logMessage.value = Encoding.ASCII.GetBytes("Konnte nicht gesendet werden: " + message.value + "\n" + ex);
                    logMessage.type = MsgData.messageType.infoNegative;

                    // Set the Event that the User Changed an Input
                    MsgSendRecivedEventArgs msgLogEventArgs = new MsgSendRecivedEventArgs();
                    msgLogEventArgs.msgData = message;
                    msgSendRecived(msgLogEventArgs);

                }
            }

        }

        /// <summary>
        /// Data Recive Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            MsgData logMessage = new MsgData();
            byte[] message = new byte[4096];
            int bytesRead;

            try
            {
                // Read the Data
                bytesRead = pcSerialPort.Read(message, 0, 4096);

                // Set the Current TimeStamp
                logMessage.setCurrentTimeStamp();

                // Save the Data to the 
                logMessage.value = new byte[bytesRead];// Create an Array with the Size of readed Data
                Array.Copy(message, logMessage.value, bytesRead);// Copy the Data to the Array
                logMessage.type = MsgData.messageType.recived;//Set the Type to Recived Message
                logMessage.connectionNumber = interfaceNumber;// Set the Interface Reference

                // Set the Event that the User Changed an Input
                MsgSendRecivedEventArgs msgLogEventArgs = new MsgSendRecivedEventArgs();
                msgLogEventArgs.msgData = logMessage;
                msgSendRecived(msgLogEventArgs);


            }
            catch (Exception ex)
            {
                // Set an error Message to the UI
                logMessage.value = Encoding.ASCII.GetBytes("Konnte nicht empfangen werden: " + "\n" + ex);
                logMessage.type = MsgData.messageType.infoNegative;

                // Set the Event that the User Changed an Input
                MsgSendRecivedEventArgs msgLogEventArgs = new MsgSendRecivedEventArgs();
                msgLogEventArgs.msgData = logMessage;
                msgSendRecived(msgLogEventArgs);

            }



        }

        /// <summary>
        /// Returns the current connection State
        /// </summary>
        /// <returns></returns>
        public int getConnectionState()
        {
            int connectionState;

            // Update the UI StatusBar
            if (pcSerialPort == null)
            {
                connectionState = 0;
            }
            else
            {
                if (pcSerialPort.IsOpen)
                {
                    connectionState = 3;
                }
                else
                {
                    connectionState = 0;
                }

            }

            return connectionState;

        }


        #region On Change Eventhandler

        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event MsgSendRecivedEventHandler MsgSendRecived;

        // Invoke the Changed event; called whenever list changes
        protected virtual void msgSendRecived(MsgSendRecivedEventArgs e)
        {
            if (MsgSendRecived != null)
                MsgSendRecived(this, e);
        }

        #endregion


        #region Disposal

        /// <summary>
        /// Clean up the required Objects
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (pcSerialPort != null)
                    {
                        pcSerialPort.Dispose();
                    }
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Public Dispose Method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

    }
}
