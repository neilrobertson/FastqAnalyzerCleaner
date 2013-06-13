﻿/// <copyright file="FqFile_SingleCore.cs" author="Neil Robertson">
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
using System.Threading;
using System.ComponentModel;

namespace FastqAnalyzerCleaner
{
    class FqFile_SingleCore : FqFile
    {
        private int LENGTH_SEQUENCE_ARRAY = 100000;
	    private int index = 0;
	
	    private String header, sequencerType, fileName;
	
	    private int totalNucs, nucleotidesCleaned;
	    private List<int> distribution;
	    private int nCount, cCount, gCount;
	    private double nPercent, cPercent, gPercent;
	    private static Dictionary<int, FqNucleotideRead> Fq_FILE_MAP;
	    private Dictionary<int, string> removedAdapters;

	    FqSequence[] fastqSeq;

        public FqFile_SingleCore()
	    {
		    fastqSeq = new FqSequence[LENGTH_SEQUENCE_ARRAY];
            Console.WriteLine("Single Core Fastq File Created.");
	    }

        public override void addFastqSequence(FqSequence fqSeq)
	    {
		    fastqSeq[index] = fqSeq;
		    index++;
		
		    if(index >= LENGTH_SEQUENCE_ARRAY - 1)
            {
                LENGTH_SEQUENCE_ARRAY = LENGTH_SEQUENCE_ARRAY + LENGTH_SEQUENCE_ARRAY;
                Array.Resize<FqSequence>(ref fastqSeq, (LENGTH_SEQUENCE_ARRAY));
            }
	    }

        public override void cleanStarts(int remove)
	    {
		    for (int i = 0; i < index; i++)
		    {
			    fastqSeq[i].cleanStarts(remove);
			    nucleotidesCleaned = nucleotidesCleaned + remove;
		    }
	    }

        public override void cleanEnds(int remove)
	    {
		    for (int i = 0; i < index; i++)
		    {
			    fastqSeq[i].cleanEnds(remove);
			    nucleotidesCleaned += remove;
		    }
		
	    }

        public override void cleanTails(int start, int end)
	    {
		    int totalRemove = start + end;
		    for (int i = 0; i < index; i++)
		    {
			    fastqSeq[i].cleanStarts(start);
			    fastqSeq[i].cleanEnds(end);
			    nucleotidesCleaned += totalRemove;
		    }
		    Console.Write("Sequence tails cleaned. Total Removed: " + totalRemove);
	    }

        public override void cleanAdapters()
        {
            List<Adapters.Adapter> adapters = Adapters.getInstance().getAdaptersList();
            removedAdapters = new Dictionary<int, string>();

            for (int i = 0; i < index; i ++)
            {
                GenericFastqInputs removedAdapter = fastqSeq[i].cleanAdapters(adapters, Fq_FILE_MAP);
                if (removedAdapter != null)
                    removedAdapters.Add(removedAdapter.SequenceIndex, removedAdapter.AdapterName);
            }
        }

        public override void removeRegion(int startBlock, int endBlock)
	    {
		
	    }

        public override String createNucleotideString()
	    {
		    StringBuilder nucleotides = new StringBuilder();
		    for (int i = 0; i < index; i++)
		    {
			    for (int j = 0; i < fastqSeq[i].getFastqSeqSize(); j++)
			    {
				    nucleotides.Append(Fq_FILE_MAP[fastqSeq[i].getFastqSeqAtPosition(j)].getNucleotide());
			    }
		    }
		    Console.Write("A string of nucleotides has been created");
		    return nucleotides.ToString();
	    }


        public override Nucleotide createNucleotideArray()
	    {
		    Nucleotide nucleotide = new Nucleotide(header);
		    for (int i = 0; i < index; i++)
		    {
			    for (int j = 0; i < fastqSeq[i].getFastqSeqSize(); j++)
			    {
				    nucleotide.addCharNucleotideArray(Fq_FILE_MAP[fastqSeq[i].getFastqSeqAtPosition(j)].getNucleotide());
			    }
		    }
		    Console.Write("An array of nucleotides has been created");
		    return nucleotide;
	    }

        public override String createFastqFormat()
	    {
		    StringBuilder fastqBuilder = new StringBuilder();
		    for (int i = 0; i < index; i++)
		    {
			    fastqBuilder.Append(fastqSeq[i].createFastqBlock(Fq_FILE_MAP));
		    }
		    Console.Write("A fastq file has been created from " + header + " File Name: " + fileName);
		    return fastqBuilder.ToString();
	    }

