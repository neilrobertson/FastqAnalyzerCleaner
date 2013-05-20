using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    /// <summary>
    /// This singleton class contains variables that set preferences for the program. This includes upper and lower boundries for
    /// nucleotide content, misread nucleotide content as well as how the sequencer is determined and whether to use multi or single 
    /// core processing for the Fastq files.
    /// </summary>
    class Preferences
    {
        private static Preferences uniqueInstance;
        private static Object syncLock = new object();

        private int ASSUMPTION_POINT = 1000;
        private int LOWER_C_BOUNDARY = 13, UPPER_C_BOUNDARY = 25;
        private int LOWER_G_BOUNDARY = 13, UPPER_G_BOUNDARY = 25;
        private int N_MISREADS_THRESHOLD = 1;
        private Boolean showSeqStats = true, sortMeanStats = false, sequencerDetermination = false, multiCore = true;

        /// <summary>
        /// Default constructor is private.  This class is initialized with a standard state that represents how the program should
        /// operate on initialization.
        /// </summary>
        private Preferences() { }

        /// <summary>
        /// Accessor method for the singleton class, returns a thread safe single instance of this class.
        /// </summary>
        /// <returns>A thread safe unique instance of the preferences class.</returns>
        public static Preferences getInstance()
        {
            if (uniqueInstance == null)
            {
                lock (syncLock)
                {
                    uniqueInstance = new Preferences();
                }
            }
            return uniqueInstance;
        }

        /// <summary>
        /// This method allows the user to reset all of the preferences in the class.
        /// </summary>
        /// <param name="LOWER_C_BOUNDARY">The lower boundry of acceptability for Cytosine content.</param>
        /// <param name="UPPER_C_BOUNDARY">The upper boundry of acceptability for Cytosine content.</param>
        /// <param name="LOWER_G_BOUNDARY">The lower boundry of acceptability for Guanine content.</param>
        /// <param name="UPPER_G_BOUNDARY">The upper boundry of acceptability for Guanine content.</param>
        /// <param name="N_MISREADS_THRESHOLD">The acceptable threshold for misread nucleotide contents.</param>
        /// <param name="ASSUMPTION_POINT">The assumption point at which the sequencer decision tree will select a sequencer type.  
        ///  This figure represents the total number of sequences to test.</param>
        /// <param name="showSeq">Show sequences as part of the sequence statistics function.</param>
        /// <param name="sortMean">Sort sequences by the mean of their statistics.</param>
        /// <param name="mCores">Use multiple or single logical cores in the processing of files (where applicable).</param>
        public void setPreferences(int LOWER_C_BOUNDARY, int UPPER_C_BOUNDARY, int LOWER_G_BOUNDARY, int UPPER_G_BOUNDARY, int N_MISREADS_THRESHOLD, int ASSUMPTION_POINT, Boolean showSeq, Boolean sortMean, Boolean mCores)
        {
            this.LOWER_C_BOUNDARY = LOWER_C_BOUNDARY;
            this.UPPER_C_BOUNDARY = UPPER_C_BOUNDARY;
            this.LOWER_G_BOUNDARY = LOWER_G_BOUNDARY;
            this.UPPER_G_BOUNDARY = UPPER_G_BOUNDARY;
            this.N_MISREADS_THRESHOLD = N_MISREADS_THRESHOLD;
            this.ASSUMPTION_POINT = ASSUMPTION_POINT;
            this.showSeqStats = showSeq;
            this.sortMeanStats = sortMean;
            this.multiCore = mCores;
        }

        /// <summary>
        /// Returns the upper Guanine acceptability threshold
        /// </summary>
        /// <returns></returns>
        public int getUpperGPref()
        {
            return LOWER_G_BOUNDARY;
        }

        /// <summary>
        /// Returns the lower Guanine acceptability threshold
        /// </summary>
        /// <returns></returns>
        public int getLowerGPref()
        {
            return UPPER_G_BOUNDARY;
        }

        /// <summary>
        /// Returns the upper Cytosine acceptability threshold
        /// </summary>
        /// <returns></returns>
        public int getUpperCPref()
        {
            return UPPER_C_BOUNDARY;
        }

        /// <summary>
        /// Returns the lower Cytosine acceptability threshold
        /// </summary>
        /// <returns></returns>
        public int getLowerCPref()
        {
            return LOWER_C_BOUNDARY;
        }

        public int getNPref()
        {
            return N_MISREADS_THRESHOLD;
        }

        public int getAssumptionPref()
        {
            return ASSUMPTION_POINT;
        }

        public Boolean getShowSeqPref()
        {
            return showSeqStats;
        }

        public Boolean getSortMeanStatsPref()
        {
            return sortMeanStats;
        }

        public void setSeqDecisionMethod(Boolean seqDec)
        {
            this.sequencerDetermination = seqDec;
        }

        public Boolean getSeqDecisionMethod()
        {
            return sequencerDetermination;
        }

        public void setMultiCoreProcessing(Boolean multi)
        {
            this.multiCore = multi;
        }

        public Boolean getMultiCoreProcessing()
        {
            return multiCore;
        }
    }
}
