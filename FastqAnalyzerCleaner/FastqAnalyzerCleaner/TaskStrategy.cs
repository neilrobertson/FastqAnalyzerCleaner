/// <copyright file="TaskStrategy.cs" author="Neil Robertson">
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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace FastqAnalyzerCleaner
{
    class TaskStrategy
    {
        private FastqGUI observer;
        private static BackgroundWorker taskWorker;
        private String taskName;
        private Stopwatch stopwatch;
        private ITaskStrategy task;
        private GenericFastqInputs genericInputs;

        public String taskTextOutput;
        public FqFile outputFqFile;

       
        public TaskStrategy(FastqGUI observer, GenericFastqInputs inputs)
        {
            this.observer = observer;
            this.taskName = inputs.TaskAction;
            this.genericInputs = inputs;

            taskWorker = new BackgroundWorker();

            taskWorker.WorkerReportsProgress = true;
            taskWorker.WorkerSupportsCancellation = true;
            taskWorker.DoWork += new DoWorkEventHandler(taskWorker_DoWork);
            taskWorker.ProgressChanged += new ProgressChangedEventHandler(observer.loadWorker_ProgressChanged);
            
            task = TaskDiscrimination.getTask(taskName);
            Console.WriteLine("New task begun: {0} On thread: {1} with state: {2}", task, taskWorker.ToString(), taskWorker.IsBusy);
        }

        public void RunTask()
        {
            if (taskWorker.IsBusy != true)
            {
                taskWorker.RunWorkerAsync(genericInputs);
            }
        }

        
        private void taskWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            GenericFastqInputs input = (GenericFastqInputs)e.Argument;

            if ((worker.CancellationPending == true))
            {
                e.Cancel = true;
            }
            else
            {
                stopwatch = new Stopwatch();
                stopwatch.Start();

                worker.ReportProgress(10, "[]");
                GenericFastqInputs taskOutputs = task.perform(input);
                worker.ReportProgress(100, "[COMPLETE]");
                outputFqFile = taskOutputs.FastqFile;
                taskTextOutput = taskOutputs.Output;
                Console.WriteLine(task.getStatement() + " Task - COMPLETED");

                observer.UpdateGUIThread(outputFqFile);
                updateGUI();
                stopwatch.Stop();
            }
        }

        private void updateGUI()
        {
            /**
            Application.Current.Dispatcher.Invoke(new SaveFile(() => {
                    // now you are on the UI thread
                    ;
                });
            */ 
            //observer.dosomething(stopwatch.Elapsed, file, outputFqFile) 
        }

        public class TaskDiscrimination
        {
            public static Dictionary<String, ITaskStrategy> storage = new Dictionary<String, ITaskStrategy>();
            public static HashSet<String> checkExists = new HashSet<String>();
            public static Boolean isSetUp = false;
            public static ITaskStrategy getTask(String taskName)
            {
                if (isSetUp == false)
                {
                    setUp();
                    isSetUp = true;
                }

                ITaskStrategy task = (ITaskStrategy)storage[taskName];
                if (task == null)
                    return DefaultTask.task as ITaskStrategy;
                return task;
            }

            public static void register(String key, ITaskStrategy value)
            {
                if (checkExists.Contains(key) == false)
                {
                    checkExists.Add(key);
                    storage.Add(key, value); 
                }
            }

            private static void setUp()
            {
                DefaultTask.register();
                SequenceStatisticsTask.register();
                EndCleanTask.register();
                StartCleanTask.register();
                TailCleanTask.register();
                ReanalyzeTask.register();
                RescanSequencerTask.register();
                CreateFastaTask.register();
                AdapterTask.register();
            }
        }

        public abstract class ITaskStrategy
        {
            abstract public GenericFastqInputs perform(GenericFastqInputs inputs);
            abstract public String getStatement();
        }

        public class DefaultTask : ITaskStrategy
        {
            public static String statement = "Default";
            public static ITaskStrategy task = new DefaultTask();
            public override GenericFastqInputs perform(GenericFastqInputs inputs)
            {
                //Show error message
                return inputs;
            }
            public override String getStatement()
            {
                return "Performing " + statement + " Task";
            }
            public static void register()
            {
                TaskDiscrimination.register(statement, task);
            }
        }

        public class SequenceStatisticsTask : ITaskStrategy
        {
            public static String statement = "Sequencer Statistics";
            public static ITaskStrategy task = new SequenceStatisticsTask();
            public override GenericFastqInputs perform(GenericFastqInputs inputs)
            {
                taskWorker.ReportProgress(80, "[PERFORMING SEQUENCE STATISTICS TASKS]");
                inputs.FastqFile.performSequenceStatistics();
                return inputs;
            }
            public override String getStatement() {
                return "Performing " + statement + " Task";
            }
            public static void register()
            {
                TaskDiscrimination.register(statement, task);
            }
        }

        private class EndCleanTask : ITaskStrategy
        {
            public static String statement = "Sequence End Cleaner";
            public static ITaskStrategy task = new EndCleanTask();
            public override GenericFastqInputs perform(GenericFastqInputs inputs)
            {
                taskWorker.ReportProgress(10, "[CLEANING SEQUENCE 5' ENDS]");
                inputs.FastqFile.cleanEnds(inputs.NucleotidesToClean);
                taskWorker.ReportProgress(40, "[PERFORMING JOINT TESTS]");
                inputs.FastqFile.Tests(); 
                return inputs;
            }
            public override String getStatement()
            {
                return "Performing " + statement + " Task";
            }
            public static void register()
            {
                TaskDiscrimination.register(statement, task);
            }
        }

        private class StartCleanTask : ITaskStrategy
        {
            public static String statement = "Sequence Start Cleaner";
            public static ITaskStrategy task = new StartCleanTask();
            public override GenericFastqInputs perform(GenericFastqInputs inputs)
            {
                taskWorker.ReportProgress(10, "[CLEANING SEQUENCE 3' ENDS]");
                inputs.FastqFile.cleanStarts(inputs.NucleotidesToClean);
                taskWorker.ReportProgress(40, "[PERFORMING JOINT TESTS]");
                inputs.FastqFile.Tests(); 
                return inputs;
            }
            public override String getStatement()
            {
                return "Performing " + statement + " Task";
            }
            public static void register()
            {
                TaskDiscrimination.register(statement, task);
            }
        }


        private class ReanalyzeTask : ITaskStrategy
        {
            public static String statement = "File Reanalysis";
            public static ITaskStrategy task = new ReanalyzeTask();
            public override GenericFastqInputs perform(GenericFastqInputs inputs)
            {
                taskWorker.ReportProgress(40, "[PERFORMING JOINT TESTS]");
                inputs.FastqFile.Tests(); 
                return inputs;
            }
            public override String getStatement()
            {
                return "Performing " + statement + " Task";
            }
            public static void register()
            {
                TaskDiscrimination.register(statement, task);
            }
        }

        private class RescanSequencerTask : ITaskStrategy
        {
            public static String statement = "Sequencer Type Rescan";
            public static ITaskStrategy task = new RescanSequencerTask();
            public override GenericFastqInputs perform(GenericFastqInputs inputs)
            {
                if (Preferences.getInstance().getSeqDecisionMethod())
                {
                    taskWorker.ReportProgress(27, "[DETERMINING SEQUENCER]");
                    SequencerDetermination seqDetermine = new SequencerDetermination(inputs.FastqFile);
                }
                else if (!Preferences.getInstance().getSeqDecisionMethod())
                {
                    taskWorker.ReportProgress(40, "[DETERMINING SEQUENCER - TREE]");
                    SequencerDecisionTree decTree = new SequencerDecisionTree(inputs.FastqFile);
                }
                return inputs;
            }
            public override String getStatement()
            {
                return "Performing " + statement + " Task";
            }
            public static void register()
            {
                TaskDiscrimination.register(statement, task);
            }
        }

        private class TailCleanTask : ITaskStrategy
        {
            public static String statement = "Sequence Tail Cleaner";
            public static ITaskStrategy task = new TailCleanTask();
            public override GenericFastqInputs perform(GenericFastqInputs inputs)
            {
                taskWorker.ReportProgress(10, "[CLEANING SEQUENCE TAILS]");
                inputs.FastqFile.cleanStarts(inputs.NucleotidesToClean);
                inputs.FastqFile.cleanEnds(inputs.NucleotidesToClean);
                taskWorker.ReportProgress(40, "[PERFORMING JOINT TESTS]");
                inputs.FastqFile.Tests(); 
                return inputs;
            }
            public override String getStatement()
            {
                return "Performing " + statement + " Task";
            }
            public static void register()
            {
                TaskDiscrimination.register(statement, task);
            }
        }

        public class CreateFastaTask : ITaskStrategy
        {
            public static String statement = "Create Fasta File";
            public static ITaskStrategy task = new DefaultTask();
            public override GenericFastqInputs perform(GenericFastqInputs inputs)
            {
                taskWorker.ReportProgress(40, "[CREATING FASTA FORMAT]");
                inputs.Output = inputs.FastqFile.createFastaFormat("");
                taskWorker.ReportProgress(100, "[FASTA FORMAT CREATED]");
                return inputs;
            }
            public override String getStatement()
            {
                return "Performing " + statement + " Task";
            }
            public static void register()
            {
                TaskDiscrimination.register(statement, task);
            }
        }

        private class AdapterTask : ITaskStrategy
        {
            public static String statement = "Clean Adapters Task";
            public static ITaskStrategy task = new AdapterTask();
            public override GenericFastqInputs perform(GenericFastqInputs inputs)
            {
                taskWorker.ReportProgress(10, "[CLEANING ADAPTERS]");
                inputs.FastqFile.cleanAdapters();
                taskWorker.ReportProgress(50, "[PERFORMING JOINT TESTS]");
                inputs.FastqFile.Tests();
                return inputs;
            }
            public override String getStatement()
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
