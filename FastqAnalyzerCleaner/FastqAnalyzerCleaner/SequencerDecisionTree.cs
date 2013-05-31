/// <copyright file="SequencerDecisionTree.cs" author="Neil Robertson">
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
using System.Diagnostics;

namespace FastqAnalyzerCleaner
{
    class SequencerDecisionTree
    {
        private String sequencerType = null;
        private int index;
        private FqFile fastqFile;
        private FqSequence fastqSeq;
        private Dictionary<int, FqNucleotideRead> map;

        // the number of nucleotides to move through before making an assumption
        int ASSUMPTION_POINT = 1000;

        Stopwatch stopwatch = new Stopwatch();

        public SequencerDecisionTree(FqFile aFastqFile)
        {
            ASSUMPTION_POINT = Preferences.getInstance().getAssumptionPref();
            fastqFile = aFastqFile;
            index = fastqFile.getFastqArraySize();
            map = fastqFile.getMap();
            DecisionTree(fastqFile);
        }

        /**
         * This method outlines the first decision to test quality data and return whether it resides 
         * in the higher ASCII regions greater than character J or in the lower regions below this 
         * point
         * @param fastqFile
         */
        private void DecisionTree(FqFile fastqFile)
	    {
		    stopwatch.Start();
		    Boolean upper = false, lower = false;
		    int i = 0, j = 0;
			
		    for (i = 0; i < index; i++)
		    {
			    fastqSeq = fastqFile.getFastqSequenceByPosition(i);
			    for (j = 0; j < fastqSeq.getFastqSeqSize(); j++)
			    {
				    char qualityValue = map[fastqSeq.getFastqSeqAtPosition(j)].getQualityRead();
				    if (qualityValue > 'J')
				    {
					    upper = true; 
					    goto search;
				    }
				    else if (qualityValue < ';')
				    {
					    lower = true;
					    goto search;
				    }
			    }	
		    }
        search:
		    if (upper == true)
		    {
			    upperTree(i, j);
		    }
		    else if (lower == true)
		    {
			    lowerTree(i, j);
		    }
	    }

        /*--------------Upper ASCII Sequencer Types-------------------*/
        /**
         * This method is created if the data set shows that the sequencer is likely from the 
         * solexa, illumina 1.3 or 1.5 branches.  startPosition is the starting point to iterate 
         * through array
         * @param startPosition
         */
        private void upperTree(int startPosition, int startPos)
	    {
		    Boolean solexa = false, jointSolexaIllumina = false;
		    int i = startPosition, j = startPos;
			
		    for (i = startPosition; i < index; i++)
		    {
			    fastqSeq = fastqFile.getFastqSequenceByPosition(i);
			    for (j = startPos; j < fastqSeq.getFastqSeqSize(); j++)
			    {
				    char qualityValue = map[fastqSeq.getFastqSeqAtPosition(j)].getQualityRead();
			
				    if (qualityValue < 'B')
				    {
					    jointSolexaIllumina = true; 
					    goto search;
				    }
				    else if (qualityValue < '@')
				    {
					    solexa = true;
					    goto search;
				    }
				    else if (i == ASSUMPTION_POINT + startPosition)
				    {
					    sequencerType = "Illumina 1.5";
					    end(i);
					    goto search;
				    }
			    }
		    }
        search:
		    if (solexa == true)
		    {
			    sequencerType = "Solexa";
			    end(i);
		    }
		    else if (jointSolexaIllumina == true)
		    {
			    this.solexaIllumina3(i, j);
		    }
	    }

        private void solexaIllumina3(int startPosition, int startPos)
	    {
		    Boolean solexa = false;
		    int i, j;
			
		    for (i = startPosition; i < index; i++)
		    {
			    fastqSeq = fastqFile.getFastqSequenceByPosition(i);
			    for (j = startPos; j < fastqSeq.getFastqSeqSize(); j++)
			    {
				    char qualityValue = map[fastqSeq.getFastqSeqAtPosition(j)].getQualityRead();
			
				    if (qualityValue < '@')
				    {
					    solexa = true; 
					    goto search;
				    }
				    else if (i == ASSUMPTION_POINT + startPosition)
				    {
					    sequencerType = "Illumina 1.3";
					    end(i);
					    goto search;
				    }
			    }
		    }
        search:
		    if (solexa == true)
		    {
			    sequencerType = "Solexa";
			    end(i);
		    }
	    }

