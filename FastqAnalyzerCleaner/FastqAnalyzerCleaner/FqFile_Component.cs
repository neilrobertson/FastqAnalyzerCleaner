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
    /// <summary>
    /// FqFile_Component is a model class that represents the structure of whole or part of a fastq file.  It 
    /// implements the IFastqFile interface and has been annotated for serialization with protocol buffers.
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class FqFile_Component : IFqFile
    {
        [ProtoMember(3, IsRequired=true, OverwriteList=true)]
        public int LENGTH_SEQUENCE_ARRAY = 10000;

        [ProtoMember(4, IsRequired = true)]
        public int index = 0;
        
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

        [ProtoMember(19, IsRequired = true)]
        public int sequencesRemoved;

        [ProtoIgnore]
        public static Dictionary<int, FqNucleotideRead> Fq_FILE_MAP;
        
        [ProtoMember(20, IsRequired = true, OverwriteList = true)]
        public Dictionary<int, string> removedAdapters;
        
        [ProtoMember(21, IsRequired = true, OverwriteList = true)]
        public FqPerBaseSatistics[] perBaseStatistics;

        [ProtoMember(22, IsRequired = true, OverwriteList = true)]
        public List<int> sequenceLengthDistribution;

        [ProtoMember(23, IsRequired = true)]
        public int componentNumber;
        
        [ProtoMember(24, IsRequired = true, OverwriteList = true)]
        public FqSequence[] fastqSeq;
	
        /// <summary>
        /// Constructor for the fqFile_component classes, initializes an array of FqSequence 
        /// </summary>
	    public FqFile_Component()
	    {
		    fastqSeq = new FqSequence[LENGTH_SEQUENCE_ARRAY];
            Console.WriteLine("Multi Core Fastq File Component Created.");
	    }

        /// <summary>
        /// Adds a FqSequence to the array and increments the index, contains provisions for increasing the size of the array
        /// for overflow
        /// </summary>
        /// <param name="fqSeq"></param>
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

        /// <summary>
        /// Cleans a specified amount of nucleotides from the start of a sequence
        /// </summary>
        /// <param name="remove">The number of nucleotides to remove from the start of a sequence</param>
        public override void cleanStarts(int remove)
        {
            int actuallyRemoved = 0;
            Parallel.For(0, index, i =>
            {
                int rem = fastqSeq[i].cleanStarts(remove);
                Interlocked.Add(ref actuallyRemoved, rem);
            });
            nucleotidesCleaned += actuallyRemoved;
            Console.WriteLine("Nucleotides cleaned from sequence ends: {0}", actuallyRemoved);
            cleanArray();
	    }

        /// <summary>
        /// Cleans a specified number of nucleotides from the end of a sequence
        /// </summary>
        /// <param name="remove">The number of nucleotides to clean from the sequence end</param>
        public override void cleanEnds(int remove)
	    {
            int actuallyRemoved = 0;
            Parallel.For(0, index, i =>
            {
                int rem = fastqSeq[i].cleanEnds(remove);
                Interlocked.Add(ref actuallyRemoved, rem);
            });
            nucleotidesCleaned += actuallyRemoved;
            Console.WriteLine("Nucleotides cleaned from sequence ends: {0}", actuallyRemoved);
            cleanArray();
	    }

        /// <summary>
        /// Cleans a specified amount of nucleotides from sequence starts and tails
        /// </summary>
        /// <param name="start">The number of nucleotides to remove from start</param>
        /// <param name="end">The number of nucleotides to remove from end</param>
        public override void cleanTails(int start, int end)
	    {
		    int totalRemove = start + end;
            Parallel.For(0, index, i =>
            {
                fastqSeq[i].cleanStarts(start);
                fastqSeq[i].cleanEnds(end);
            });
            cleanArray();
		    Console.Write("Sequence tails cleaned. Total Removed: " + totalRemove);
	    }

        /// <summary>
        /// Cleans adapter sequences from seqeuences
        /// </summary>
        public override void cleanAdapters()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Adapters.getInstance();
            removedAdapters = new Dictionary<int, string>();

            Parallel.For(0, index, new ParallelOptions { MaxDegreeOfParallelism = Preferences.getInstance().getCoresToUse() },
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

        /// <summary>
        /// Removes sequences below mean theshold
        /// </summary>
        /// <param name="threshold">The mean threshold</param>
        public override void removeBelowMeanThreshold(int threshold)
        {
            Parallel.For(0, index, i =>
            {
                if (fastqSeq[i].getMean() < threshold)
                {
                    fastqSeq[i].setRemoveSequence(true);
                    Interlocked.Add(ref nucleotidesCleaned, fastqSeq[i].getFastqSeqSize());   
                }
            });
            cleanArray();
        }

        /// <summary>
        /// Remove sequences that contain misread nucleotides
        /// </summary>
        public override void removeSequencesWithMisreads()
        {
            Parallel.For(0, index, i =>
            {
                if (fastqSeq[i].getNCount() > 0)
                {
                    fastqSeq[i].setRemoveSequence(true);
                    Interlocked.Add(ref nucleotidesCleaned, fastqSeq[i].getFastqSeqSize());
                }
            });
            cleanArray();
        }

        /// <summary>
        /// Removes sequences from the component that are above a GC threshold.
        /// </summary>
        /// <param name="threshold">The GC threshold in a percentile. ie 'threshold = 50' corresponds to 50%</param>
        public override void removeSequencesAboveGCThreshold(int threshold)
        {
            Parallel.For(0, index, i =>
            {
                if ((((double)fastqSeq[i].getCCount() + (double)fastqSeq[i].getGCount()/(double) fastqSeq[i].getFastqSeqSize()) * 100) > threshold)
                {
                    fastqSeq[i].setRemoveSequence(true);
                    Interlocked.Add(ref nucleotidesCleaned, fastqSeq[i].getFastqSeqSize());
                }
            });
            cleanArray();
        }

        /// <summary>
        /// Removes sequences from the component that are below a base number threshold
        /// </summary>
        /// <param name="threshold">The threshold at which all sequences smaller are to be removed from the component.</param>
        public override void removeBelowSequenceLength(int threshold)
        {
            Parallel.For(0, index, i =>
            {
                if (fastqSeq[i].getFastqSeqSize() <= threshold)
                {
                    fastqSeq[i].setRemoveSequence(true);
                    Interlocked.Add(ref nucleotidesCleaned, fastqSeq[i].getFastqSeqSize());
                }
            });
            cleanArray();
        }

        /// <summary>
        /// Find sequences with a specific queried sequence.
        /// </summary>
        public override List<FqSequence> findSequence(String sequence)
        {
            List<FqSequence> foundSequences = new List<FqSequence>();
            object locker = new object();
            Parallel.For(0, index, i =>
            {
                FqSequence result = fastqSeq[i].findSequence(sequence, Fq_FILE_MAP);
                if (result != null)
                    lock(locker)
                        foundSequences.Add(result);
            });
            Console.WriteLine("Found {0} Sequences Containing: {1}", foundSequences.Count, sequence);
            return foundSequences;
        }

        /// <summary>
        /// Deprecated method
        /// </summary>
        /// <param name="startBlock"></param>
        /// <param name="endBlock"></param>
        public override void removeRegion(int startBlock, int endBlock)
	    {
            throw new NotImplementedException();
	    }

        /// <summary>
        /// Creates a nucleotide string for the complete file component
        /// </summary>
        /// <returns>A string of nucleotides for the whole file components sequences</returns>
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

        /// <summary>
        /// Creates an array of nucleotides from the whole sequence
        /// </summary>
        /// <returns>An array of Nucleotide objects</returns>
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

        /// <summary>
        /// Creates a fastq file format from the fqFileComponent
        /// </summary>
        /// <returns>A string in the format of a fastq file for the entire component</returns>
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

        /// <summary>
        /// Creates a fasta format style string for the entire fq file component
        /// </summary>
        /// <param name="message">A message you would like to include in the fasta header</param>
        /// <returns>A string of nucleotides for the fqFileComponent in the fasta format</returns>
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

        /// <summary>
        /// Performs a distribution test on the quality reads of the fastq file component
        /// </summary>
        /// <returns>A list that outlines the distribution of qualities within this file component</returns>
        public override List<int> performDistributionTest()
	    {
            fillDistributionList();
            int subZeroOffset = SequencerDiscriminator.getSequencerSpecifier(sequencerType).getSubZeroQualities();

            Parallel.For(0, index, new ParallelOptions { MaxDegreeOfParallelism = (Preferences.getInstance().getCoresToUse())},
                // Initialize the local states
            () => new FqSequence_InputsOuptuts(sequencerType, "Sequence Tests", this),
                // Accumulate the thread-local computations in the loop body
            (i, loop, syncLists) =>
            {
                for (int j = 0; i < fastqSeq[i].getFastqSeqSize(); j++)
                {
                    int qualityScore = Fq_FILE_MAP[fastqSeq[i].getFastqSeqAtPosition(j)].getQualityScore();
                    int currentPop = syncLists.distributes[(qualityScore + subZeroOffset)];
                    syncLists.distributes[(qualityScore + subZeroOffset)] = (currentPop + 1);
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

        /// <summary>
        /// Performs sequence statistics on each of the sequences within the file component
        /// </summary>
        public override void performSequenceStatistics()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Parallel.For(0, index, i =>
            {
                fastqSeq[i].performStats(sequencerType, Fq_FILE_MAP);
            });

            stopwatch.Stop();
            Console.WriteLine("Statistics Performed in " + stopwatch.Elapsed);
        }

        /// <summary>
        /// Performs tests on the nucleotides contained within this file component
        /// </summary>
        public override void performNucleotideTests()
	    {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

		    totalNucleotides();
		    resetCounts();

            Parallel.For(0, index, new ParallelOptions { MaxDegreeOfParallelism = (Preferences.getInstance().getCoresToUse()) },
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

        /// <summary>
        /// Perform both distribution and nucleotide tests on fqfile component
        /// </summary>
        public override void performJointTests()
	    {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

		    totalNucleotides();
		    resetCounts();

		    fillDistributionList();

            int subZeroOffset = SequencerDiscriminator.getSequencerSpecifier(sequencerType).getSubZeroQualities();

            Parallel.For(0, index, new ParallelOptions { MaxDegreeOfParallelism = (Preferences.getInstance().getCoresToUse()) },
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
                   
                   int currentPop = syncLists.distributes[(qualityScore + subZeroOffset)];
                   syncLists.distributes[(qualityScore + subZeroOffset)] = (currentPop + 1);
                   
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


        /// <summary>
        /// Prefered combined method for testing file, combines nucleotide, distribution and per base sequence statistics 
        /// tests into one multi
        /// </summary>
        public override void Tests()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            object locker = new object();

            totalNucleotides();
            resetCounts();

            fillDistributionList();
            fillSequenceLengthDistributionList();

            BuildPerSequenceStatisticsList();

            Parallel.For(0, index, new ParallelOptions { MaxDegreeOfParallelism = (Preferences.getInstance().getCoresToUse()) },
                // Initialize the local states
            () => new FqSequence_InputsOuptuts(sequencerType, "Sequence Tests", this),
                // Accumulate the thread-local computations in the loop body
            (i, loop, syncLists) =>
            {
                syncLists = fastqSeq[i].Tests(Fq_FILE_MAP, syncLists);
                Interlocked.Add(ref cCount, syncLists.cCount);
                Interlocked.Add(ref gCount, syncLists.gCount);
                Interlocked.Add(ref nCount, syncLists.nCount);

                lock (locker)
                    this.CombinePerBaseStatisticsLists(syncLists.perSeqQuals, syncLists.sequenceLength); 
                
                return syncLists;
            },
            syncLists =>
            {
                lock (locker)
                    this.CombineDistributionLists(syncLists.distributes);   
            }
            );
            int count = 0;

            for (int i = (maxSeqSize - 1); i >= 0; i--)
            {
                int numAtLength = perBaseStatistics[i].GetPerBaseCount();
                sequenceLengthDistribution[i] += (numAtLength - count);
                count += (numAtLength - count);
                perBaseStatistics[i].reconcileBaseSatistics();
            }

            nPercent = (((double)nCount / totalNucs) * 100);
            cPercent = (((double)cCount / totalNucs) * 100);
            gPercent = (((double)gCount / totalNucs) * 100);
            stopwatch.Stop();
            Console.Write("TESTS completed: " + stopwatch.Elapsed + "\n");
        }
        
        /// <summary>
        /// Reconciled some of the distribution outputs for the fastq components tests.
        /// </summary>
        /// <param name="distributes"></param>
        /// <param name="seqStats"></param>
        /// <param name="seqLength"></param>
        private void reconcileOutputs(List<int> distributes, int[] seqStats, int seqLength)
        {
            this.CombineDistributionLists(distributes);
            this.CombinePerBaseStatisticsLists(seqStats, seqLength);
        }
       
        /// <summary>
        /// Adds a sequences quality distribution to the global distribution.
        /// </summary>
        /// <param name="distributes"></param>
        private void CombineDistributionLists(List<int> distributes)
        {
            for (int i = 0; i < distribution.Count; i++)
            {
                distribution[i] += distributes[i];
            }
        }

        /// <summary>
        /// Combines the seqeunces statistics to the perbase sequence statistics object array
        /// </summary>
        /// <param name="seqStats">The array of per base sequence statistics</param>
        /// <param name="seqLength"></param>
        private void CombinePerBaseStatisticsLists(int[] seqStats, int seqLength)
        {
            for (int i = 0; i < seqLength; i++)
            {
                perBaseStatistics[i].addBaseQuality(seqStats[i]);
            }
        }

        /// <summary>
        /// Initializes a new per-base-sequence-statistics array for the object.
        /// </summary>
        private void BuildPerSequenceStatisticsList()
        {
            perBaseStatistics = new FqPerBaseSatistics[maxSeqSize];
            for (int i = 0; i < maxSeqSize; i++)
                perBaseStatistics[i] = new FqPerBaseSatistics();
        }

        /// <summary>
        /// Initializes a distribution list for the object.
        /// </summary>
        private void fillSequenceLengthDistributionList()
        {
            sequenceLengthDistribution = new List<int>(maxSeqSize);
            for (int i = 0; i < maxSeqSize; i++)
                sequenceLengthDistribution.Add(0);
        }

        /// <summary>
        /// Initializes the distribution list for the object.
        /// </summary>
        private void fillDistributionList()
	    {
            distribution = new List<int>(SequencerDiscriminator.getSequencerSpecifier(sequencerType).getDistributionSpread());
		    for (int j = 0; j <= SequencerDiscriminator.getSequencerSpecifier(sequencerType).getDistributionSpread(); j++)
			    distribution.Add(0);
	    }

        /// <summary>
        /// Calculates the total nucleotides for the component.  Including maximum and minimum sequences sizes.
        /// </summary>
        /// <returns></returns>
        public override int totalNucleotides()
	    {
		    totalNucs = 0;
            if (index > 0)
            {
                maxSeqSize = fastqSeq[0].getFastqSeqSize();
                minSeqSize = fastqSeq[0].getFastqSeqSize();
                int seqNucs = 0;

                for (int i = 0; i < index; i++)
                {
                    seqNucs = fastqSeq[i].getFastqSeqSize();
                    totalNucs += seqNucs;
                    if (seqNucs > maxSeqSize) maxSeqSize = seqNucs;
                    if (seqNucs < minSeqSize) minSeqSize = seqNucs;
                }
            }
		    return totalNucs;
	    }
        
        /// <summary>
        /// Creates a list of the fastq sequences in this component
        /// </summary>
        /// <returns>The list of fastq sequence objects</returns>
        public override List<FqSequence> createFastqSeqList()
	    {
            List<FqSequence> seqList = new List<FqSequence>(index);
		    
		    for (int i = 0; i < index; i++)
		    {
			    seqList.Add(fastqSeq[i]);
		    }
		    return seqList;
	    }
        
        /// <summary>
        /// Cleans the component of any sequences that have been earmarked for removal
        /// </summary>
        public override void cleanArray()
        {
            int removed = 0;
            var temp = new List<FqSequence>();
            foreach (FqSequence fqSeq in fastqSeq)
            {
                if (fqSeq.getRemoveSequence() == false && fqSeq != null)
                    temp.Add(fqSeq);
                else
                    removed++;
            }
            fastqSeq = temp.ToArray();
            index -= removed;
            sequencesRemoved += removed;
            Console.WriteLine("Sequences removed from Component: {0}", removed);
        }

        /// <summary>
        /// Resets the nucleotide counts for the component
        /// </summary>
        private void resetCounts()
	    {
		    nCount = 0;
		    cCount = 0;
		    gCount = 0;
	    }

        /// <summary>
        /// Mutator method for the fastq filenam
        /// </summary>
        /// <param name="fileName"></param>
        public override void setFastqFileName(String fileName)
	    {
		    this.fileName = fileName;
	    }

        /// <summary>
        /// Mutator method sets sequencer type
        /// </summary>
        /// <param name="sequencerType"></param>
        public override void setSequencerType(String sequencerType)
	    {
		    this.sequencerType = sequencerType;
	    }

        /// <summary>
        /// Mutator method sets number of nucleotides cleaned
        /// </summary>
        /// <param name="numberCleaned"></param>
        public override void setNucleotidesCleaned(int numberCleaned)
	    {
		    this.nucleotidesCleaned = numberCleaned;
	    }

        /// <summary>
        /// Returns the index for the array of fastq sequence objects
        /// </summary>
        /// <returns></returns>
        public override int getFastqArraySize()
	    {
		    return index;
	    }

        /// <summary>
        /// Returns the array of fastq sequences
        /// </summary>
        /// <returns></returns>
        public override FqSequence[] getFastqFile()
	    {
		    return fastqSeq;
	    }

        /// <summary>
        /// Gets the header for the first sequence object
        /// </summary>
        /// <returns></returns>
        public override String getFastqHeader()
	    {
		    return header;
	    }

        /// <summary>
        /// Returns the filename for the fastq component
        /// </summary>
        /// <returns></returns>
        public override String getFileName()
	    {
		    return fileName;
	    }

        /// <summary>
        /// Returns the sequencer type for the fastq component
        /// </summary>
        /// <returns></returns>
        public override String getSequencerType()
	    {
		    return sequencerType;	
	    }

        /// <summary>
        /// Returns teh nucleotides cleaned from the fastq component
        /// </summary>
        /// <returns></returns>
        public override int getNucleotidesCleaned()
	    {
		    return nucleotidesCleaned;
	    }

        /// <summary>
        /// Returns the quality score distribution for the fastq object
        /// </summary>
        /// <returns></returns>
        public override List<int> getDistribution()
        {
            return distribution;
        }
        
        /// <summary>
        /// Returns the sequence length distribution
        /// </summary>
        /// <returns></returns>
        public override List<int> getSequenceLengthDistribution()
        {
            return sequenceLengthDistribution;
        }

        /// <summary>
        /// returns the total nucleotides for the component
        /// </summary>
        /// <returns></returns>
        public override int getTotalNucleotides()
	    {
		    return totalNucs;
	    }
        
        /// <summary>
        /// Returns the sequence object at position i
        /// </summary>
        /// <param name="i">the location of the sequence object witnin the array</param>
        /// <returns></returns>
        public override FqSequence getFastqSequenceByPosition(int i) 
	    {	
		    return fastqSeq[i];
	    }

        /// <summary>
        /// Returns the misread nucleotides in the component
        /// </summary>
        /// <returns></returns>
        public override int getNCount()
	    {
		    return nCount;
	    }

        /// <summary>
        /// Returns the cytosine count
        /// </summary>
        /// <returns></returns>
        public override int getCCount()
	    {
		    return cCount;
	    }

        /// <summary>
        /// returns the guanine count
        /// </summary>
        /// <returns></returns>
        public override int getGCount()
	    {
		    return gCount;
	    }

        /// <summary>
        /// Returns the percent of nucleotides that are cytosine
        /// </summary>
        /// <returns></returns>
        public override double cContents()
	    {	
		    return cPercent;
	    }

        /// <summary>
        /// returns the percent of nucleotides that are guanine
        /// </summary>
        /// <returns></returns>
        public override double gContents()
	    {
		    return gPercent;
	    }

        /// <summary>
        /// Returns the percent of nucleotides that are misread
        /// </summary>
        /// <returns></returns>
        public override double nContents()
	    {	
		    return nPercent;
	    }

        /// <summary>
        /// Returns the size of the largest sequence in the component
        /// </summary>
        /// <returns></returns>
        public override int getMaxSeqSize()
        {
            return maxSeqSize;
        }

        /// <summary>
        /// Returns the size of the smallest sequence in the component
        /// </summary>
        /// <returns></returns>
        public override int getMinSeqSize()
        {
            return minSeqSize;
        }

        /// <summary>
        /// Returns the number of sequences that have been removed from the component
        /// </summary>
        /// <returns></returns>
        public override int getSequencesRemoved()
        {
            return sequencesRemoved;
        }

        /// <summary>
        /// Mutator sets the component number corresponding to the order parsed from the loaded file
        /// </summary>
        /// <param name="number"></param>
        public override void setComponentNumber(int number)
        {
            this.componentNumber = number;
        }

        /// <summary>
        /// Returns the component number for this component block
        /// </summary>
        /// <returns></returns>
        public override int getComponentNumber()
        {
            return componentNumber;
        }

        /// <summary>
        /// Returns the nucleotide/quality read file map
        /// </summary>
        /// <returns></returns>
        public override Dictionary<int, FqNucleotideRead> getMap()
	    {
		    return Fq_FILE_MAP;
	    }

        /// <summary>
        /// Returns a dictionary of the removed adapaters
        /// </summary>
        /// <returns></returns>
        public override Dictionary<int, string> getRemovedAdapters()
        {
            return removedAdapters;
        }

        /// <summary>
        /// Sets the header for the fastq component
        /// </summary>
        public override void setHeader()
        {
            int LOCATION = 0;
            if (index > 0)
            {
                header = fastqSeq[LOCATION].getMachineName();
            }
        }

        /// <summary>
        /// Sets the nucleotide/quality read map to the component
        /// </summary>
        /// <param name="map"></param>
        public override void setFqHashMap(Dictionary<int, FqNucleotideRead> map)
        {
            Fq_FILE_MAP = map; 
        }

        /// <summary>
        /// Calculates the sequencer specific quality scores for each component
        /// </summary>
        public override void calculateMapQualities()
        {
            Fq_FILE_MAP = HashFastq.calculateHashQualities(sequencerType, Fq_FILE_MAP);
        }

        /// <summary>
        /// Returns the per base statistics array
        /// </summary>
        /// <returns></returns>
        public override FqPerBaseSatistics[] GetPerBaseStatisticsArray()
        {
            return perBaseStatistics;
        }
    }
}
