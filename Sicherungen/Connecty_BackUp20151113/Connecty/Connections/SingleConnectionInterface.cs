using System.Threading;
using System.Threading.Tasks;

namespace Connecty
{
    class SingleConnectionInterface
    {

        public ConnectionSettings connectionSettings { get; set; }
        private ConnectionInterface connection1; // Interface for the Connections


        /// <summary>
        /// Construktor
        /// </summary>
        /// <param name="obj"></param>
        public SingleConnectionInterface()
        {
            connection1 = new ConnectionInterface(1);
            connection1.MsgSendRecived += new MsgSendRecivedEventHandler(SendRecivedEventHandler);
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

            // Start the Connections
            connection1.Connect();
        }

        /// <summary>
        /// Disconnecting Method
        /// </summary>
        public void Disconnect()
        {
            // Disconnect the Connections
            connection1.Disconnect();

        }

        /// <summary>
        /// Try to send the Data to the Requested Connection
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public int Send(MsgData message)
        {

            connection1.Send(message);

            return 1;
        }

        /// <summary>
        /// Return the Current Connection State of the Connection we are using right now...
        /// </summary>
        /// <returns></returns>
        public int getConnectionState()
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

    }
}
