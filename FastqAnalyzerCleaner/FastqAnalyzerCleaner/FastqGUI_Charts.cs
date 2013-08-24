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
    public class FastqGUI_Charts
    {
        private System.Drawing.Point CHART_LOCATION = new System.Drawing.Point(2, 33);
        private System.Drawing.Size CHART_SIZE = new System.Drawing.Size(901, 471);

        private String currentChartType = FastqChartTypes.Distribution.ToString();

        private FastqGUI observer;

        private FqFile_Component_Details componentDetails;

        private ZedGraphControl graphControl;

        public FastqGUI_Charts(FastqGUI observe)
        {
            this.observer = observe;
        }

        

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

        public void DrawSequenceStatistics()
        {
            /**
            Console.WriteLine("Drawing new sequence length distribution!");
            FqPerBaseSatistics[] perBaseStatistics = componentDetails.perBaseStatistics;
            //double[] x = new double[sequenceDistribution.Length];
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
            BarItem myBar1 = myPane.AddBar(FastqChartTypes.SequenceLengthDistribution.ToString(), spl1, Color.Blue);
            

            // I add all three functions just to be sure it refeshes the plot.
            graphControl.AxisChange();
            graphControl.Invalidate();
            graphControl.Refresh();
             * **/
        }

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
            LineItem myLine1 = myPane.AddCurve(FastqChartTypes.SequenceLengthDistribution.ToString(), spl1, Color.Red);
            myLine1.Line.Width = 3.0F;

            // I add all three functions just to be sure it refeshes the plot.
            graphControl.AxisChange();
            graphControl.Invalidate();
            graphControl.Refresh();
        }

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

            myPane.Title.Text = "Sequence Length Distributioin" + componentDetails.getGraphName();
 
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

        public void ClearCharts()
        {

        }

        /***********************************Chart Type Enum************************************/

        public enum FastqChartTypes
        {
            Distribution = 1,
            PerBaseSequenceStatistics = 2,
            SequenceLengthDistribution = 3
        }

       
    }
}
