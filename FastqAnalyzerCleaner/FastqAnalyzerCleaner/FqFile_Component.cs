/// <copyright file="FqFile_Component.cs" author="Neil Robertson">
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
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using ProtoBuf;

namespace FastqAnalyzerCleaner
{
    [Serializable]
    [ProtoContract]
    public class FqFile_Component : IFqFile
    {
        [ProtoMember(3, IsRequired=true, OverwriteList=true)]
        public int LENGTH_SEQUENCE_ARRAY = 100000;
        [ProtoMember(4, IsRequired = true)]
        public int index;
        [ProtoMember(5, IsRequired = true)]
        public String header;
        [ProtoMember(6, IsRequired = true)]
        public String sequencerType;
        [ProtoMember(7, IsRequired = true)]
        public String fileName;
        [ProtoMember(8, IsRequired = true)]
        public int totalNucs;
        [ProtoMember(9, IsRequired = true)]
        public int nucleotidesCleaned;
        [ProtoMember(10, IsRequired = true)]
        public List<int> distribution;
        [ProtoMember(11, IsRequired = true)]
        public int nCount;
        [ProtoMember(12, IsRequired = true)]
        public int cCount;
        [ProtoMember(13, IsRequired = true)]
        public int gCount;
        [ProtoMember(14, IsRequired = true)]
        public int maxSeqSize = 0;
        [ProtoMember(15, IsRequired = true)]
        public int minSeqSize = 0;
        [ProtoMember(16, IsRequired = true)]
        public double nPercent;
        [ProtoMember(17, IsRequired = true)]
        public double cPercent;
        [ProtoMember(18, IsRequired = true)]
        public double gPercent;
        [ProtoIgnore]
        public static Dictionary<int, FqNucleotideRead> Fq_FILE_MAP;
        [ProtoMember(20, IsRequired = true, OverwriteList = true)]
        public Dictionary<int, string> removedAdapters;
        [ProtoMember(21, IsRequired = true, OverwriteList = true)]
        public FqPerBaseSatistics[] perBaseStatistics;
        [ProtoMember(23, IsRequired = true)]
        public readonly int FREE_PROCESSOR_CORE = 1;
        [ProtoMember(24, IsRequired = true, OverwriteList = true)]
        public FqSequence[] fastqSeq;
	
	    public FqFile_Component()
	    {
            index = 0;
		    fastqSeq = new FqSequence[LENGTH_SEQUENCE_ARRAY];
            Console.WriteLine("Multi Core Fastq File Component Created.");
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
			    nucleotidesCleaned += remove;
		    }
            Console.WriteLine("Sequence 5' Ends Cleaned. Nucleotides Removed: {0}", nucleotidesCleaned);
	    }

        public override void cleanEnds(int remove)
	    {
		    for (int i = 0; i < index; i++)
		    {
			    fastqSeq[i].cleanEnds(remove);
			    nucleotidesCleaned += remove;
		    }
            Console.WriteLine("Sequence 5' Ends Cleaned. Nucleotides Removed: {0}", nucleotidesCleaned);
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Adapters.getInstance();
            removedAdapters = new Dictionary<int, string>();

            Parallel.For(0, index, new ParallelOptions { MaxDegreeOfParallelism = (Environment.ProcessorCount - FREE_PROCESSOR_CORE) },
             // Initialize the local states
             () => new FqSequence_InputsOuptuts(sequencerType, "Adapter Removal", this),
             // Accumulate the thread-local computations in the loop body
             (i, loop, adapters) =>
             {
                 adapters.removedAdapters = fastqSeq[i].cleanAdapters(adapters.adapters, Fq_FILE_MAP);
                 return adapters;
             },
             adapters =>
             {
                Object locker = new object();
                lock (locker)
                {
                    if (adapters.removedAdapters != null)
                        removedAdapters.Add(adapters.removedAdapters.SequenceIndex, adapters.removedAdapters.AdapterName);
                }
             });

            stopwatch.Stop();
            Console.WriteLine("Statistics Performed in " + stopwatch.Elapsed);
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
            Parallel.For(0, index, new ParallelOptions { MaxDegreeOfParallelism = (Environment.ProcessorCount - FREE_PROCESSOR_CORE)},
                // Initialize the local states
            () => new FqSequence_InputsOuptuts(sequencerType, "Sequence Tests", this),
                // Accumulate the thread-local computations in the loop body
            (i, loop, syncLists) =>
            {
                for (int j = 0; i < fastqSeq[i].getFastqSeqSize(); j++)
                {
                    int qualityScore = Fq_FILE_MAP[fastqSeq[i].getFastqSeqAtPosition(j)].getQualityScore();
                    int currentPop = syncLists.distributes[qualityScore];
                    syncLists.distributes[qualityScore] = (currentPop + 1);
                }
                return syncLists;
            },
            syncLists =>
            {
                Object locker = new object();
                lock (locker)
                { this.CombineDistributionLists(syncLists.distributes); }
            });
		    return distribution;
	    }

