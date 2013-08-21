using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    public class FqSequence_InputsOuptuts
    {
        public static readonly String ADAPTER_TASK = "Adapter Removal";
        public static readonly String SEQUENCE_TESTS_TASK = "Sequence Tests";

            public List<int> distributes {get; set;}
            public int[] perSeqQuals {get; set;}
            public List<Adapters.Adapter> adapters {get; set;}
            public GenericFastqInputs removedAdapters { get; set; }
            public Int32 cCount { get; set; }
            public Int32 gCount { get; set; }
            public Int32 nCount { get; set; }
            public int sequenceLength { get; set; }
            public int subZeroOffset { get; set; }

            public FqSequence_InputsOuptuts(String sequencerType, String taskType, FqFile_Component fqFile)
            {
                if (taskType == SEQUENCE_TESTS_TASK)
                {
                    distributes = new List<int>(40);
                    for (int j = 0; j <= SequencerDiscriminator.getSequencerSpecifier(sequencerType).getDistributionSpread(); j++)
                    {
                        distributes.Add(0);
                    }
                    perSeqQuals = new int[fqFile.getMaxSeqSize()];
                    subZeroOffset = SequencerDiscriminator.getSequencerSpecifier(sequencerType).getSubZeroQualities();
                }
                else if (taskType == ADAPTER_TASK)
                {
                    adapters = Adapters.getInstance().getAdaptersList();
                    removedAdapters = null;
                }

            }
    }
}
