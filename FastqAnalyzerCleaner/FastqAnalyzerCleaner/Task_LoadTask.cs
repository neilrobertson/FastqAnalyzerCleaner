using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    public class Task_LoadTask : ITaskStrategy
    {
        public static String statement = "Loading Task";
        public static ITaskStrategy task = new Task_LoadTask();
        public static String reportStatement = "[TESTING FILES]";

        public override GenericFastqInputs perform(GenericFastqInputs inputs)
        {
            
            SequencerDecisionTree decTree = new SequencerDecisionTree(inputs.FastqFile);
            
            inputs.FastqFile.Tests();
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
