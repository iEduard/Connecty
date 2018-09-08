using System;
using System.Runtime.Serialization;

namespace Connecty
{
    [Serializable()]
    class ObjectToSerialize : ISerializable
    {

        private ConnectionSettings connectionSettings;
        private ApplicationSettings applicationSettings;
        private ViewSettings viewSettings;


        /// <summary>
        /// Public getter and setter for the ConnectionSettings
        /// </summary>
        public ConnectionSettings ConnectionSettings
        {
            get { return this.connectionSettings; }
            set { this.connectionSettings = value; }

        }

        /// <summary>
        /// Public getter and setter for the ApplicationSettings
        /// </summary>
        public ApplicationSettings ApplicationSettings
        {

            get { return this.applicationSettings; }
            set { this.applicationSettings = value; }

        }

        /// <summary>
        /// Public getter and setter for the ViewSettings
        /// </summary>
        public ViewSettings ViewSettings
        {
            get { return this.viewSettings; }
            set { this.viewSettings = value; }
        }

        /// <summary>
        /// Empty Constructor
        /// </summary>
        public ObjectToSerialize()
        {
        }

        /// <summary>
        /// Constructor to set the Settings from the Readed File
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public ObjectToSerialize(SerializationInfo info, StreamingContext ctxt)
        {
            this.connectionSettings = (ConnectionSettings)info.GetValue("connectionSettings", typeof(ConnectionSettings));
            this.applicationSettings = (ApplicationSettings)info.GetValue("applicationSettings", typeof(ApplicationSettings));
            this.viewSettings = (ViewSettings)info.GetValue("viewSettings", typeof(ViewSettings));
        }

        /// <summary>
        /// Function to store the Data in the File
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("connectionSettings", this.ConnectionSettings);
            info.AddValue("applicationSettings", this.applicationSettings);
            info.AddValue("viewSettings", this.viewSettings);
        }

    }
}
