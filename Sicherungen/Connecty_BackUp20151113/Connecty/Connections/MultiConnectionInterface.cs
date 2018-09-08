using System;
using System.Threading;
using System.Threading.Tasks;

namespace Connecty
{
    class MultiConnectionInterface
    {

        public ConnectionSettings connectionSettings { get; set; }
        private ConnectionInterface connection1; // Interface for the Connections
        private ConnectionInterface connection2; // Interface for the Connections

        /// <summary>
        /// Construktor
        /// </summary>
        /// <param name="obj"></param>
        public MultiConnectionInterface()
        {
            connection1 = new ConnectionInterface(1);
            connection1.interfaceNumber = 1;
            connection1.MsgSendRecived += new MsgSendRecivedEventHandler(SendRecivedEventHandler);

            connection2 = new ConnectionInterface(2);
            connection2.interfaceNumber = 2;
            connection2.MsgSendRecived += new MsgSendRecivedEventHandler(SendRecivedEventHandler);
        }

        /// <summary>
        /// Connect Method
        /// </summary>
        public void Connect()
        {

            // First of all we call the Disconnect. Maybe their is a Connection allready Running
            Disconnect();

            // Set the Connection Settings
            connection1.connectionSettings = connectionSettings.connection1; // Set the Current Settings to the Connection Object
            connection2.connectionSettings = connectionSettings.connection2; // Set the Current Settings to the Connection Object

            // Start the Connections
            connection1.Connect();
            connection2.Connect();
        }

        /// <summary>
        /// Disconnecting Method
        /// </summary>
        public void Disconnect()
        {
            // Disconnect the Connections
            connection1.Disconnect();
            connection2.Disconnect();
        }

        /// <summary>
        /// Try to send the Data to the Requested Connection
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public int Send(MsgData message)
        {
            if (message.connectionNumber == 1)
            {
                connection1.Send(message);
            }
            else
            {
                connection2.Send(message);
            }

            return 1;
        }


        /// <summary>
        /// Function to ask for the Current Connection State of the Connection 1
        /// </summary>
        /// <returns>The Current Connection State of the Connection 1</returns>
        public int getConnection1State()
        {

            int currentConnectionState = 0;

            if (connectionSettings.connection1.currentConnectionSetting == 1)
            {
                if (connection1.tcpConnection != null)
                {
                    currentConnectionState = connection1.tcpConnection.getConnectionState();
                }
            }
            else if (connectionSettings.connection1.currentConnectionSetting == 2)
            {
                if (connection1.serialConnection != null)
                {
                    currentConnectionState = connection1.serialConnection.getConnectionState();
                }
            }

            return currentConnectionState;

        }

        /// <summary>
        /// Function to ask for the Current Connection State of the Connection 2
        /// </summary>
        /// <returns>The Current Connection State of the Connection 2</returns>
        public int getConnection2State()
        {

            int currentConnectionState = 0;

            if (connectionSettings.connection2.currentConnectionSetting == 1)
            {
                if (connection2.tcpConnection != null)
                {
                    currentConnectionState = connection2.tcpConnection.getConnectionState();
                }
            }
            else if (connectionSettings.connection2.currentConnectionSetting == 2)
            {
                if (connection2.serialConnection != null)
                {
                    currentConnectionState = connection2.serialConnection.getConnectionState();
                }
            }

            return currentConnectionState;

        }

        /// <summary>
        /// Redirect the Log Messages Fromm the Connections
        /// </summary>
        /// <param name="logMessage"></param>
        public void SendRecivedEventHandler(object sender, MsgSendRecivedEventArgs e)
        {

            ConnectionInterface connection = sender as ConnectionInterface;

            if (e.msgData.type == MsgData.messageType.recived)
            {

                // Create a new Send Object!
                MsgData sendData = new MsgData(e.msgData.value, MsgData.messageType.redirect);

                if (connection.interfaceNumber == 1)
                {
                    connection2.Send(sendData);
                }
                else
                {
                    connection1.Send(sendData);
                }

                // Update the MsgLog bevore we send the Data to the Corresponding Data
                msgSendRecived(e);


            }
            else if (e.msgData.type == MsgData.messageType.send)
            {
                // Update the Msg Log with the injected Message by the User
                msgSendRecived(e);
            }
            else if (e.msgData.type == MsgData.messageType.infoPositive || e.msgData.type == MsgData.messageType.infoNegative)
            {
                msgSendRecived(e);
            }
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


    }
}
