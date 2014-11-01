using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace angjwcf.Service.Entity
{
    public struct angjwcfConstants
    {
        public const string HtmlFolder = "Content\\Html";

        public static string DefaultLogFileName { get { return (Utility.getAbsolutePath("Log", "Error" + DateTimeStampAsString + ".log", true)); } }
        public static string DefaultLogFolder { get { return (Utility.getAbsolutePath("Log", true)); } }
        public static string DefaultDataPath { get { return (Utility.getAbsolutePath("Data", true)); } }
        public static string HtmlFullPath { get { return (Utility.getAbsolutePath(HtmlFolder, true)); } }
        public static string DefaultConfigPath { get { return (Utility.getAbsolutePath("Config", "angjwcf.cfg", true)); } }
        public static string DateTimeStampAsString { get { return (DateTime.Now.ToString("ddMMMyyyy_hhmm")); } }
    }
}
