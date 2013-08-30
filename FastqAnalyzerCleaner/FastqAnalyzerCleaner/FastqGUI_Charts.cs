/// <copyright file="FastqGUI_Charts.cs" author="Neil Robertson">
/// Copyright (c) 2013 All Right Reserved, Neil Alistair Robertson - neil.alistair.robertson@hotmail.co.uk
///
/// This code is the property of Neil Robertson.  Permission must be sought before reuse.
/// It has been written explicitly for the MRes Bioinfomatics course at the University 
/// of Glasgow, Scotland under the supervision of Derek Gatherer.
///
/// </copyright>
/// <author>Neil Robertson</author>
/// <email>neil.alistair.robertson@hotmail.co.uk</email>
/// <date>2013-06-1</date>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using System.Drawing;

namespace FastqAnalyzerCleaner
{
    /// <summary>
    /// Main Charting class for the program, utilizes the Zedgraph.dll charting package, and draws three chart forms dependant on 
    /// the requests sent from the GUI
    /// </summary>
    public class FastqGUI_Charts
    {
        private System.Drawing.Point CHART_LOCATION = new System.Drawing.Point(2, 33);
        private System.Drawing.Size CHART_SIZE = new System.Drawing.Size(901, 471);

        private String currentChartType = FastqChartTypes.Distribution.ToString();

        private FastqGUI observer;

        private FqFile_Component_Details componentDetails;

        private ZedGraphControl graphControl;

        /// <summary>
        /// Default constructor accepts instance of the Gui as an observer to the class
        /// </summary>
        /// <param name="observe"></param>
        public FastqGUI_Charts(FastqGUI observe)
        {
            this.observer = observe;
        }

        /// <summary>
        /// This method called from the GUI decides which chart to draw and is generally called from the GUI.  It checks that the chart type 
        /// and component details to be drawn are not already the current ones available within the class and then draws
        /// a graph based on the chart type string that corresponds to the enum FastqChartTypes within this class.
        /// </summary>
        /// <param name="details">Current details to draw</param>
        /// <param name="chartType">The chart type to draw, based on the enum FastqChartTypes</param>
        /// <param name="chart">The ZedGraph control panel. ie the panel the charts are to be drawn on</param>
        public void DrawCurrentChartSelection(FqFile_Component_Details details, String chartType, ZedGraphControl chart)
        {
            if (!chartType.Equals(currentChartType) || !details.Equals(componentDetails))
            {
                this.componentDetails = details;
                this.currentChartType = chartType;
                this.graphControl = chart;

                Console.WriteLine("Drawing new chart");
                if (currentChartType == FastqChartTypes.PerBaseSequenceStatistics.ToString())
                    DrawSequenceStatistics();
                else if (currentChartType == FastqChartTypes.Distribution.ToString())
                    DrawDistribution();
                else if (currentChartType == FastqChartTypes.SequenceLengthDistribution.ToString())
                    DrawSequenceLengthDistribution();
            }
        }

        /// <summary>
        /// Draws a sequence statistic chart for the FastqComponent_Details handle within this class
        /// </summary>
        public void DrawSequenceStatistics()
        {
            Console.WriteLine("Drawing new sequence length distribution!");
            FqPerBaseSatistics[] perBaseStatistics = componentDetails.perBaseStatistics;
            double[] x = new double[perBaseStatistics.Length];

            PointPairList boxList = new PointPairList();
            PointPairList lowerWhisker = new PointPairList();
            PointPairList upperWhisker = new PointPairList();
            PointPairList medians = new PointPairList();
            
            int count = 1;
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = count;
                boxList.Add((double)x[i], (double)perBaseStatistics[i].ThirdQuartile, (double)perBaseStatistics[i].FirstQuartile);
                medians.Add((double)x[i], (double)perBaseStatistics[i].Median);
                upperWhisker.Add((double)x[i], (double)perBaseStatistics[i].UpperThreshold);
                lowerWhisker.Add((double)x[i], (double)perBaseStatistics[i].LowerThreshold);
                count++;
            }

            Size size = graphControl.ClientSize;
            Rectangle rect = new Rectangle();
            rect.Size = size;

            graphControl.GraphPane = new GraphPane();
            graphControl.GraphPane.CurveList.Clear();
            graphControl.GraphPane.Rect = rect;

            GraphPane myPane = graphControl.GraphPane;

            myPane.Title.Text = "Per Base Statistics " + componentDetails.getGraphName();

            // set X and Y axis titles
            myPane.XAxis.Title.Text = "Base Position";
            myPane.YAxis.Title.Text = "Qualities";

            CurveItem median = myPane.AddCurve("Median", medians, Color.Green, SymbolType.None);
            LineItem myLine = (LineItem)median;
            myLine.Line.IsVisible = true;
            myLine.Line.IsAntiAlias = true;
            myLine.Symbol.Fill.Type = FillType.Solid;
            myLine.Symbol.Size = 1;
            
            HiLowBarItem myCurve = myPane.AddHiLowBar("Quartiles", boxList, Color.Black);
            myCurve.Bar.Fill.Type = FillType.Solid;
            myCurve.Bar.Fill.Color = Color.Yellow;
            myCurve.Bar.Border.Color = Color.Yellow;

