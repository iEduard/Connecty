using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;

namespace Connecty
{

    /// <summary>
    /// This Class provides the Functionality of the Simulation
    /// </summary>
    public class SimulationInterface
    {

        private Thread simulationWorkingThread;
        private bool stopThread = false;
        private bool pauseThread = false;
        public bool continiousWorking { get; set; }
        private Simulation_State state = Simulation_State.Stop;
        private ObservableCollection<Simulation_Job> sequenceJobs;
        private MsgData lastMsg;

        const char SPACE_VALUE = '$';


        /// <summary>
        /// Constructor
        /// </summary>
        public SimulationInterface(ObservableCollection<Simulation_Job> jobs)
        {
            sequenceJobs = jobs;
            triggerSimulationStateChangedEvent(Simulation_State.Stop);
        }


        #region Control from Master and to the Master

        /// <summary>
        /// Control of the Simulation Working State
        /// </summary>
        /// <param name="requestedState">Set the State of the Simulation State Stop = Stopping end ending the Thread // Pause = Stop the Simulation but dont end it // Run = Start the Simulation </param>
        public void RequestStateChange(Simulation_State requestedState)
        {

            switch (requestedState)
            {

                // Stop the Simulation Thread
                case Simulation_State.Stop:

                    // If the Thread is Paused we wake him up to kill him!
                    if (state == Simulation_State.Pause)
                    {
                        lastMsg = null; // Discard the recived Msg's if we got some during the Pause state
                        simulationWorkingThread.Interrupt();
                        pauseThread = false;
                    }

                    stopThread = true;

                    break;
                
                // Pause the Simulation Thread
                case Simulation_State.Pause:

                    pauseThread = true;

                    break;
                
                // Start the Simulation Thread
                case Simulation_State.Run:

                    if (state == Simulation_State.Pause)
                    {
                        lastMsg = null; // Discard the recived Msg's if we got some during the Pause state
                        simulationWorkingThread.Interrupt();
                        pauseThread = false;
                    }
                    else
                    {
                        simulationWorkingThread = new Thread(new ThreadStart(WorkingThread));
                        simulationWorkingThread.Start();
                        lastMsg = null; // Discard the recived Msg's if we got some during the Pause state
                    }

                    break;

                default:
                    // Console.WriteLine("No valit Simulation State Request enterd");
                    break;


            }
        }

        /// <summary>
        /// Start with new Sim Data
        /// </summary>
        /// <param name="requestedState"></param>
        /// <param name="jobs"></param>
        public void RequestStateChange(Simulation_State requestedState, ObservableCollection<Simulation_Job> jobs)
        {
            sequenceJobs = jobs;
            RequestStateChange(requestedState);
        }


        /// <summary>
        /// Update of the Input is Available
        /// </summary>
        /// <param name="newInputMsg"></param>
        public void InputUpdate(MsgData newInputMsg)
        {
            lastMsg = newInputMsg;
        }

        /// <summary>
        /// Trigger function to start the Simulation State Chnge Event
        /// </summary>
        /// <param name="currentState"></param>
        private void triggerSimulationStateChangedEvent(Simulation_State currentState)
        {

            state = currentState;
            SimulationStateUpdateEventArgs myArgument = new SimulationStateUpdateEventArgs();
            myArgument.simulationState = currentState;
            simulationStateChanged(myArgument);
        }


        /// <summary>
        /// Trigger the Aktive Job Changed Event
        /// </summary>
        /// <param name="job"></param>
        private void triggerAktiveJobChangedEvent(Simulation_Job job)
        {

            AktiveJobChangedEventArgs myArgument = new AktiveJobChangedEventArgs();
            myArgument.Job = job;
            aktiveJobChanged(myArgument);
        }


        #endregion


        #region The Working Thing


        /// <summary>
        /// The actually working thread. This is the method where the magic happens
        /// </summary>
        private void WorkingThread()
        {
            // Set the Stop Thread to False
            stopThread = false;


            while (!stopThread)
            {

                foreach (Simulation_Job job in sequenceJobs)
                {

                    bool jobDone = false;

                    triggerAktiveJobChangedEvent(job);

                    while (!jobDone)
                    {

                        triggerSimulationStateChangedEvent(Simulation_State.Run);

                        switch (job.Type)
                        {

                            case Smimulation_SequenceType.Delay:

                                //Console.WriteLine(" Delay is Active ");
                                DelayThread(job);
                                jobDone = true;
                                break;

                            case Smimulation_SequenceType.WaitFor:

                                //Console.WriteLine(" Wait for is active ");
                                jobDone = WaitForMsg(job.Value);
                                break;

                            case Smimulation_SequenceType.Send:

                                //Console.WriteLine(" Send is active ");
                                SendData(job.Value);// Calling up the Eventhandler for the Main Programm
                                jobDone = true;
                                break;

                        }


                        if (stopThread)
                        {
                            triggerSimulationStateChangedEvent(Simulation_State.Stop);
                            return;
                        }
                        // Pause the Thread
                        while (pauseThread)
                        {
                            try
                            {
                                triggerSimulationStateChangedEvent(Simulation_State.Pause);
                                Thread.Sleep(Timeout.Infinite);// Send  the Thread to Sleep
                            }
                            catch
                            {
                                pauseThread = false;

                                // Set the Run State to the UI only if the StopFlag is not True!
                                if (!stopThread)
                                {
                                    triggerSimulationStateChangedEvent(Simulation_State.Run);
                                }
                            }

                        }
                    }
                }

                // Stop the Simulation if the Continous Mode is disabled
                if (!continiousWorking)
                {
                    stopThread = true;
                }
            }

            // State has changed. To Stop.
            triggerSimulationStateChangedEvent(Simulation_State.Stop);
        }


