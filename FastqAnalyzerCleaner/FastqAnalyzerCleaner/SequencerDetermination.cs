/// <copyright file="SequencerDetermination.cs" author="Neil Robertson">
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
    class SequencerDetermination
    {
        private String sequencerType = null;
        private char sequencerIdentifierCode = '0';
        private Dictionary<int, FqNucleotideRead> map;

        public SequencerDetermination(IFqFile fastqFile)
        {
            map = fastqFile.getMap();
            sequencer(fastqFile);
        }

        private void sequencer(IFqFile fastqFile)
	    {
		    String sequencer = null;
		
		    Boolean completed = false;
		
		    Boolean sangerLowerBoundary = false, sangerUpperBoundary = false, solexaLowerBoundary = false, 
				    sharedUpperBoundary = false, illuminaThreeLowerBoundary = false, illuminaFiveLowerBoundary = false, 
					    illuminaEightLowerBoundary = false, illuminaNineLowerBoundary = false, illuminaEightNineUpperBoundary = false;
		
		    // char variables for the upper and lower boundaries of the sequencer types
		    char sangerLower = '!';
		    char sangerUpper = 'I';
		    char solexaLower = ';';
		    char solilmixedUpper = 'h';
		    char illuminathreeLower = '@';
		    char illuminafiveLower = 'B';
		    char illuminaeightLower = '!';
		    char illuminanineLower = '#';
		    char illuminamixedUpper = 'J';
		
		    Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
		
		    while (completed != true)
		    {
				for (int i = 0; i < fastqFile.getFastqArraySize();i++)
				{
					FqSequence fqSeq = fastqFile.getFastqSequenceByPosition(i);
					for (int j = 0; j < fqSeq.getFastqSeqSize(); j++)
					{
						char qualityValue = map[fqSeq.getFastqSeqAtPosition(j)].getQualityRead();
							
						if (qualityValue == sangerLower)		sangerLowerBoundary = true;
						else if (qualityValue == sangerUpper)	sangerUpperBoundary = true;
						else if (qualityValue == solexaLower) 	solexaLowerBoundary = true;
						else if (qualityValue == solilmixedUpper)	sharedUpperBoundary = true;
						else if (qualityValue == illuminathreeLower)illuminaThreeLowerBoundary = true;
						else if (qualityValue == illuminafiveLower)	illuminaFiveLowerBoundary = true;
						else if (qualityValue == illuminaeightLower)illuminaEightLowerBoundary = true;
						else if (qualityValue == illuminanineLower)	illuminaNineLowerBoundary = true;
						else if (qualityValue == illuminamixedUpper)illuminaEightNineUpperBoundary = true;
					}
				}
		
			if (sangerLowerBoundary != false && sangerUpperBoundary != false && illuminaEightNineUpperBoundary !=  true)
			{
					sequencer = "Sanger";
					completed = true;
			}
			else if (solexaLowerBoundary != false && sharedUpperBoundary != false)
			{
					sequencer = "Solexa";
					completed = true;
			}
			else if (illuminaThreeLowerBoundary != false && sharedUpperBoundary != false && solexaLowerBoundary != true)
			{
					sequencer = "Illumina 1.3";
					completed = true;
			}
			else if (illuminaFiveLowerBoundary != false && sharedUpperBoundary != false && illuminaThreeLowerBoundary != true)
			{
					sequencer = "Illumina 1.5";
					completed = true;					
			}
			else if (illuminaEightLowerBoundary != false && illuminaEightNineUpperBoundary != false)
			{
					sequencer = "Illumina 1.8";
					completed = true;					
			}
			else if (illuminaNineLowerBoundary != false && illuminaEightNineUpperBoundary != false && illuminaEightLowerBoundary != true)
			{
					sequencer = "Illumina 1.9";
					completed = true;					
			}	
			else if (completed != true)
			{
				sequencer = "Default";
				completed = true;
			}
			else if ((sequencer == null))
			{
				sequencer = "Default";
				completed = true;
			}
		}
		this.sequencerType = sequencer;
		
		fastqFile.setSequencerType(sequencerType);

        if (fastqFile is FqFile_Component)
        {
            fastqFile.setFqHashMap(FastqController.getInstance().GetFqFileMap().ConstructSequencerSpecificReadMap(sequencerType));
            Console.WriteLine("Calculating and setting sequencer specific file map to component");
        }

		stopwatch.Stop();
		Console.WriteLine("Time To Determine Sequencer:  " + stopwatch.Elapsed);
        Console.WriteLine("Sequencer Name: " + sequencer);
        Console.WriteLine("File contains {0} sequences", fastqFile.getFastqArraySize());
	}

        public String getSequencerType()
        {
            return sequencerType;
        }

        public char getIdentifierCode()
        {
            return sequencerIdentifierCode;
        }
    }
}