            CurveItem lthresholds = myPane.AddCurve("Lower Threshold", lowerWhisker, Color.Red, SymbolType.HDash);
            LineItem lthreshline = (LineItem)lthresholds;
            lthreshline.Line.IsVisible = false;
            lthreshline.Symbol.Fill.Type = FillType.Solid;
            lthreshline.Symbol.Size = 1;
            lthreshline.Symbol.Fill.Color = Color.Red;

            CurveItem uppthesh = myPane.AddCurve("Upper Threshold", upperWhisker, Color.Red, SymbolType.HDash);
            LineItem uthreshline = (LineItem)uppthesh;
            uthreshline.Line.IsVisible = false;
            uthreshline.Symbol.Fill.Type = FillType.Solid;
            uthreshline.Symbol.Size = 1;
            uthreshline.Symbol.Fill.Color = Color.Red;

            graphControl.Focus();
            graphControl.AxisChange();
            graphControl.Invalidate();
            graphControl.Refresh();
             
        }

        /// <summary>
        /// Draws a distribution for the quality scores for the FastqComponent_Details handle within this class
        /// </summary>
        public void DrawDistribution()
        {
            Console.WriteLine("Drawing new sequence length distribution!");
            double[] sequenceDistribution = componentDetails.Distribution.Select(i => (double)i).ToArray();
            double[] x = new double[sequenceDistribution.Length];
            int count = 0;
            for (int i = 0; i < x.Length; i++)
            {
                x[i] = count - SequencerDiscriminator.getSequencerSpecifier(componentDetails.sequencerType).getSubZeroQualities();
                count++;
            }


            Size size = graphControl.ClientSize;
            Rectangle rect = new Rectangle();
            rect.Size = size;

            // This is to remove all plots
            graphControl.GraphPane = new GraphPane();
            graphControl.GraphPane.CurveList.Clear();
            graphControl.GraphPane.Rect = rect;

            // GraphPane object holds one or more Curve objects (or plots)
            GraphPane myPane = graphControl.GraphPane;

            myPane.Title.Text = "Quality Distribution " + componentDetails.getGraphName();

            // set X and Y axis titles
            myPane.XAxis.Title.Text = "Quality";
            myPane.YAxis.Title.Text = "Nucleotide Count";

            // PointPairList holds the data for plotting, X and Y arrays
            PointPairList spl1 = new PointPairList(x, sequenceDistribution);

            // Add bars to myPane object
            BarItem myBar1 = myPane.AddBar(FastqChartTypes.SequenceLengthDistribution.ToString(), spl1, Color.Red);
            LineItem myLine1 = myPane.AddCurve(FastqChartTypes.SequenceLengthDistribution.ToString(), spl1, Color.Red, SymbolType.None);
            myLine1.Line.Width = 1.0F;

            // I add all three functions just to be sure it refeshes the plot.
            graphControl.AxisChange();
            graphControl.Invalidate();
            graphControl.Refresh();
        }

        /// <summary>
        /// Draws a chart that outlines the sequence length distributions for the FastqComponent_Details handle within this class
        /// </summary>
        public void DrawSequenceLengthDistribution()
        {
            Console.WriteLine("Drawing new sequence length distribution!");
            double[] sequenceDistribution = componentDetails.SequenceLengthDistribution.Select(i => (double)i).ToArray();
            double[] x = new double[sequenceDistribution.Length];
            int count = 1;
            for(int i = 0; i < x.Length; i++)
            {
                x[i] = count;
                count ++;
            }

            Size size = graphControl.ClientSize;
            Rectangle rect = new Rectangle();
            rect.Size = size;

            // This is to remove all plots
            graphControl.GraphPane = new GraphPane();
            graphControl.GraphPane.CurveList.Clear();
            graphControl.GraphPane.Rect = rect;

            // GraphPane object holds one or more Curve objects (or plots)
            GraphPane myPane = graphControl.GraphPane;
            myPane.IsBoundedRanges = true;

            myPane.Title.Text = "Sequence Length Distribution " + componentDetails.getGraphName();
 
            // set X and Y axis titles
            myPane.XAxis.Title.Text = "Sequence Length";
            myPane.YAxis.Title.Text = "Number of Sequences";

            // PointPairList holds the data for plotting, X and Y arrays
            PointPairList spl1 = new PointPairList(x, sequenceDistribution);

            // Add bars to myPane object
            BarItem myBar1 = myPane.AddBar(FastqChartTypes.SequenceLengthDistribution.ToString(), spl1, Color.Blue);

            // I add all three functions just to be sure it refeshes the plot.
            graphControl.AxisChange();
            graphControl.Invalidate();
            graphControl.Refresh();
        }


        /***********************************Chart Type Enum************************************/

        /// <summary>
        /// FastqChartTypes outlines the chart types that this class supports and is data bound to the combo box selector in the 
        /// GUI class 
        /// </summary>
        public enum FastqChartTypes
        {
            Distribution = 1,
            PerBaseSequenceStatistics = 2,
            SequenceLengthDistribution = 3
        }

       
    }
}
