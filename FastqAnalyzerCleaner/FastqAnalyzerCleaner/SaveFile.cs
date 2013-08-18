/// <copyright file="SaveFile.cs" author="Neil Robertson">
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
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FastqAnalyzerCleaner
{
	///<summary>
	///Simple class allows the saving of simple string objects to be saved via a savefiledialogue COM object
	///</summary>
    class SaveFile
    {
        private String fileName, message, filter, saveAction;
        private FastqGUI observer;
        private IFqFile fqFile;
        private BackgroundWorker saveWorker;

        public static readonly string FASTQ_SAVE_ACTION = "Save Fastq";
        public static readonly string FASTA_SAVE_ACTION = "Save Fasta";
        public static readonly string CSV_SAVE_ACTION = "Save CSV";

		///<summary>
		///Default constructor for the SaveFile class, accepts string to save and parameters for the savefiledialogue window
		///</summary>
		///<param name="output">The string to be saved</param>
		///<param name="message">Message to be displayed on the save file dialogue window</param>
		///<param name="filter">String to filter access to file types in the savefiledialogue window</param>
        public SaveFile(IFqFile file, String message, FastqGUI o, String saveType, String filter = "Text File|*.txt|FastqFile|*.fq")
        {
            this.fqFile = file;
            this.message = message;
            this.filter = filter;
            this.observer = o;
            this.saveAction = saveType;
        }

		///<summary>
		///Method creates the savefiledialogue and saves string to disk
		///</summary>
        public void Save()
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = filter;
            save.Title = message;
            
            if (save.ShowDialog() == DialogResult.OK)
            {
                fileName = save.FileName;
                saveWorker = new BackgroundWorker();

                saveWorker.WorkerReportsProgress = true;
                saveWorker.WorkerSupportsCancellation = true;
                saveWorker.DoWork += new DoWorkEventHandler(saveWorker_DoWork);
                saveWorker.ProgressChanged += new ProgressChangedEventHandler(observer.loadWorker_ProgressChanged);

                if (saveWorker.IsBusy != true)
                {
                    saveWorker.RunWorkerAsync();
                }
            }
        }

        private void saveWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (saveAction == FASTQ_SAVE_ACTION)
            {
                SaveFastqAction(fqFile, fileName);
            }
            else if (saveAction == FASTA_SAVE_ACTION)
            {
                SaveFastqAction(fqFile, fileName);
            }
            else if (saveAction == CSV_SAVE_ACTION)
            {
                SaveCSVAction(fqFile, fileName);
            }
        }

        private void SaveFastqAction(IFqFile fq, String fileName)
        {
            StreamWriter writer;
            try
            {
                writer = new StreamWriter(@fileName);
                saveWorker.ReportProgress(40, "[CREATING FASTQ FORMAT]");
                for (int i = 0; i < fqFile.getFastqArraySize(); i++)
                {
                    writer.Write(fqFile.getFastqSequenceByPosition(i).createFastqBlock(fqFile.getMap()));
                }
                saveWorker.ReportProgress(100, "[FASTQ FORMAT CREATED]");
                writer.Flush();
                writer.Close();
            }
            catch (IOException exception)
            {
                Console.WriteLine(exception.ToString());
                UserResponse.ErrorResponse(exception.ToString());
            }
        }

        private void SaveFastaAction(IFqFile fq, String fileName)
        {
            saveWorker.ReportProgress(40, "[CREATING FASTA FORMAT]");
            String output = fqFile.createFastaFormat("");
            saveWorker.ReportProgress(100, "[FASTA FORMAT CREATED]");

            StreamWriter writer;
            try
            {
                writer = new StreamWriter(@fileName);

                writer.Write(output);

                writer.Flush();
                writer.Close();
            }
            catch (IOException exception)
            {
                Console.WriteLine(exception.ToString());
                UserResponse.ErrorResponse(exception.ToString());
            }
        }

        private void SaveCSVAction(IFqFile fq, String fileName)
        {
            StreamWriter writer;

            string COMMA_DELIMITER = ",";
            string[] output;

            try
            {
                writer = new StreamWriter(@fileName);
                saveWorker.ReportProgress(30, "[CREATING CSV FORMAT]");
                
                output = new string[] {"Sequence Index", "Header", "Total Nucleotides", "G Count", "C Count", "Misread Count", "Lower Threshold", "First Quartile",
                                        "Median", "Mean", "Third Quartile", "Upper Threshold" };
                writer.WriteLine(string.Join(COMMA_DELIMITER, output));

                for (int i = 0; i < fqFile.getFastqArraySize(); i++)
                {
                    FqSequence fqSeq = fqFile.getFastqSequenceByPosition(i);
                    output = new string[] { fqSeq.getSeqIndex().ToString(), fqSeq.getSequenceHeader(), fqSeq.getFastqSeqSize().ToString(), fqSeq.getGCount().ToString(), 
                                            fqSeq.getCCount().ToString(), fqSeq.getNCount().ToString(), fqSeq.getLowerThreshold().ToString(),
                                            fqSeq.getFirstQuartile().ToString(), fqSeq.getMedian().ToString(), fqSeq.getMean().ToString(),
                                            fqSeq.getThirdQuartile().ToString(), fqSeq.getUpperThreshold().ToString(), 
                                            fqSeq.createSequenceString(fqFile.getMap()) };
                    writer.WriteLine(string.Join(COMMA_DELIMITER, output));
                }
                saveWorker.ReportProgress(100, "[FASTQ FORMAT CREATED]");
                writer.Flush();
                writer.Close();
            }
            catch (IOException exception)
            {
                Console.WriteLine(exception.ToString());
                UserResponse.ErrorResponse(exception.ToString());
            }
        }

    }
}
