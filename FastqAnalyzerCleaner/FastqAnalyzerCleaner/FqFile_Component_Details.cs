using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    public class FqFile_Component_Details
    {
        public String ComponentName { get; set; } 
        public String sequencerType { get; set; }
        public String fileName { get; set; }
        public int TotalNucs { get; set; }
        public int NucleotidesCleaned { get; set; }
        public List<int> Distribution { get; set; }
        public int NCount { get; set; }
        public int CCount { get; set; }
        public int GCount { get; set; }
        public int MaxSeqSize { get; set; }
        public int MinSeqSize { get; set; }
        public double NPercent { get; set; }
        public double CPercent { get; set; }
        public double GPercent { get; set; }
        public Dictionary<int, string> RemovedAdapters { get; set; }
        public FqPerBaseSatistics[] perBaseStatistics;
   
        public FqFile_Component_Details()
        {

        }

        public void ContructComponentDetails(IFqFile component)
        {
            this.ComponentName = component.getFileName();
            this.sequencerType = component.getSequencerType();
            this.TotalNucs = component.getTotalNucleotides();
            this.NucleotidesCleaned = component.getNucleotidesCleaned();
            this.Distribution = component.getDistribution();
            this.NCount = component.getNCount();
            this.CCount = component.getCCount();
            this.GCount = component.getGCount();
            this.MaxSeqSize = component.getMaxSeqSize();
            this.MinSeqSize = component.getMinSeqSize();
            this.NPercent = component.nContents();
            this.CPercent = component.cContents();
            this.GPercent = component.gContents();
            this.RemovedAdapters = component.getRemovedAdapters();
            this.perBaseStatistics = component.GetPerBaseStatisticsArray();
        }

        public void CombineFqFile_Component_DetailsScores(FqFile_Component_Details inputDetails)
        {
            this.sequencerType = inputDetails.sequencerType;
            this.TotalNucs += inputDetails.TotalNucs;
            this.NucleotidesCleaned += inputDetails.NucleotidesCleaned;
            this.NCount += inputDetails.NCount;
            this.CCount += inputDetails.CCount;
            this.GCount += inputDetails.GCount;
            if (inputDetails.MaxSeqSize > this.MaxSeqSize)
                this.MaxSeqSize = inputDetails.MaxSeqSize;
            if (inputDetails.MinSeqSize < this.MinSeqSize)
                this.MinSeqSize = inputDetails.MinSeqSize;
            CalculatePercents();
            CombineDistributionLists(inputDetails.Distribution);
        }

        private void CalculatePercents()
        {
            NPercent = (((double)NCount / TotalNucs) * 100);
            CPercent = (((double)CCount / TotalNucs) * 100);
            GPercent = (((double)GCount / TotalNucs) * 100);
        }

        private void CombineDistributionLists(List<int> inputDistributions)
        {
            if (Distribution == null)
                InitializeDistributionList();

            for (int i = 0; i < inputDistributions.Count; i++)
            {
                Distribution[i] += inputDistributions[i];
            }
        }

        private void InitializeDistributionList()
        {
            Distribution = new List<int>(40);
            for (int j = 0; j <= SequencerDiscriminator.getSequencerSpecifier(sequencerType).getDistributionSpread(); j++)
            {
                Distribution.Add(0);
            }
        }

        public void OutputToConsole()
        {
            Console.WriteLine("NCount: {0} NPercent: {1}", NCount, NPercent);
            Console.WriteLine("CCount: {0} CPercent: {1}", CCount, CPercent);
            Console.WriteLine("GCount: {0} GPercent: {1}", GCount, GPercent);

            Console.WriteLine("Total Nucs {0} MinSeqSize: {1} MaxSeqSize: {2}", TotalNucs, MinSeqSize, MaxSeqSize);

            Console.WriteLine("Nucleotides Cleaned: {0}", NucleotidesCleaned);
        }
    }
}
