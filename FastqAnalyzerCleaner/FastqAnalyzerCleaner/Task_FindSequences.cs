using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    public class Task_FindSequences : ITaskStrategy
    {
        public static String statement = "Finding Sequences";
        public static ITaskStrategy task = new Task_FindSequences();
        public static String reportStatement = "[FINDING SEQUENCES]";

        public override GenericFastqInputs perform(GenericFastqInputs inputs)
        {
            List<FqSequence> foundSequences = inputs.FastqFile.findSequence(inputs.NucleotideSequence);
            FastqController.getInstance().GetFqFileMap().AddSequenceQueryResults(foundSequences);
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
