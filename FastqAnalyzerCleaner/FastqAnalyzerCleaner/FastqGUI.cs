// <copyright file="FastqGUI.cs" author="Neil Robertson">
// Copyright (c) 2013 All Right Reserved, Neil Alistair Robertson - neil.alistair.robertson@hotmail.co.uk
//
// This code is the property of Neil Robertson.  Permission must be sought before reuse.
// It has been written explicitly for the MRes Bioinfomatics course at the University 
// of Glasgow, Scotland under the supervision of Derek Gatherer.
//
// </copyright>
// <author>Neil Robertson</author>
// <email>neil.alistair.robertson@hotmail.co.uk</email>
// <date>2013-06-1</date>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;


namespace FastqAnalyzerCleaner 
{
    public partial class FastqGUI : Form
    {
        public static FqFile fqFile = null;
        private const String FILE_DIALOGUE_FILTER = "All files (*.*)|*.*|Text Files (*.txt)|*.txt|Fastq files (*.fq)|*.fq|Fastq files (*.fastq)|*.fastq";

        public FastqGUI()
        {
            InitializeComponent();
        }

        

        public void setFastqFile(FqFile processedFastqFile)
        {
            fqFile = processedFastqFile;
        }

        private void openFastqFile(object sender, EventArgs e)
        {
            fqFile = null;
            GC.Collect();
            BackgroundWorker loadWorker = new BackgroundWorker();

            loadWorker.WorkerReportsProgress = true;
            loadWorker.WorkerSupportsCancellation = true;
            loadWorker.DoWork += new DoWorkEventHandler(loadWorker_DoWork);
            loadWorker.ProgressChanged += new ProgressChangedEventHandler(loadWorker_ProgressChanged);
            if (loadWorker.IsBusy != true)
            {
                OpenFastqDialogue.Filter = FILE_DIALOGUE_FILTER;
                if (OpenFastqDialogue.ShowDialog() == DialogResult.OK)
                {
                    FileStream inStr = new FileStream(OpenFastqDialogue.FileName, FileMode.Open);
                    InputFq input = new InputFq(inStr, OpenFastqDialogue.FileName);
                    loadWorker.RunWorkerAsync(input);
                }
            }
        }
        
        private void loadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            InputFq input = (InputFq)e.Argument;
            FileStream inStr = input.fileStream;
            String fileName = input.fileName;
            
            if ((worker.CancellationPending == true))
            {
                e.Cancel = true;
            }
            else
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                worker.ReportProgress(3, "[PARSING FILE]");
                ParseFastq parseFq = new ParseFastq(inStr, fileName);

