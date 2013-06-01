/// <copyright file="SequencerIllumina9.cs" author="Neil Robertson">
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
    class Sequencer_Illumina9 : Sequencer
    {
        public static Sequencer sequencer = new Sequencer_Illumina9();

        private readonly static String sequencerName = "Illumina 1.9";

	    private String machineName, multiplexIndex;
	    private int flowCellLane, tileNo, xCoOrd, yCoOrd, pairMember;
	
	    public static readonly int SEQUENCER_SPECIFIC_ASCII_SUBRTRACTION = 33;
	    public static readonly int qualityDistributionSpread = 40;

        private Sequencer_Illumina9() { }

        public override String getStatement()
        {
            return "This data is from a " + sequencerName;
        }
       
  	    public static void register()
  	    {
            //SequencerSpecifier value = sequencer as SequencerSpecifier;
  		    SequencerDiscriminator.register(sequencerName, sequencer);
 	    }
  	
	    /*
	     * method returns subtracts sequencer specific ASCII values to return specific numerical quality score
	     */

        public override int getQualityScore(char qualityValue)
        {
            int qualityScore;
            int ASCII_CHARACTER_NUMERICAL_VALUE = (int)qualityValue;
            qualityScore = ASCII_CHARACTER_NUMERICAL_VALUE - SEQUENCER_SPECIFIC_ASCII_SUBRTRACTION;

            return qualityScore;
        }

	    /*
	     * TODO: incomplete method converts quality score to phred probabity scores
	     */
        public override int getPhredProbability(char qualityValue)
        {
            int phredProbabilityScore = 0;
            return phredProbabilityScore;
        }

        public override int getDistributionSpread()
        {
            return qualityDistributionSpread;
        }

        public override String getMachineName(String header)
        {
            //@HWI-B5-690_0051_FC:3:1:5007:1023#ACAGTG/1
            String[] head = header.Split(':');
            machineName = head[0];
            return machineName;
        }


        public override int getFlowCellLane(String header)
        {
            //@HWI-B5-690_0051_FC:3:1:5007:1023#ACAGTG/1
            String[] head = header.Split(':');
            flowCellLane = int.Parse(head[1]);
            return flowCellLane;
        }

        public override int getTileNumber(String header)
        {
            //@HWI-B5-690_0051_FC:3:1:5007:1023#ACAGTG/1
            String[] head = header.Split(':');
            tileNo = int.Parse(head[2]);
            return tileNo;
        }

        public override int getXCoOrd(String header)
        {
            //@HWI-B5-690_0051_FC:3:1:5007:1023#ACAGTG/1
            String[] head = header.Split(':');
            xCoOrd = int.Parse(head[3]);
            return xCoOrd;
        }

        public override int getYCoOrd(String header)
        {
            //@HWI-B5-690_0051_FC:3:1:5007:1023#ACAGTG/1
            String[] head = header.Split(':');
            yCoOrd = int.Parse(head[4]);
            return yCoOrd;
        }


        public override String getMultiplexIndex(String header)
        {
            //@HWI-B5-690_0051_FC:3:1:5007:1023#ACAGTG/1
            String[] head = header.Split('#');
            String[] part = head[1].Split('/');
            multiplexIndex = part[0];
            return multiplexIndex;
        }


        public override int getPairMember(String header)
        {
            //@HWI-B5-690_0051_FC:3:1:5007:1023#ACAGTG/1
            String[] head = header.Split('#');
            String[] part = head[1].Split('/');
            pairMember = int.Parse(part[1]);
            return pairMember;
        }
    }
}
