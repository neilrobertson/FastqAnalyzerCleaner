using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FastqAnalyzerCleaner
{
    public class Task_SaveCSV : ITaskStrategy
    {
        public static String statement = "Save CSV";
        public static ITaskStrategy task = new Task_SaveCSV();
        public static String reportStatement = "[SAVING CSV]";

        public override GenericFastqInputs perform(GenericFastqInputs inputs)
        {
            StreamWriter writer;
            String extension = Path.GetExtension(inputs.SaveFileName);
            String fileName = Path.GetFileNameWithoutExtension(inputs.SaveFileName);
            String[] part = Path.GetFileNameWithoutExtension(inputs.FastqFile.getFileName()).Split('_');
            String number = part[part.Length - 1];

            String FullName = String.Format(@"{0}_{1}{2}", fileName, number, extension);

            string COMMA_DELIMITER = ",";
            string[] output;

            IFqFile fqFile = inputs.FastqFile;

            writer = new StreamWriter(FullName);
                
            output = new string[] {"Sequence Index", "Header", "Total Nucleotides", "G Count", "C Count", "Misread Count", "Lower Threshold", "First Quartile",
                                    "Median", "Mean", "Third Quartile", "Upper Threshold" };
            writer.WriteLine(string.Join(COMMA_DELIMITER, output));

            for (int i = 0; i < fqFile.getFastqArraySize(); i++)
            {
                FqSequence fqSeq = fqFile.getFastqSequenceByPosition(i);
                output = new string[] { fqSeq.getSeqIndex().ToString(), fqSeq.getSequenceHeader(), fqSeq.getFastqSeqSize().ToString(), fqSeq.getGCount().ToString(), 
                                        fqSeq.getCCount().ToString(), fqSeq.getNCount().ToString(), fqSeq.getLowerThreshold().ToString(),
                                        fqSeq.getFirstQuartile().ToString(), fqSeq.getMedian().ToString(), fqSeq.getMean().ToString(),
                                        fqSeq.getThirdQuartile().ToString(), fqSeq.getUpperThreshold().ToString(), 
                                        fqSeq.createSequenceString(fqFile.getMap()) };
                writer.WriteLine(string.Join(COMMA_DELIMITER, output));
            }

            writer.Flush();
            writer.Close();
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
