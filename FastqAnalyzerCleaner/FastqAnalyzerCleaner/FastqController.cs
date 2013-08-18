using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;

namespace FastqAnalyzerCleaner
{
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

        public FastqController()
        {
            CONTROLLER_STATE = FastqControllerState.STATE_READY;
            DirectoryController.getInstance().SetWorkingDirectory();
            FlushMemoryOfProtobinFiles();
        }

        public static FastqController getInstance()
        {
            if (uniqueInstance == null)
            {
                lock (acessSync)
                    uniqueInstance = new FastqController();
            }
            return uniqueInstance;
        }

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

                    observer.UpdateGUIThread(null);
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

        public void BuildFqFileMap(IFqFile component)
        {
            FqFile_Component_Details componentDetails = new FqFile_Component_Details();
            componentDetails.ContructComponentDetails(component);
            
            fqFileMap.GetFqFileComponentDetailsMap()[component.getFileName()] = componentDetails;
        }

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

        public void ControllerStateFailureResponse(String mainMessage, String headerMessage)
        {
            UserResponse.ErrorResponse(mainMessage, headerMessage);
            Console.WriteLine("Program Error. Exception Caught: {0}", mainMessage);
            FlushMemoryOfProtobinFiles();
            fqFileMap = null;
            CONTROLLER_STATE = FastqControllerState.STATE_READY;
        }

        public FqFileMap CreateNewFastqFile(String fileName, long fileLength)
        {
            fqFileMap = new FqFileMap();
            fqFileMap.FileName = fileName;
            fqFileMap.FileLength = fileLength;
            return fqFileMap;
        }

        public void addFqFileComponentDirectory(String fqComponentDirectory)
        {
            if (fqFileMap != null)
            {
                fqFileMap.getFileComponentDirectories().Add(fqComponentDirectory);
            }
        }

        public void PrimeForNewFile()
        {
            fqFileMap = null;
            FlushMemoryOfProtobinFiles();
        }

        public void SetObserver(FastqGUI observer)
        {
            this.observer = observer;
        }

        public FqFileMap GetFqFileMap()
        {
            return fqFileMap;
        }

        public void FlushMemoryOfProtobinFiles()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Environment.CurrentDirectory);
            FileInfo[] files = directoryInfo.GetFiles(ProtocolBuffersSerialization.PROTOBUF_FILE_WIDCARD)
                                            .Where(p => p.Extension == ProtocolBuffersSerialization.PROTOBUF_FILE_PREFIX).ToArray();
            foreach (FileInfo file in files)
            {
                try
                {
                    file.Attributes = FileAttributes.Normal;
                    Console.WriteLine("Deleting File: {0}", file.FullName);
                    File.Delete(file.FullName);
                }
                catch (IOException exception)
                {
                    Console.WriteLine("Fqprotobin file flush failed: {0}", exception.ToString());
                }
            }
        }

        public enum FastqControllerState
        {
            STATE_READY = 1,
            IN_PROGRESS = 2,
            ERROR = 3,
            PARSING = 4
        }

    }
}