        public override void performSequenceStatistics()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Parallel.For(0, index, i =>
            {
                fastqSeq[i].performStats(sequencerType, Fq_FILE_MAP);
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

            Parallel.For(0, index, new ParallelOptions { MaxDegreeOfParallelism = (Environment.ProcessorCount - FREE_PROCESSOR_CORE) },
                // Initialize the local states
             () => new System.Collections.Concurrent.BlockingCollection<int>(),
                // Accumulate the thread-local computations in the loop body
             (i, loop, misRead) =>
             {
                 for (int j = 0; j < fastqSeq[i].getFastqSeqSize(); j++)
                 {
                     char nucleotide = Fq_FILE_MAP[fastqSeq[i].getFastqSeqAtPosition(j)].getNucleotide();

                     if (nucleotide == 'N') Interlocked.Increment(ref nCount);
                     else if (nucleotide == 'C') Interlocked.Increment(ref cCount);
                     else if (nucleotide == 'G') Interlocked.Increment(ref gCount);
                 }
                 return misRead;
             },
                misRead =>
                {
                    Object locker = new object();
                    lock (locker)
                    { }
                }
             );
		
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

            Parallel.For(0, index, new ParallelOptions { MaxDegreeOfParallelism = (Environment.ProcessorCount - FREE_PROCESSOR_CORE) },
            // Initialize the local states
            () => new FqSequence_InputsOuptuts(sequencerType, "Sequence Tests", this),
            // Accumulate the thread-local computations in the loop body
            (i, loop, syncLists) =>
            {
               for (int j = 0; j < fastqSeq[i].getFastqSeqSize(); j++)
               {
                   char nucleotide = Fq_FILE_MAP[fastqSeq[i].getFastqSeqAtPosition(j)].getNucleotide();

                   if (nucleotide == 'N') Interlocked.Increment(ref nCount);
                   else if (nucleotide == 'C') Interlocked.Increment(ref cCount);
                   else if (nucleotide == 'G') Interlocked.Increment(ref gCount);

                   int qualityScore = Fq_FILE_MAP[fastqSeq[i].getFastqSeqAtPosition(j)].getQualityScore();
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
                        this.CombineDistributionLists(syncLists.distributes);
                    }
                }
            );

            nPercent = (((double) nCount / totalNucs) * 100);
            cPercent = (((double) cCount / totalNucs) * 100);
            gPercent = (((double) gCount / totalNucs) * 100);
            stopwatch.Stop();
            Console.Write("Joint tests completed: " + stopwatch.Elapsed + "\n");
	    }

        public override void Tests()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            object locker = new object();

            totalNucleotides();
            resetCounts();

            fillDistributionList();

            BuildPerSequenceStatisticsList();
            
            Parallel.For(0, index, new ParallelOptions { MaxDegreeOfParallelism = (Environment.ProcessorCount - FREE_PROCESSOR_CORE) },
                // Initialize the local states
            () => new FqSequence_InputsOuptuts(sequencerType, "Sequence Tests", this),
                // Accumulate the thread-local computations in the loop body
            (i, loop, syncLists) =>
            {
                syncLists = fastqSeq[i].Tests(Fq_FILE_MAP, syncLists);
                Interlocked.Add(ref cCount, syncLists.cCount);
                Interlocked.Add(ref gCount, syncLists.gCount);
                Interlocked.Add(ref nCount, syncLists.nCount);

                //object locker = new object();
                lock (locker)
                    this.CombinePerBaseStatisticsLists(syncLists.perSeqQuals, syncLists.sequenceLength); 
                
                return syncLists;
            },
            syncLists =>
            {
                //object locker = new object();
                lock (locker)
                    this.CombineDistributionLists(syncLists.distributes);   
            }
            );
            Parallel.For(0, maxSeqSize, i =>
            {
                perBaseStatistics[i].reconcileBaseSatistics();
            });

            nPercent = (((double)nCount / totalNucs) * 100);
            cPercent = (((double)cCount / totalNucs) * 100);
            gPercent = (((double)gCount / totalNucs) * 100);
            stopwatch.Stop();
            Console.Write("TESTS completed: " + stopwatch.Elapsed + "\n");
        }

        private void reconcileOutputs(List<int> distributes, int[] seqStats, int seqLength)
        {
            this.CombineDistributionLists(distributes);
            this.CombinePerBaseStatisticsLists(seqStats, seqLength);
        }
       
        private void CombineDistributionLists(List<int> distributes)
        {
            for (int i = 0; i < distribution.Count; i++)
            {
                distribution[i] += distributes[i];
            }
        }

        private void CombinePerBaseStatisticsLists(int[] seqStats, int seqLength)
        {
            for (int i = 0; i < seqLength; i++)
            {
                perBaseStatistics[i].addBaseQuality(seqStats[i]);
            }
        }

        private void BuildPerSequenceStatisticsList()
        {
            perBaseStatistics = new FqPerBaseSatistics[maxSeqSize];
            for (int i = 0; i < maxSeqSize; i++)
                perBaseStatistics[i] = new FqPerBaseSatistics();
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
            maxSeqSize = 0;
            minSeqSize = 0;
            int seqNucs = 0;

		    for (int i = 0; i < index; i++)
		    {
			    seqNucs = fastqSeq[i].getFastqSeqSize();
                totalNucs += seqNucs;
                if (seqNucs > maxSeqSize) maxSeqSize = seqNucs;
                if (seqNucs < minSeqSize) minSeqSize = seqNucs;
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

        public override int getMaxSeqSize()
        {
            return maxSeqSize;
        }

        public override int getMinSeqSize()
        {
            return minSeqSize;
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

        public override FqPerBaseSatistics[] GetPerBaseStatisticsArray()
        {
            return perBaseStatistics;
        }
    }
}