        /*-----------------Lower ASCII Sequencer Regions-------------------*/

        private void lowerTree(int startPosition, int startPos)
	    {
		    Boolean sangerEight = false, illuminaEightNine = false;
		    int i = startPosition;
		    int j = startPos;
		
		    for (i = startPosition; i < index; i++)
		    {
			    fastqSeq = fastqFile.getFastqSequenceByPosition(i);
			    for (j = startPos; j < fastqSeq.getFastqSeqSize(); j++)
			    {
				    char qualityValue = map[fastqSeq.getFastqSeqAtPosition(j)].getQualityRead();
			
				    if (qualityValue < '#')
				    {
					    sangerEight = true; 
					    goto search;
				    }
				    else if (qualityValue > 'I')
				    {
					    illuminaEightNine = true;
					    goto search;
				    }
			    }
		    }
        search:	
		    if (sangerEight == true)
		    {
			    this.sangerIlluminaEight(i, j);
		    }
		    else if (illuminaEightNine == true)
		    {
			    this.illuminaEightNine(i, j);
		    }
	    }

        private void sangerIlluminaEight(int startPosition, int startPos)
	    {
		    Boolean sanger = false;
		    int i, j;
		
		    for (i = startPosition; i < index; i++)
		    {
			    fastqSeq = fastqFile.getFastqSequenceByPosition(i);
			    for (j = startPos; j < fastqSeq.getFastqSeqSize(); j++)
			    {
				    char qualityValue = map[fastqSeq.getFastqSeqAtPosition(j)].getQualityRead();
			
				    if (qualityValue > 'I')
				    {
					    sanger = true; 
					    goto search;
				    }
				    else if (i == ASSUMPTION_POINT + startPosition)
				    {
					    sequencerType = "Illumina 1.8";
					    end(i);
					    goto search;
				    }
			    }
		    }	
	      search:	
		    if (sanger == true)
		    {
			    sequencerType = "Sanger";
			    end(i);
		    }
	    }

        private void illuminaEightNine(int startPosition, int startPos)
	    {
		    Boolean illuminaEight = false;
		    int i = startPosition, j = startPos;
			
		    for (i = startPosition; i < index; i++)
		    {
			    fastqSeq = fastqFile.getFastqSequenceByPosition(i);
			    for (j = startPos; j < fastqSeq.getFastqSeqSize(); j++)
			    {
				    char qualityValue = map[fastqSeq.getFastqSeqAtPosition(j)].getQualityRead();
			
				    if (qualityValue < '#')
				    {
					    illuminaEight = true; 
					    goto search;
				    }
				    else if (i == ASSUMPTION_POINT + startPosition)
				    {
					    sequencerType = "Illumina 1.9";
					    end(i);
					    goto search;
				    }
			    }
		    }
        search:
		    if (illuminaEight == true)
		    {
			    sequencerType = "Illumina 1.8";
			    end(i);
		    }
	    }

        /*
         * end point
         */
        private void end(int endPoint)
	    {
		    int i = endPoint;
            stopwatch.Stop();
		    fastqFile.setSequencerType(sequencerType);
            Console.Write("Sequencer Type: " + sequencerType + "\n");
            Console.Write("Time To Determine Sequencer (Decision Tree):  " + stopwatch.Elapsed + "   In " + i + " hits" + "\n");
            Console.Write("Array Size: " + fastqFile.getFastqArraySize() + "\n");
	    }


        public void setAssumptionPoint(int anASSUMPTION_POINT)
        {
            ASSUMPTION_POINT = anASSUMPTION_POINT;
        }

        public String getSequencerType()
        {
            return sequencerType;
        } 
    }
}
