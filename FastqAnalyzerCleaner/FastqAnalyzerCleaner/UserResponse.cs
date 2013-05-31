/// <copyright file="UserResponse.cs" author="Neil Robertson">
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
