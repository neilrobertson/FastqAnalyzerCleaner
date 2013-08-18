using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    public class Task_RemoveBelowMeanThreshold : ITaskStrategy
    {
        public static String statement = "Remove Below Mean Threshold Task";
        public static ITaskStrategy task = new Task_RemoveBelowMeanThreshold();
        public static String reportStatement = "[REMOVING SEQUENCES]";

        public override GenericFastqInputs perform(GenericFastqInputs inputs)
        {
            //inputs.FastqFile.FindSequence(inputs.NucleotideSequence);
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
