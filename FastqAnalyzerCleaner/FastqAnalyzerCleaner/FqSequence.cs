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
        private int sequenceLength;
        private int nucleotidesCleaned = 0;
        private int MAX_LINE_LENGTH = 105;

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
            for (int i = 0; i < index - remove; i++)
            {
                fastqSequence[i] = fastqSequence[i + remove];
            }
            index = index - remove;
            sequenceLength = sequenceLength - remove;
        }

        public void cleanEnds(int remove)
        {
            index = index - remove;
            sequenceLength = sequenceLength - remove;
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

        public String roundTwoDecimals(double d)
        {
            return d.ToString("N2");
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
    }
}
