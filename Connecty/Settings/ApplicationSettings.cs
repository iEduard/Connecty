using System;
using System.Runtime.Serialization;
using System.Windows;

namespace Connecty
{
    [Serializable()]
    public class ApplicationSettings : ISerializable
    {

        /// <summary>
        ///  Define the Class Variables
        /// </summary>
        public int msgLogRingBufferSize { get; set; }       // Size of the Ringbuffer for the Msg Log
        public int sendHistorySize { get; set; }            // Count of stored Send Messages 
        public Point position { get; set; }                 // Position of the Window when the Application was closed
        public double height { get; set; }                  // Height of the Main Window
        public double width { get; set; }                   // Width of the Main Window
        public bool debugModeIsActive { get; set; }         // TRUE = Debug Mode is aktiv / FALSE = Debug Mode is inactive     
        public bool expertModeIsActive { get; set; }        // TRUE = Expert Mode is aktiv more Settings will be Shown / FALSE = Export Mode is inactive     
        public string lastQuickButtonsPath { get; set; }    // The last path of the Quickbuttons
        public double msgLogZoomFactor { get; set; }        // The Zoom Factor for the MSG Box
        public string uiTheme{ get; set; }                  // Current Selected UI Theme of the Application
        public bool uiSpecialCharSetItalic { get; set; }    // Settings for the Special Char Italic Representation
        public bool uiSpecialCharSetBold { get; set; }      // Settings for the Special Char Bold Representation
        public bool uiSpecialCharSetColorChange { get; set; } // Settings for the Special Char Brightnes Representation

        /// <summary>
        /// Constructor
        /// </summary>
        public ApplicationSettings()
        {
            this.msgLogRingBufferSize = 4096;
            this.sendHistorySize = 20;

            this.debugModeIsActive = false;

            this.position = new Point(50, 50);
            this.height = 500;
            this.width = 600;

            this.lastQuickButtonsPath = "c:\\";

            this.msgLogZoomFactor = 1;

            // UI Representation
            this.uiTheme = "dark";
            this.uiSpecialCharSetBold = false;
            this.uiSpecialCharSetItalic = true;
            this.uiSpecialCharSetColorChange = true;
        }

        /// <summary>
        /// Load the Settings from the  inary file...
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public ApplicationSettings(SerializationInfo info, StreamingContext ctxt)
        {
            try
            {
                this.msgLogRingBufferSize = (int)info.GetValue("msgLogRingBufferSize", typeof(int));
            }
            catch
            {
                this.msgLogRingBufferSize = 255;
            }

            try
            {
                this.sendHistorySize = (int)info.GetValue("sendHistorySize", typeof(int));
            }
            catch
            {
                this.sendHistorySize = 20;
            }

            try
            {
                this.debugModeIsActive = (bool)info.GetValue("debugModeIsActive", typeof(bool));
            }
            catch
            {
                this.debugModeIsActive = false;
            }


            try
            {
                this.expertModeIsActive = (bool)info.GetValue("expertModeIsActive", typeof(bool));
            }
            catch
            {
                this.expertModeIsActive = false;
            }


            try
            {
                this.lastQuickButtonsPath = (string)info.GetValue("lastQuickButtonsPath", typeof(string));
            }
            catch
            {
                this.lastQuickButtonsPath = "c:\\";
            }

            try
            {
                this.msgLogZoomFactor = (double)info.GetValue("msgLogZoomFactor", typeof(double));
            }
            catch
            {
                this.msgLogZoomFactor = 1;
            }

            try
            {
                this.uiTheme = (string)info.GetValue("uiTheme", typeof(string));
            }
            catch
            {
                this.uiTheme = "dark";
            }

            try
            {
                this.uiSpecialCharSetItalic = (bool)info.GetValue("uiSpecialCharSetItalic", typeof(bool));
            }
            catch
            {
                this.uiSpecialCharSetItalic = true;
            }

            try
            {
                this.uiSpecialCharSetBold = (bool)info.GetValue("uiSpecialCharSetBold", typeof(bool));
            }
            catch
            {
                this.uiSpecialCharSetBold = true;
            }

            try
            {
                this.uiSpecialCharSetColorChange = (bool)info.GetValue("uiSpecialCharSetColorChange", typeof(bool));
            }
            catch
            {
                this.uiSpecialCharSetColorChange = true;
            }


            try
            {


                Point _position = (Point)info.GetValue("position", typeof(Point));
                double _height = (double)info.GetValue("height", typeof(double));
                double _width = (double)info.GetValue("width", typeof(double));

                 bool outOfBounds = (_position.X <= SystemParameters.VirtualScreenLeft - width) ||
                                    (_position.Y <= SystemParameters.VirtualScreenTop - _height) ||
                                    (SystemParameters.VirtualScreenLeft +
                                        SystemParameters.VirtualScreenWidth <= _position.X) ||
                                    (SystemParameters.VirtualScreenTop +
                                        SystemParameters.VirtualScreenHeight <= _position.Y);


                if (outOfBounds)
                {
                    this.height = 700;
                    this.width = 600;
                    this.position = new Point((SystemParameters.PrimaryScreenWidth / 2) - (this.width / 2), (SystemParameters.PrimaryScreenHeight / 2) - (this.height / 2));

                }
                else
                {
                    this.position = _position;
                    this.height = _height;
                    this.width = _width;
                }



            }
            catch
            {


                this.position = new Point(500, 500);
                this.height = 700;
                this.width = 600;


            }

        }

        /// <summary>
        /// Add the Settings 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("msgLogRingBufferSize", this.msgLogRingBufferSize);
            info.AddValue("sendHistorySize", this.sendHistorySize);

            info.AddValue("debugModeIsActive", this.debugModeIsActive);
            info.AddValue("expertModeIsActive", this.expertModeIsActive);

            info.AddValue("position", this.position);
            info.AddValue("height", this.height);
            info.AddValue("width", this.width);

            info.AddValue("lastQuickButtonsPath", this.lastQuickButtonsPath);

            info.AddValue("msgLogZoomFactor", this.msgLogZoomFactor);

            info.AddValue("uiTheme", this.uiTheme);

            info.AddValue("uiSpecialCharSetItalic", this.uiSpecialCharSetItalic);
            info.AddValue("uiSpecialCharSetBold", this.uiSpecialCharSetBold);
            info.AddValue("uiSpecialCharSetColorChange", this.uiSpecialCharSetColorChange);

        }


    }
}
