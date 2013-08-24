// <copyright file="FastqController.cs" author="Neil Robertson">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;

namespace FastqAnalyzerCleaner
{
    /// <summary>
    /// This Singleton class is the main controller class for the program.  It contains methods that manage resource use and core methods,
    /// for initializing and launching tasks.
    /// </summary>
    public class FastqController
    {
        private static object acessSync = new object();
        private static FastqController uniqueInstance = null;

        private Queue<String> toDeserialize;
        private Queue<FqFile_Component> toPerform = new Queue<FqFile_Component>();
        private Queue<FqFile_Component> toSerialize = new Queue<FqFile_Component>();

        public FqFileMap fqFileMap;

        public static FastqControllerState CONTROLLER_STATE;

        private FastqGUI observer;

        private ProtocolBuffersSerialization protobufSerialization;
        private Stopwatch sw;

        /// <summary>
        /// Constructor for the fastqController sets the controller state to ready, calls the directory controller to change 
        /// the directory environment for the program and flushes all .fqprotobin files from memory that were in use in previous sessions.
        /// </summary>
        public FastqController()
        {
            CONTROLLER_STATE = FastqControllerState.STATE_READY;
            DirectoryController.getInstance().SetWorkingDirectory();
            FlushMemoryOfProtobinFiles();
        }

        /// <summary>
        /// Accessor for this singleton class, locked for syncronized use. 
        /// </summary>
        /// <returns></returns>
        public static FastqController getInstance()
        {
            lock (acessSync)
            {
                if (uniqueInstance == null)
                    uniqueInstance = new FastqController();
            }
            return uniqueInstance;
        }

        /// <summary>
        /// First point of call for the creation of a task within this class.  Checks controller state is ready, then builds 
        /// a background worker thread for tasks and launches thread.
        /// </summary>
        /// <param name="genericInputs">Input details that are necessary within the task, including potential file components</param>
        public void InitializeAction(GenericFastqInputs genericInputs)
        {
            if (fqFileMap != null && CONTROLLER_STATE == FastqControllerState.STATE_READY)
            {
                BackgroundWorker taskWorker = new BackgroundWorker();

                taskWorker.WorkerReportsProgress = true;
                taskWorker.WorkerSupportsCancellation = true;
                taskWorker.DoWork += new DoWorkEventHandler(RunTask);
                taskWorker.ProgressChanged += new ProgressChangedEventHandler(observer.loadWorker_ProgressChanged);
                taskWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(observer.loadWorker_Completed);

                if (taskWorker.IsBusy != true)
                {
                    taskWorker.RunWorkerAsync(genericInputs);
                }
            }
        }

        /// <summary>
        /// Entry point for the thread, allows controller to create unified method signature for the PerformAction method.
        /// Method checks threads state and hands of to PerformAction.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RunTask(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            GenericFastqInputs input = (GenericFastqInputs)e.Argument;

            if ((worker.CancellationPending == true))
            {
                e.Cancel = true;
            }
            else
            {
                PerformAction(worker, input);
            }
        }

