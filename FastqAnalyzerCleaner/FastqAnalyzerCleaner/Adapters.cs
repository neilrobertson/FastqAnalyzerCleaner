/// <copyright file="Adapters.cs" author="Neil Robertson">
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
    /// <summary>
    /// Adapter class contains resources required for building and storing a structure of Adapter seqeuence types
    /// </summary>
    public class Adapters  
    {
        private static object sync_lock;
        private static Adapters uniqueInstance = null;

        private static List<Adapter> adapters;
        private int largestAdapterSequence;

        public readonly static String ADAPTER_SERIALIZATION_NAME = "adptr.bin";

        /// <summary>
        /// Constructor attempts to deserialize the adapter class into memory and finds length of largest adapter.
        /// </summary>
        private Adapters()
        {
            adapters = DeserializeAdapterResource();
            largestAdapterSequence = discoverLargestAdapterSequence();
        }

        /// <summary>
        /// Accessor for the unique instance of this singleton class
        /// </summary>
        /// <returns>The unique instance of this class</returns>
        public static Adapters getInstance()
        {
            sync_lock = new object();
            lock (sync_lock)
            {
                if (uniqueInstance == null)
                    uniqueInstance = new Adapters();
                return uniqueInstance;
            }
        }

        /// <summary>
        /// Adds an adapter to the adapters list
        /// </summary>
        /// <param name="adapter"></param>
        public void AddAdapter(Adapter adapter)
        {
            adapters.Add(adapter);
            SerializeAdapterResource(adapters);
        }

        /// <summary>
        /// Accessor method that returns the adapter list
        /// </summary>
        /// <returns></returns>
        public List<Adapter> getAdaptersList()
        {
            return adapters;
        }

        /// <summary>
        /// Accessor method returns the size of the largest adapter
        /// </summary>
        /// <returns></returns>
        public int getLargestAdapterSize()
        {
            return largestAdapterSequence;
        }

        /// <summary>
        /// Method calculates the largest adapter sequence
        /// </summary>
        /// <returns>An integer that conforms to the size of the largest adapter</returns>
        private int discoverLargestAdapterSequence()
        {
            int largest = 0;
            foreach (Adapter adapter in adapters)
            {
                if (adapter.AdapterSequence.Length > largest)
                    largest = adapter.AdapterSequence.Length;
            }
            return largest;
        }

        /// <summary>
        /// Parses the hard coded adapter string so that program comes with in built adapter resource
        /// </summary>
        /// <returns></returns>
        public static List<Adapter> ParseAdapterResource()
        {
            List<Adapter> parsed_adapters = new List<Adapter>();
            string adapter_text = getHardCodedAdapters();
            String[] lines = adapter_text.Split('\n');
            foreach (String line in lines)
            {
                String[] adapter_parts = line.Split('#');
                Adapter adapter = new Adapter(adapter_parts[0], adapter_parts[1]);
                parsed_adapters.Add(adapter);
            }
            return parsed_adapters;
        }

        /// <summary>
        /// Large string of hard coded adapter seqeuences
        /// </summary>
        /// <returns></returns>
        public static String getHardCodedAdapters()
        {
            return ">Primer1#AAGCAGTGGTATCAACGCAGAGT\n" +
                   ">Primer2#AAGCAGTGGTATCAACGCAGAGTACGCGGG\n" +
                   ">Primer3#AAGCAGTGGTATCAACGCAGAGTACT\n" +
                   ">Primer4#AAGCAGTGGTATCAACGCAGAGTACTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT\n" +
                   ">SMART_primer5#GAGTCCAGAAGAGTGCGATGAGTACTTTTTTTTTTTTTTTTTTTTTTTTTTTTTT\n" +
                   ">Nextera#CTGTCTCTTATACACATCT\n" +
                   ">Standard_Illumina#AGATCGGAAGAGC\n" +
                   ">Variant#GCCGGAGCTCTGCAGATATC";
        }

        /// <summary>
        /// Deserializes the adapters resource into memory
        /// </summary>
        /// <returns></returns>
        public static List<Adapter> DeserializeAdapterResource()
        {
            try
            {
                using (Stream stream = File.Open(@ADAPTER_SERIALIZATION_NAME, FileMode.Open))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    List<Adapter> tempAdapterResource = (List<Adapter>)bin.Deserialize(stream);
                    Console.WriteLine("Adapter resource deserialized from disk successfully.");
                    return tempAdapterResource;
                }
            }
            catch (System.Runtime.Serialization.SerializationException)
            {
                Console.WriteLine("Adapter resource deserialization failed. File does not exist.  Attempting to create file!!");
                SerializeAdapterResource();
                return DeserializeAdapterResource();
            }
            catch (IOException)
            {
                Console.WriteLine("Adapter resource deserialization failed. File does not exist. Attempting to create file!!");
                SerializeAdapterResource();
                return DeserializeAdapterResource();
            }
        }

        /// <summary>
        /// Serializes the adapters resource to disk
        /// </summary>
        /// <param name="resource"></param>
        public static void SerializeAdapterResource(List<Adapter> resource = null)
        {
            List<Adapter> tempAdapterResource;
            if (resource == null)
                tempAdapterResource = ParseAdapterResource();
            else
                tempAdapterResource = resource;

            try
            {
                using (Stream stream = File.Open(@ADAPTER_SERIALIZATION_NAME, FileMode.Create))
                {
                    BinaryFormatter bin = new BinaryFormatter();
                    bin.Serialize(stream, tempAdapterResource);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Creation of serialized adapter resource failed.");
                Console.Write(e.StackTrace);
            }
        }

        /// <summary>
        /// Inner Adapter class, contains the structure for a single adapter
        /// </summary>
        [Serializable]
        public class Adapter
        {
            public String AdapterSequence { get; set; }
            public String AdapterName { get; set; }

            public Adapter(String name, String seq)
            {
                this.AdapterName = name;
                this.AdapterSequence = seq;
            }
        }
    }
}
