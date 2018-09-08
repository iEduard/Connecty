using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connecty
{
    /// <summary>
    /// This Static Class is a collection of conversion Functions
    /// </summary>
    static class Converty
    {

        private static string startDelemiter = "<h";
        private static string endDelimiter = ">";

        #region Conversions for the ASCII String

        /// <summary>
        /// Get the Value from the Message and convert it to an ASCII Charakter String
        /// This is used to show User hints
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string msgDataToAsciiChar(MsgData msg)
        {
            // Convert the Array of byte to a string
            ASCIIEncoding encoder = new ASCIIEncoding();
            return encoder.GetString(msg.value, 0, (int)msg.value.LongLength);

        }

        /// <summary>
        /// Convert Array of Byte to a String and replace the Non ASCII Bytes and ASCII Special CHARS to
        /// a defined set of ASCII Signs
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string msgDataToSpecialAsciiString(byte[] msg)
        {

            ASCIIEncoding encoder = new ASCIIEncoding();

            byte[] myByteArray = replaceFromNonAsciiChar(msg, startDelemiter, endDelimiter);
            return asciiSpecialToString(encoder.GetString(myByteArray, 0, (int)myByteArray.LongLength));

        }

        /// <summary>
        /// Convert the String to an Array of Byte and replace the Set of ASCII Signs that indicates a NON ASCII Char
        /// or a ASCII Special CHAR to the coresponding Byte Value
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] specialAsciiStringToMsgData(string msg)
        {

            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] myByte = replaceToNonAsciiChar(encoder.GetBytes(stringToAsciiSpecial(msg)), startDelemiter, endDelimiter);

            return myByte;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringData"></param>
        /// <returns></returns>
        private static string asciiSpecialToString(string stringData)
        {

            // Replace the ASCII Chars to something human can read
            stringData = stringData.Replace(((char)0).ToString(), "<NUL>");
            stringData = stringData.Replace(((char)1).ToString(), "<SOH>");
            stringData = stringData.Replace(((char)2).ToString(), "<STX>");
            stringData = stringData.Replace(((char)3).ToString(), "<ETX>");
            stringData = stringData.Replace(((char)4).ToString(), "<EOT>");
            stringData = stringData.Replace(((char)5).ToString(), "<ENQ>");
            stringData = stringData.Replace(((char)6).ToString(), "<ACK>");
            stringData = stringData.Replace(((char)7).ToString(), "<BEL>");
            stringData = stringData.Replace(((char)8).ToString(), "<BS>");
            stringData = stringData.Replace(((char)9).ToString(), "<HT>");
            stringData = stringData.Replace(((char)10).ToString(), "<LF>");
            stringData = stringData.Replace(((char)11).ToString(), "<VT>");
            stringData = stringData.Replace(((char)12).ToString(), "<FF>");
            stringData = stringData.Replace(((char)13).ToString(), "<CR>");
            stringData = stringData.Replace(((char)14).ToString(), "<SO>");
            stringData = stringData.Replace(((char)15).ToString(), "<SI>");
            stringData = stringData.Replace(((char)16).ToString(), "<DLE>");
            stringData = stringData.Replace(((char)17).ToString(), "<DC1>");
            stringData = stringData.Replace(((char)18).ToString(), "<DC2>");
            stringData = stringData.Replace(((char)19).ToString(), "<DC3>");
            stringData = stringData.Replace(((char)20).ToString(), "<DC4>");
            stringData = stringData.Replace(((char)21).ToString(), "<NAK>");
            stringData = stringData.Replace(((char)22).ToString(), "<SYN>");
            stringData = stringData.Replace(((char)23).ToString(), "<ETB>");
            stringData = stringData.Replace(((char)24).ToString(), "<CAN>");
            stringData = stringData.Replace(((char)25).ToString(), "<EM>");
            stringData = stringData.Replace(((char)26).ToString(), "<SUB>");
            stringData = stringData.Replace(((char)27).ToString(), "<ESC>");
            stringData = stringData.Replace(((char)28).ToString(), "<FS>");
            stringData = stringData.Replace(((char)29).ToString(), "<GS>");
            stringData = stringData.Replace(((char)30).ToString(), "<RS>");
            stringData = stringData.Replace(((char)31).ToString(), "<US>");
            stringData = stringData.Replace(((char)127).ToString(), "<DEL>");
            return stringData;
        }

        /// <summary>
        /// Replace the Strings to the Corresponding ASCII Signs
        /// </summary>
        /// <param name="stringData"></param>
        /// <returns></returns>
        private static string stringToAsciiSpecial(string stringData)
        {
            // Replace the Special ASCII Chars
            stringData = stringData.Replace("<NUL>", ((char)0).ToString());
            stringData = stringData.Replace("<SOH>", ((char)1).ToString());
            stringData = stringData.Replace("<STX>", ((char)2).ToString());
            stringData = stringData.Replace("<ETX>", ((char)3).ToString());
            stringData = stringData.Replace("<EOT>", ((char)4).ToString());
            stringData = stringData.Replace("<ENQ>", ((char)5).ToString());
            stringData = stringData.Replace("<ACK>", ((char)6).ToString());
            stringData = stringData.Replace("<BEL>", ((char)7).ToString());
            stringData = stringData.Replace("<BS>", ((char)8).ToString());
            stringData = stringData.Replace("<HT>", ((char)9).ToString());
            stringData = stringData.Replace("<LF>", ((char)10).ToString());
            stringData = stringData.Replace("<VT>", ((char)11).ToString());
            stringData = stringData.Replace("<FF>", ((char)12).ToString());
            stringData = stringData.Replace("<CR>", ((char)13).ToString());
            stringData = stringData.Replace("<SO>", ((char)14).ToString());
            stringData = stringData.Replace("<SI>", ((char)15).ToString());
            stringData = stringData.Replace("<DLE>", ((char)16).ToString());
            stringData = stringData.Replace("<DC1>", ((char)17).ToString());
            stringData = stringData.Replace("<DC2>", ((char)18).ToString());
            stringData = stringData.Replace("<DC3>", ((char)19).ToString());
            stringData = stringData.Replace("<DC4>", ((char)20).ToString());
            stringData = stringData.Replace("<NAK>", ((char)21).ToString());
            stringData = stringData.Replace("<SYN>", ((char)22).ToString());
            stringData = stringData.Replace("<ETB>", ((char)23).ToString());
            stringData = stringData.Replace("<CAN>", ((char)24).ToString());
            stringData = stringData.Replace("<EM>", ((char)25).ToString());
            stringData = stringData.Replace("<SUB>", ((char)26).ToString());
            stringData = stringData.Replace("<ESC>", ((char)27).ToString());
            stringData = stringData.Replace("<FS>", ((char)28).ToString());
            stringData = stringData.Replace("<GS>", ((char)29).ToString());
            stringData = stringData.Replace("<RS>", ((char)30).ToString());
            stringData = stringData.Replace("<US>", ((char)31).ToString());
            stringData = stringData.Replace("<DEL>", ((char)127).ToString());

            return stringData;
        }

        /// <summary>
        /// Replace the non ASCII CHAR in the Array of Byte to a set of Human Readable ASCII Signs
        /// So to Say Bytes from 128 to 255
        /// The Result will be shown as folows:
        /// xxFFyy
        /// xx = Start Sign of the Non ASCII Char
        /// FF = Value of he CHAR/Byte in HEX
        /// yy = End Sign of the Non ASCII Char 
        /// </summary>
        /// <param name="msgData"></param>
        /// <param name="startSign"></param>
        /// <param name="endSign"></param>
        /// <returns></returns>
        private static byte[] replaceFromNonAsciiChar(byte[] msgData, string startSign, string endSign)
        {
            LinkedList<byte> convertedData = new LinkedList<byte>();

            // Search the Data for non ASCII Chars
            foreach (byte data in msgData)
            {

                // Check if we found a non ASCII char
                if(data > 127){

                    // Set the Start Delemiter in front
                    foreach(char myChar in startSign)
                    {
                        convertedData.AddLast((byte)myChar);

                    }

                    // Convert the Data to an HEX String
                    foreach (char myChar in Convert.ToInt32(data).ToString("x2").ToUpper().ToCharArray())
                    {
                        convertedData.AddLast((byte)myChar);
                    }

                    // Set the End Delimiter at the end
                    foreach (char myChar in endSign)
                    {
                        convertedData.AddLast((byte)myChar);
                    }
                
                }
                // An ASCII Sign was found
                else 
                {
                    convertedData.AddLast(data);
                }
            }

            return convertedData.ToArray();
        }


        /// <summary>
        /// Replace the non ASCII CHAR (From the String) in the Array of Byte to the Coresponding byte Value
        /// So to Say Bytes from 128 to 255
        /// The Result will be shown as folows:
        /// xxFFyy
        /// xx = Start Sign of the Non ASCII Char
        /// FF = Value of he CHAR/Byte in HEX
        /// yy = End Sign of the Non ASCII Char
        /// </summary>
        /// <param name="msgData"></param>
        /// <param name="startSign"></param>
        /// <param name="endSign"></param>
        /// <returns></returns>
        private static byte[] replaceToNonAsciiChar(byte[] msgData, string startSign, string endSign)
        {

            int nonCharSize = startSign.Length + endSign.Length + 2;
            bool nonCharFound = false;
            bool checkResult = false;

            LinkedList<byte> convertedData = new LinkedList<byte>();


            for (int i = 0; i < msgData.Length; i++)
            {
                // Preset the Helper
                nonCharFound = false;
                checkResult = true;

                // check if the non ascii char has even enough space to exist
                if (i + nonCharSize <= msgData.Length)
                {
                    
                    // Check the Start Delimiter
                    // Copy the Data to the Linked List
                    for (int x = 0; x < startSign.Length; x++)
                    {
                        if (msgData[i + x] != startSign.ToCharArray()[x])
                        {
                            checkResult = false;
                        }

                    }

                    // Check the Enddelimiter
                    // Copy the Data to the Linked List
                    for (int x = 0; x < endSign.Length; x++)
                    {
                        if (msgData[i + startSign.Length + 2 + x] != endSign.ToCharArray()[x])
                        {
                            checkResult = false;
                        }

                    }

                    // Check the Result
                    if(checkResult)
                    {

                        char msbChar = (char)msgData[i + startSign.Length];
                        char lsbChar = (char)msgData[i + startSign.Length + 1];

                        string myString = msbChar.ToString() + lsbChar.ToString();

                        convertedData.AddLast(Convert.ToByte(myString, 16));
                        // We found a Non Char
                        nonCharFound = true;

                        // Set the count Variable to jump over the nonChar Bytes
                        i = i + nonCharSize;

                        // Keep in mind that we will count up after the For loop finisjed
                        i = i - 1; // :-) Just to keep the Math above simple

                    }
                }


                // Check if a Non Char Was Found otherwise copy the Data
                if (!nonCharFound)
                {
                    convertedData.AddLast(msgData[i]);
                }


            }




            return convertedData.ToArray();

        }

        #endregion


        #region Conversions for the HEX / DEC and Binary View

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string msgDataToHexData(MsgData msg)
        {

            var sb = new StringBuilder();
            foreach (char t in msg.value)
            {
                //sb.Append(String.Format("{0:X}", t));
                sb.Append(Convert.ToInt32(t).ToString("x2") + " ");
            }

            return sb.ToString().ToUpper();

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string msgDataToDecData(MsgData msg)
        {

            var sb = new StringBuilder();
            foreach (char t in msg.value)
                sb.Append(Convert.ToInt32(t).ToString("G") + " ");
            return sb.ToString();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string msgDataToBinData(MsgData msg)
        {

            var sb = new StringBuilder();
            foreach (byte t in msg.value)
            {
                sb.Append(Convert.ToString(t, 2) + " ");
            }
            return sb.ToString();

        }

        #endregion

    }
}
