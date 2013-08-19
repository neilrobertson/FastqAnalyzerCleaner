using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FastqAnalyzerCleaner
{
    public class FqFileMap
    {
        public static int FQ_BLOCK_LIMIT = 10000;

        public List<String> fqFileComponents { get; set; }
        public String FileGUID { get; set; } 

        public String SequencerType { get; set; }

        public Dictionary<int, FqNucleotideRead> FqReadMap { get; set; }

        public String FileName { get; set; }
        public long FileLength { get; set; }
        public String FastqBlocks { get; set; }
        public String FastqFileFormatType { get; set; }

        public FqFile_Component_Details GlobalDetails { get; set; }
        public Dictionary<String, FqFile_Component_Details> ComponentMap;

        public Boolean isMapConstructed = false;

        public List<FqSequence> QueriedSequences;

        public FqFileMap()
        {
            fqFileComponents = new List<String>();
            ComponentMap = new Dictionary<string, FqFile_Component_Details>();
            FileGUID = Guid.NewGuid().ToString();
        }

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

        public void CalculateGlobalFileScores()
        {
            GlobalDetails = new FqFile_Component_Details();

            for (int i = 0; i < fqFileComponents.Count; i++)
            {
                FqFile_Component_Details currentDetails = ComponentMap[fqFileComponents[i]];

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
            return fqFileComponents;
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
