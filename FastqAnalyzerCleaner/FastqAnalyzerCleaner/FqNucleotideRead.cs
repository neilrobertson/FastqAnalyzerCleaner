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
