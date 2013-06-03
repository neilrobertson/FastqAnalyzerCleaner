using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    class GenericFastqInputs
    {
        public FqFile FastqFile { get; set; }
        public String TaskAction { get; set; }
        public int NucleotidesToClean { get; set; }
        public String Output { get; set; }

        public GenericFastqInputs() { }
    }
}
