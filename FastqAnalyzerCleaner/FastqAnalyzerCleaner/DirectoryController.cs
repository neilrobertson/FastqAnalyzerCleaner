/// <copyright file="DirectoryController.cs" author="Neil Robertson">
/// Copyright (c) 2013 All Right Reserved, Neil Alistair Robertson - neil.alistair.robertson@hotmail.co.uk
///
/// This code is the property of Neil Robertson.  Permission must be sought before reuse.
/// It has been written explicitly for the MRes Bioinfomatics course at the University 
/// of Glasgow, Scotland under the supervision of Derek Gatherer.
///
/// </copyright>
/// <author>Neil Robertson</author>
/// <email>neil.alistair.robertson@hotmail.co.uk</email>
/// <date>2013-06-1</date>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FastqAnalyzerCleaner
{
    /// <summary>
    /// The directory controller class will create a folder for fqprotobin files and set the programs environment directory to this
    /// folder
    /// </summary>
    public class DirectoryController
    {
        private static object syncLocker = new object();
        private static DirectoryController uniqueInstance;

        private static char DIRECTORY_SEPERATOR_CHARACTER = System.IO.Path.DirectorySeparatorChar;
        private static String PROTOBIN_DIRECTORY;

        /// <summary>
        /// Constructor for the DirectoryController class, creates the environment specific directory string, regardless of operating system
        /// </summary>
        private DirectoryController()
        {
            PROTOBIN_DIRECTORY = String.Format(@"{0}protobin_fq{1}", DIRECTORY_SEPERATOR_CHARACTER, DIRECTORY_SEPERATOR_CHARACTER);
        }

        /// <summary>
        /// Accessor method for this singleton, locked for synchronized access. Returns a unique instance of this class.
        /// </summary>
        /// <returns>A unique instance of the directory controller class</returns>
        public static DirectoryController getInstance()
        {
            lock (syncLocker)
            {
                if (uniqueInstance == null)
                    uniqueInstance = new DirectoryController();
                return uniqueInstance;
            }
        }

        /// <summary>
        /// Creates a folder for the desired working environment directory and sets the active environment directory 
        /// to this path.
        /// </summary>
        /// <returns>Returns a boolean if resetting the directory was successful</returns>
        public Boolean SetWorkingDirectory()
        {
            try
            {
                String path = Directory.GetCurrentDirectory();
                String target = String.Format(@"{0}{1}", path, PROTOBIN_DIRECTORY);

                if (Directory.Exists(target) == false)
                {
                    Directory.CreateDirectory(target);
                }
                Environment.CurrentDirectory = target;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("You are not in the working directory: {0}", e.ToString());
                return false;
            }
        }


    }
}
