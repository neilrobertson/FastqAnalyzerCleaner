/// <copyright file="FqFileSpecifier.cs" author="Neil Robertson">
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
                fqFile = new FqFile_MultiCore();
                return fqFile;
            }
            else
            {
                fqFile = new FqFile_SingleCore();
                return fqFile;
            }
        }

    }
}
