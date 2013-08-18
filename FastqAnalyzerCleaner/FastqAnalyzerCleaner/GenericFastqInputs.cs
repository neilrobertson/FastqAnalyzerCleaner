using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FastqAnalyzerCleaner
{
    public class GenericFastqInputs
    {
        public IFqFile FastqFile { get; set; }
        public FqFile_Component FqFileComponent { get; set; }
        public String TaskAction { get; set; }
        public int NucleotidesToClean { get; set; }
        public int MeanThreshold { get; set; }
        public String Output { get; set; }
        public int SequenceIndex { get; set; }
        public String AdapterName { get; set; }
        public String NucleotideSequence { get; set; }
        public List<int> SequenceIndexes = new List<int>();
        public String SaveFileName { get; set; }
        public StreamWriter SaveStreamWriter { get; set; }

        public GenericFastqInputs() { }

        public List<int> GetSequenceIndexes()
        {
            return SequenceIndexes;
        }

        public void InitializeStreamWriter(String file)
        {
            SaveStreamWriter = new StreamWriter(@file);
        }
    }
}
