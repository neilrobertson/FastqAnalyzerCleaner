/// <copyright file="FqFile_MultiCore.cs" author="Neil Robertson">
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

namespace FastqAnalyzerCleaner
{
    class MultiCore_FqFile : FqFile
    {
        private int LENGTH_SEQUENCE_ARRAY = 100000;
	    private int index = 0;
	
	    private String header, sequencerType, fileName;
	
	    private int totalNucs, nucleotidesCleaned, misreads;
	    private List<int> misreadLocations;
	    private List<int> distribution;
	    private long nCount, cCount, gCount;
	    private double nPercent, cPercent, gPercent;
	    private static Dictionary<int, FqNucleotideRead> map;

        private readonly int FREE_PROCESSOR_CORE = 1;
	
	    FqSequence[] fastqSeq;
	
	    public MultiCore_FqFile()
	    {
            map = HashFastq.deserializeHashmap();
		    fastqSeq = new FqSequence[LENGTH_SEQUENCE_ARRAY];
            Console.WriteLine("Multi Core Fastq File Created.");
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
				    nucleotides.Append(map[fastqSeq[i].getFastqSeqAtPosition(j)].getNucleotide());
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
				    nucleotide.addCharNucleotideArray(map[fastqSeq[i].getFastqSeqAtPosition(j)].getNucleotide());
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
			    fastqBuilder.Append(fastqSeq[i].createFastqBlock(map));
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
				    fastaLine.Append(map[fastqSeq[i].getFastqSeqAtPosition(j)].getNucleotide());
	
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
            Parallel.For(0, index, new ParallelOptions { MaxDegreeOfParallelism = (Environment.ProcessorCount - FREE_PROCESSOR_CORE)},
                // Initialize the local states
            () => new JointSyncLists(sequencerType),
                // Accumulate the thread-local computations in the loop body
            (i, loop, syncLists) =>
            {
                for (int j = 0; i < fastqSeq[i].getFastqSeqSize(); j++)
                {
                    int qualityScore = map[fastqSeq[i].getFastqSeqAtPosition(j)].getQualityScore();
                    int currentPop = syncLists.distributes[qualityScore];
                    syncLists.distributes[qualityScore] = (currentPop + 1);
                }
                return syncLists;
            },
            syncLists =>
            {
                Object locker = new object();
                lock (locker)
                { this.combineDistributionLists(syncLists.distributes); }
            });
		    return distribution;
	    }

        public override void performSequenceStatistics()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Parallel.For(0, index, i =>
            {
                fastqSeq[i].performStats(sequencerType, map);
                //fastqSeq[i].deconstructHeader(sequencerType);
            });

            stopwatch.Stop();
            Console.WriteLine("Statistics Performed in " + stopwatch.Elapsed);
        }

