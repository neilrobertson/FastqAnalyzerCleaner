// <copyright file="FastqGUI_Display.cs" author="Neil Robertson">
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
using System.Windows.Forms;
using System.Drawing;
using System.Windows;
using System.IO;

namespace FastqAnalyzerCleaner
{
    /// <summary>
    /// This class composes and displays all texual output for the GUI
    /// </summary>
    public class FastqGUI_Display
    {
        private FqFileMap fqMap;
        private FastqGUI observer;
        private RichTextBox textBox;

        /// <summary>
        /// Default constructor for the class, accepts an instance of the main gui class as an observer.
        /// </summary>
        /// <param name="observe"></param>
        public FastqGUI_Display(FastqGUI observe)
        {
            this.observer = observe;
        }

        /// <summary>
        /// Key method in the display class, makes a decision on what to output based on the previous active task from the generic
        /// inputs transport class.  Also accepts instance of the richtextbox which will display the output.
        /// </summary>
        /// <param name="input">Instance of the transport class.  Used in this instance to inform the display class of previous activity</param>
        /// <param name="textBox">The rich text box that will display the outputs</param>
        public void Update(GenericFastqInputs input, RichTextBox textBox)
        {
            this.fqMap = FastqController.getInstance().GetFqFileMap();
            this.textBox = textBox;

            if (input.TaskAction == Task_FindSequences.statement)
                AddFoundSequenceData();
            else
            {
                ClearText();
                DisplayFileDetails();
                DisplayFastqFileDetails();
                DisplayComponentDetais();
            }
        }

        /// <summary>
        /// Clears all prior text from the text box.
        /// </summary>
        public void ClearText()
        {
            textBox.Clear();
        }

