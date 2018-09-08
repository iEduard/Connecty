using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace Connecty
{
    public static class UserHelper
    {
        // Pre Aded Variables for Future use
        private static string helpFileName = "ConnectyHilfe.chm";
        private static string fileLocation = "";

        /// <summary>
        /// Show up the User Help CHM File
        /// </summary>
        public static void showUserHelp()
        {
            // Check if the File Exists
            if (checkHelpFileEist()) 
            {
                openHelpFile();
            }
            else
            {
                createHelpFile();
                openHelpFile();
            }

        }

        /// <summary>
        /// Check if the Help File exist
        /// </summary>
        private static bool checkHelpFileEist()
        {
            long fileSize = 0;
            long resourceSize = 0;

            // Check if the File Exist
            if(File.Exists("ConnectyHilfe.chm")){

                // Read the Resource Help File Length
                using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Connecty.Resources.ConnectyHilfe.chm"))
                {
                    resourceSize = stream.Length;
                    stream.Close();
                }
                
                // Read the Length of the File Existing
                using (FileStream fs = File.Open("ConnectyHilfe.chm", FileMode.Open)) 
                {
                    fileSize = fs.Length;
                    fs.Close();
                }

                // Compare those thwo File Sizes
                if (fileSize != resourceSize)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            else
            {
                return false;
            }

        }


        /// <summary>
        /// Create the file on the Filesystem from the Resource File
        /// </summary>
        private static void createHelpFile()
        {
            //Write the File to the FileSystem
            using (System.IO.Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Connecty.Resources.ConnectyHilfe.chm"))
            {
                
                using (System.IO.FileStream fileStream = new System.IO.FileStream(System.IO.Path.Combine("ConnectyHilfe.chm"), System.IO.FileMode.Create))
                {
                    for (int i = 0; i < stream.Length; i++)
                    {
                        fileStream.WriteByte((byte)stream.ReadByte());
                    }
                    fileStream.Close();
                }
            }

            
        }


        /// <summary>
        /// Open up the CHM Help File
        /// </summary>
        private static void openHelpFile()
        {
            System.Diagnostics.Process.Start(@"ConnectyHilfe.chm");
        }

    }
}
