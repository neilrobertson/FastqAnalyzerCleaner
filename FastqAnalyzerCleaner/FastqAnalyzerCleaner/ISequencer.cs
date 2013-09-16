/// <copyright file="ISequencer.cs" author="Neil Robertson">
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
	///<summary>
	///SequencerSpecifier is an abstract class that is an interface and is inherited by each sequencer type class.
	///This class contains the methods that are used by each sequencer type.
	///</summary>
    public abstract class ISequencer
    {
        abstract public string getStatement();
        abstract public int getQualityScore(char qualityValue);
        abstract public double getPHREDScore(int qualityScore);
        abstract public string getMachineName(string header);
        abstract public string getMultiplexIndex(string header);
        abstract public int getPairMember(string header);
        abstract public int getTileNumber(string header);
        abstract public int getXCoOrd(string header);
        abstract public int getYCoOrd(string header);
        abstract public int getFlowCellLane(string header);
        abstract public int getDistributionSpread();
        abstract public int getSubZeroQualities();
    }
}
