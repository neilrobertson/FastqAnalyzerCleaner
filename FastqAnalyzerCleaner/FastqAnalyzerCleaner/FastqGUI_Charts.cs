using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace FastqAnalyzerCleaner
{
    public class FastqGUI_Charts : Chart
    {

        private ChartArea chartArea1;
        private Legend legend1;
        private Series series1;

        public FastqGUI_Charts()
        {

            CreateChartArea();
            BeginInit();
            
        }

        public void CreateChartArea()
        {
            chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            series1 = new System.Windows.Forms.DataVisualization.Charting.Series();

            // 
            // FastqGUI_Chart
            // 
            chartArea1.Name = "ChartArea1";
            this.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.Legends.Add(legend1);
            this.Location = new System.Drawing.Point(2, 33);
            this.Name = "FastqGUI_Chart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Testing that subclassing adds function to FastqGUI";
            this.Series.Add(series1);
            this.Size = new System.Drawing.Size(901, 471);
            this.TabIndex = 0;
            this.Text = "FastqGUI_Charts";
        }


    }
}
