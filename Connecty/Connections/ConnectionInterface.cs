using System;

namespace Connecty
{

    /// <summary>
    /// This Function is used to Handle the Different Connections through only one Interface
    /// </summary>
    class ConnectionInterface : IDisposable
    {
        // Define some Variables 
        public SingleConnectionSettings connectionSettings { get; set; }
        public TcpConnection tcpConnection;// Connection Object for the TCP IP Connection
        public rs232Connection serialConnection;// Connection Object for the Serial Connection
        public int interfaceNumber { get; set; }// Number of the Connection Interface

        private bool disposed = false; // to detect redundant calls


        /// <summary>
        /// Construktor
        /// </summary>
        /// <param name="obj"></param>
        public ConnectionInterface(int interfaceRef) // MainWindow obj
        {
            interfaceNumber = interfaceRef;
        }

        /// <summary>
        /// Connect Method
        /// </summary>
        public void Connect()
        {

            // First of all we call the Disconnect. Maybe their is a Connection allready Running
            Disconnect();

            if (connectionSettings.currentConnectionSetting == 1)
            {
                // If the Object Ref is NUll create a new one
                if (tcpConnection == null)
                {
                    tcpConnection = new TcpConnection();
                    tcpConnection.interfaceNumber = interfaceNumber;
                    tcpConnection.MsgSendRecived += new MsgSendRecivedEventHandler(SendRecivedEventHandler);
                }

                tcpConnection.settings = connectionSettings.tcpSettings;
                tcpConnection.connect();
            }
            else if (connectionSettings.currentConnectionSetting == 2)
            {
                if (serialConnection == null)
                {
                    // First try to Disconnect
                    serialConnection = new rs232Connection();
                    serialConnection.interfaceNumber = interfaceNumber;
                    serialConnection.MsgSendRecived += new MsgSendRecivedEventHandler(SendRecivedEventHandler);
                }

                serialConnection.pcSerialParam = connectionSettings.serialSettings;
                serialConnection.connect();

            }
        }

        /// <summary>
        /// Disconnecting Method
        /// </summary>
        public void Disconnect()
        {
            if (tcpConnection != null)
            {
                tcpConnection.disconnect();
            }

            if (serialConnection != null)
            {
                serialConnection.disconnect();
            }
        }

        /// <summary>
        /// Try to send the Data to the Requested Connection
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public int Send(MsgData message)
        {

            if (connectionSettings.currentConnectionSetting == 1)
            {
                if (tcpConnection != null)
                {
                    tcpConnection.send(message);
                }
            }
            else if (connectionSettings.currentConnectionSetting == 2)
            {
                if (serialConnection != null)
                {
                    serialConnection.send(message);
                }
            }

            return 1;

        }

        /// <summary>
        /// Redirect the Log Messages Fromm the Connections
        /// </summary>
        /// <param name="logMessage"></param>
        public void SendRecivedEventHandler(object sender, MsgSendRecivedEventArgs e)
        {

            // Set the Event that the User Changed an Input
            msgSendRecived(e);

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
                    if (tcpConnection != null)
                    {
                        tcpConnection.Dispose(); ;
                    }
                    if (serialConnection != null)
                    {
                        serialConnection.Dispose(); ;
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
