using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FastqAnalyzerCleaner
{
    class UserResponse
    {
        public static UserResponse uniqueInstance;
        public static Object syncLock = new object();

        public UserResponse()
        {

        }

        public static void ErrorResponse(String mainMessage, String headerMessage = "Program Error")
        {
            MessageBox.Show(mainMessage,
            headerMessage,
            MessageBoxButtons.OK,
            MessageBoxIcon.Error,
            MessageBoxDefaultButton.Button1);
        }

        public static void WarningResponse(String mainMessage, String headerMessage = "Program Warning")
        {
            MessageBox.Show(mainMessage,
            headerMessage,
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button1);
        }

        public static void InformationResponse(String mainMessage, String headerMessage = "For your information!")
        {
            MessageBox.Show(mainMessage,
            headerMessage,
            MessageBoxButtons.OK,
            MessageBoxIcon.Asterisk,
            MessageBoxDefaultButton.Button1);
        }

        public static UserResponse getInstance()
        {
            if (uniqueInstance == null)
            {
                lock (syncLock)
                {
                    uniqueInstance = new UserResponse();
                }
            }
            return uniqueInstance;
        }
    }
}
