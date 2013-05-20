using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
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
