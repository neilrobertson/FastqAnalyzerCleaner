using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FastqAnalyzerCleaner
{
    class SaveFile
    {
        private String output, message, filter;

        public SaveFile(String output, String message, String filter = "Text File|*.txt|FastqFile|*.fq")
        {
            this.output = output;
            this.message = message;
            this.filter = filter;
        }

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