                if (parseFq.getFastqFileCheck() == true)
                {
                    fqFile = parseFq.parse();

                    if (Preferences.getInstance().getSeqDecisionMethod())
                    {
                        worker.ReportProgress(27, "[DETERMINING SEQUENCER]");
                        SequencerDetermination seqDetermine = new SequencerDetermination(fqFile);
                    }
                    else if (!Preferences.getInstance().getSeqDecisionMethod())
                    {
                        worker.ReportProgress(40, "[DETERMINING SEQUENCER - TREE]");
                        SequencerDecisionTree decTree = new SequencerDecisionTree(fqFile);
                    }
                    worker.ReportProgress(35, "[DESERIALIZING FASTQ MAP]");
                    fqFile.calculateMapQualities();

                    worker.ReportProgress(65, "[PERFORMING JOINT TESTS]");
                    fqFile.performJointTests();
                    Console.WriteLine("Joint Test Results Completed on " + fqFile.getTotalNucleotides() + " Nucleotides");
                    Console.WriteLine("Joint Test Results: " + fqFile.getGCount() + "G   " + Math.Round(fqFile.gContents(), 2) + "%   " + fqFile.getCCount() + "C " + Math.Round(fqFile.cContents(), 2) + " %");
                    Console.WriteLine("Misreads:  " + fqFile.getMisreadLocations().Count);
                    Console.WriteLine("Distribution:  " + fqFile.getDistribution().Count);

                    worker.ReportProgress(85, "[PERFORMING STATS]");
                    fqFile.performSequenceStatistics();
                    for (int i = 0; i < 20; i++)
                    {
                        FqSequence fqSeq = fqFile.getFastqSequenceByPosition(i);
                        Console.WriteLine("Stats for Sequence "+(i+1)+": LB: {0}  1Q: {1}  median: {2} Mean: {3} 3Q: {4} UB: {5}", fqSeq.getLowerThreshold(), fqSeq.getFirstQuartile(), fqSeq.getMedian(), Math.Round(fqSeq.getMean(),2) , fqSeq.getThirdQuartile(), fqSeq.getUpperThreshold());
                    }
                    Console.WriteLine("Stats Performed");
                    //Enable GUI
                    worker.ReportProgress(100, "[FILE LOADED]");
                }
                else
                {
                    //Disable gui 
                    UserResponse.InformationResponse("File does not conform to standard format.", "File Error");
                }
                stopwatch.Stop();
                Console.Write("Done: " + stopwatch.Elapsed + "\n");
            }
        }

        public void loadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = (e.ProgressPercentage);
            toolStripStatusLabel1.Text = (String) e.UserState;
        }

        public class InputFq
        {
            public FileStream fileStream { get; set; }
            public String fileName { get; set; }
            public InputFq(FileStream fs, String fileName)
            {
                this.fileName = fileName;
                this.fileStream = fs;
            }
        }

        private void saveFastqToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                SaveFile save = new SaveFile(fqFile, "Save Fastq File", this, "Save Fastq", FILE_DIALOGUE_FILTER);
                save.Save();
            }
        }

        private void saveFastaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                SaveFile save = new SaveFile(fqFile, "Save Fasta File", this, FILE_DIALOGUE_FILTER);
                save.Save();
            }
        }

        private void clean3EndsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                InputBoxResult result = InputBox.Show("Enter a number of nucleotides to clean from sequence 3' starts:", "Clean Sequence Starts", "", new InputBoxValidatingHandler(inputBox_Validating));
                if (result.OK)
                {
                    if (HelperMethods.safeParseInt(result.Text.Trim()) == true)
                    {
                        GenericFastqInputs inputs = new GenericFastqInputs();
                        inputs.NucleotidesToClean = Int32.Parse(result.Text.Trim());
                        inputs.TaskAction = "Sequence Start Cleaner";
                        inputs.FastqFile = fqFile;
                        Console.WriteLine(inputs.TaskAction);
                        TaskStrategy task = new TaskStrategy(this, inputs);
                        task.RunTask();
                    }
                }
            }
        }

        private void clean5EndsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                InputBoxResult result = InputBox.Show("Enter a number of nucleotides to clean from sequence 5' ends:", "Clean Sequence Ends", "", new InputBoxValidatingHandler(inputBox_Validating));
                if (result.OK)
                {
                    if (HelperMethods.safeParseInt(result.Text.Trim()) == true)
                    {
                        GenericFastqInputs inputs = new GenericFastqInputs();
                        inputs.NucleotidesToClean = Int32.Parse(result.Text.Trim());
                        inputs.TaskAction = "Sequence End Cleaner";
                        inputs.FastqFile = fqFile;
                        Console.WriteLine(inputs.TaskAction);
                        TaskStrategy task = new TaskStrategy(this, inputs);
                        task.RunTask();
                    }
                }
            }
        }

        private void cleanTailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                InputBoxResult result = InputBox.Show("Enter a number of nucleotides to clean from tails:", "Clean Sequence Tails", "", new InputBoxValidatingHandler(inputBox_Validating));
                if (result.OK)
                {
                    if (HelperMethods.safeParseInt(result.Text.Trim()) == true)
                    {
                        GenericFastqInputs inputs = new GenericFastqInputs();
                        inputs.NucleotidesToClean = Int32.Parse(result.Text.Trim());
                        inputs.TaskAction = "Sequence Tail Cleaner";
                        inputs.FastqFile = fqFile;
                        Console.WriteLine(inputs.TaskAction);
                        TaskStrategy task = new TaskStrategy(this, inputs);
                        task.RunTask();
                    }
                }
            }
        }

        private void rescanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                GenericFastqInputs inputs = new GenericFastqInputs();
                inputs.TaskAction = "Sequencer Type Rescan";
                inputs.FastqFile = fqFile;
                Console.WriteLine(inputs.TaskAction);
                TaskStrategy task = new TaskStrategy(this, inputs);
                task.RunTask();
            }
        }

        private void createFastaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                GenericFastqInputs inputs = new GenericFastqInputs();
                inputs.TaskAction = "Create Fasta File";
                inputs.FastqFile = fqFile;
                Console.WriteLine(inputs.TaskAction);
                TaskStrategy task = new TaskStrategy(this, inputs);
                task.RunTask();
            }
        }

        private void showSequenceStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                GenericFastqInputs inputs = new GenericFastqInputs();
                inputs.TaskAction = "Sequencer Statistics";
                inputs.FastqFile = fqFile;
                Console.WriteLine(inputs.TaskAction);
                TaskStrategy task = new TaskStrategy(this, inputs);
                task.RunTask();
            }
        }

        private void reanalyzeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                GenericFastqInputs inputs = new GenericFastqInputs();
                inputs.TaskAction = "File Reanalysis";
                inputs.FastqFile = fqFile;
                Console.WriteLine(inputs.TaskAction);
                TaskStrategy task = new TaskStrategy(this, inputs);
                task.RunTask();
            }
        }

        private void Clean_Sweep_Radio_CheckedChanged(object sender, EventArgs e)
        {
            Preferences.getInstance().setSeqDecisionMethod(true);
        }

        private void Decision_Tree_Radio_CheckedChanged(object sender, EventArgs e)
        {
            Preferences.getInstance().setSeqDecisionMethod(false);
        }

        private void Single_Core_Radio_CheckedChanged(object sender, EventArgs e)
        {
            Preferences.getInstance().setMultiCoreProcessing(false);
        }

        private void Multi_Core_Radio_CheckedChanged(object sender, EventArgs e)
        {
            Preferences.getInstance().setMultiCoreProcessing(true);
        }

        private void inputBox_Validating(object sender, InputBoxValidatingArgs e)
        {
            if (e.Text.Trim().Length == 0)
            {
                e.Cancel = true;
                e.Message = "Required";
            }
            else if (HelperMethods.safeParseInt(e.Text.Trim()) == false)
            {
                e.Cancel = true;
                e.Message = "Required";
            }
        }

        private void Charts_Combo_Selector_SelectedIndexChanged(object sender, EventArgs e)
        {
            FastqGUI_Charts.FastqChartTypes chartType;
            Enum.TryParse<FastqGUI_Charts.FastqChartTypes>(Charts_Combo_Selector.SelectedValue.ToString(), out chartType);
            FastqGUI_Charts.SelectChartType(chartType);
        }

        

             
    }
}
