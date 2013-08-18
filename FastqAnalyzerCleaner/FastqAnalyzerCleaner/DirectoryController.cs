using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FastqAnalyzerCleaner
{
    public class DirectoryController
    {
        private static object syncLocker = new object();
        private static DirectoryController uniqueInstance;

        private static char DIRECTORY_SEPERATOR_CHARACTER = System.IO.Path.DirectorySeparatorChar;
        private static String PROTOBIN_DIRECTORY;


        private DirectoryController()
        {
            PROTOBIN_DIRECTORY = String.Format(@"{0}protobin_fq{1}", DIRECTORY_SEPERATOR_CHARACTER, DIRECTORY_SEPERATOR_CHARACTER);
        }

        public static DirectoryController getInstance()
        {
            lock (syncLocker)
            {
                if (uniqueInstance == null)
                    uniqueInstance = new DirectoryController();
                return uniqueInstance;
            }
        }

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
