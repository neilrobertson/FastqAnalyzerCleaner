using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    public class Task_RescanSequencerTask : ITaskStrategy
    {
        public static String statement = "Sequencer Type Rescan";
        public static ITaskStrategy task = new Task_RescanSequencerTask();
        public static String reportStatement = "[DETERMINING SEQUENCER]";

        public override GenericFastqInputs perform(GenericFastqInputs inputs)
        {
            if (Preferences.getInstance().getSeqDecisionMethod())
            {
                SequencerDetermination seqDetermine = new SequencerDetermination(inputs.FastqFile);
            }
            else if (!Preferences.getInstance().getSeqDecisionMethod())
            {
                SequencerDecisionTree decTree = new SequencerDecisionTree(inputs.FastqFile);
            }
            return inputs;
        }

        public override void confirmTaskEnd()
        {

        }

        public override string getReportStatement()
        {
            return reportStatement;
        }

        public override String getStatement()
        {
            return "Performing " + statement + " Task";
        }

        public static void register()
        {
            TaskDiscrimination.register(statement, task);
        }
    }
}
