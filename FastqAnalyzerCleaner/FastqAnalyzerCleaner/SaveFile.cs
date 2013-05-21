using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FastqAnalyzerCleaner
{
	///<summary>
	///Simple class allows the saving of simple string objects to be saved via a savefiledialogue COM object
	///</summary>
    class SaveFile
    {
        private String output, message, filter;

		///<summary>
		///Default constructor for the SaveFile class, accepts string to save and parameters for the savefiledialogue window
		///</summary>
		///<param name="output">The string to be saved</param>
		///<param name="message">Message to be displayed on the save file dialogue window</param>
		///<param name="filter">String to filter access to file types in the savefiledialogue window</param>
        public SaveFile(String output, String message, String filter = "Text File|*.txt|FastqFile|*.fq")
        {
            this.output = output;
            this.message = message;
            this.filter = filter;
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
                System.IO.FileStream fs = (System.IO.FileStream)save.OpenFile();

                using (StreamWriter sw = new StreamWriter(save.FileName))  
                {  
                   sw.Write(output);  
                   sw.Flush();  
                   sw.Close();  
                }  
            }
        }
    }
}
