using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Connecty
{
    /// <summary>
    /// Interaktionslogik für Simulation_UI.xaml
    /// </summary>
    public partial class Simulation_UI : Window
    {

        #region Local Variables


        /// <summary>
        /// Simulation Interface. Is handling all the Simulation things
        /// </summary>
        private SimulationInterface simulationInterface;

        /// <summary>
        /// Current State of the Simulation Interface
        /// </summary>
        private Simulation_State currentSimulationState;

        /// <summary>
        /// Observable List for the Data Grid and the Simulation Interface.
        /// </summary>
        public ObservableCollection<Simulation_Job> Jobs { get; private set; }

        /// <summary>
        /// Holds the Item of the Currently Active Item in the Data Grid
        /// Can be used to change the Row Color of the Active Item Back if the Next Job is active or the 
        /// Simulation is Stopped
        /// </summary>
        private DataGridRow lastAktiveRow;

        #region Command Bindings

        // Routed Commands for the Toolbar Buttons and the HotKeys
        public static RoutedCommand startPausSimRoutedCommand = new RoutedCommand();
        public static RoutedCommand stopSimRoutedCommand = new RoutedCommand();
        public static RoutedCommand openSimRoutedCommand = new RoutedCommand();
        public static RoutedCommand saveSimRoutedCommand = new RoutedCommand();
        public static RoutedCommand addSimJobRoutedCommand = new RoutedCommand();
        public static RoutedCommand removeSimJobRoutedCommand = new RoutedCommand();
        public static RoutedCommand changeConiniousSimModeRoutedCommand = new RoutedCommand();
        #endregion


        #endregion

        #region Local Constants

        /// <summary>
        /// Color of the active Job Row during the active Simulation
        /// </summary>
        private static Brush activeRowColor = Brushes.LightGreen;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings"></param>
        public Simulation_UI()
        {
            // init of the Class
            classInit();

        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings"></param>
        public Simulation_UI(string simPath)
        {

            // Init of the Class
            classInit();

            // Create the neded objects
            Simulation_ImportExport mySimulationExporter = new Simulation_ImportExport();
            LinkedList<Simulation_Job> simulationJobs = mySimulationExporter.ReadSimulationKonfiguration(simPath);

            if (simulationJobs != null)
            {
                // Clear all the Data if we loaded an Valid Data
                Jobs.Clear();

                foreach (Simulation_Job jobItem in simulationJobs)
                {
                    Jobs.Add(jobItem);
                }
            }


        }


        #endregion


        #region UI Design Functions

        /// <summary>
        /// Set the Title of the Window
        /// </summary>
        /// <param name="title"></param>
        public void SetWindowTitle(string title)
        {
            this.Title = title;
        }

        /// <summary>
        /// Function is called after load of the Toolbar
        /// In this Function we will hide the Toolbar DropDown Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
            var mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness();
            }
        }

        /// <summary>
        /// Highlighting DataGrid Row with the Active Job
        /// </summary>
        /// <param name="job"></param>
        private void highlightDataGridRow(Simulation_Job job)
        {

            JobsDataGrid.SelectedItem = null;

            if (lastAktiveRow != null)
            {
                lastAktiveRow.Background = Brushes.White;
            }

            lastAktiveRow = (DataGridRow)JobsDataGrid.ItemContainerGenerator.ContainerFromItem(job);

            if (lastAktiveRow != null)
            {
                lastAktiveRow.Background = activeRowColor;
            }


        }

        /// <summary>
        /// Function to control the User Input State
        /// </summary>
        private void disableUserInput()
        {
            // Disable the MenuBar
            toolBarAddJob.IsEnabled = false;
            JobsDataGrid.IsEnabled = false;

        }

        /// <summary>
        /// Enable the UserINput
        /// </summary>
        private void enableUserInput()
        {
            // Disable the MenuBar
            toolBarAddJob.IsEnabled = true;
            JobsDataGrid.IsEnabled = true;
        }

        /// <summary>
        /// Update the UI with the new Button Style
        /// </summary>
        private void setContinuousModeBackground()
        {

            if (simulationInterface.continiousWorking)
            {
                toolBarContinousModeOnOff.Background = Brushes.LightGreen;
            }
            else
            {
                toolBarContinousModeOnOff.Background = Brushes.LightPink;
            }

        }

        #endregion

        #region UI Update Handler from SimulationInterface


        /// <summary>
        /// Event Handler from the Simulation Interface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simStateChenged(object sender, SimulationStateUpdateEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Threading.DispatcherOperationCallback(delegate
            {
                // Update the UI via a function
                updateSimulationStatusUi(e.simulationState);
                return null;
            }), null);
        }

        /// <summary>
        /// Update the UI with the Current Connection State
        /// </summary>
        /// <param name="currentState"></param>
        private void updateSimulationStatusUi(Simulation_State currentState)
        {
            currentSimulationState = currentState;

            switch (currentState)
            {

                case Simulation_State.Pause:
                    disableUserInput();
                    statusBarSimulationState.Text = " Pausiert ";
                    statusBarSimulationState.Background = System.Windows.Media.Brushes.Yellow;
                    break;

                case Simulation_State.Run:
                    disableUserInput();
                    statusBarSimulationState.Text = " Läuft ";
                    statusBarSimulationState.Background = System.Windows.Media.Brushes.Green;
                    break;

                case Simulation_State.Stop:
                    enableUserInput();
                    statusBarSimulationState.Text = " Gestoppt ";
                    statusBarSimulationState.Background = System.Windows.Media.Brushes.Red;
                    // Set the last Row Background back to White
                    if (lastAktiveRow != null)
                    {
                        lastAktiveRow.Background = System.Windows.Media.Brushes.White;
                    }
                    break;

            }
        }

        /// <summary>
        /// Eventhandler for new Jobs active
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aktiveJobChanged(object sender, AktiveJobChangedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Threading.DispatcherOperationCallback(delegate
            {
                // Update the UI via a function
                highlightDataGridRow(e.Job);
                return null;
            }), null);
        }


        #endregion

        #region Events pass through


        /// <summary>
        /// Update of the Input is Available pass it through to the Interface
        /// </summary>
        /// <param name="newInputMsg"></param>
        public void InputUpdate(MsgData newInputMsg)
        {
            if (simulationInterface != null)
            {
                simulationInterface.InputUpdate(newInputMsg);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void msgSend(object sender, MsgSendRecivedEventArgs e)
        {
            msgSendRecived(e);
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




        #endregion

        #region Private internal used Functions

        private void classInit()
        {
            // Create the yet empty Job list
            Jobs = new ObservableCollection<Simulation_Job>();

            // Create the Simulation Interface
            simulationInterface = new SimulationInterface(Jobs);
            simulationInterface.RequestStateChange(Simulation_State.Run);
            simulationInterface.MsgSendRecived += new MsgSendRecivedEventHandler(msgSend);
            simulationInterface.SimulationStateChanged += new SimulationStateChangedEventHandler(simStateChenged);
            simulationInterface.AktiveJobChanged += new AktiveJobChangedEventHandler(aktiveJobChanged);

            // Initialize the components of the UI
            InitializeComponent();

            // Set the DataContex for the UI
            DataContext = this;

            //simulationInterface = new SimulationInterface(Jobs);
            updateSimulationStatusUi(Simulation_State.Stop);

            // Update the UI
            setContinuousModeBackground();

            // Add the Command Bindings
            createCommandBindings();
        }


        /// <summary>
        /// Remove selected from the Listview item
        /// </summary>
        private void removeItem()
        {
            // Escape if their is no selection
            if (JobsDataGrid.SelectedIndex == -1)
            {
                return;
            }

            // get the current selection
            int selectedIndex = JobsDataGrid.SelectedIndex;



            // Remmove the selected Item
            Jobs.RemoveAt(JobsDataGrid.SelectedIndex);

            // Change the Selected index
            if (Jobs.Count <= selectedIndex)
            {
                selectedIndex--;
            }

            // Set the new Selection
            JobsDataGrid.SelectedIndex = selectedIndex;

            // Set the Focus to the Object. So we see the right Selection color
            JobsDataGrid.Focus();
        }

        #endregion

        #region Window Closing

        /// <summary>
        /// Window Closing Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Call the Dispose Function
            Dispose();
        }

        /// <summary>
        /// Clean up the mess. This function Stops all started Threads and cleans up al unnessesary Part we do not need anymore
        /// </summary>
        public void Dispose()
        {
            // If the Window is Closed stop the Thread
            if (simulationInterface != null)
            {
                simulationInterface.RequestStateChange(Simulation_State.Stop);
            }
        }

        #endregion

        #region CommandBindings

        /// <summary>
        /// Add the Hot Keys and Key Bindings to the UI
        /// </summary>
        private void createCommandBindings()
        {
            try
            {
                
                // ----------------------------------------------------------------------------------------------
                // Add the StartPauseSim Commands
                CommandBinding cb = new CommandBinding(startPausSimRoutedCommand, StartPausSim_CommandEventHandler);
                this.CommandBindings.Add(cb);
                toolBarStartPausSim.Command = startPausSimRoutedCommand;

                KeyGesture kg = new KeyGesture(Key.F11, ModifierKeys.None);
                InputBinding ib = new InputBinding(startPausSimRoutedCommand, kg);
                this.InputBindings.Add(ib);

                // ----------------------------------------------------------------------------------------------
                // Add the StopSim Commands
                cb = new CommandBinding(stopSimRoutedCommand, StopSim_CommandEventHandler);
                this.CommandBindings.Add(cb);
                toolBarStopSim.Command = stopSimRoutedCommand;

                kg = new KeyGesture(Key.F12, ModifierKeys.None);
                ib = new InputBinding(stopSimRoutedCommand, kg);
                this.InputBindings.Add(ib);

                // ----------------------------------------------------------------------------------------------
                // Add the OpenSim Commands
                cb = new CommandBinding(openSimRoutedCommand, OpenSim_CommandEventHandler);
                this.CommandBindings.Add(cb);
                toolBarOpen.Command = openSimRoutedCommand;

                kg = new KeyGesture(Key.O, ModifierKeys.Control);
                ib = new InputBinding(openSimRoutedCommand, kg);
                this.InputBindings.Add(ib);

                // ----------------------------------------------------------------------------------------------
                // Add the SaveSim Commands
                cb = new CommandBinding(saveSimRoutedCommand, SaveSim_CommandEventHandler);
                this.CommandBindings.Add(cb);
                toolBarSave.Command = saveSimRoutedCommand;

                kg = new KeyGesture(Key.S, ModifierKeys.Control);
                ib = new InputBinding(saveSimRoutedCommand, kg);
                this.InputBindings.Add(ib);

                // ----------------------------------------------------------------------------------------------
                // Add the Add new Sim Job Commands
                cb = new CommandBinding(addSimJobRoutedCommand, AddSimJob_CommandEventHandler);
                this.CommandBindings.Add(cb);
                toolBarAddJob.Command = addSimJobRoutedCommand;

                kg = new KeyGesture(Key.N, ModifierKeys.Control);
                ib = new InputBinding(addSimJobRoutedCommand, kg);
                this.InputBindings.Add(ib);

                // ----------------------------------------------------------------------------------------------
                // Add the Remove Sim Job Commands
                cb = new CommandBinding(removeSimJobRoutedCommand, RemoveSimJob_CommandEventHandler);
                this.CommandBindings.Add(cb);
                toolBarRemoveJob.Command = removeSimJobRoutedCommand;

                // This is a standard from the Datagrid already
                //kg = new KeyGesture(Key.Delete, ModifierKeys.None);
                //ib = new InputBinding(removeSimJobRoutedCommand, kg);
                //this.InputBindings.Add(ib);


                // ----------------------------------------------------------------------------------------------
                // Add the Change Continous Mode Commands
                cb = new CommandBinding(changeConiniousSimModeRoutedCommand, ChangeConinuousSimMode_CommandEventHandler);
                this.CommandBindings.Add(cb);
                toolBarContinousModeOnOff.Command = changeConiniousSimModeRoutedCommand;

                
                kg = new KeyGesture(Key.C, ModifierKeys.Control);
                ib = new InputBinding(changeConiniousSimModeRoutedCommand, kg);
                this.InputBindings.Add(ib);


            }
            catch
            {
                //handle exception error
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartPausSim_CommandEventHandler(object sender, ExecutedRoutedEventArgs e)
        {
            switch (currentSimulationState)
            {

                case Simulation_State.Stop:

                    // Try to exit the Edit Mode of the Cell
                    JobsDataGrid.CommitEdit();
                    JobsDataGrid.SelectedIndex = -1;
                    JobsDataGrid.CommitEdit();

                    simulationInterface.RequestStateChange(Simulation_State.Run, Jobs);
                    break;

                case Simulation_State.Pause:
                    simulationInterface.RequestStateChange(Simulation_State.Run);
                    break;

                case Simulation_State.Run:
                    simulationInterface.RequestStateChange(Simulation_State.Pause);
                    break;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopSim_CommandEventHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (simulationInterface != null)
            {
                simulationInterface.RequestStateChange(Simulation_State.Stop);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenSim_CommandEventHandler(object sender, ExecutedRoutedEventArgs e)
        {
            // Check the State of the Simulation. Only if the Simulation is stopped we can load an existing Simulation
            if (currentSimulationState != Simulation_State.Stop)
            {
                // Configure the message box to be displayed
                string messageBoxText = "Simulation kann nicht geladen werden. Erst die Simulation stoppen.";
                string caption = "Simulation läuft";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;

                // Display message box
                MessageBox.Show(messageBoxText, caption, button, icon);

                // Exit the Function
                return;

            }

            // Create the neded objects
            Simulation_ImportExport mySimulationExporter = new Simulation_ImportExport();
            LinkedList<Simulation_Job> simulationJobs = mySimulationExporter.ReadSimulationKonfiguration();

            if (simulationJobs != null)
            {
                // Clear all the Data if we loaded an Valid Data
                Jobs.Clear();

                foreach (Simulation_Job jobItem in simulationJobs)
                {
                    Jobs.Add(jobItem);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveSim_CommandEventHandler(object sender, ExecutedRoutedEventArgs e)
        {
            // Try to exit the Edit Mode of the Cell
            JobsDataGrid.CommitEdit();
            JobsDataGrid.SelectedIndex = -1;
            JobsDataGrid.CommitEdit();

            LinkedList<Simulation_Job> simulationJobs = new LinkedList<Simulation_Job>();
            foreach (Simulation_Job jobItem in Jobs)
            {
                simulationJobs.AddLast(jobItem);
            }

            Simulation_ImportExport mySimulationExporter = new Simulation_ImportExport();
            mySimulationExporter.ExportSimulationDataToFileSystem(simulationJobs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSimJob_CommandEventHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (JobsDataGrid.SelectedIndex == -1)
            {
                Jobs.Add(new Simulation_Job(Smimulation_SequenceType.WaitFor, "New Item"));
            }
            else
            {
                Jobs.Insert(JobsDataGrid.SelectedIndex + 1, new Simulation_Job(Smimulation_SequenceType.WaitFor, "New Item"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveSimJob_CommandEventHandler(object sender, ExecutedRoutedEventArgs e)
        {
            removeItem();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeConinuousSimMode_CommandEventHandler(object sender, ExecutedRoutedEventArgs e)
        {
            // Exit the Function if we do not get an Object...
            if (simulationInterface == null)
            {
                return;
            }

            // Set the new Setup to the Interface
            simulationInterface.continiousWorking = !simulationInterface.continiousWorking;

            // Update the UI
            setContinuousModeBackground();
        }


        #endregion

        #region Debug the Bug
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lSimulationState_MouseDown(object sender, MouseButtonEventArgs e)
        {



        }

        #endregion


    }

}
