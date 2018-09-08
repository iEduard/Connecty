using System.ComponentModel;
using System.Windows.Media;

namespace Connecty
{
    public class Simulation_Job : INotifyPropertyChanged
    {

        // Private Variables
        private Smimulation_SequenceType _type;
        private string _value;

        public Smimulation_SequenceType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                NotifyPropertyChanged("Type");
            }
        }

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyPropertyChanged("Value");
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public Simulation_Job(Smimulation_SequenceType paramType, string paramMessage)
        {
            this._type= paramType;
            this._value = paramMessage;
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private Helpers

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
