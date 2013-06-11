/// <copyright file="FqSequence.cs" author="Neil Robertson">
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

namespace FastqAnalyzerCleaner
{
    public class FqSequence
    {
        private String header;
        private String infoLine;
        private int index = 0;
        private int sequenceIndex;
        private double mean;
        private int median, lowerThreshold, upperThreshold, firstQuart, thirdQuart;
        private String machineName;
        private int flowCellLane, tileNumber, xCoord, yCoord;
        private int gCount = 0, cCount = 0, nCount = 0;
        private int sequenceLength;
        private int nucleotidesCleaned = 0;
        private int MAX_LINE_LENGTH = 105;

        private Boolean removeSeq = false;

        private int[] fastqSequence;

        public FqSequence(int seqIndex, String head, String info, int seqLen)
        {
            this.sequenceIndex = seqIndex;
            this.header = head;
            this.infoLine = info;
            this.sequenceLength = seqLen;
            MAX_LINE_LENGTH = sequenceLength;

            fastqSequence = new int[MAX_LINE_LENGTH];
        }

        public void addNucleotideRead(int hashcode)
        {
            fastqSequence[index] = hashcode;
            index++;

            if (index >= MAX_LINE_LENGTH)
            {
                MAX_LINE_LENGTH = MAX_LINE_LENGTH + 5;
                Array.Resize<int>(ref fastqSequence, (MAX_LINE_LENGTH));
            }
        }

        public void createSequenceList()
        {

        }

        public void cleanStarts(int remove)
        {
            if (remove > index)
            {
                removeSeq = true;
            }
            else
            {
                for (int i = 0; i < index - remove; i++)
                {
                    fastqSequence[i] = fastqSequence[i + remove];
                }
                index = index - remove;
                sequenceLength = sequenceLength - remove;
            }
        }

        public void cleanEnds(int remove)
        {
            if (remove > index)
            {
                removeSeq = true;
            }
            else
            {
                index = index - remove;
                sequenceLength = sequenceLength - remove;
            }
        }

        public GenericFastqInputs cleanAdapters(List<Adapters.Adapter> adapters, Dictionary<int, FqNucleotideRead> map)
        {
            String sequence = buildSelectSequenceString(0, Adapters.getInstance().getLargestAdapterSize(), map);
            GenericFastqInputs removedAdapter = null;
            foreach (Adapters.Adapter adapter in adapters)
            {
                if (sequence.Substring(0, adapter.AdapterSequence.Length).Equals(adapter.AdapterSequence))
                {
                    //adapter found
                    Console.WriteLine("Adapter found in sequence {0}: {1}", sequenceIndex, adapter.AdapterName);
                    cleanStarts(adapter.AdapterSequence.Length);
                    removedAdapter = new GenericFastqInputs();
                    removedAdapter.AdapterName = adapter.AdapterName;
                    removedAdapter.SequenceIndex = sequenceIndex;
                }
            }
            return removedAdapter;
        }

        public FqFile_MultiCore.ParallelInputs Tests(Dictionary<int, FqNucleotideRead> map, FqFile_MultiCore.ParallelInputs inputs)
        {
            int qualitySum = 0;
            List<int> qualities = new List<int>();
            resetCounts();

            for (int i = 0; i < index; i++)
            {
                char nucleotide = map[fastqSequence[i]].getNucleotide();

                if (nucleotide == 'N') nCount++;
                else if (nucleotide == 'C') cCount++;
                else if (nucleotide == 'G') gCount++;

                int qualityScore = map[fastqSequence[i]].getQualityScore();
                qualitySum += qualityScore;
                qualities.Add(qualityScore);
                if (qualityScore >= 0)
                {
                    int currentPop = inputs.distributes[qualityScore];
                    inputs.distributes[qualityScore] = (currentPop + 1);
                }
            }
            inputs.cCount = cCount;
            inputs.gCount = gCount;
            inputs.nCount = nCount;

            qualities.Sort();
            int size = qualities.Count;
            mean = (double)qualitySum / sequenceLength;
            median = (int)qualities[(size / 2)];
            thirdQuart = (int)qualities[((size / 4) * 3)];
            firstQuart = (int)qualities[((size / 4))];
            lowerThreshold = (int)qualities[0];
            upperThreshold = (int)qualities[size - 1];

            return inputs;   
        }

