using System;
using System.Threading;

namespace Connecty
{

    /// <summary>
    /// This Function is used to Handle the Different Connections through only one Interface
    /// </summary>
    class FunctionInterface : IDisposable
    {
        // Define some Variables 
        public ConnectionSettings connectionSettings { get; set; } // Duplicate of the Settings durring the Connect
        private SingleConnectionInterface singleConnection; // Interface for the Connections
        private MultiConnectionInterface multiConnection; // Interface for the Connections

        private Timer stateTimer; // Timer for the Asynchronous update of the Statusbar

        public int connectionStateCommuniation1 { get; private set; }
        public int connectionStateCommuniation2 { get; private set; }

        private bool disposed = false; // to detect redundant calls


        /// <summary>
        /// Construktor
        /// </summary>
        public FunctionInterface()
        {

            // Create the objects...
            singleConnection = new SingleConnectionInterface();
            multiConnection = new MultiConnectionInterface();
            
            // Add the EventListeners...
            singleConnection.MsgSendRecived += new MsgSendRecivedEventHandler(SendRecivedEventHandler);
            multiConnection.MsgSendRecived += new MsgSendRecivedEventHandler(SendRecivedEventHandler);


        }

        /// <summary>
        /// Connect Method
        /// </summary>
        public void Connect(ConnectionSettings settingsForTheCommunicaton)
        {
            // Only get the Date if the local Values are null
            if ( connectionSettings == null)
            {
                // OK First of all copy the Settings Object.
                // Clone the Settings Object. becouse We are gone to change the Settings withe the interfacing of the users..
                connectionSettings = (ConnectionSettings)ObjectCopier.Clone(settingsForTheCommunicaton);
            }

            if (connectionSettings.functionSelect == 0 && 0 != singleConnection.getConnectionState())
            {
                return;
            }
            else if (connectionSettings.functionSelect == 1 && ( 0 != multiConnection.getConnection1State() || 0 != multiConnection.getConnection2State() ) )
            {
                return;
            }



            // OK First of all copy the Settings Object.
            // Clone the Settings Object. becouse We are gone to change the Settings withe the interfacing of the users..
            connectionSettings = (ConnectionSettings)ObjectCopier.Clone(settingsForTheCommunicaton);

            // bevore we try to connect lets Disconnect every maybe runnung Communications
            //Disconnect();
            

            if ( connectionSettings.functionSelect == 0 )
            {
                singleConnection.connectionSettings = connectionSettings;
                singleConnection.Connect();

            }
            else if ( connectionSettings.functionSelect == 1 )
            {
                multiConnection.connectionSettings = connectionSettings;
                multiConnection.Connect();


            }

            // Start the StatusBar Update Ticker
            stateTimer = new Timer(new TimerCallback(updateStatusBar_Tick));
            stateTimer.Change(0, 200);


        }

        /// <summary>
        /// Disconnecting Method
        /// </summary>
        public void Disconnect()
        {
            
            singleConnection.Disconnect();
            multiConnection.Disconnect();
        }

        /// <summary>
        /// Try to send the Data to the Requested Connection
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Not yet Implemented feature to return the Send was Completed or not</returns>
        public int Send(MsgData message)
        {

            message.type = MsgData.messageType.send;
                        
            if (connectionSettings.functionSelect == 0)
            {
                singleConnection.Send(message);
            }
            else if (connectionSettings.functionSelect == 1)
            {
                multiConnection.Send(message);                
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
            sendRecivedEvent(e);//EventArgs.Empty
        }

        /// <summary>
        /// Timer Tick to update the current Connection State
        /// </summary>
        /// <param name="sender"></param>
        private void updateStatusBar_Tick(object sender)
        {

            // Create a new Event Argument
            ConnectionStateUpdateEventArgs eventAargument = new ConnectionStateUpdateEventArgs();

            /// read the Current State of the Connection
            
            if (connectionSettings.functionSelect == 0)
            {
                eventAargument.connection1State = singleConnection.getConnectionState();
                eventAargument.connection2State = -1;// Hide the Second Communication

            }
            else if (connectionSettings.functionSelect == 1)
            {
                eventAargument.connection1State = multiConnection.getConnection1State();
                eventAargument.connection2State = multiConnection.getConnection2State();
            } 

            // Call the Notifiy Function to Raise the Event
            statusbarUpdate(eventAargument);


            connectionStateCommuniation1 = eventAargument.connection1State;
            connectionStateCommuniation2 = eventAargument.connection2State;
        }

        #region Events


        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event ConnectionStateUpdateEventHandler StatusbarUpdate;

        // Invoke the Changed event; called whenever list changes
        protected virtual void statusbarUpdate(ConnectionStateUpdateEventArgs e)
        {
            if (StatusbarUpdate != null)
                StatusbarUpdate(this, e);
        }


        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event MsgSendRecivedEventHandler SendRecived;

        // Invoke the Changed event; called whenever list changes
        protected virtual void sendRecivedEvent(MsgSendRecivedEventArgs e)
        {
            if (SendRecived != null)
                SendRecived(this, e);
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
                    if (stateTimer != null)
                    {
                        stateTimer.Dispose();
                    }
                    if (singleConnection != null)
                    {
                        singleConnection.Dispose();
                    }
                    if (multiConnection != null)
                    {
                        multiConnection.Dispose();
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
