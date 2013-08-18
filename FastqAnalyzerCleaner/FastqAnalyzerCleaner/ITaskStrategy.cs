using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastqAnalyzerCleaner
{
    public abstract class ITaskStrategy
    {
        abstract public GenericFastqInputs perform(GenericFastqInputs inputs);
        abstract public String getStatement();
        abstract public String getReportStatement();
        abstract public void confirmTaskEnd();
    }
}
