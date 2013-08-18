using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    public class Task_EndCleanTask : ITaskStrategy
    {
        public static String statement = "Sequence End Cleaner";
        public static ITaskStrategy task = new Task_EndCleanTask();
        public static String reportStatement = "[CLEANING SEQUENCE 3' ENDS]";

        public override GenericFastqInputs perform(GenericFastqInputs inputs)
        {
            inputs.FastqFile.cleanEnds(inputs.NucleotidesToClean);
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
