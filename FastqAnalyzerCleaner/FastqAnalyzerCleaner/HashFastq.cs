/// <copyright file="HashFastq.cs" author="Neil Robertson">
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FastqAnalyzerCleaner
{
    class HashFastq
    {
        private Dictionary<int, FqNucleotideRead> map;
	    private HashSet<int> checkExists;

        public readonly static String FQ_FILE_MAP_SERIALIZING_NAME = "fqhsh.bin";
	
	    public HashFastq()
	    {
		    map = new Dictionary<int, FqNucleotideRead>();
		    checkExists = new HashSet<int>();
	    }
	
	    public void addNucleotide(FqNucleotideRead fqRead, int hashcode)
	    {
            if (checkExists.Contains(hashcode) == false)
            {
                checkExists.Add(hashcode);
                map.Add(hashcode, fqRead);
            }   
	    }

        public static Dictionary<int, FqNucleotideRead> deserializeHashmap()
        {
            try
            {
                using (Stream stream = File.Open(@FQ_FILE_MAP_SERIALIZING_NAME, FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    Dictionary<int, FqNucleotideRead> hashmap = (Dictionary<int, FqNucleotideRead>)bin.Deserialize(stream);
                    Console.Write("Fastq mapping file deserialized from disk successfully! \n");
                    return hashmap;
                }
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                Console.Write("Fastq mapping file deserialization FAILED!  Attempting to create file!! \n");
                serializeFastqHashmap();
                return deserializeHashmap();
            }
            catch (IOException)
            {
                Console.Write("Fastq mapping file deserialization FAILED!  Attempting to create file!! \n");
                serializeFastqHashmap();
                return deserializeHashmap();
            }
        }

        public static void serializeFastqHashmap()
        {
            Dictionary<int, FqNucleotideRead> hashmap = HashFastq.createHashMap();
            try
            {
                using (Stream stream = File.Open(@FQ_FILE_MAP_SERIALIZING_NAME, FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, hashmap);
                }
            }
            catch (IOException e)
            {
                Console.Write("Creation of serialized fastq Fq_FILE_MAP failed \n");
                Console.Write(e.StackTrace);
            }
        }

        private static Dictionary<int, FqNucleotideRead> createHashMap()
        {
            Dictionary<int, FqNucleotideRead> hashmap = new Dictionary<int, FqNucleotideRead>();
            char[] nucleotides = { 'A', 'G', 'C', 'T', 'U', 'N' };

            char[] qualities = { '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-',
                                   '.', '/', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':',
                                   ';', '<', '=', '>', '?', '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G',
                                   'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
                                   'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_', '`', 'a',
                                   'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 
                                   'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '{', 
                                   '|', '}', '~' };

            FqNucleotideRead fqRead;
            HashSet<int> existance = new HashSet<int>();

            for (int i = 0; i < nucleotides.Length; i++)
            {
                for (int j = 0; j < qualities.Length; j++)
                {
                    fqRead = new FqNucleotideRead(nucleotides[i], qualities[j]);
                    int hashcode = fqRead.getProxyCode();
                    if (existance.Contains(hashcode) == false)
                    {
                        existance.Add(hashcode);
                        hashmap.Add(hashcode, fqRead);
                    }  
                }
            }
            return hashmap;
        }

        public static Dictionary<int, FqNucleotideRead> calculateHashQualities(String sequencerType, Dictionary<int, FqNucleotideRead> hashmap)
        {
            foreach (FqNucleotideRead fqRead in hashmap.Values)
            {
                fqRead.calculateQualityScore(sequencerType);
            }
            return hashmap;
        }
	
	    public void disposeExistanceSet()
	    {
		    checkExists = null;
		    Console.Write(map);
	    }
	
	    public Dictionary<int, FqNucleotideRead> getMap()
	    {
		    return map;
	    }
    }
}
