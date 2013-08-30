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
using ProtoBuf;
using System.Text.RegularExpressions;

namespace FastqAnalyzerCleaner
{
    [Serializable]
    [ProtoContract]
    public class FqSequence
    {
        [ProtoMember(1, IsRequired = true)]
        public String Header;
        [ProtoMember(2, IsRequired = true)]
        public String InfoLine;
        [ProtoMember(3, IsRequired = true)]
        public int index = 0;
        [ProtoMember(4, IsRequired = true)]
        public int SequenceIndex;
        [ProtoMember(5, IsRequired = true)]
        public double Mean;
        [ProtoMember(6, IsRequired = true)]
        public int Median;
        [ProtoMember(7, IsRequired = true)]
        public int LowerThreshold;
        [ProtoMember(8, IsRequired = true)]
        public int UpperThreshold;
        [ProtoMember(9, IsRequired = true)]
        public int FirstQuartile;
        [ProtoMember(10, IsRequired = true)]
        public int ThirdQuartile;
        [ProtoMember(11, IsRequired = true)]
        public String MachineName;
        [ProtoMember(12, IsRequired = true)]
        public int FlowCellLane;
        [ProtoMember(13, IsRequired = true)]
        public int TileNumber;
        [ProtoMember(14, IsRequired = true)]
        public int XCoord;
        [ProtoMember(15, IsRequired = true)]
        public int YCoord;
        [ProtoMember(16, IsRequired = true)]
        public int GCount;
        [ProtoMember(17, IsRequired = true)]
        public int CCount;
        [ProtoMember(18, IsRequired = true)]
        public int NCount;
        [ProtoMember(19, IsRequired = true)]
        public int SEQUENCE_LENGTH;
        [ProtoMember(20, IsRequired = true)]
        public int NucleotidesCleaned;
        [ProtoMember(21, IsRequired = true)]
        public int MAX_LINE_LENGTH = 105;
        [ProtoMember(22, IsRequired = true)]
        public Boolean RemoveSequence = false;
        [ProtoMember(23, IsRequired = true, OverwriteList = true)]
        public int[] fastqSequence;
        [ProtoMember(24, IsRequired = true)]
        public int MatchKey;

        public FqSequence() { }

        public FqSequence(int seqIndex, String head, String info, int seqLen)   //uint64??
        {
            this.SequenceIndex = seqIndex;
            this.Header = head;
            this.InfoLine = info;
            this.SEQUENCE_LENGTH = seqLen;
            MAX_LINE_LENGTH = SEQUENCE_LENGTH;

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

        public int cleanStarts(int remove)
        {
            if (remove >= index)
            {
                RemoveSequence = true;
                return index;
            }
            else
            {
                for (int i = 0; i < index - remove; i++)
                {
                    fastqSequence[i] = fastqSequence[i + remove];
                }
                index = index - remove;
                SEQUENCE_LENGTH = SEQUENCE_LENGTH - remove;
                return remove;
            }
        }

        public int cleanEnds(int remove)
        {
            if (remove >= index)
            {
                RemoveSequence = true;
                return index;
            }
            else
            {
                index = index - remove;
                SEQUENCE_LENGTH = SEQUENCE_LENGTH - remove;
                return remove;
            }
        }

        public FqSequence findSequence(String seq, Dictionary<int, FqNucleotideRead> map)
        {
            string input = createSequenceString(map);

            Match match = Regex.Match(input, seq.ToUpper(), RegexOptions.IgnoreCase);

            if (match.Success)
            {
                MatchKey = match.Groups[1].Index;
                return this;
            }
            return null;
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
                    Console.WriteLine("Adapter found in sequence {0}: {1}", SequenceIndex, adapter.AdapterName);
                    cleanStarts(adapter.AdapterSequence.Length);
                    removedAdapter = new GenericFastqInputs();
                    removedAdapter.AdapterName = adapter.AdapterName;
                    removedAdapter.SequenceIndex = SequenceIndex;
                    InfoLine = InfoLine + "||Removed Adapter:" + adapter.AdapterName;
                }
            }
            return removedAdapter;
        }

