﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    public class Task_RemoveSequencesBelowLength  : ITaskStrategy
    {
        public static String statement = "Remove Below Sequence Length";
        public static ITaskStrategy task = new Task_RemoveSequencesBelowLength();
        public static String reportStatement = "[REMOVING SEQUENCES]";

        public override GenericFastqInputs perform(GenericFastqInputs inputs)
        {
            inputs.FastqFile.removeBelowSequenceLength(inputs.LengthThreshold);
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
