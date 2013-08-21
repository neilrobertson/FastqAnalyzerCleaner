using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FastqAnalyzerCleaner
{
    public class FastqGUI_Charts
    {
        private System.Drawing.Point CHART_LOCATION = new System.Drawing.Point(2, 33);
        private System.Drawing.Size CHART_SIZE = new System.Drawing.Size(901, 471);

        private FastqChartTypes currentChartType = FastqChartTypes.Distribution;

        private IFqFile fqFile = null;

        

        public FastqGUI_Charts()
        {
            
        }

        public void SelectChartType(FastqChartTypes chartType)
        {
            if (currentChartType != chartType)
            {
                this.currentChartType = chartType;
                
                Console.WriteLine("Chart type selected: {0}", currentChartType.ToString());
            }
        }

        public void DrawCurrentChartSelection()
        {  
            ClearCharts();
            if (fqFile != null)
            {
                Console.WriteLine("Drawing new chart");
                if (currentChartType == FastqChartTypes.PerBaseSequenceStatistics)
                    DrawSequenceStatistics();
                else if (currentChartType == FastqChartTypes.Distribution)
                    DrawDistribution();
            }
        }

        public void DrawSequenceStatistics()
        {
            
        }

        public void DrawDistribution()
        {
            
        }

        public void ClearCharts()
        {

        }

        /***********************************Chart Type Enum************************************/

        public enum FastqChartTypes
        {
            Distribution = 1,
            PerBaseSequenceStatistics = 2
        }

       
    }
}
