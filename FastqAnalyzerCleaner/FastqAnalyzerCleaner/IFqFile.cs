using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FastqAnalyzerCleaner
{
    abstract class FqFile
    {
        abstract public void addFastqSequence(FqSequence fqSeq);
        abstract public void cleanStarts(int remove);
        abstract public void cleanEnds(int remove);
        abstract public void cleanTails(int start, int end);
        abstract public void removeRegion(int startBlock, int endBlock);
        abstract public String createNucleotideString();
        abstract public Nucleotide createNucleotideArray();
        abstract public String createFastqFormat();
        abstract public String createFastaFormat(String message);
        abstract public List<int> performDistributionTest();
        abstract public void performSequenceStatistics();
        abstract public void performNucleotideTests();
        abstract public void performJointTests();
        abstract public void findMisreads();
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
        abstract public List<int> getMisreadLocations();
        abstract public List<int> getDistribution();
        abstract public int totalNucleotides();
        abstract public int getMisreads();
        abstract public int getTotalNucleotides();
        abstract public FqSequence getFastqSequenceByPosition(int i);
        abstract public double getNCount();
        abstract public double getCCount();
        abstract public double getGCount();
        abstract public double cContents();
        abstract public double gContents();
        abstract public double nContents();
        abstract public Dictionary<int, FqNucleotideRead> getMap();
        abstract public void calculateMapQualities();
        abstract public void setHeader();
    }
}
