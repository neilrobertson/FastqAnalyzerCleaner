using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Windows;

namespace FastqAnalyzerCleaner
{
    public class FastqGUI_Display : RichTextBox
    {
        public FqFileMap fqMap;

        public FastqGUI_Display()
        {

        }

        public void Update(GenericFastqInputs input)
        {
            fqMap = FastqController.getInstance().GetFqFileMap();
            if (input.TaskAction == Task_FindSequences.statement)
                AddFoundSequenceData();
            else
            {
                DisplayFileDetails();
                DisplayFastqFileDetails();
            }
        }

        public void DisplayFileDetails()
        {
            this.AppendText("FASTQ File Sucessfully Loaded!\n\n");


            this.SelectionFont = new Font(this.Font.ToString(), 9, FontStyle.Bold);
            this.AppendText("[FILE DETAILS]\n\n");

            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Regular);
            this.AppendText("File Name: ");
            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Bold);
            this.AppendText(fqMap.FileName);

            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Regular);
            this.AppendText("\t\t\tFile Size: ");
            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Bold);
            this.AppendText(Math.Round(HelperMethods.ConvertBytesToMegabytes(fqMap.FileLength), 2).ToString()+"MB\n\n");

            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Regular);
            this.AppendText("Sequencer Type: ");
            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Bold);
            this.AppendText(fqMap.SequencerType);

            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Regular);
            this.AppendText("\t\t\tFile Format: ");
            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Bold);
            this.AppendText(fqMap.FastqFileFormatType + "\n\n");

            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Regular);
            this.AppendText("Last Action: ");
            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Bold);
            this.AppendText(fqMap.LastTask);

            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Regular);
            this.AppendText("\t\tTime Taken: ");
            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Bold);
            this.AppendText(fqMap.TimeTaken + "\n\n");

            this.AppendText("");
        }

        public void DisplayFastqFileDetails()
        {
            this.SelectionFont = new Font(this.Font.ToString(), 9, FontStyle.Bold);
            this.AppendText("[GLOBAL FASTQ CONTENTS]\n\n");

            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Regular);
            this.AppendText("Total Sequences: ");
            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Bold);
            this.AppendText(fqMap.GlobalDetails.TotalSequences.ToString());

            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Regular);
            this.AppendText("\t\t\tTotal Nucleotides: ");
            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Bold);
            this.AppendText(fqMap.GlobalDetails.TotalNucs.ToString() + "\n\n");

            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Regular);
            this.AppendText("Cytosine Count: ");
            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Bold);
            this.AppendText(fqMap.GlobalDetails.CCount.ToString());
            this.AppendText("\t" + Math.Round(fqMap.GlobalDetails.CPercent, 2).ToString() + "\n");

            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Regular);
            this.AppendText("Guanine Count: ");
            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Bold);
            this.AppendText(fqMap.GlobalDetails.GCount.ToString());
            this.AppendText("\t" + Math.Round(fqMap.GlobalDetails.GPercent, 2).ToString() + "\n");

            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Regular);
            this.AppendText("Failed Read Count: ");
            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Bold);
            this.AppendText(fqMap.GlobalDetails.NCount.ToString());
            this.AppendText("\t" + Math.Round(fqMap.GlobalDetails.NPercent, 4).ToString() + "\n\n");

            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Regular);
            this.AppendText("Smallest Sequence Length: ");
            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Bold);
            this.AppendText(fqMap.GlobalDetails.MinSeqSize.ToString());

            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Regular);
            this.AppendText("\t\t\tLargest Sequence Length: ");
            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Bold);
            this.AppendText(fqMap.GlobalDetails.MaxSeqSize.ToString() + "\n\n");

            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Regular);
            this.AppendText("Nucleotides Cleaned: ");
            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Bold);
            this.AppendText(fqMap.GlobalDetails.NucleotidesCleaned.ToString());

            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Regular);
            this.AppendText("\t\t\tSequences Removed: ");
            this.SelectionFont = new Font(this.Font.ToString(), 8, FontStyle.Bold);
            this.AppendText(fqMap.GlobalDetails.SequencesRemoved.ToString() + "\n\n");
        }

        public void DisplayNucleotideSequenceCount()
        {

        }

        public void DisplayNucleotideReads()
        {

        }

        public void DisplaySequenceSizeRange()
        {

        }

        public void DisplaySequenceStatistics()
        {

        }

        public void AddFoundSequenceData()
        {

        }
    }
}
