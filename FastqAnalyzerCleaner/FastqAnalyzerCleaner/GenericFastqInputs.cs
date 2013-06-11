using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    public class GenericFastqInputs
    {
        public FqFile FastqFile { get; set; }
        public String TaskAction { get; set; }
        public int NucleotidesToClean { get; set; }
        public String Output { get; set; }
        public int SequenceIndex { get; set; }
        public String AdapterName { get; set; }

        public GenericFastqInputs() { }
    }
}
