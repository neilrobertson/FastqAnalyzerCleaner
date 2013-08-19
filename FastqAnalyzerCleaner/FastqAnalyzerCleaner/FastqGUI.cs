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
using System.Threading;


namespace FastqAnalyzerCleaner 
{
    /// <summary>
    /// The main GUI class for the program
    /// </summary>
    public partial class FastqGUI : Form
    {
        private static FastqController fastqController;
        private const String FILE_DIALOGUE_FILTER = "All files (*.*)|*.*|Text Files (*.txt)|*.txt|Fastq files (*.fq)|*.fq|Fastq files (*.fastq)|*.fastq";
        private BackgroundWorker loadWorker;

        /// <summary>
        /// Constructor for the main GUI class. Initializes the components that form the GUI, obtains an instance of the controller
        /// and sets this class as its observer.
        /// </summary>
        public FastqGUI()
        {
            InitializeComponent();
            fastqController = FastqController.getInstance();
            fastqController.SetObserver(this);
        }

        /// <summary>
        /// Delegate method to update GUI from non COM thread.  Matches method signiture for UpdateGUIThread.
        /// </summary>
        /// <param name="newFqFile"></param>
        private delegate void UpdateFastqGUI(GenericFastqInputs input);

        /// <summary>
        /// Method updates the GUI when on this thread or calls an invoke upon it for instances when called from non 
        /// COM thread.  Ensures that alterations that occur within the GUI occur within its on thread model.
        /// </summary>
        /// <param name="newFqFile"></param>
        public void UpdateGUIThread(GenericFastqInputs input)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new UpdateFastqGUI(UpdateGUIThread), new object[] { input });
                return;
            }
            UpdateGUI(input);
        }

        /// <summary>
        /// Updates the GUI.  Calls the FastqGUI_Display and FastqGUI_Charts classes to update the information contained within them.
        /// </summary>
        /// <param name="newFqFile"></param>
        public void UpdateGUI(GenericFastqInputs input)
        {
            //fqFile = newFqFile;

            //FastqGUI_Output.getInstance().OutputFileDataToConsole(fqFile);
            Console.WriteLine("Total Memory Allocated: {0}", HelperMethods.ConvertBytesToMegabytes(GC.GetTotalMemory(false)));
            // UpdateDisplay 
            //FastqGUI_Charts.DrawCurrentChartSelection(fqFile);
        }

        /// <summary>
        /// Obtains the file details that are to be processed and initializes the backgroundworker that will process the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFastqFile(object sender, EventArgs e)
        {
            loadWorker = new BackgroundWorker();

            loadWorker.WorkerReportsProgress = true;
            loadWorker.WorkerSupportsCancellation = true;
            loadWorker.DoWork += new DoWorkEventHandler(loadWorker_DoWork);
            loadWorker.ProgressChanged += new ProgressChangedEventHandler(loadWorker_ProgressChanged);
            loadWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(loadWorker_Completed);
            
            if (loadWorker.IsBusy != true)
            {
                OpenFastqDialogue.Filter = FILE_DIALOGUE_FILTER;
                if (OpenFastqDialogue.ShowDialog() == DialogResult.OK)
                {
                    FastqController.getInstance().PrimeForNewFile();
                    GC.Collect();

                    FileStream inStr = new FileStream(OpenFastqDialogue.FileName, FileMode.Open);
                    InputFq input = new InputFq(inStr, OpenFastqDialogue.FileName);
                    loadWorker.RunWorkerAsync(input);
                }
            }
        }
        
        /// <summary>
        /// Parses file into component chunks through the parseFastq class before handing component details to FastqController
        /// for processing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            InputFq input = (InputFq)e.Argument;
            FileStream fileStream = input.fileStream;
            String fileName = input.fileName;

            ParseFastq parseFq;
            
            if ((worker.CancellationPending == true))
            {
                e.Cancel = true;
            }
            else
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    worker.ReportProgress(3, "[PARSING FILE]");
                    parseFq = new ParseFastq(fileStream, fileName);

                    if (parseFq.getFastqFileCheck() == true && FastqController.CONTROLLER_STATE == FastqController.FastqControllerState.STATE_READY)
                    {
                        //Create new fqFileMap in controller and prime state for load
                        FastqController.CONTROLLER_STATE = FastqController.FastqControllerState.PARSING;
                        FastqController.getInstance().CreateNewFastqFile(fileName, parseFq.GetFastqFileLength());

                        int fqFileComponentNumber = 1;
                        ProtocolBuffersSerialization protoBuf = new ProtocolBuffersSerialization();

                        // Uses IEnummerable yield return to parse components back to this class via this foreach loop
                        foreach (FqFile_Component fqFileComponent in parseFq.ParseComponents())
                        {
                            Double progressPercent = (Double)(((fqFileComponentNumber-1) * FqFileMap.FQ_BLOCK_LIMIT) / (Double)(parseFq.GetLineCount() / 4));
                            worker.ReportProgress((int)(progressPercent*100), ParseFastq.REPORT_STATEMENT);

                            if (fqFileComponent.getFastqArraySize() >= 1)
                            {
                                int threadId;
                                String componentFileName = FastqController.getInstance().GetFqFileMap().FileGUID + "_" + Path.GetFileNameWithoutExtension(fileName) + "_" + fqFileComponentNumber + ProtocolBuffersSerialization.PROTOBUF_FILE_PREFIX;
                                fqFileComponent.setFastqFileName(componentFileName);

                                ProtocolBuffersSerialization.ProbufSerializeFqFile_AsyncMethodCaller caller
                                    = new ProtocolBuffersSerialization.ProbufSerializeFqFile_AsyncMethodCaller(protoBuf.ProtobufSerializeFqFile);

                                IAsyncResult result = caller.BeginInvoke(fqFileComponent, componentFileName, out threadId, null, null);

                                Boolean returnValue = caller.EndInvoke(out threadId, result);

                                if (returnValue == false)
                                {
                                    UserResponse.ErrorResponse("File serialization methods failed, please check you have sufficient memory and restart the application", "File Error");
                                    Console.WriteLine("Serialization failed");
                                    loadWorker.CancelAsync();
                                }
                                else
                                {
                                    FastqController.getInstance().addFqFileComponentDirectory(componentFileName);
                                }
                                fqFileComponentNumber++;
                            }
                        }
                        parseFq.CloseReader();

                        FastqController.getInstance().GetFqFileMap().InitializeReadMap();

                        GenericFastqInputs inputs = new GenericFastqInputs();
                        inputs.TaskAction = Task_LoadTask.statement;
                        FastqController.getInstance().PerformAction(loadWorker, inputs);
                    }
                    else
                    {
                        worker.ReportProgress(0, "");
                        UserResponse.InformationResponse("File does not conform to standard format.", "File Error");
                        parseFq.CloseReader();
                    }
                }
                catch (IOException exception)
                {
                    Console.Write(exception.StackTrace);
                    UserResponse.ErrorResponse(exception.ToString());
                }
               
                FastqController.CONTROLLER_STATE = FastqController.FastqControllerState.STATE_READY;
                stopwatch.Stop();
            }
        }

        /// <summary>
        /// Reports progress state to the GUI progress bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void loadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = (e.ProgressPercentage);
            toolStripStatusLabel1.Text = (String) e.UserState;
        }

        /// <summary>
        /// Method allows the cancellation of backgroundworker thread for loading and processing files.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void loadWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                UserResponse.ErrorResponse(e.Error.Message, "Load Error");
            }
            if (e.Cancelled)
            {
                UserResponse.InformationResponse("Loading cancelled at request");
            }
            FastqController.CONTROLLER_STATE = FastqController.FastqControllerState.STATE_READY;
            Console.WriteLine("Process ended.");
        }

        /// <summary>
        /// Internal class for passing both filename and fileStream to worker thread within the confines of the required method 
        /// signature
        /// </summary>
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

        /// <summary>
        /// Responds to button click on the Save Fastq Menu Item, opens dialogue to select filename and then passes details to the 
        /// controller class for processing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveFastqToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FastqController.getInstance().fqFileMap != null && FastqController.CONTROLLER_STATE == FastqController.FastqControllerState.STATE_READY)
            {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = FILE_DIALOGUE_FILTER;
                save.Title = "Save Fastq File";

                if (save.ShowDialog() == DialogResult.OK)
                {
                    String fileName = save.FileName;
                    GenericFastqInputs inputs = new GenericFastqInputs();
                    inputs.SaveFileName = fileName;
                    inputs.InitializeStreamWriter(fileName);
                    inputs.TaskAction = Task_SaveFastq.statement;
                    FastqController.getInstance().InitializeAction(inputs);
                }
            }
        }

        /// <summary>
        /// Responds to button click on the Save CSV Menu Item, opens dialogue to select filename and then passes details to the 
        /// controller class for processing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveCSVDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FastqController.getInstance().fqFileMap != null && FastqController.CONTROLLER_STATE == FastqController.FastqControllerState.STATE_READY)
            {
                SaveFileDialog save = new SaveFileDialog();
                save.Filter = "CSV File|*.csv|Text File|*.txt";
                save.Title = "Save CSV File";

                if (save.ShowDialog() == DialogResult.OK)
                {
                    String fileName = save.FileName;
                    GenericFastqInputs inputs = new GenericFastqInputs();
                    inputs.SaveFileName = fileName;
                    inputs.TaskAction = Task_SaveCSV.statement;
                    FastqController.getInstance().InitializeAction(inputs);
                }
            }
        }  
        
        /// <summary>
        /// Method responds to button clicks on the clean 3 ends tool strip menu item.  It obtains int for nucleotides to clean 
        /// from sequences and hands to controller for processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clean3EndsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FastqController.getInstance().fqFileMap != null && FastqController.CONTROLLER_STATE == FastqController.FastqControllerState.STATE_READY)
            {
                InputBoxResult result = InputBox.Show("Enter a number of nucleotides to clean from sequence 3' starts:", "Clean Sequence Starts", "", new InputBoxValidatingHandler(inputBox_Validating));
                if (result.OK)
                {
                    if (HelperMethods.safeParseInt(result.Text.Trim()) == true)
                    {
                        GenericFastqInputs inputs = new GenericFastqInputs();
                        inputs.NucleotidesToClean = Int32.Parse(result.Text.Trim());
                        inputs.TaskAction = Task_EndCleanTask.statement;
                        FastqController.getInstance().InitializeAction(inputs);
                    }
                }
            }
        }

        /// <summary>
        /// Method responds to button clicks on the clean 5 ends tool strip menu item.  It obtains int for nucleotides to clean 
        /// from sequences and hands to controller for processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clean5EndsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FastqController.getInstance().fqFileMap != null && FastqController.CONTROLLER_STATE == FastqController.FastqControllerState.STATE_READY)
            {
                InputBoxResult result = InputBox.Show("Enter a number of nucleotides to clean from sequence 5' ends:", "Clean Sequence Ends", "", new InputBoxValidatingHandler(inputBox_Validating));
                if (result.OK)
                {
                    if (HelperMethods.safeParseInt(result.Text.Trim()) == true)
                    {
                        GenericFastqInputs inputs = new GenericFastqInputs();
                        inputs.NucleotidesToClean = Int32.Parse(result.Text.Trim());
                        inputs.TaskAction = Task_StartCleanTask.statement;
                        FastqController.getInstance().InitializeAction(inputs);
                    }
                }
            }
        }

        /// <summary>
        /// Method responds to button clicks on the tail clean tool strip menu item.  It obtains int for nucleotides to clean 
        /// from sequences and hands to controller for processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cleanTailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FastqController.getInstance().fqFileMap != null && FastqController.CONTROLLER_STATE == FastqController.FastqControllerState.STATE_READY)
            {
                InputBoxResult result = InputBox.Show("Enter a number of nucleotides to clean from tails:", "Clean Sequence Tails", "", new InputBoxValidatingHandler(inputBox_Validating));
                if (result.OK)
                {
                    if (HelperMethods.safeParseInt(result.Text.Trim()) == true)
                    {
                        GenericFastqInputs inputs = new GenericFastqInputs();
                        inputs.NucleotidesToClean = Int32.Parse(result.Text.Trim());
                        inputs.TaskAction = Task_TailCleanTask.statement;
                        FastqController.getInstance().InitializeAction(inputs);
                    }
                }
            }
        }

        /// <summary>
        /// Method responds to button clicks on the rescan tool strip menu iteam and then hands details to controller for processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rescanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FastqController.getInstance().fqFileMap != null && FastqController.CONTROLLER_STATE == FastqController.FastqControllerState.STATE_READY)
            {
                GenericFastqInputs inputs = new GenericFastqInputs();
                inputs.TaskAction = Task_RescanSequencerTask.statement;
                FastqController.getInstance().InitializeAction(inputs);
            }
        }

        /// <summary>
        /// Method responds to button clicks on the show sequence statistics tool strip menu iteam and then hands details to controller for processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showSequenceStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FastqController.getInstance().fqFileMap != null && FastqController.CONTROLLER_STATE == FastqController.FastqControllerState.STATE_READY)
            {
                GenericFastqInputs inputs = new GenericFastqInputs();
                inputs.TaskAction = Task_SequenceStatisticsTask.statement;
                FastqController.getInstance().InitializeAction(inputs);
            }
        }

        /// <summary>
        /// Method responds to button clicks on the reanalyze tool strip menu iteam and then hands details to controller for processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reanalyzeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FastqController.getInstance().fqFileMap != null && FastqController.CONTROLLER_STATE == FastqController.FastqControllerState.STATE_READY)
            {
                GenericFastqInputs inputs = new GenericFastqInputs();
                inputs.TaskAction = Task_ReanalyzeTask.statement;
                FastqController.getInstance().InitializeAction(inputs);
            }
        }

        /// <summary>
        /// Method responds to button clicks on the remove adapters tool strip menu iteam and then hands details to controller for processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeAdapterSequencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FastqController.getInstance().fqFileMap != null && FastqController.CONTROLLER_STATE == FastqController.FastqControllerState.STATE_READY)
            {
                GenericFastqInputs inputs = new GenericFastqInputs();
                inputs.TaskAction = Task_AdapterTask.statement;
                FastqController.getInstance().InitializeAction(inputs);
            }
        }

        /// <summary>
        /// Method responds to button clicks on the find sequences tool strip menu iteam and then hands details to controller for processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void findSequenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FastqController.getInstance().fqFileMap != null && FastqController.CONTROLLER_STATE == FastqController.FastqControllerState.STATE_READY)
            {
                InputBoxResult result = InputBox.Show("Enter a sequence you wish to find:", "Find Sequence", "", new InputBoxValidatingHandler(inputBox_SequenceValidating));
                if (result.OK)
                {
                    GenericFastqInputs inputs = new GenericFastqInputs();
                    inputs.NucleotideSequence = result.Text.Trim();
                    inputs.TaskAction = Task_FindSequences.statement;
                    FastqController.getInstance().GetFqFileMap().InitializeNewSequenceSearchList();
                    FastqController.getInstance().InitializeAction(inputs);
                }
            }
        }

        /// <summary>
        /// Method responds to button clicks on the "remove sequences > with failed reads" menu item.  It hands the task to the controller for
        /// processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void withFailedReadsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FastqController.getInstance().fqFileMap != null && FastqController.CONTROLLER_STATE == FastqController.FastqControllerState.STATE_READY)
            {
                GenericFastqInputs inputs = new GenericFastqInputs();
                inputs.TaskAction = Task_RemoveMisSeqeuence.statement;
                FastqController.getInstance().InitializeAction(inputs);
            }
        }

        /// <summary>
        /// Method corresponds to the "remove sequences > below mean threshold" button click.  It accepts a value for the mean threshold and then
        /// passes to the controller for processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void belowMeanThresholdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FastqController.getInstance().fqFileMap != null && FastqController.CONTROLLER_STATE == FastqController.FastqControllerState.STATE_READY)
            {
                InputBoxResult result = InputBox.Show("Enter the Phred Mean Threshold For Sequences:", "Remove Sequences", "", new InputBoxValidatingHandler(inputBox_MeanThresholdValidating));
                if (result.OK)
                {
                    if (HelperMethods.safeParseInt(result.Text.Trim()) == true)
                    {
                        GenericFastqInputs inputs = new GenericFastqInputs();
                        inputs.MeanThreshold = Int32.Parse(result.Text.Trim());
                        inputs.TaskAction = Task_RemoveBelowMeanThreshold.statement;
                        FastqController.getInstance().InitializeAction(inputs);
                    }
                }
            }
        }

        /// <summary>
        /// Changes the preferences item for the Clean Sweep sequencer determination method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clean_Sweep_Radio_CheckedChanged(object sender, EventArgs e)
        {
            Preferences.getInstance().setSeqDecisionMethod(true);
        }

        /// <summary>
        /// Changes the preferences item for the Decision Tree sequencer determination method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Decision_Tree_Radio_CheckedChanged(object sender, EventArgs e)
        {
            Preferences.getInstance().setSeqDecisionMethod(false);
        }

        /// <summary>
        /// Changes the preferences item for the Single Core processing method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Single_Core_Radio_CheckedChanged(object sender, EventArgs e)
        {
            Preferences.getInstance().setMultiCoreProcessing(false);
        }

        /// <summary>
        /// Changes the preferences item for the Single Core processing method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Multi_Core_Radio_CheckedChanged(object sender, EventArgs e)
        {
            Preferences.getInstance().setMultiCoreProcessing(true);
        }

        /// <summary>
        /// Changes the chart type via this combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Charts_Combo_Selector_SelectedIndexChanged(object sender, EventArgs e)
        {
            FastqGUI_Charts.FastqChartTypes chartType;
            Enum.TryParse<FastqGUI_Charts.FastqChartTypes>(Charts_Combo_Selector.SelectedValue.ToString(), out chartType);
            FastqGUI_Charts.SelectChartType(chartType);
            
        }

        /// <summary>
        /// Changes the preferences via the preferences Gui
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changePreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PreferencesGUI preferencesGUI = new PreferencesGUI();

        }

        /// <summary>
        /// Flushes memory of fqprotobin files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void flushMemoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FastqController.getInstance().FlushMemoryOfProtobinFiles();
        }

        /// <summary>
        /// Exits program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Application.AllowQuit == true)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Validation method for the input box for an integer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            else
            {
                int i = Int32.Parse(e.Text.Trim());
                if (i > FastqController.getInstance().GetFqFileMap().GlobalDetails.MaxSeqSize || i < 1)
                {
                    e.Cancel = true;
                    e.Message = "Required";
                }
            }
        }

        /// <summary>
        /// Validation method for input box for sequence data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inputBox_SequenceValidating(object sender, InputBoxValidatingArgs e)
        {
            if (e.Text.Trim().Length == 0)
            {
                e.Cancel = true;
                e.Message = "Required";
            }
            else if (Nucleotide.isDNA(e.Text.Trim()) == false)
            {
                e.Cancel = true;
                e.Message = "Required";
            }
        }

        /// <summary>
        /// Validation method for input box for mean threshold
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inputBox_MeanThresholdValidating(object sender, InputBoxValidatingArgs e)
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
            else
            {
                int i = Int32.Parse(e.Text.Trim());
                if (i > 40 || i < 1)
                {
                    e.Cancel = true;
                    e.Message = "Required";
                }
            }
        }
       
    }
}
