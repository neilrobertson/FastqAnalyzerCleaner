using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    public class Task_DefaultTask : ITaskStrategy
    {
        public static String statement = "Default";
        public static ITaskStrategy task = new Task_DefaultTask();
        public static String reportStatement = "[DEFAULT TASK]";
        public override GenericFastqInputs perform(GenericFastqInputs inputs)
        {
            //Show error message
            return inputs;
        }

        public override void confirmTaskEnd()
        {

        }

        public override String getStatement()
        {
            return "Performing " + statement + " Task";
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