        /// <summary>
        /// Displays the higher order details about the file that is in the programs memory.
        /// </summary>
        public void DisplayFileDetails()
        {
            
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 9, FontStyle.Bold);
            textBox.AppendText("\n\t[FILE DETAILS]\n\n");

            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText("\tFile Name: ");
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Bold);
            textBox.AppendText(Path.GetFileName(fqMap.FileName));

            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText("\t\tFile Size: ");
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Bold);
            textBox.AppendText(Math.Round(HelperMethods.ConvertBytesToMegabytes(fqMap.FileLength), 2).ToString()+"MB\n\n");

            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText("\tSequencer Type: ");
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Bold);
            textBox.AppendText(fqMap.SequencerType);

            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText("\tFile Format: ");
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Bold);
            textBox.AppendText(fqMap.FastqFileFormatType + "\n\n");

            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText("\tLast Action: ");
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Bold);
            textBox.AppendText(fqMap.LastTask);

            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText("\t\tTime Taken: ");
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Bold);
            textBox.AppendText(fqMap.TimeTaken + "\n\n");
            
            textBox.AppendText("");
        }

        /// <summary>
        /// Displays details of the processed fastq file
        /// </summary>
        public void DisplayFastqFileDetails()
        {
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 9, FontStyle.Bold);
            textBox.AppendText("\n\t[GLOBAL FASTQ CONTENTS]\n\n");

            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText("\tTotal Sequences: ");
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText(fqMap.GlobalDetails.TotalSequences.ToString());

            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText("\t\tTotal Nucleotides: ");
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText(fqMap.GlobalDetails.TotalNucs.ToString() + "\n\n");

            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText("\tCytosine Count: ");
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText(fqMap.GlobalDetails.CCount.ToString());
            textBox.AppendText("\t" + Math.Round(fqMap.GlobalDetails.CPercent, 2).ToString() + "(%) \n");

            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText("\tGuanine Count: ");
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText(fqMap.GlobalDetails.GCount.ToString());
            textBox.AppendText("\t" + Math.Round(fqMap.GlobalDetails.GPercent, 2).ToString() + "(%) \n");

            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText("\tFailed Read Count: ");
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText(fqMap.GlobalDetails.NCount.ToString());
            textBox.AppendText("\t" + Math.Round(fqMap.GlobalDetails.NPercent, 4).ToString() + "(%) \n\n");

            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText("\tSmallest Sequence Length: ");
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText(fqMap.GlobalDetails.MinSeqSize.ToString());

            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText("\tLargest Sequence Length: ");
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText(fqMap.GlobalDetails.MaxSeqSize.ToString() + "\n\n");

            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText("\tNucleotides Cleaned: ");
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText(fqMap.GlobalDetails.NucleotidesCleaned.ToString());

            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText("\t\tSequences Removed: ");
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
            textBox.AppendText(fqMap.GlobalDetails.SequencesRemoved.ToString() + "\n\n");
        }

        /// <summary>
        /// Displays details of the components the file was broken into.
        /// </summary>
        public void DisplayComponentDetais()
        {
            foreach (String dir in fqMap.getFileComponentDirectories())
            {
                FqFile_Component_Details componentDetails = fqMap.GetFqFileComponentDetailsMap()[dir];

                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Bold);
                textBox.AppendText("\t[" + componentDetails.getGraphName() + " CONTENTS]\n\n");

                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText("\tTotal Sequences: ");
                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText(componentDetails.TotalSequences.ToString());

                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText("\t\tTotal Nucleotides: ");
                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText(componentDetails.TotalNucs.ToString() + "\n\n");

                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText("\tCytosine Count: ");
                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText(componentDetails.CCount.ToString());
                textBox.AppendText("\t" + Math.Round(componentDetails.CPercent, 2).ToString() + "(%) \n");

                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText("\tGuanine Count: ");
                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText(componentDetails.GCount.ToString());
                textBox.AppendText("\t" + Math.Round(componentDetails.GPercent, 2).ToString() + "(%) \n");

                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText("\tFailed Read Count: ");
                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText(componentDetails.NCount.ToString());
                textBox.AppendText("\t" + Math.Round(componentDetails.NPercent, 4).ToString() + "(%) \n\n");

                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText("\tSmallest Sequence Length: ");
                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText(componentDetails.MinSeqSize.ToString());

                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText("\tLargest Sequence Length: ");
                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText(componentDetails.MaxSeqSize.ToString() + "\n\n");

                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText("\tNucleotides Cleaned: ");
                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText(componentDetails.NucleotidesCleaned.ToString());

                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText("\t\tSequences Removed: ");
                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText(componentDetails.SequencesRemoved.ToString() + "\n\n");
            }
        }
        
        /// <summary>
        /// Displays detail about the sequences that were queried by the user.
        /// </summary>
        public void AddFoundSequenceData()
        {
            List<FqSequence> sequences = fqMap.GetQueriedSequences();
            textBox.SelectionFont = new Font(textBox.Font.ToString(), 9, FontStyle.Bold);
            textBox.AppendText("\t[QUERIED SEQUENCES]\n\n");

            foreach (FqSequence sequence in sequences)
            {
                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText("\t\tSequence: ");
                textBox.SelectionFont = new Font("Courier New", 8, FontStyle.Bold);
                textBox.AppendText(sequence.createSequenceString(fqMap.FqReadMap) + "\n");

                textBox.SelectionFont = new Font(textBox.Font.ToString(), 8, FontStyle.Regular);
                textBox.AppendText("\t\tMean: ");
                textBox.AppendText(Math.Round(sequence.getMean(), 2).ToString());
                textBox.AppendText("  NCount: ");
                textBox.AppendText(sequence.getNCount().ToString());
                textBox.AppendText("  Cytosine(%): ");
                textBox.AppendText(Math.Round((sequence.getCCount() / (double)sequence.getFastqSeqSize()), 2).ToString());
                textBox.AppendText("  Guanine(%): ");
                textBox.AppendText(Math.Round((sequence.getGCount() / (double)sequence.getFastqSeqSize()), 2).ToString());
                textBox.AppendText("  Removed Nucleotides: ");
                textBox.AppendText(sequence.getNucleotidesCleaned().ToString() + "\n\n");
            }
        }
    }
}
