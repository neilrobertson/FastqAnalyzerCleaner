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

        private delegate void UpdateFastqGUI(FqFile newFqFile);

        public void UpdateGUIThread(FqFile newFqFile)
        {
            if (this.InvokeRequired)
            {
                // Pass the same function to BeginInvoke,
                // but the call would come on the correct
                // thread and InvokeRequired will be false.
                this.BeginInvoke(new UpdateFastqGUI(UpdateGUIThread), new object[] { newFqFile });
                Console.WriteLine("Program updating GUI on the COM Thread");
                return;
            }

            UpdateGUI(newFqFile);
        }

        public void UpdateGUI(FqFile newFqFile)
        {
            fqFile = newFqFile;

            OutputFileDataToConsole();
        }

        private void openFastqFile(object sender, EventArgs e)
        {
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
                    fqFile = null;
                    GC.Collect();

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

            SequencerDetermination seqDetermine;
            SequencerDecisionTree decisionTree;

            FqFile new_fqFile;
            ParseFastq parseFq;
            
            if ((worker.CancellationPending == true))
            {
                e.Cancel = true;
            }
            else
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                worker.ReportProgress(3, "[PARSING FILE]");
                parseFq = new ParseFastq(inStr, fileName);

                if (parseFq.getFastqFileCheck() == true)
                {
                    new_fqFile = parseFq.parse();
                    //parseFq.initByteParseFastq();
                    //new_fqFile = parseFq.parseByteFastq();
                    if (new_fqFile != null)
                    {
                        worker.ReportProgress(37, "[DESERIALIZING FASTQ MAP]");
                        new_fqFile.setFqHashMap(HashFastq.deserializeHashmap());

                        worker.ReportProgress(40, "[DETERMINING SEQUENCER]");
                        if (Preferences.getInstance().getSeqDecisionMethod())
                            seqDetermine = new SequencerDetermination(new_fqFile);
                        else if (!Preferences.getInstance().getSeqDecisionMethod())
                            decisionTree = new SequencerDecisionTree(new_fqFile);

                        new_fqFile.calculateMapQualities();

                        worker.ReportProgress(65, "[PERFORMING TESTS]");
                        new_fqFile.Tests();
                        worker.ReportProgress(100, "[FILE LOADED]");

                        UpdateGUIThread(new_fqFile);
                        Console.WriteLine("File Load and Analysis Complete: {0}s", stopwatch.Elapsed);
                    }
                }
                else
                {
                    worker.ReportProgress(0, "");
                    UserResponse.InformationResponse("File does not conform to standard format.", "File Error");
                }
                stopwatch.Stop();
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
                SaveFile save = new SaveFile(fqFile, "Save Fastq File", this, SaveFile.FASTQ_SAVE_ACTION, FILE_DIALOGUE_FILTER);
                save.Save();
            }
        }

        private void saveFastaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                SaveFile save = new SaveFile(fqFile, "Save Fastq File", this, SaveFile.FASTA_SAVE_ACTION ,FILE_DIALOGUE_FILTER);
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
                        inputs.TaskAction = TaskStrategy.StartCleanTask.statement;
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
                        inputs.TaskAction = TaskStrategy.EndCleanTask.statement;
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
                        inputs.TaskAction = TaskStrategy.TailCleanTask.statement;
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
                inputs.TaskAction = TaskStrategy.RescanSequencerTask.statement;
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
                inputs.TaskAction = TaskStrategy.CreateFastaTask.statement;
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
                inputs.TaskAction = TaskStrategy.SequenceStatisticsTask.statement;
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
                inputs.TaskAction = TaskStrategy.ReanalyzeTask.statement;
                inputs.FastqFile = fqFile;
                Console.WriteLine(inputs.TaskAction);
                TaskStrategy task = new TaskStrategy(this, inputs);
                task.RunTask();
            }
        }

        private void removeAdapterSequencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                GenericFastqInputs inputs = new GenericFastqInputs();
                inputs.TaskAction = TaskStrategy.AdapterTask.statement;
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

        private void Charts_Combo_Selector_SelectedIndexChanged(object sender, EventArgs e)
        {
            FastqGUI_Charts.FastqChartTypes chartType;
            Enum.TryParse<FastqGUI_Charts.FastqChartTypes>(Charts_Combo_Selector.SelectedValue.ToString(), out chartType);
            FastqGUI_Charts.SelectChartType(chartType);
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

        private void OutputFileDataToConsole()
        {
            Console.WriteLine("Joint Test Results Completed on " + fqFile.getTotalNucleotides() + " Nucleotides");
            Console.WriteLine("Joint Test Results: " + fqFile.getGCount() + "G   " + Math.Round(fqFile.gContents(), 2) + "%   " + fqFile.getCCount() + "C " + Math.Round(fqFile.cContents(), 2) + " %");
            Console.WriteLine("Misreads:  " + fqFile.getNCount());
            Console.WriteLine("Distribution:  " + fqFile.getDistribution().Count);
            Console.WriteLine("Stats Performed");
            for (int i = 0; i < 20; i++)
            {
                FqSequence fqSeq = fqFile.getFastqSequenceByPosition(i);
                Console.WriteLine("--  Stats for Sequence " + (i + 1) + ": LB: {0}  1Q: {1}  median: {2} Mean: {3} 3Q: {4} UB: {5}", fqSeq.getLowerThreshold(), fqSeq.getFirstQuartile(), fqSeq.getMedian(), Math.Round(fqSeq.getMean(), 2), fqSeq.getThirdQuartile(), fqSeq.getUpperThreshold());
            }
            for (int i = 0; i < fqFile.getDistribution().Count; i++)
                Console.WriteLine("--->  Quality Score: {0}   Count: {1}", i, fqFile.getDistribution()[i]);
        }
       
    }
}
