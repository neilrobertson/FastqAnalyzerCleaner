using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms;

namespace FastqAnalyzerCleaner
{
    public class FastqGUI_Charts : Chart
    {
        private System.Drawing.Point CHART_LOCATION = new System.Drawing.Point(2, 33);
        private System.Drawing.Size CHART_SIZE = new System.Drawing.Size(901, 471);

        private ChartArea chart_Area;
        private Legend legend;
        private Series series;

        public FastqGUI_Charts()
        {
            CreateChartArea();
        }

        public void CreateChartArea()
        {
            chart_Area = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            legend = new System.Windows.Forms.DataVisualization.Charting.Legend();
            series = new System.Windows.Forms.DataVisualization.Charting.Series();

            // 
            // FastqGUI_Chart
            // 
            chart_Area.Name = "ChartArea";
            this.ChartAreas.Add(chart_Area);
            legend.Name = "Legend";
            this.Legends.Add(legend);
            this.Location = CHART_LOCATION;
            this.Name = "FastqGUI_Chart";
            series.ChartArea = "ChartArea";
            series.Legend = "Legend";
            series.Name = "Sequence Mean Quality";
            this.Series.Add(series);
            this.Size = CHART_SIZE;
            this.TabIndex = 0;
            this.Text = "FastqGUI_Charts";
        }

        public void SelectChartType()
        {
             
             
        }

        public enum FastqChartTypes
        {
            FastqMeanStatistics = 1,
            Distribution = 2
        }
    }
}
