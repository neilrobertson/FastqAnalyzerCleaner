// <copyright file="FqFileMap.cs" author="Neil Robertson">
// Copyright (c) 2013 All Right Reserved, Neil Alistair Robertson - neil.alistair.robertson@hotmail.co.uk
//
// This code is the property of Neil Robertson.  Permission must be sought before reuse.
// It has been written explicitly for the MRes Bioinfomatics course at the University 
// of Glasgow, Scotland under the supervision of Derek Gatherer.
//
// </copyright>
// <author>Neil Robertson</author>
// <email>neil.alistair.robertson@hotmail.co.uk</email>
// <date>2013-06-1</date>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FastqAnalyzerCleaner
{
    /// <summary>
    /// This class contains vital higher order details for the serialized components. Including higher order statistical data, 
    /// sequencer information nucleotide/quality read maps, global details, component details, file information as
    /// well as methods that allow for the addition and manipulation of this data.
    /// </summary>
    public class FqFileMap
    {
        public static int FQ_BLOCK_LIMIT = 10000;

        public List<String> fqFileDirectories { get; set; }
        public String FileGUID { get; set; } 

        public String SequencerType { get; set; }

        public Dictionary<int, FqNucleotideRead> FqReadMap { get; set; }

        public String FileName { get; set; }
        public long FileLength { get; set; }
        public String FastqBlocks { get; set; }
        public String FastqFileFormatType { get; set; }

        public FqFile_Component_Details GlobalDetails { get; set; }
        public Dictionary<String, FqFile_Component_Details> ComponentMap;

        public String LastTask { get; set; }
        public String TimeTaken { get; set; }

        public Boolean isMapConstructed = false;

        public List<FqSequence> QueriedSequences;

        /// <summary>
        /// Default constructor for the class, initializes a list for containing the directories of the fastq component files,
        /// a map that allows the construction of statistical data for each component and a GUID relavent to this file map class
        /// </summary>
        public FqFileMap()
        {
            fqFileDirectories = new List<String>();
            ComponentMap = new Dictionary<string, FqFile_Component_Details>();
            FileGUID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Constructs the sequencer specific read map by calling the HashFastq class to deserialize it from memory.
        /// After it is constructed the same class is used to calculate relavent quality scores for each of the nucleotide/quality 
        /// pairings.
        /// </summary>
        /// <param name="sequencerType"></param>
        /// <returns></returns>
        public Dictionary<int, FqNucleotideRead> ConstructSequencerSpecificReadMap(String sequencerType)
        {
            if (FqReadMap == null)
                FqReadMap = HashFastq.deserializeHashmap();

            if (isMapConstructed == false)
            {
                this.SequencerType = sequencerType;
                FqReadMap = HashFastq.calculateHashQualities(SequencerType, FqReadMap);
                isMapConstructed = true;
            }
            return FqReadMap;
        }

        public void InitializeNewSequenceSearchList()
        {
            QueriedSequences = new List<FqSequence>();
        }

        public void AddSequenceQueryResults(List<FqSequence> results)
        {
            QueriedSequences.AddRange(results);
        }

        public void SortQueriedSequences()
        {
            QueriedSequences = QueriedSequences.OrderBy(x => x.Mean).ToList();
        }

        public void CalculateGlobalFileScores()
        {
            GlobalDetails = new FqFile_Component_Details();

            for (int i = 0; i < fqFileDirectories.Count; i++)
            {
                FqFile_Component_Details currentDetails = ComponentMap[fqFileDirectories[i]];

                GlobalDetails.Combine_FqFile_Component_DetailsScores(currentDetails);
            }
        }

        public void InitializeReadMap()
        {
            if (FqReadMap == null)
                FqReadMap = HashFastq.deserializeHashmap();
        }

        public String getFileGUID()
        {
            return FileGUID;
        }

        public List<String> getFileComponentDirectories()
        {
            return fqFileDirectories;
        }

        public Dictionary<String, FqFile_Component_Details> GetFqFileComponentDetailsMap()
        {
            return ComponentMap;
        }

        public List<FqSequence> GetQueriedSequences()
        {
            return QueriedSequences;
        }
    }
}
