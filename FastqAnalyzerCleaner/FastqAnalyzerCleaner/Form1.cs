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

        public void SaveOutputs(object sender, ProgressChangedEventArgs e)
        {
            String output = null;
            SaveFile save = new SaveFile(output, "Save Fastq File", null);
            save.Save();
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
                //parseFq.initByteParseFastq();
                //fqFile = parseFq.parseByteFastq();
                //parseFq.checkFastqFile();

                if (parseFq.getFastqFileCheck())
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
                    Console.Write("Joint Test Results Completed on " + fqFile.getTotalNucleotides() + " Nucleotides \n");
                    Console.Write("Joint Test Results: " + fqFile.getGCount() + "G   " + Math.Round(fqFile.gContents(), 2) + "%   " + fqFile.getCCount() + "C " + Math.Round(fqFile.cContents(), 2) + " % \n");
                    Console.WriteLine("Misreads:  " + fqFile.getMisreadLocations().Count);
                    Console.WriteLine("Distribution:  " + fqFile.getDistribution().Count);

                    worker.ReportProgress(85, "[PERFORMING STATS]");
                    fqFile.performSequenceStatistics();
                    Console.WriteLine("Stats Performed");
                    
                    worker.ReportProgress(100, "[FILE LOADED]");
                }
                else
                {
                    //Disable gui 
                    //Warn its non fastqFile
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
                String taskName = "Create Fastq File";
                Console.WriteLine(taskName);
                TaskStrategy task = new TaskStrategy(this, fqFile, taskName);
                task.RunTask();
                fqFile = task.outputFqFile;
                String output = task.taskTextOutput; 
                SaveFile save = new SaveFile(output, "Save Fastq File", null);
                save.Save();
            }
        }

        private void saveFastaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fqFile != null)
            {
                String taskName = "Create Fasta File";
                Console.WriteLine(taskName);
                TaskStrategy task = new TaskStrategy(this, fqFile, taskName);
                task.RunTask();
                fqFile = task.outputFqFile;
                String output = task.taskTextOutput;
                SaveFile save = new SaveFile(output, "Save Fasta File", null);
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