        public override void performNucleotideTests()
	    {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

		    totalNucleotides();
		    resetCounts();
		    misreadLocations = new List<int>(100);

            Parallel.For(0, index, new ParallelOptions { MaxDegreeOfParallelism = (Environment.ProcessorCount - FREE_PROCESSOR_CORE) },
                // Initialize the local states
             () => new System.Collections.Concurrent.BlockingCollection<int>(),
                // Accumulate the thread-local computations in the loop body
             (i, loop, misRead) =>
             {
                 for (int j = 0; j < fastqSeq[i].getFastqSeqSize(); j++)
                 {
                     char nucleotide = map[fastqSeq[i].getFastqSeqAtPosition(j)].getNucleotide();

                     if (nucleotide == 'N') misRead.Add((i * fastqSeq[i].getFastqSeqSize()) + j + 1);
                     else if (nucleotide == 'C') Interlocked.Increment(ref cCount);
                     else if (nucleotide == 'G') Interlocked.Increment(ref gCount);
                 }
                 return misRead;
             },
                misRead => {
                    Object locker = new object();
                    lock (locker)
                    { misreadLocations.AddRange(misRead); }
                }
             );
		    nCount = misreadLocations.Count;
            misreadLocations.Sort();
		
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

            misreadLocations = new List<int>(totalNucs / 100000);
		
		    fillDistributionList();

            Parallel.For(0, index, new ParallelOptions { MaxDegreeOfParallelism = (Environment.ProcessorCount - FREE_PROCESSOR_CORE) },
            // Initialize the local states
            () => new JointSyncLists(sequencerType),
            // Accumulate the thread-local computations in the loop body
            (i, loop, syncLists) =>
            {
               for (int j = 0; j < fastqSeq[i].getFastqSeqSize(); j++)
               {
                   char nucleotide = map[fastqSeq[i].getFastqSeqAtPosition(j)].getNucleotide();
                  
                   if (nucleotide == 'N') syncLists.misRead.Add((i * fastqSeq[i].getFastqSeqSize()) + j + 1);
                   else if (nucleotide == 'C') Interlocked.Increment(ref cCount);
                   else if (nucleotide == 'G') Interlocked.Increment(ref gCount);

                   int qualityScore = map[fastqSeq[i].getFastqSeqAtPosition(j)].getQualityScore();
                   if (qualityScore >= 0)
                   {
                       int currentPop = syncLists.distributes[qualityScore];
                       syncLists.distributes[qualityScore] = (currentPop + 1);
                   }
               }
                return syncLists;
            },
            syncLists => {
                    Object locker = new object();
                    lock (locker)
                    { 
                        this.misreadLocations.AddRange(syncLists.misRead);
                        this.combineDistributionLists(syncLists.distributes);
                    }
                }
            );
            misreadLocations.Sort();
		    nCount = misreadLocations.Count;

            nPercent = (((double) nCount / totalNucs) * 100);
            cPercent = (((double) cCount / totalNucs) * 100);
            gPercent = (((double) gCount / totalNucs) * 100);
            stopwatch.Stop();
            Console.Write("Joint tests completed: " + stopwatch.Elapsed + "\n");
	    }

        public class JointSyncLists
        {
            public List<int> distributes {get; set;}
            public List<int> misRead {get; set;}

            public JointSyncLists(String sequencerType)
            {
                distributes = new List<int>(40);
                for (int j = 0; j <= SequencerDiscriminator.getSequencerSpecifier(sequencerType).getDistributionSpread(); j++)
                {
                    distributes.Add(0);
                }
                misRead = new List<int>();
            }
        }

        private void combineDistributionLists(List<int> distributes)
        {
            for (int i = 0; i < distribution.Count; i++)
            {
                distribution[i] += distributes[i];
            }
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

        public override void findMisreads()
	    {
		    misreadLocations =  new List<int>();
		    for (int i = 0; i < index; i++)
		    {
			    for (int j = 0; i < fastqSeq[i].getFastqSeqSize(); j++)
			    {
				    if (map[fastqSeq[i].getFastqSeqAtPosition(j)].getNucleotide() == 'N') misreadLocations.Add((i * fastqSeq[i].getFastqSeqSize()) + j + 1);
			    }
		    }
		    misreads = misreadLocations.Count;
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

        public override List<int> getMisreadLocations()
	    {
		    return misreadLocations;
	    }

        public override List<int> getDistribution()
        {
            return distribution;
        }

        public override int getMisreads()
	    {
		    return misreads;
	    }

        public override int getTotalNucleotides()
	    {
		    return totalNucs;
	    }

        public override FqSequence getFastqSequenceByPosition(int i) 
	    {	
		    return fastqSeq[i];
	    }

        public override double getNCount()
	    {
		    return nCount;
	    }

        public override double getCCount()
	    {
		    return cCount;
	    }

        public override double getGCount()
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
		    return map;
	    }

        public override void setHeader()
        {
            int LOCATION = 0;
            header = fastqSeq[LOCATION].getMachineName();
        }

        public override void calculateMapQualities()
        {
            map = HashFastq.calculateHashQualities(sequencerType, map);
        }
    }
}
