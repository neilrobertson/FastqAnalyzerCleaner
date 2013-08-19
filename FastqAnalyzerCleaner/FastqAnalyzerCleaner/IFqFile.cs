/// <copyright file="FqFile.cs" author="Neil Robertson">
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
using System.ComponentModel;


namespace FastqAnalyzerCleaner
{
    ///<summary>
    ///This abstract class is an interface and inherited by both single core and multicore fastq file classes.
    ///This classes represents the abstract methods that the fastq file classes must inherit.
    ///</summary>
    [Serializable]
    public abstract class IFqFile
    {
        public static readonly char INITIAL_HEADER_CHARACTER = '@';
        public static readonly char INITIAL_INFO_LINE_CHARACTER = '+';

        abstract public void addFastqSequence(FqSequence fqSeq);
        abstract public void cleanStarts(int remove);
        abstract public void cleanEnds(int remove);
        abstract public void cleanTails(int start, int end);
        abstract public void cleanAdapters();
        abstract public void removeBelowMeanThreshold(int threshold);
        abstract public void removeSequencesWithMisreads();
        abstract public List<FqSequence> findSequence(String sequence);
        abstract public void removeRegion(int startBlock, int endBlock);
        abstract public String createNucleotideString();
        abstract public Nucleotide createNucleotideArray();
        abstract public String createFastqFormat();
        abstract public String createFastaFormat(String message);
        abstract public List<int> performDistributionTest();
        abstract public void performSequenceStatistics();
        abstract public void performNucleotideTests();
        abstract public void performJointTests();
        abstract public void Tests();
        abstract public List<FqSequence> createFastqSeqList();
        abstract public void setFastqFileName(String fileName);
        abstract public void setSequencerType(String sequencerType);
        abstract public void setNucleotidesCleaned(int numberCleaned);
        abstract public int getFastqArraySize();
        abstract public FqSequence[] getFastqFile();
        abstract public String getFastqHeader();
        abstract public String getFileName();
        abstract public String getSequencerType();
        abstract public int getNucleotidesCleaned();
        abstract public List<int> getDistribution();
        abstract public List<int> getSequenceLengthDistribution();
        abstract public int totalNucleotides();
        abstract public int getTotalNucleotides();
        abstract public FqSequence getFastqSequenceByPosition(int i);
        abstract public int getNCount();
        abstract public int getCCount();
        abstract public int getGCount();
        abstract public double cContents();
        abstract public double gContents();
        abstract public double nContents();
        abstract public int getMaxSeqSize();
        abstract public int getMinSeqSize();
        abstract public int getSequencesRemoved();
        abstract public Dictionary<int, FqNucleotideRead> getMap();
        abstract public void calculateMapQualities();
        abstract public void setHeader();
        abstract public Dictionary<int, string> getRemovedAdapters();
        abstract public void setFqHashMap(Dictionary<int, FqNucleotideRead> map);
        abstract public void cleanArray();
        abstract public FqPerBaseSatistics[] GetPerBaseStatisticsArray();
    }
}