        /// <summary>
        /// The key method in the controller class, where fastqFile_components are serialized and deserialized in and out of 
        /// the classes queues for processing.  The tasks are constructed through an interface - ITaskStrategy - which 
        /// is designated through the task abstract factory class.  After processing, files are deserialized and a details class
        /// for each component is populated, as are the global scores.
        /// </summary>
        /// <param name="worker">The backgroundworker thread</param>
        /// <param name="input">Generic inputs, including taskname and any further details necessary to complete a task such as nucleotide scores etc.</param>
        public void PerformAction(BackgroundWorker worker, GenericFastqInputs input)
        {
            BackgroundWorker loadWorker = worker;

            if (fqFileMap != null && (CONTROLLER_STATE == FastqControllerState.STATE_READY || CONTROLLER_STATE == FastqControllerState.PARSING))
            {
                sw = new Stopwatch();
                sw.Start();
                try
                {
                    toDeserialize = new Queue<String>(fqFileMap.getFileComponentDirectories());
                    PrimeFqFileComponentQueue();
                    protobufSerialization = new ProtocolBuffersSerialization();

                    ITaskStrategy task = TaskDiscrimination.getTask(input.TaskAction);
                    Console.WriteLine("Performing {0}", task.getStatement());

                    int count = 0;
                    while (toPerform.Count != 0)
                    {
                        Double progressPercent = (Double)(count / (Double)fqFileMap.getFileComponentDirectories().Count);
                        loadWorker.ReportProgress((int)(progressPercent * 100), task.getReportStatement());

                        FqFile_Component activeComponent = toPerform.Dequeue();
                        activeComponent.setFqHashMap(fqFileMap.FqReadMap);

                        if (toDeserialize.Count != 0)
                        {
                            int threadId;
                            ProtocolBuffersSerialization.ProbufDeserializeFqFile_AsyncMethodCaller caller
                                            = new ProtocolBuffersSerialization.ProbufDeserializeFqFile_AsyncMethodCaller(protobufSerialization.ProtobufDerializeFqFile);
                            String componentFileName = toDeserialize.Dequeue();
                            IAsyncResult result = caller.BeginInvoke(componentFileName, out threadId, null, null);

                            Console.WriteLine("\n*** Processing: {0} ***\n", activeComponent.getFileName());
                            input.FastqFile = activeComponent;
                            input = task.perform(input);
                            activeComponent = (FqFile_Component)input.FastqFile;
                            BuildFqFileMap(activeComponent);

                            FqFile_Component returnValue = caller.EndInvoke(out threadId, result);
                            toPerform.Enqueue(returnValue);
                        }
                        else if (toDeserialize.Count == 0)
                        {
                            Console.WriteLine("\n*** Processing: {0} ***\n", activeComponent.getFileName());
                            input.FastqFile = activeComponent;
                            input = task.perform(input);
                            activeComponent = (FqFile_Component)input.FastqFile;
                            BuildFqFileMap(activeComponent);
                        }
                        toSerialize.Enqueue(activeComponent);
                        SerializeFqComponentToMemory();
                        count++;
                    }

                    SerializeRemainingFqComponents();
                    task.confirmTaskEnd();
                    Console.WriteLine("\n*********\n");
                    fqFileMap.CalculateGlobalFileScores();
                    fqFileMap.GlobalDetails.OutputToConsole();
                    loadWorker.ReportProgress(100, task.getReportStatement());

                    sw.Stop();
                    Console.WriteLine("Task: {0} Completed in Time: {1}", task.getStatement(), sw.Elapsed);

                    fqFileMap.LastTask = input.TaskAction;
                    fqFileMap.TimeTaken = sw.Elapsed.ToString();
                    observer.UpdateGUIThread(input);
                }
                catch (IOException exception)
                {
                    ControllerStateFailureResponse(exception.ToString(), "Error");
                }
                catch (InsufficientMemoryException exception)
                {
                    ControllerStateFailureResponse(exception.ToString(), "Error");
                }
                catch (OutOfMemoryException exception)
                {
                    ControllerStateFailureResponse(exception.ToString(), "Error");
                }
                catch (ArithmeticException exception)
                {
                    ControllerStateFailureResponse(exception.ToString(), "Error");
                }
                sw.Stop();
            }
        }

        /// <summary>
        /// Creates a fqFile_component_details class from the core information that is best still stored within the programs memory.
        /// These classes populate a dictionary within the fqfilemap class where the component filename is their key.
        /// </summary>
        /// <param name="component">The component who's details are to be stored</param>
        public void BuildFqFileMap(IFqFile component)
        {
            FqFile_Component_Details componentDetails = new FqFile_Component_Details();
            componentDetails.ContructComponentDetails(component);
            
            fqFileMap.GetFqFileComponentDetailsMap()[component.getFileName()] = componentDetails;
        }

        /// <summary>
        /// Priming the component queue, used prior to the work of the tasks starting, this method ensures that there are 
        /// some components deserialized into memory for use.
        /// </summary>
        public void PrimeFqFileComponentQueue()
        {
            int threadId;
            ProtocolBuffersSerialization protoBuf = new ProtocolBuffersSerialization();
            String componentName = toDeserialize.Dequeue();
            FqFile_Component component = protoBuf.ProtobufDerializeFqFile(componentName, out threadId);
            toPerform.Enqueue(component);
            String secondComponentName = toDeserialize.Dequeue();
            FqFile_Component secondComponent = protoBuf.ProtobufDerializeFqFile(secondComponentName, out threadId);
            toPerform.Enqueue(secondComponent);
        }