        public override String createFastaFormat(String message)
	    {
		    int FASTA_LINE_LENGTH = 70;
		
		    StringBuilder fastaFile = new StringBuilder();
		    StringBuilder fastaLine = new StringBuilder();
				
		    fastaFile.Append(">" + message + "\t" + header + "\n");
		
		    for (int i = 0; i < index; i++)
		    {
			    for (int j = 0; j < fastqSeq[i].getFastqSeqSize(); j++)
			    {
				    fastaLine.Append(Fq_FILE_MAP[fastqSeq[i].getFastqSeqAtPosition(j)].getNucleotide());
	
				    if (FASTA_LINE_LENGTH == fastaLine.Length)
				    {
					    fastaLine.Append("\n");
					    fastaFile.Append(fastaLine.ToString());
					    fastaLine = new StringBuilder();
				    }
			    }
		    }	
		    fastaLine.Append("\n");
		    fastaFile.Append(fastaLine.ToString());
		    Console.Write("A fasta format has been created from the fastq file");
		    return fastaFile.ToString();
	    }

        public override List<int> performDistributionTest()
	    {
            fillDistributionList();
            for (int i = 0; i < index; i++)
            {
                for (int j = 0; i < fastqSeq[i].getFastqSeqSize(); j++)
                {
                    int qualityScore = Fq_FILE_MAP[fastqSeq[i].getFastqSeqAtPosition(j)].getQualityScore();
                    int currentPop = distribution[qualityScore];
                    distribution[qualityScore] = (currentPop + 1);
                }
            }
		    return distribution;
	    }

