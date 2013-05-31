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
    public partial class Form1 : Form
    {
        private FqFile fqFile = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
                openFileDialog1.Filter = "All files (*.*)|*.*|Fastq files (*.fq)|*.fq";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    FileStream inStr = new FileStream(openFileDialog1.FileName, FileMode.Open);
                    InputFq input = new InputFq(inStr, openFileDialog1.FileName);
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
                SaveFile save = new SaveFile(fqFile, "Save Fastq File", this, "Save Fastq");
                save.Save();
            }
        }

        private void saveFastaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                SaveFile save = new SaveFile(fqFile, "Save Fasta File", this, "Save Fasta", "All files (*.*)|*.*|Fastq files (*.fq)|*.fq|Fastq files (*.fastq)|*.fastq");
                save.Save();
            }
        }

        private void clean3EndsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                String taskName = "Sequence Start Cleaner";
                Console.WriteLine(taskName);
                TaskStrategy task = new TaskStrategy(this, fqFile, taskName);
                task.RunTask();
                fqFile = task.outputFqFile;
            }
        }

        private void clean5EndsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                String taskName = "Sequence End Cleaner";
                Console.WriteLine(taskName);
                TaskStrategy task = new TaskStrategy(this, fqFile, taskName);
                task.RunTask();
                fqFile = task.outputFqFile;
            }
        }

        private void cleanTailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                String taskName = "Sequence Tail Cleaner";
                Console.WriteLine(taskName);
                TaskStrategy task = new TaskStrategy(this, fqFile, taskName);
                task.RunTask();
                fqFile = task.outputFqFile;
            }
        }

        private void rescanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                String taskName = "Sequencer Type Rescan";
                Console.WriteLine(taskName);
                TaskStrategy task = new TaskStrategy(this, fqFile, taskName);
                task.RunTask();
                fqFile = task.outputFqFile;
            }
        }

        private void createFastaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                String taskName = "Create Fasta File";
                Console.WriteLine(taskName);
                TaskStrategy task = new TaskStrategy(this, fqFile, taskName);
                task.RunTask();
                fqFile = task.outputFqFile;
            }
        }

        private void showSequenceStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                String taskName = "Sequencer Statistics";
                Console.WriteLine(taskName);
                TaskStrategy task = new TaskStrategy(this, fqFile, taskName);
                task.RunTask();
                fqFile = task.outputFqFile;
            }
        }

        private void reanalyzeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                String taskName = "File Reanalysis";
                Console.WriteLine(taskName);
                TaskStrategy task = new TaskStrategy(this, fqFile, taskName);
                task.RunTask();
                fqFile = task.outputFqFile;
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
             
    }
}
