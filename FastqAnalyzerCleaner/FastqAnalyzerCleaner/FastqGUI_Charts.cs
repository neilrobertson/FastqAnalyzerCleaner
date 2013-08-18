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

        private FastqChartTypes currentChartType = FastqChartTypes.Distribution;

        private IFqFile fqFile = null;

        private ChartArea chart_Area;
        private Legend legend;
        private Series series;
        private Title title;

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

        public void DrawCurrentChartSelection(IFqFile file)
        {  
            this.fqFile = file;
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
            chart_Area = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            legend = new System.Windows.Forms.DataVisualization.Charting.Legend();
            series = new System.Windows.Forms.DataVisualization.Charting.Series();
            title = new System.Windows.Forms.DataVisualization.Charting.Title();

            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();

            chart_Area.Name = "Fastq Quality Distribiution";
            this.ChartAreas.Add(chart_Area);

            legend.Name = "Legend";
            this.Legends.Add(legend);
            this.Location = CHART_LOCATION;
            this.Name = "FastqGUI_Chart";

            series.ChartArea = "Fastq Quality Distribiution";
            series.Legend = "Legend";
            series.Name = "Global Quality Distribution";
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Bar;
            series.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            series.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            this.Series.Add(series);
            this.Size = CHART_SIZE;
            this.TabIndex = 0;
            this.Text = "FastqGUI_Chart";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

            List<int> distribution = fqFile.getDistribution();
            for (int i = 0; i < distribution.Count; i++)
            {
                // Add X and Y values for a point. 
                this.Series["Global Quality Distribution"].Points.AddXY(i+1, distribution[i]);
            }
            Console.WriteLine("HERE");
            this.Invalidate();
        }

        public void ClearCharts()
        {
            this.Series.Clear();
            this.ChartAreas.Clear();
            this.Legends.Clear();
            this.Titles.Clear();
        }

        /***********************************Chart Type Enum************************************/

        public enum FastqChartTypes
        {
            Distribution = 1,
            PerBaseSequenceStatistics = 2
        }

        /***********************************Abstract Chart Factory*****************************/


        public class TaskDiscrimination
        {
            public static Dictionary<String, IFastqGUI_Chart_Type> storage = new Dictionary<String, IFastqGUI_Chart_Type>();
            public static HashSet<String> checkExists = new HashSet<String>();
            public static Boolean isSetUp = false;
            public static IFastqGUI_Chart_Type getChart(String taskName)
            {
                if (isSetUp == false)
                {
                    setUp();
                    isSetUp = true;
                }
                IFastqGUI_Chart_Type task = (IFastqGUI_Chart_Type)storage[taskName];
                if (task == null)
                    return ChartType_SequenceStatistics.task as IFastqGUI_Chart_Type;
                return task;
            }

            public static void register(String key, IFastqGUI_Chart_Type value)
            {
                if (checkExists.Contains(key) == false)
                {
                    checkExists.Add(key);
                    storage.Add(key, value);
                }
            }

            private static void setUp()
            {
                ChartType_SequenceStatistics.register();
                ChartType_QualityDistribution.register();
            }
        }

        public abstract class IFastqGUI_Chart_Type
        {
            abstract public void DrawChart();
            abstract public String GetStatement();
        }

        /******************************Chart Type Subclasses***********************************/

        public class ChartType_SequenceStatistics : IFastqGUI_Chart_Type
        {
            public static String statement = FastqChartTypes.PerBaseSequenceStatistics.ToString();
            public static IFastqGUI_Chart_Type task = new ChartType_SequenceStatistics();

            public ChartArea chart_Area;
            public Legend legend;
            public Series series;

            public override void DrawChart()
            {
                
            }
            public override String GetStatement()
            {
                return "Performing " + statement + " Task";
            }
            public static void register()
            {
                TaskDiscrimination.register(statement, task);
            }
        }

        public class ChartType_QualityDistribution : IFastqGUI_Chart_Type
        {
            public static String statement = FastqChartTypes.Distribution.ToString();
            public static IFastqGUI_Chart_Type task = new ChartType_QualityDistribution();
            public override void DrawChart()
            {
                throw new NotImplementedException();
            }
            public override String GetStatement()
            {
                return "Performing " + statement + " Task";
            }
            public static void register()
            {
                TaskDiscrimination.register(statement, task);
            }
        }
    }
}
