using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
	///<summary>
	///SequencerSpecifier is an abstract class that is an interface and is inherited by each sequencer type class.
	///This class contains the methods that are used by each sequencer type.
	///</summary>
    abstract class SequencerSpecifier
    {
        abstract public string getStatement();
        abstract public int getPhredProbability(char qualityValue);
        abstract public int getQualityScore(char qualityValue);
        abstract public string getMachineName(string header);
        abstract public string getMultiplexIndex(string header);
        abstract public int getPairMember(string header);
        abstract public int getTileNumber(string header);
        abstract public int getXCoOrd(string header);
        abstract public int getYCoOrd(string header);
        abstract public int getFlowCellLane(string header);
        abstract public int getDistributionSpread();
    }
}
