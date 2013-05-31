/// <copyright file="Nucleotide.cs" author="Neil Robertson">
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

namespace FastqAnalyzerCleaner
{
    class Nucleotide
    {
        private String header;

        private int LENGTH_NUCLEOTIDE_ARRAY = 1000;
        private int index = 0;

        char[] nucleotides;

        public Nucleotide() 
        {
            nucleotides = new char[LENGTH_NUCLEOTIDE_ARRAY];
        }

        public Nucleotide(String aHeader)
        {
            this.header = aHeader;

            nucleotides = new char[LENGTH_NUCLEOTIDE_ARRAY];
        }

        public void addCharNucleotideArray(char aNucleotide)
        {
            nucleotides[index] = aNucleotide;
            index++;

            //  Method to grow the array
            if (index >= LENGTH_NUCLEOTIDE_ARRAY)
            {
                Array.Resize<char>(ref nucleotides, (LENGTH_NUCLEOTIDE_ARRAY * 2));
            }
        }


        public Boolean isDNA(String dna)
        {

            char[] validCodes = { 'T', 'C', 'A', 'G', 'N' };
            Boolean validOrNot = true; // Assume that dna is a valid string.

            for (int position = 0; position < dna.Length; position++)
            {
                char nextChar = dna[position];
                if (!(nextChar == validCodes[0] || nextChar == validCodes[1] || nextChar == validCodes[2] || nextChar == validCodes[3] || nextChar == validCodes[4]))
                {
                    validOrNot = false;
                    break;
                }
            }
            return (validOrNot);
        }


        public int getNucleotideArraySize()
        {
            return index;
        }

        public char getNucleotideByPosition(int index)
        {
            return nucleotides[index];
        }

        public char[] getNucleotideArray()
        {
            return nucleotides;
        }

        public String getNucleotideArrayHeader()
        {
            return header;
        }

        public void setNucleotideArrayHeader(String aHeader)
        {
            this.header = aHeader;
        }
    }
}
