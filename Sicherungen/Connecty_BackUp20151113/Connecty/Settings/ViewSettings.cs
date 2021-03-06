﻿using System;
using System.Runtime.Serialization;
using System.Windows.Media;

namespace Connecty
{
    [Serializable()]
    public class ViewSettings : ISerializable
    {

        /// <summary>
        ///  Define the Class Variables
        /// </summary>
        public bool showTimeStamp { get; set; }              // TRUE := Show the TimeStamp in the Message Log // FALSE := Hide the TimeStamp in the Message Log

        /// <summary>
        /// the Current Settings for the view 
        /// 0 = Standard ASCII Signs as a String 
        /// 1 = ASCII Encoding HEX Values
        /// 2 = ASCII Encoding Decimal Values
        /// 3 = ASCII Encoding Binary Values
        /// </summary>
        public int dataPresentation { get; set; }


        public Color sendColor { get; set; } 
        public Color receiveColor { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        public ViewSettings()
        {
            showTimeStamp = false;
            dataPresentation = 0; // Default show the ASCII Signs
            sendColor = Brushes.Blue.Color;
            receiveColor = Brushes.Magenta.Color;

        }


        /// <summary>
        /// Change the Color of the Current view
        /// </summary>
        /// <param name="currentColor"></param>
        /// <returns></returns>
        public void changeSendReciveColor(int snedReceiveSelect)
        {
            Color currentColor;

            // Get the Current Color
            if (snedReceiveSelect == 1)
            {
                currentColor = sendColor;

            }
            else
            {
                currentColor = receiveColor;
            }


            if (currentColor == Colors.DarkBlue)
            {
                currentColor = Colors.DarkCyan;
            }
            else if (currentColor == Colors.DarkCyan)
            {
                currentColor = Colors.DarkGreen;
            }
            else if (currentColor == Colors.DarkGreen)
            {
                currentColor = Colors.DarkKhaki;
            }
            else if (currentColor == Colors.DarkKhaki)
            {
                currentColor = Colors.DarkMagenta;
            }
            else if (currentColor == Colors.DarkMagenta)
            {
                currentColor = Colors.DarkOrange;
            }
            else if (currentColor == Colors.DarkOrange)
            {
                currentColor = Colors.DarkRed;
            }
            else
            {
                currentColor = Colors.DarkBlue;
            }


            // Set the Color the the actual requested Settings Value
            if (snedReceiveSelect == 1)
            {
                sendColor = currentColor;

            }
            else
            {
                receiveColor = currentColor;
            }

        }



        /// <summary>
        /// Load the Settings vrom the Binary Data
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public ViewSettings(SerializationInfo info, StreamingContext ctxt)
        {
            try
            {
                this.showTimeStamp = (bool)info.GetValue("showTimeStamp", typeof(bool));
            }catch(Exception err)
            {

                // Set the Show TimeStamp if we are not able to read the Binary Settings
                this.showTimeStamp = true;

                // Set a Message to the Log
                Console.WriteLine("Das laden der Zeitstempeleinstellungen hat nicht geklappt");
            }

            try
            {
                this.dataPresentation = (int)info.GetValue("dataPresentation", typeof(int));
            }
            catch(Exception err)
            {
                // Set the ASCII Data Representatiion in case we are not able to load the Binary Files
                this.dataPresentation = 1;

                // Set a Message to the Log
                Console.WriteLine("Das laden der Ansichtsdarstellung hat nicht geklappt");
            }

            
            try
            {
                this.sendColor = Color.FromArgb((byte)info.GetValue("sendColorA", typeof(byte)),
                            (byte)info.GetValue("sendColorR", typeof(byte)),
                            (byte)info.GetValue("sendColorG", typeof(byte)),
                            (byte)info.GetValue("sendColorB", typeof(byte)));

            } catch(Exception err)
            {
                // If we are not able to read the Settings we will set one
                this.sendColor = Colors.DarkRed;

                // Set up a Message to the Log
                Console.WriteLine("Das laden der Sende Farbe hat nicht geklappt");

            }

            try
            {
                this.receiveColor = Color.FromArgb((byte)info.GetValue("receiveColorA", typeof(byte)),
                               (byte)info.GetValue("receiveColorR", typeof(byte)),
                               (byte)info.GetValue("receiveColorG", typeof(byte)),
                               (byte)info.GetValue("receiveColorB", typeof(byte)));

            }
            catch(Exception err)
            {
                // If we are not able to read the Settings we will set one
                this.receiveColor = Colors.DarkBlue;

                // Set up a Message to the Log
                Console.WriteLine("Das laden der Ansichtsdarstellung hat nicht geklappt");

            }



        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ctxt"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("showTimeStamp", this.showTimeStamp);
            info.AddValue("dataPresentation", this.dataPresentation);


            info.AddValue("sendColorR", this.sendColor.R);
            info.AddValue("sendColorG", this.sendColor.G);
            info.AddValue("sendColorB", this.sendColor.B);
            info.AddValue("sendColorA", this.sendColor.A);

            info.AddValue("receiveColorR", this.receiveColor.R);
            info.AddValue("receiveColorG", this.receiveColor.G);
            info.AddValue("receiveColorB", this.receiveColor.B);
            info.AddValue("receiveColorA", this.receiveColor.A);

        }


    }
}
