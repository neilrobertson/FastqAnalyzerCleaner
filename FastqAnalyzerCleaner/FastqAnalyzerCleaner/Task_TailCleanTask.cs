using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    public class Task_TailCleanTask : ITaskStrategy
    {
        public static String statement = "Sequence Tail Cleaner";
        public static ITaskStrategy task = new Task_TailCleanTask();
        public static String reportStatement = "[CLEANING SEQUENCE TAILS]";

        public override GenericFastqInputs perform(GenericFastqInputs inputs)
        {
            inputs.FastqFile.cleanStarts(inputs.NucleotidesToClean);
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
