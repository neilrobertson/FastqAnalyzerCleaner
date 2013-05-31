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
        private Form1 observer;
        private FqFile fqFile;
        private BackgroundWorker saveWorker;

		///<summary>
		///Default constructor for the SaveFile class, accepts string to save and parameters for the savefiledialogue window
		///</summary>
		///<param name="output">The string to be saved</param>
		///<param name="message">Message to be displayed on the save file dialogue window</param>
		///<param name="filter">String to filter access to file types in the savefiledialogue window</param>
        public SaveFile(FqFile file, String message, Form1 o, String saveType, String filter = "Text File|*.txt|FastqFile|*.fq")
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
            save.ShowDialog();

            if (save.FileName != "")
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
            if (saveAction == "Save Fastq")
            {
                SaveFastqAction(fqFile, fileName);
            }
            else if (saveAction == "Save Fasta")
            {
                SaveFastqAction(fqFile, fileName);
            }
        }

        private void SaveFastqAction(FqFile fq, String fileName)
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

        private void SaveFastaAction(FqFile fq, String fileName)
        {
            saveWorker.ReportProgress(40, "[CREATING FASTA FORMAT]");
            String output = fqFile.createFastaFormat("");
            saveWorker.ReportProgress(100, "[FASTA FORMAT CREATED]");

            StreamWriter writer;
            try
            {
                writer = new StreamWriter(@fileName);
                
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
