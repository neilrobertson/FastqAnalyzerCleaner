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
        private Form1 observer;
        private static BackgroundWorker taskWorker;
        private FqFile inputFqFile;
        private String taskName;
        private Stopwatch stopwatch;
        private ITaskStrategy task;

        public String taskTextOutput { get; set; }
        public FqFile outputFqFile { get; set; }

       
        public TaskStrategy(Form1 observer, FqFile inputFile, String taskName)
        {
            this.observer = observer;
            this.inputFqFile = inputFile;
            this.taskName = taskName;

            taskWorker = new BackgroundWorker();

            taskWorker.WorkerReportsProgress = true;
            taskWorker.WorkerSupportsCancellation = true;
            taskWorker.DoWork += new DoWorkEventHandler(taskWorker_DoWork);
            taskWorker.ProgressChanged += new ProgressChangedEventHandler(observer.loadWorker_ProgressChanged);
            
            task = TaskDiscrimination.getTask(taskName);
        }

        public void RunTask()
        {
            if (taskWorker.IsBusy != true)
            {
                WorkerInput input = new WorkerInput(inputFqFile, taskName);
                taskWorker.RunWorkerAsync(input);
            }
        }

        
        private void taskWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            WorkerInput input = (WorkerInput)e.Argument;
            FqFile file = input.fqFile;
            String taskName = input.taskName;

            if ((worker.CancellationPending == true))
            {
                e.Cancel = true;
            }
            else
            {
                stopwatch = new Stopwatch();
                stopwatch.Start();

                TaskInputs taskInputs = new TaskInputs(file, null);
                worker.ReportProgress(10, "[]");
                TaskInputs taskOutputs = task.perform(taskInputs);
                worker.ReportProgress(100, "[COMPLETE]");
                outputFqFile = taskOutputs.fqFile;
                taskTextOutput = taskOutputs.output;
                Console.WriteLine(task.getStatement() + " Task - COMPLETED");

                //updateGUI();
                stopwatch.Stop();
            }
        }

        private void updateGUI(object sender, ProgressChangedEventArgs e)
        {
            /**
            Application.Current.Dispatcher.Invoke(new SaveFile(() => {
                    // now you are on the UI thread
                    ;
                });
            */
                 
            //observer.dosomething(stopwatch.Elapsed, file, outputFqFile) 
        }

        public class WorkerInput
        {
            public FqFile fqFile { get; set; }
            public String taskName { get; set; }
            public WorkerInput(FqFile fqFile, String fileName)
            {
                this.fqFile = fqFile;
                this.taskName = taskName;
            }
        }

        public class TaskDiscrimination
        {
            public static Dictionary<String, ITaskStrategy> storage = new Dictionary<String, ITaskStrategy>();
            public static HashSet<String> checkExists = new HashSet<String>();

            public static ITaskStrategy getTask(String taskName)
            {
                setUp();

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
            }
        }

        public abstract class ITaskStrategy
        {
            abstract public TaskInputs perform(TaskInputs inputs);
            abstract public String getStatement();
        }

        public class TaskInputs
        {
            public String output { get; set; }
            public FqFile fqFile { get; set; }
            public TaskInputs(FqFile fqFile, String output)
            {
                this.fqFile = fqFile;
                this.output = output;
            }
        }

        public class DefaultTask : ITaskStrategy
        {
            public static String statement = "Default";
            public static ITaskStrategy task = new DefaultTask();
            public override TaskInputs perform(TaskInputs inputs)
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
            public override TaskInputs perform(TaskInputs inputs)
            {
                taskWorker.ReportProgress(80, "[PERFORMING SEQUENCE STATISTICS TASKS]");
                inputs.fqFile.performSequenceStatistics();
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
            public override TaskInputs perform(TaskInputs inputs)
            {
                taskWorker.ReportProgress(10, "[CLEANING SEQUENCE 5' ENDS]");
                inputs.fqFile.cleanEnds(5);
                taskWorker.ReportProgress(40, "[PERFORMING JOINT TESTS]");
                inputs.fqFile.performJointTests();
                taskWorker.ReportProgress(80, "[PERFORMING SEQUENCE STATISTICS TASKS]");
                inputs.fqFile.performSequenceStatistics();
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
            public override TaskInputs perform(TaskInputs inputs)
            {
                taskWorker.ReportProgress(10, "[CLEANING SEQUENCE 3' ENDS]");
                inputs.fqFile.cleanStarts(5);
                taskWorker.ReportProgress(40, "[PERFORMING JOINT TESTS]");
                inputs.fqFile.performJointTests();
                taskWorker.ReportProgress(80, "[PERFORMING SEQUENCE STATISTICS TASKS]");
                inputs.fqFile.performSequenceStatistics();
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
            public override TaskInputs perform(TaskInputs inputs)
            {
                taskWorker.ReportProgress(40, "[PERFORMING JOINT TESTS]");
                inputs.fqFile.performJointTests();
                taskWorker.ReportProgress(80, "[PERFORMING SEQUENCE STATISTICS TASKS]");
                inputs.fqFile.performSequenceStatistics();
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
            public override TaskInputs perform(TaskInputs inputs)
            {
                if (Preferences.getInstance().getSeqDecisionMethod())
                {
                    taskWorker.ReportProgress(27, "[DETERMINING SEQUENCER]");
                    SequencerDetermination seqDetermine = new SequencerDetermination(inputs.fqFile);
                }
                else if (!Preferences.getInstance().getSeqDecisionMethod())
                {
                    taskWorker.ReportProgress(40, "[DETERMINING SEQUENCER - TREE]");
                    SequencerDecisionTree decTree = new SequencerDecisionTree(inputs.fqFile);
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
            public override TaskInputs perform(TaskInputs inputs)
            {
                taskWorker.ReportProgress(10, "[CLEANING SEQUENCE TAILS]");
                inputs.fqFile.cleanStarts(5);
                taskWorker.ReportProgress(40, "[PERFORMING JOINT TESTS]");
                inputs.fqFile.performJointTests();
                taskWorker.ReportProgress(80, "[PERFORMING SEQUENCE STATISTICS TASKS]");
                inputs.fqFile.performSequenceStatistics();
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
