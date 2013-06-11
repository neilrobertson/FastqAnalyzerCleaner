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
    public class Adapters  
    {
        private static object sync_lock;
        private static Adapters uniqueInstance = null;

        private static List<Adapter> adapters;
        private int largestAdapterSequence;

        private Adapters()
        {
            adapters = DeserializeAdapterResource();
            largestAdapterSequence = discoverLargestAdapterSequence();
        }

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

        public void AddAdapter(Adapter adapter)
        {
            adapters.Add(adapter);
            SerializeAdapterResource(adapters);
        }

        public List<Adapter> getAdaptersList()
        {
            return adapters;
        }

        public int getLargestAdapterSize()
        {
            return largestAdapterSequence;
        }

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

        public static List<Adapter> DeserializeAdapterResource()
        {
            try
            {
                using (Stream stream = File.Open(@"adptr.bin", FileMode.Open))
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

        public static void SerializeAdapterResource(List<Adapter> resource = null)
        {
            List<Adapter> tempAdapterResource;
            if (resource == null)
                tempAdapterResource = ParseAdapterResource();
            else
                tempAdapterResource = resource;

            try
            {
                using (Stream stream = File.Open(@"adptr.bin", FileMode.Create))
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
