using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Custom.Helpers
{
    public static class LogHelper
    {
        /// <summary>
        /// This Method will generate a text file to serve as Audit Trail. Files are created daily.
        /// </summary>
        /// <param name="folderPathName">default value if empty = /App_Data/Sitefinity/Logs/</param>
        /// <param name="filename">filename of the log file</param>
        /// <param name="message">message to be captured in the log file</param>
        public static void CreateLog(string filename, string message, bool hasBreakLine = false, string folderPathName = "~/App_Data/Sitefinity/Logs/")
        {
            //Telerik.Sitefinity.Abstractions.Log.Write(courseItemMaster.GetValue("Title") + " is locked.");
            string filePath = "~/App_Data/Sitefinity/Logs/" + DateTime.Now.ToLocalTime().ToString("MMM_dd_yyyy") + "-" + filename;
            string path = System.Web.HttpContext.Current.Server.MapPath(filePath);
            StreamWriter sw = new StreamWriter(path, true);

            string timeStamp = DateTime.Now.ToShortDateString().ToString() + " " + DateTime.Now.ToLongTimeString().ToString() + ": ";
            sw.WriteLine(timeStamp + message);
            if (hasBreakLine)
                sw.WriteLine("");
            sw.Flush();
            sw.Close();
        }

        public static void CreateTrace(string message)
        {
            Telerik.Sitefinity.Abstractions.Log.Write(message);
        }
    }
}