using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    class SequencerSolexa : SequencerSpecifier
    {
        public static SequencerSpecifier sequencer = new SequencerSolexa();

        private readonly static String sequencerName = "Solexa";

	    private String machineName, multiplexIndex;
	    private int flowCellLane, tileNo, xCoOrd, yCoOrd, pairMember;
	
	    private readonly int SEQUENCER_SPECIFIC_ASCII_SUBRTRACTION = 64;
	    private readonly int qualityDistributionSpread = 40;

        private SequencerSolexa() { }

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
            
            if (qualityScore <= 10)
                qualityScore = reconcileSubTenQualities(qualityScore);

            return qualityScore;
        }

        private int reconcileSubTenQualities(int qualityScore)
        {
            switch (qualityScore)
            {
                case 10: qualityScore = 10;
                    break;
                case 9: qualityScore = 10;
                    break;
                case 8: qualityScore = 9;
                    break;
                case 7: qualityScore = 8;
                    break;
                case 6: qualityScore = 7;
                    break;
                case 5: qualityScore = 6;
                    break;
                case 4: qualityScore = 5;
                    break;
                case 3: qualityScore = 5;
                    break;
                case 2: qualityScore = 4;
                    break;
                case 1: qualityScore = 4;
                    break;
                case 0: qualityScore = 3;
                    break;
                case -1: qualityScore = 3;
                    break;
                case -2: qualityScore = 2;
                    break;
                case -3: qualityScore = 2;
                    break;
                case -4: qualityScore = 1;
                    break;
                case -5: qualityScore = 0;
                    break;
            }
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
