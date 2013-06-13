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



namespace FastqAnalyzerCleaner
{
    [Serializable]
    public class FqNucleotideRead
    {
        private char nucleotide;
        private char qualityRead;
        private int phredQuality;

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
            return phredQuality;
        }

        public void calculateQualityScore(string sequencerType)
        {
            phredQuality = SequencerDiscriminator.getSequencerSpecifier(sequencerType).getQualityScore(qualityRead);
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
            return ("Nuc: " + nucleotide + " Score: " + qualityRead + "Hashcode:  " + hashcode());
        }

        public int hashcode()
        {
            return (nucleotide * 1000) + qualityRead;
        }

    }
}