        public FqSequence_InputsOuptuts Tests(Dictionary<int, FqNucleotideRead> map, FqSequence_InputsOuptuts inputs)
        {
            int qualitySum = 0;
            List<int> qualities = new List<int>();
            
            resetCounts();

            for (int i = 0; i < index; i++)
            {
                char nucleotide = map[fastqSequence[i]].getNucleotide();

                if (nucleotide == 'N') NCount++;
                else if (nucleotide == 'C') CCount++;
                else if (nucleotide == 'G') GCount++;

                int qualityScore = map[fastqSequence[i]].getQualityScore();
                qualitySum += qualityScore;
                qualities.Add(qualityScore);

                int currentPop = inputs.distributes[(qualityScore + inputs.subZeroOffset)];
                inputs.distributes[(qualityScore + inputs.subZeroOffset)] = (currentPop + 1);
                
            }
            inputs.cCount = CCount;
            inputs.gCount = GCount;
            inputs.nCount = NCount;

            for (int i = 0; i < qualities.Count; i++)
                inputs.perSeqQuals[i] = qualities[i];

            inputs.sequenceLength = index;

            qualities.Sort();
            int size = qualities.Count;
            Mean = (double)qualitySum / SEQUENCE_LENGTH;
            Median = (int)qualities[(size / 2)];
            ThirdQuartile = (int)qualities[((size / 4) * 3)];
            FirstQuartile = (int)qualities[((size / 4))];
            LowerThreshold = (int)qualities[0];
            UpperThreshold = (int)qualities[size - 1];

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
            Mean = (double) qualitySum / SEQUENCE_LENGTH;
            Median = (int) qualities[(size / 2)];
            ThirdQuartile = (int) qualities[((size / 4) * 3)];
            FirstQuartile = (int) qualities[((size / 4))];
            LowerThreshold = (int) qualities[0];
            UpperThreshold = (int) qualities[size - 1];
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
            fqBlock.Append(Header + "\n");
            fqBlock.Append(seq.ToString() + "\n");
            fqBlock.Append(InfoLine + "\n");
            fqBlock.Append(qual.ToString() + "\n");
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
            MachineName = SequencerDiscriminator.getSequencerSpecifier(sequencerType).getMachineName(Header);
            FlowCellLane = SequencerDiscriminator.getSequencerSpecifier(sequencerType).getFlowCellLane(Header);
            TileNumber = SequencerDiscriminator.getSequencerSpecifier(sequencerType).getTileNumber(Header);
            XCoord = SequencerDiscriminator.getSequencerSpecifier(sequencerType).getXCoOrd(Header);
            YCoord = SequencerDiscriminator.getSequencerSpecifier(sequencerType).getYCoOrd(Header); ;
        }

        public String recontructHeader()
        {
            String reconstructedHeader = "@" + MachineName + ":" + FlowCellLane + ":" + TileNumber + ":" + XCoord + ":" + YCoord;
            return reconstructedHeader;
        }

        private void resetCounts()
        {
            NCount = 0;
            CCount = 0;
            GCount = 0;
        }

        public int getSeqIndex()
        {
            return SequenceIndex;
        }

        public String getSequenceHeader()
        {
            return Header;
        }

        public double getMean()
        {
            return Mean;
        }

        public int getMedian()
        {
            return Median;
        }

        public int getLowerThreshold()
        {
            return LowerThreshold;
        }

        public int getUpperThreshold()
        {
            return UpperThreshold;
        }

        public int getFirstQuartile()
        {
            return FirstQuartile;
        }

        public int getThirdQuartile()
        {
            return ThirdQuartile;
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
            return NucleotidesCleaned;
        }

        public int getFlowCellLane()
        {
            return FlowCellLane;
        }

        public int getTileNumber()
        {
            return TileNumber;
        }

        public int getXCoord()
        {
            return XCoord;
        }

        public int getYCoord()
        {
            return YCoord;
        }

        public String getMachineName()
        {
            return MachineName;
        }

        public Boolean getRemoveSequence()
        {
            return RemoveSequence;
        }

        public void setRemoveSequence(Boolean remove)
        {
            this.RemoveSequence = remove;
        }

        public int getNCount()
        {
            return NCount;
        }

        public int getCCount()
        {
            return CCount;
        }

        public int getGCount()
        {
            return GCount;
        }

    }
}
