using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FastqAnalyzerCleaner
{
    public class Task_SaveFastq  : ITaskStrategy
    {
        public static String statement = "Save Fastq";
        public static ITaskStrategy task = new Task_SaveFastq();
        public static String reportStatement = "[SAVING FASTQ]";

        public StreamWriter writer;

        public override GenericFastqInputs perform(GenericFastqInputs inputs)
        {
            writer = inputs.SaveStreamWriter;
            FqFile_Component component = (FqFile_Component) inputs.FastqFile;

            for (int i = 0; i < component.getFastqArraySize(); i++)
            {
                writer.Write(component.getFastqSequenceByPosition(i).createFastqBlock(component.getMap()));
            }
            
            return inputs;
        }

        public override void confirmTaskEnd()
        {
            writer.Flush();
            writer.Close();

            task = new Task_SaveFastq();
            register();
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