        public override void performSequenceStatistics()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < index; i ++)
            {
                fastqSeq[i].performStats(sequencerType, Fq_FILE_MAP);
                //fastqSeq[i].deconstructHeader(sequencerType);
            }
            stopwatch.Stop();
            Console.WriteLine("Statistics Performed in " + stopwatch.Elapsed);
        }

        public override void performNucleotideTests()
	    {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

		    totalNucleotides();
		    resetCounts();

            for (int i = 0; i < index; i++)
            {
                 for (int j = 0; j < fastqSeq[i].getFastqSeqSize(); j++)
                 {
                     char nucleotide = Fq_FILE_MAP[fastqSeq[i].getFastqSeqAtPosition(j)].getNucleotide();

                     if (nucleotide == 'N') nCount++;
                     else if (nucleotide == 'C') cCount ++;
                     else if (nucleotide == 'G') gCount ++;
                 }
            }
		
		    nPercent = (((double) nCount  / totalNucs) * 100);
		    cPercent = (((double) cCount / totalNucs) * 100);
		    gPercent = (((double) gCount / totalNucs) * 100);

            stopwatch.Stop();
            Console.Write("Nucleotide tests completed: " + stopwatch.Elapsed + "\n");
	    }

        public override void performJointTests()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            totalNucleotides();
            resetCounts();

            fillDistributionList();

            for (int i = 0; i < index; i++)
            {
                for (int j = 0; j < fastqSeq[i].getFastqSeqSize(); j++)
                {
                    char nucleotide = Fq_FILE_MAP[fastqSeq[i].getFastqSeqAtPosition(j)].getNucleotide();

                    if (nucleotide == 'N') nCount++;
                    else if (nucleotide == 'C') cCount ++;
                    else if (nucleotide == 'G') gCount ++;

                    int qualityScore = Fq_FILE_MAP[fastqSeq[i].getFastqSeqAtPosition(j)].getQualityScore();
                    if (qualityScore >= 0)
                    {
                        int currentPop = distribution[qualityScore];
                        distribution[qualityScore] = (currentPop + 1);
                    }
                }
            }
            
            nPercent = (((double)nCount / totalNucs) * 100);
            cPercent = (((double)cCount / totalNucs) * 100);
            gPercent = (((double)gCount / totalNucs) * 100);
            stopwatch.Stop();
            Console.Write("Joint tests completed: " + stopwatch.Elapsed + "\n");
        }

        public override void Tests()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            totalNucleotides();
            resetCounts();

            fillDistributionList();

            for (int i = 0; i < index; i++)
            {
                for (int j = 0; j < fastqSeq[i].getFastqSeqSize(); j++)
                {
                    char nucleotide = Fq_FILE_MAP[fastqSeq[i].getFastqSeqAtPosition(j)].getNucleotide();

                    if (nucleotide == 'N') nCount++;
                    else if (nucleotide == 'C') cCount++;
                    else if (nucleotide == 'G') gCount++;

                    int qualityScore = Fq_FILE_MAP[fastqSeq[i].getFastqSeqAtPosition(j)].getQualityScore();
                    if (qualityScore >= 0)
                    {
                        int currentPop = distribution[qualityScore];
                        distribution[qualityScore] = (currentPop + 1);
                    }
                }
            }            

            nPercent = (((double)nCount / totalNucs) * 100);
            cPercent = (((double)cCount / totalNucs) * 100);
            gPercent = (((double)gCount / totalNucs) * 100);
            stopwatch.Stop();
            Console.Write("Joint tests completed: " + stopwatch.Elapsed + "\n");
        }
	
	    private void fillDistributionList()
	    {
            distribution = new List<int>(40);
		    for (int j = 0; j <= SequencerDiscriminator.getSequencerSpecifier(sequencerType).getDistributionSpread(); j++)
		    {
			    distribution.Add(0);
		    }
	    }

        public override int totalNucleotides()
	    {
		    totalNucs = 0;
		    for (int i = 0; i < index; i++)
		    {
			    totalNucs = totalNucs + fastqSeq[i].getFastqSeqSize();
		    }
		    return totalNucs;
	    }

        public override List<FqSequence> createFastqSeqList()
	    {
            List<FqSequence> seqList = new List<FqSequence>(index);
		    
		    for (int i = 0; i < index; i++)
		    {
			    seqList.Add(fastqSeq[i]);
		    }
		    return seqList;
	    }

        public override void cleanArray()
        {
            var temp = new List<FqSequence>();
            foreach (FqSequence fqSeq in fastqSeq)
            {
                if (fqSeq.removeSequence() == false || fqSeq != null)
                    temp.Add(fqSeq);
            }
            fastqSeq = temp.ToArray();
        }
	
	    private void resetCounts()
	    {
		    nCount = 0;
		    cCount = 0;
		    gCount = 0;
	    }

        public override void setFastqFileName(String fileName)
	    {
		    this.fileName = fileName;
	    }

        public override void setSequencerType(String sequencerType)
	    {
		    this.sequencerType = sequencerType;
	    }

        public override void setNucleotidesCleaned(int numberCleaned)
	    {
		    this.nucleotidesCleaned = numberCleaned;
	    }

        public override int getFastqArraySize()
	    {
		    return index;
	    }

        public override FqSequence[] getFastqFile()
	    {
		    return fastqSeq;
	    }

        public override String getFastqHeader()
	    {
		    return header;
	    }

        public override String getFileName()
	    {
		    return fileName;
	    }

        public override String getSequencerType()
	    {
		    return sequencerType;	
	    }

        public override int getNucleotidesCleaned()
	    {
		    return nucleotidesCleaned;
	    }

        public override List<int> getDistribution()
        {
            return distribution;
        }

        public override int getTotalNucleotides()
	    {
		    return totalNucs;
	    }

        public override FqSequence getFastqSequenceByPosition(int i) 
	    {	
		    return fastqSeq[i];
	    }

        public override int getNCount()
	    {
		    return nCount;
	    }

        public override int getCCount()
	    {
		    return cCount;
	    }

        public override int getGCount()
	    {
		    return gCount;
	    }

        public override double cContents()
	    {	
		    return cPercent;
	    }

        public override double gContents()
	    {
		    return gPercent;
	    }

        public override double nContents()
	    {	
		    return nPercent;
	    }

        public override Dictionary<int, FqNucleotideRead> getMap()
	    {
		    return Fq_FILE_MAP;
	    }

        public override Dictionary<int, string> getRemovedAdapters()
        {
            return removedAdapters;
        }

        public override void setHeader()
        {
            int LOCATION = 0;
            header = fastqSeq[LOCATION].getMachineName();
        }

        public override void setFqHashMap(Dictionary<int, FqNucleotideRead> map)
        {
            Fq_FILE_MAP = map;
        }

        public override void calculateMapQualities()
        {
            Fq_FILE_MAP = HashFastq.calculateHashQualities(sequencerType, Fq_FILE_MAP);
        }

    }
}
