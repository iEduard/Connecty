using System;

namespace Connecty
{



    #region Msg Send and Recived Eventhandler

    /// <summary>
    /// The Event Argument with the Send or the Recived Msg from Type MSG Data
    /// </summary>
    public class MsgSendRecivedEventArgs : EventArgs
    {
        public MsgData msgData { get; set; }
    }

    /// <summary>
    /// The Event For the Msg Send or Recive
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MsgSendRecivedEventHandler(object sender, MsgSendRecivedEventArgs e);


    #endregion


    #region StatusBar update with the Current Connection State

    /// <summary>
    /// 
    /// </summary>
    public class ConnectionStateUpdateEventArgs : EventArgs
    {
        public int connection1State { get; set; }
        public int connection2State { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ConnectionStateUpdateEventHandler(object sender, ConnectionStateUpdateEventArgs e);

    #endregion


    #region Simulation State Update

    public class SimulationStateUpdateEventArgs : EventArgs
    {
        public Simulation_State simulationState { get; set; }
    }

    public delegate void SimulationStateChangedEventHandler(object sender, SimulationStateUpdateEventArgs e);



    #endregion


    #region Aktive JobChanged

    public class AktiveJobChangedEventArgs : EventArgs
    {
        public Simulation_Job Job { get; set; }
    }

    public delegate void AktiveJobChangedEventHandler(object sender, AktiveJobChangedEventArgs e);



    #endregion


}