        public void performStats(String sequencerType, Dictionary<int, FqNucleotideRead> map)
        {
            int qualitySum = 0;
            List<int> qualities = new List<int>();
            
            for (int i = 0; i < index; i++)
            {
                int qualityScore = map[fastqSequence[i]].getQualityScore();
                qualitySum += qualityScore;
                qualities.Add(qualityScore);
            }
            qualities.Sort();
            int size = qualities.Count;
            mean = (double) qualitySum / sequenceLength;
            median = (int) qualities[(size / 2)];
            thirdQuart = (int) qualities[((size / 4) * 3)];
            firstQuart = (int) qualities[((size / 4))];
            lowerThreshold = (int) qualities[0];
            upperThreshold = (int) qualities[size - 1];
        }

        public int countMisreads(Dictionary<int, FqNucleotideRead> map)
        {
            int N_Misreads = 0;
            for (int i = 0; i < index; i++)
            {
                if (map[fastqSequence[i]].getNucleotide() == 'N') N_Misreads++;
            }
            return N_Misreads;
        }

        public String createSequenceString(Dictionary<int, FqNucleotideRead> map)
        {
            StringBuilder sequence = new StringBuilder();
            for (int i = 0; i < index; i++)
            {
                sequence.Append(map[fastqSequence[i]].getNucleotide());
            }
            return sequence.ToString();
        }

        public String createFastqBlock(Dictionary<int, FqNucleotideRead> map)
        {
            StringBuilder seq = new StringBuilder();
            StringBuilder qual = new StringBuilder();
            StringBuilder fqBlock = new StringBuilder();
            for (int i = 0; i < index; i++)
            {
                seq.Append(map[fastqSequence[i]].getNucleotide());
                qual.Append(map[fastqSequence[i]].getQualityRead());
            }
            fqBlock.Append(header + "\n");
            fqBlock.Append(seq + "\n");
            fqBlock.Append(infoLine + "\n");
            fqBlock.Append(qual + "\n");
            return fqBlock.ToString();
        }

        public String buildSelectSequenceString(int start, int end,  Dictionary<int, FqNucleotideRead> map)
        {
            StringBuilder builder = new StringBuilder();

            if (end > index)
                end = index;

            if (start < 0 || start > end)
                start = 0;

            for (int i = start; i < end; i++)
            {
                builder.Append(map[fastqSequence[i]].getNucleotide());
            }
            return builder.ToString();
        }

        public int sequenceTailsOffset()
        {
            int tailsOffset = 0;
            return tailsOffset;
        }

        public void deconstructHeader(String sequencerType)
        {
            machineName = SequencerDiscriminator.getSequencerSpecifier(sequencerType).getMachineName(header);
            flowCellLane = SequencerDiscriminator.getSequencerSpecifier(sequencerType).getFlowCellLane(header);
            tileNumber = SequencerDiscriminator.getSequencerSpecifier(sequencerType).getTileNumber(header);
            xCoord = SequencerDiscriminator.getSequencerSpecifier(sequencerType).getXCoOrd(header);
            yCoord = SequencerDiscriminator.getSequencerSpecifier(sequencerType).getYCoOrd(header); ;
        }

        public String recontructHeader()
        {
            String reconstructedHeader = "@" + machineName + ":" + flowCellLane + ":" + tileNumber + ":" + xCoord + ":" + yCoord;
            return reconstructedHeader;
        }

        private void resetCounts()
        {
            nCount = 0;
            cCount = 0;
            gCount = 0;
        }

        public String getSeqIndex()
        {
            return "Seq Index: " + sequenceIndex;
        }

        public int getSequenceIndex()
        {
            return sequenceIndex;
        }

        public String getSequence()
        {
            return header;
        }

        public double getMean()
        {
            return mean;
        }

        public int getMedian()
        {
            return median;
        }

        public int getLowerThreshold()
        {
            return lowerThreshold;
        }

        public int getUpperThreshold()
        {
            return upperThreshold;
        }

        public int getFirstQuartile()
        {
            return firstQuart;
        }

        public int getThirdQuartile()
        {
            return thirdQuart;
        }

        public int getFastqSeqSize()
        {
            return index;
        }

        public int getFastqSeqAtPosition(int index)
        {
            return fastqSequence[index];
        }

        public int[] getFastqArray()
        {
            return fastqSequence;
        }

        public int getNucleotidesCleaned()
        {
            return nucleotidesCleaned;
        }

        public int getFlowCellLane()
        {
            return flowCellLane;
        }

        public int getTileNumber()
        {
            return tileNumber;
        }

        public int getXCoord()
        {
            return xCoord;
        }

        public int getYCoord()
        {
            return yCoord;
        }

        public String getMachineName()
        {
            return machineName;
        }

        public Boolean removeSequence()
        {
            return removeSeq;
        }

        public int getNCount()
        {
            return nCount;
        }

        public int getCCount()
        {
            return cCount;
        }

        public int getGCount()
        {
            return gCount;
        }
    }
}
