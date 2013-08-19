// <copyright file="FastqController.cs" author="Neil Robertson">
// Copyright (c) 2013 All Right Reserved, Neil Alistair Robertson - neil.alistair.robertson@hotmail.co.uk
//
// This code is the property of Neil Robertson.  Permission must be sought before reuse.
// It has been written explicitly for the MRes Bioinfomatics course at the University 
// of Glasgow, Scotland under the supervision of Derek Gatherer.
//
// </copyright>
// <author>Neil Robertson</author>
// <email>neil.alistair.robertson@hotmail.co.uk</email>
// <date>2013-06-1</date>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    /// <summary>
    /// Simple class for outputting the details of a fqFile to the console
    /// </summary>
    public class FastqGUI_Output
    {
        private static FastqGUI_Output uniqueInstance;
        private static object syncLock = new object();

        /// <summary>
        /// Private constructor for the class
        /// </summary>
        private FastqGUI_Output() { }

        /// <summary>
        /// Accessor returns unique instance of this singleton class
        /// </summary>
        /// <returns>The unique instance of this singleton class</returns>
        public static FastqGUI_Output getInstance() 
        {
            lock (syncLock)
            {
                if (uniqueInstance == null)
                    uniqueInstance = new FastqGUI_Output();

                return uniqueInstance;
            }
        }

        /// <summary>
        /// Outputs the data for a FastqFile to the console, keeping code clean in other classes
        /// </summary>
        /// <param name="fqFile"></param>
        public void OutputFileDataToConsole(IFqFile fqFile)
        {
            Console.WriteLine("Joint Test Results Completed on " + fqFile.getTotalNucleotides() + " Nucleotides");
            Console.WriteLine("Joint Test Results: " + fqFile.getGCount() + "G   " + Math.Round(fqFile.gContents(), 2) + "%   " + fqFile.getCCount() + "C " + Math.Round(fqFile.cContents(), 2) + " %");
            Console.WriteLine("Misreads:  " + fqFile.getNCount());
            Console.WriteLine("Nucleotides Cleaned: {0}", fqFile.getNucleotidesCleaned());
            Console.WriteLine("Distribution:  " + fqFile.getDistribution().Count);
            Console.WriteLine("Stats Performed");
            for (int i = 0; i < 20; i++)
            {
                FqSequence fqSeq = fqFile.getFastqSequenceByPosition(i);
                Console.WriteLine("--  -Stats for Sequence " + (i + 1) + ": LB: {0}  1Q: {1}  Median: {2} Mean: {3} 3Q: {4} UB: {5}", fqSeq.getLowerThreshold(), fqSeq.getFirstQuartile(), fqSeq.getMedian(), Math.Round(fqSeq.getMean(), 2), fqSeq.getThirdQuartile(), fqSeq.getUpperThreshold());
            }
            for (int i = 0; i < fqFile.getDistribution().Count; i++)
                Console.WriteLine("--->  Quality Score: {0}   Count: {1}", i, fqFile.getDistribution()[i]);
            for (int i = 0; i < fqFile.GetPerBaseStatisticsArray().Length; i ++)
                Console.WriteLine("===> BaseStatistic: {0} \tCount: {1} LB: {2}  1Q: {3}  Median: {4} Mean: {5} 3Q: {6} UB: {7}", i, fqFile.GetPerBaseStatisticsArray()[i].BaseCount, fqFile.GetPerBaseStatisticsArray()[i].LowerThreshold, fqFile.GetPerBaseStatisticsArray()[i].FirstQuartile, fqFile.GetPerBaseStatisticsArray()[i].Median, fqFile.GetPerBaseStatisticsArray()[i].Mean, fqFile.GetPerBaseStatisticsArray()[i].ThirdQuartile, fqFile.GetPerBaseStatisticsArray()[i].UpperThreshold);

        }
    }
}