        /// <summary>
        /// Serializes remaining file components onto disk after task has completed.
        /// </summary>
        public void SerializeRemainingFqComponents()
        {
            ProtocolBuffersSerialization protoBuf = new ProtocolBuffersSerialization();
            while (toSerialize.Count != 0)
            {
                int threadId;
                FqFile_Component fqFileComponent = toSerialize.Dequeue();
                Boolean result = protoBuf.ProtobufSerializeFqFile(fqFileComponent, fqFileComponent.getFileName(), out threadId);
            }
        }

        /// <summary>
        /// Method asynchronously serializes components to memory.
        /// </summary>
        private void SerializeFqComponentToMemory()
        {
            while (toSerialize.Count != 0)
            {
                int threadId;
                FqFile_Component component = toSerialize.Dequeue();
                ProtocolBuffersSerialization protoBuf = new ProtocolBuffersSerialization();
                ProtocolBuffersSerialization.ProbufSerializeFqFile_AsyncMethodCaller caller
                                    = new ProtocolBuffersSerialization.ProbufSerializeFqFile_AsyncMethodCaller(protoBuf.ProtobufSerializeFqFile);

                IAsyncResult result = caller.BeginInvoke(component, component.getFileName(), out threadId, null, null);

                Boolean returnValue = caller.EndInvoke(out threadId, result);
            }
        }

        /// <summary>
        /// Method to be used in the event of the failure of the PerformAction method experiencing some exception.
        /// Method warns user and flushes memory of fqprotobin files before returning program to initial state.
        /// </summary>
        /// <param name="mainMessage">Main message to send to user</param>
        /// <param name="headerMessage">Header message to send to user</param>
        public void ControllerStateFailureResponse(String mainMessage, String headerMessage)
        {
            UserResponse.ErrorResponse(mainMessage, headerMessage);
            Console.WriteLine("Program Error. Exception Caught: {0}", mainMessage);
            FlushMemoryOfProtobinFiles();
            fqFileMap = null;
            CONTROLLER_STATE = FastqControllerState.STATE_READY;
        }

        /// <summary>
        /// Creates the basis for loading a new fastq file into the program.
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="fileLength">Length of the file</param>
        /// <returns></returns>
        public FqFileMap CreateNewFastqFile(String fileName, long fileLength)
        {
            fqFileMap = new FqFileMap();
            fqFileMap.FileName = fileName;
            fqFileMap.FileLength = fileLength;
            return fqFileMap;
        }

        /// <summary>
        /// Adds a filename and directory of a fqprotobin file component to the fqfilemaps directory list
        /// </summary>
        /// <param name="fqComponentDirectory">The fqprotobin filename/directory</param>
        public void addFqFileComponentDirectory(String fqComponentDirectory)
        {
            if (fqFileMap != null)
            {
                fqFileMap.getFileComponentDirectories().Add(fqComponentDirectory);
            }
        }

        /// <summary>
        /// Resets the state of a controller for loading a new file
        /// </summary>
        public void PrimeForNewFile()
        {
            fqFileMap = null;
            FlushMemoryOfProtobinFiles();
        }

        /// <summary>
        /// Sets the observer to the fastqgui controller
        /// </summary>
        /// <param name="observer">The gui observer</param>
        public void SetObserver(FastqGUI observer)
        {
            this.observer = observer;
        }

        /// <summary>
        /// Accessor method to obtain the fqfilemap
        /// </summary>
        /// <returns>The active instance of the fqfilemap class</returns>
        public FqFileMap GetFqFileMap()
        {
            return fqFileMap;
        }

        /// <summary>
        /// Flushes the programs environment directory of fqprotobin files
        /// </summary>
        public void FlushMemoryOfProtobinFiles()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Environment.CurrentDirectory);
            FileInfo[] files = directoryInfo.GetFiles(ProtocolBuffersSerialization.PROTOBUF_FILE_WIDCARD)
                                            .Where(p => p.Extension == ProtocolBuffersSerialization.PROTOBUF_FILE_EXTENSION).ToArray();
            
            try
            {
                foreach (FileInfo file in files)
                {
                    file.Attributes = FileAttributes.Normal;
                    Console.WriteLine("Deleting File: {0}", file.FullName);
                    File.Delete(file.FullName);
                }
            }
            catch (IOException exception)
            {
                Console.WriteLine("Fqprotobin file flush failed: {0}", exception.ToString());
            }
        }

        /// <summary>
        /// Enum for the FastqController class to determine controller state.
        /// </summary>
        public enum FastqControllerState
        {
            STATE_READY = 1,
            IN_PROGRESS = 2,
            ERROR = 3,
            PARSING = 4
        }

    }
}
