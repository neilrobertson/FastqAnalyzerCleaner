using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    public class Task_SequenceStatisticsTask : ITaskStrategy
    {
        public static String statement = "Sequencer Statistics";
        public static ITaskStrategy task = new Task_SequenceStatisticsTask();
        public static String reportStatement = "[PERFORMING SEQUENCE STATISTICS TASKS]";

        public override GenericFastqInputs perform(GenericFastqInputs inputs)
        {
            inputs.FastqFile.performSequenceStatistics();
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