        /// <summary>
        /// Blocking call to wait for a specific MSG to recive 
        /// </summary>
        /// <param name="data">Msg Data to be recived</param>
        private bool WaitForMsg(string data)
        {
            bool returnVal = false;

            // Only check the Data if thei are Valid
            if (lastMsg != null && lastMsg.type == MsgData.messageType.recived)
            {

                if (compareRecivedAndSimMessage(lastMsg, data))
                {
                    lastMsg = null;// before leaving the loop erase the Data
                    returnVal = true;
                }

            }
            return returnVal;

        }

        /// <summary>
        /// Compare the simulation Data with the recived Data. With all the Fancy Placeholders and Special ASCII Chars
        /// </summary>
        /// <param name="recivedMsg"></param>
        /// <param name="simulationMsg"></param>
        /// <returns></returns>
        private bool compareRecivedAndSimMessage(MsgData recivedMsg, string simulationMsg)
        {

            // Clocking the Thread till we found what we are Searching for
            string _myData = Converty.msgDataToSpecialAsciiString(lastMsg.value);

            // Check if we got an Spacer int the String
            if (simulationMsg.IndexOf(SPACE_VALUE.ToString()) > -1)
            {


                string[] _splittedSimulationMsg = simulationMsg.Split(new Char[] { SPACE_VALUE });
                bool dataValid = true;

                string _myString = _myData;

                foreach (string simulationMsgPart in _splittedSimulationMsg)
                {
                    if (!(_myString.IndexOf(simulationMsgPart) > -1))
                    {
                        // Cut the String that we fount out of the Buffer. So in this case we will check the Abendency in the Messages
                        _myString = _myString.Substring(_myString.IndexOf(simulationMsgPart) + simulationMsgPart.Length);
                        dataValid = false;
                    }
                }

                if (dataValid)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else if (_myData == simulationMsg)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Send the Data to the Eventhandler.. To update the Main UI and send the Data to teh Connection
        /// </summary>
        private void SendData(string data)
        {
            // Set the User Hint to the TextBox
            MsgData _logMessage = new MsgData();
            _logMessage.value = Converty.specialAsciiStringToMsgData(data);
            _logMessage.type = MsgData.messageType.send;

            // Set the Event that the User Changed an Input
            MsgSendRecivedEventArgs _msgLogEventArgs = new MsgSendRecivedEventArgs();
            _msgLogEventArgs.msgData = _logMessage;
            msgSendRecived(_msgLogEventArgs);
        }

        /// <summary>
        /// Set the Thread to sleep
        /// </summary>
        /// <param name="delayInMs"></param>
        private void DelayThread(Simulation_Job job)
        {
            try
            {
                // Delay / Stop the Thread wit the given MS from the Settings
                Thread.Sleep(Convert.ToInt32(job.Value));
            }
            catch
            {
                // Show a Message that the Input was Invalid
                // Configure the message box to be displayed
                string messageBoxText = "Delay Einstellungen sind ungültig. Die Verzögerung wird nicht ausgeführt";
                string caption = "Ungültige Eingabe für das Verzögern";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;

                // Display message box
                MessageBox.Show(messageBoxText, caption, button, icon);
            }

        }

        #endregion


        #region On Change Eventhandler


        /// <summary>
        /// 
        /// </summary>
        public event SimulationStateChangedEventHandler SimulationStateChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void simulationStateChanged(SimulationStateUpdateEventArgs e)
        {
            if (SimulationStateChanged != null)
                SimulationStateChanged(this, e);
        }


        // An event that clients can use to be notified whenever the
        // elements of the list change.
        public event MsgSendRecivedEventHandler MsgSendRecived;

        // Invoke the Changed event; called whenever list changes
        protected virtual void msgSendRecived(MsgSendRecivedEventArgs e)
        {
            if (MsgSendRecived != null)
                MsgSendRecived(this, e);
        }




        public event AktiveJobChangedEventHandler AktiveJobChanged;


        protected virtual void aktiveJobChanged(AktiveJobChangedEventArgs e)
        {
            if (AktiveJobChanged != null)
                AktiveJobChanged(this, e);
        }


        #endregion


        #region Get Job Info as a String

        /// <summary>
        /// Return the currently active Job
        /// </summary>
        /// <returns></returns>
        public String GetActiveJob()
        {
            return "I am Batman";
        }

        /// <summary>
        /// Return the next job that will be active
        /// </summary>
        /// <returns></returns>
        public String GetNextJob()
        {
            return "I am Batman";
        }

        /// <summary>
        /// Return the requested Job Type and the Param in one defined String
        /// </summary>
        /// <param name="JobNumber"></param>
        /// <returns></returns>
        private String GetJobAsString(int JobNumber)
        {
            return "I am Batman";
        }

        #endregion


    }
}
