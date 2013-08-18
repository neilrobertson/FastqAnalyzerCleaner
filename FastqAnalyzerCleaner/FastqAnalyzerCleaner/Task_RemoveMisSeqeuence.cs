using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    public class Task_RemoveMisSeqeuence : ITaskStrategy
    {
        public static String statement = "Remove Sequences With Misreads";
        public static ITaskStrategy task = new Task_RemoveMisSeqeuence();
        public static String reportStatement = "[CLEANING SEQUENCES TASK]";

        public override GenericFastqInputs perform(GenericFastqInputs inputs)
        {
            //Show error message
            return inputs;
        }
        public override String getStatement()
        {
            return "Performing " + statement + " Task";
        }

        public override void confirmTaskEnd()
        {

        }

        public override string getReportStatement()
        {
            return reportStatement;
        }

        public static void register()
        {
            TaskDiscrimination.register(statement, task);
        }
    }
}
