/// <copyright file="FqNucleotideRead.cs" author="Neil Robertson">
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


namespace FastqAnalyzerCleaner
{
    [Serializable]
    [ProtoContract]
    public class FqNucleotideRead
    {
        [ProtoMember(1)]
        private char nucleotide;
        [ProtoMember(2)]
        private char qualityRead;
        [ProtoMember(3)]
        private int qualityScore;
        [ProtoMember(4)]
        private double PHREDScore;


        public FqNucleotideRead() { }

        public FqNucleotideRead(char nucleotide, char qualityRead)
        {
            this.nucleotide = nucleotide;
            this.qualityRead = qualityRead;
        }

        public void resetFqNucleotideRead(char nucleotide, char qualityRead)
        {
            this.nucleotide = nucleotide;
            this.qualityRead = qualityRead;
        }

        public char getNucleotide()
        {
            return nucleotide;
        }

        public char getQualityRead()
        {
            return qualityRead;
        }

        public int getQualityScore()
        {
            return qualityScore;
        }

        public double getPHREDScore()
        {
            return PHREDScore;
        }

        public void calculateQualityScore(string sequencerType)
        {
            qualityScore = SequencerDiscriminator.getSequencerSpecifier(sequencerType).getQualityScore(qualityRead);
            PHREDScore = SequencerDiscriminator.getSequencerSpecifier(sequencerType).getPHREDScore(qualityScore);
        }

        public void setNucleotide(char nuc)
        {
            this.nucleotide = nuc;
        }

        public void setQualityRead(char qual)
        {
            this.qualityRead = qual;
        }

        public String toString()
        {
            return ("Nuc: " + nucleotide + " Score: " + qualityRead + "Hashcode:  " + getProxyCode());
        }

        public int getProxyCode()
        {
            return (nucleotide * 1000) + qualityRead;
        }

    }
}
