using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace FastqAnalyzerCleaner
{
    public class FastqGUI_Display : RichTextBox
    {
        public static FastqGUI_Display uniqueInstance;
        public static object locker = new object();

        public FastqGUI_Display()
        {
            this.Enabled = false;
            this.BackColor = Color.Azure;
        }

        public static FastqGUI_Display getInstance()
        {
            lock(locker)
            {
                if (uniqueInstance == null)
                    uniqueInstance = new FastqGUI_Display();
            }
            return uniqueInstance;
        }

        public void DisplayFileDetails()
        {

        }

        public void DisplayFastqFileDetails()
        {

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
    }
}
