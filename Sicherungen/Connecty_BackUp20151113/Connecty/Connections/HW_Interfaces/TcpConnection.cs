using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace Connecty
{
    class TcpConnection
    {
        private TcpListenerEx tcpListener;
        private Thread listenThread;
        private Thread clientThread;
        public tcpIpSettings settings { get; set; }
        private bool stopThreads = false;
        private TcpClient tcpClient;
        public int interfaceNumber { get; set; }// Number of the Connection Interface

        /// <summary>
        /// Constructor
        /// </summary>
        public TcpConnection()
        {
            // Preset the Settings for the TCP IP Connections
            settings = new tcpIpSettings();
        }

        /// <summary>
        /// Connect the TCP Server or Client Depending on the Settings
        /// </summary>
        public void connect()
        {

            stopThreads = false;


            // Client is set up
            if(settings.clientServerSelection.Equals("Client")){

                this.listenThread = new Thread(new ThreadStart(connectClientToServer));
                this.listenThread.Start();

            }// Server is set up
            else if (settings.clientServerSelection.Equals("Server"))
            {
                this.tcpListener = new TcpListenerEx(IPAddress.Any, settings.port); //IPAddress.Parse(settings.ip)
                this.listenThread = new Thread(new ThreadStart(ListenForClients));
                this.listenThread.Start();
            }

        }

        /// <summary>
        /// Disconnect the Client or the Server
        /// </summary>
        public void disconnect()
        {
            // Stop the Bakground Threads
            stopThreads = true;

            // Try to stop the Client if its Available
            if(tcpClient != null )
            {
                if (tcpClient.Connected)
                {
                    tcpClient.Close();
                }
            }


            // Try to stop the Listener
            try
            {
                this.tcpListener.Server.Close();
                this.tcpListener.Stop();

                /*while (listenThread.IsAlive)
                {
                    ;//Wait till Thread has Stopped
                }*/
            }
            catch (Exception)
            {
                ;// Double nothing
            }


        }

        /// <summary>
        /// Listen for incoming Clients
        /// </summary>
        private void ListenForClients()
        {

            while (!stopThreads)
            {

                // Try to start the Listener
                try
                {
                    this.tcpListener.Start();
                }
                catch
                {

                    // Update the UI
                    updateUi("TCP Server Could not Start on Port:" + settings.port, MsgData.messageType.infoNegative);

                    return;
                }


                // Set the Status Server is Startet
                updateUi("TCP Server Startet on Port:" + settings.port, MsgData.messageType.infoPositive);

                // Wait for a Client to connect
                try
                {
                    //blocks until a client has connected to the server
                    TcpClient client = this.tcpListener.AcceptTcpClient();

                    // Set the User Hint to the TextBox
                    updateUi("Client connected with IP:" + client.Client.RemoteEndPoint.ToString(), MsgData.messageType.infoPositive);

                    //create a thread to handle communication 
                    //with connected client
                    clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                    clientThread.Start(client);

                    while (!stopThreads && client.Connected)
                    {
                        this.tcpListener.Stop();
                        Thread.Sleep(2000);
                        ;// Wait til the Client is disconnected or we want to the User wants to Disconnect

                    }

                    // Set the User Hint to the 
                    updateUi("Server Stopped", MsgData.messageType.infoNegative);
                
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error occured: " + e);

                    // Set the User Hint to the TextBox
                    updateUi("Server Stopped: ", MsgData.messageType.infoNegative);
                }
            }

        }

        /// <summary>
        /// Try to connect to a Server as Client
        /// </summary>
        public void connectClientToServer()
        {
            stopThreads = false;

            while (!stopThreads)
            {


                tcpClient = new TcpClient();

                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(settings.ip), settings.port);
                try
                {
                    // Connect the client to the Server
                    tcpClient.Connect(serverEndPoint);

                    if (tcpClient.Connected)
                    {
                        // Update the UI

                        // Set the User Hint to the Rich TextBox
                        updateUi("Client with Server-IP:" + tcpClient.Client.RemoteEndPoint.ToString() + "connected", MsgData.messageType.infoPositive);

                        // Start the Communication Thread
                        HandleClientComm(tcpClient);

                        if (!settings.restartTcpServer)
                        {
                            stopThreads = true;
                        }
                    }
                }
                catch (Exception)
                {
                    // Update the UI
                    updateUi("Client could not connect to Server-IP:" + settings.ip + ":" + settings.port.ToString(), MsgData.messageType.infoNegative);

                }
            }

        }

        /// <summary>
        /// Handling the Client Communication
        /// </summary>
        /// <param name="client"></param>
        private void HandleClientComm(object client)
        {
            MsgData logMessage;

            tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;
            bool stopClientComm = false;

            while (!stopThreads && !stopClientComm)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    //a socket error has occured
                    if (!settings.restartTcpServer)
                    {
                        stopClientComm = true;
                        stopThreads = true;
                    }
                    else
                    {
                        stopClientComm = true;
                    }

                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    if (!settings.restartTcpServer)
                    {
                        stopClientComm = true;
                        stopThreads = true;
                    }
                    else
                    {
                        stopClientComm = true;
                    }
                    break;
                }

                // Save the Data to the 
                logMessage = new MsgData();

                // Set the Current TimeStamp
                logMessage.setCurrentTimeStamp();

                logMessage.value = new byte[bytesRead];// Create an Array with the Size of readed Data
                Array.Copy(message, logMessage.value, bytesRead);// Copy the Data to the Array
                logMessage.type = 0;//Set the Type to Recived Message
                logMessage.connectionNumber = interfaceNumber;


                // Set the Event that the User Changed an Input
                MsgSendRecivedEventArgs msgLogEventArgs = new MsgSendRecivedEventArgs();
                msgLogEventArgs.msgData = logMessage;
                msgSendRecived(msgLogEventArgs);

            }

            // Update the UI
            updateUi("Client disconnected with IP:" + settings.ip + ":" + settings.port.ToString(), MsgData.messageType.infoNegative);

            tcpClient.Close();



        }

        /// <summary>
        /// Send Data to the Client or Server
        /// </summary>
        /// <param name="message"></param>
        public void send(MsgData message)
        {
            if (tcpClient.Connected)
            {
                NetworkStream clientStream = tcpClient.GetStream();
                clientStream.Write(message.value, 0, message.value.Length);
                clientStream.Flush();

                message.setCurrentTimeStamp();// Set the Time Stamp
                message.connectionNumber = interfaceNumber;// Set the reference to the Interface

                // Set the Event that the User Changed an Input
                MsgSendRecivedEventArgs msgLogEventArgs = new MsgSendRecivedEventArgs();
                msgLogEventArgs.msgData = message;
                msgSendRecived(msgLogEventArgs);
            }

        }


        #region UI Update functions


        /// <summary>
        /// Update the ui With an Log Message and trigger the UpdateStatusBar
        /// </summary>
        /// <param name="rtbMsg">Message for the Log Window</param>
        /// <param name="msgType">Msg Type</param>
        private void updateUi(string rtbMsg, MsgData.messageType msgType)
        {

            // Set the User Hint to the TextBox
            MsgData logMessage = new MsgData();
            logMessage.value = Encoding.ASCII.GetBytes(rtbMsg);
            logMessage.type = msgType;

            // Set the Event that the User Changed an Input
            MsgSendRecivedEventArgs msgLogEventArgs = new MsgSendRecivedEventArgs();
            msgLogEventArgs.msgData = logMessage;
            msgSendRecived(msgLogEventArgs);

        }

        #endregion


        /// <summary>
        /// Get the current Connection State
        /// </summary>
        /// <returns></returns>
        public int getConnectionState()
        {
            int connectionState = 0;

            try
            {

                // Check the TCP Client
                if (this.tcpClient != null)
                {
                    // uiReference.UpdateStatusBarOnDifferentThread(ConnectionState);
                    if (this.tcpClient.Connected)
                    {
                        connectionState = 3;
                    }
                    else if (!(this.tcpClient.Connected))
                    {
                        connectionState = 0;
                    }

                }

                // Check the Listener State
                if (this.tcpListener != null)
                {
                    if (this.tcpListener.Active)
                    {
                        connectionState = 1;
                    }
                    else if (this.tcpClient == null)
                    {
                        connectionState = 0;

                    }
                }


            }
            // Something unpredicted happend
            catch (Exception err)
            {
                connectionState = 0;
            }

            // Return the current State
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

    }
}
