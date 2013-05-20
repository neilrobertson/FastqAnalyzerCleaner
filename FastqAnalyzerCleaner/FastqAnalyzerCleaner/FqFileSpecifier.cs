using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    class FqFileSpecifier
    {
        private static FqFileSpecifier uniqueInstance;
        private static object syncLock = new object();

        private FqFileSpecifier() { }

        public static FqFileSpecifier getInstance()
        {
            if (uniqueInstance == null)
            {
                lock (syncLock)
                {
                    uniqueInstance = new FqFileSpecifier();
                }
            }
            return uniqueInstance;
        }

        public FqFile getFqFile(Boolean fqFileType)
        {
            FqFile fqFile;
            if (fqFileType)
            {
                fqFile = new MultiCore_FqFile();
                return fqFile;
            }
            else
            {
                fqFile = new SingleCore_FqFile();
                return fqFile;
            }
        }

    }
}
