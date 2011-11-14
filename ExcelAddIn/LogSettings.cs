/*
 * (c) 2011 ThoughtWorks, Inc.
 */

namespace ExcelAddIn
{
    /// <summary>
    /// Class to handle persistance of log file settings for the add-in
    /// </summary>
    public static class LogSettings
    {
        private static readonly Properties.Settings Settings = new Properties.Settings();

        /// <summary>
        /// Indicates whether trace logging is on or off
        /// </summary>
        public static bool Trace
        {
            get { return Settings.Trace; }
            set
            {
                Settings.Trace = value; 
                Settings.Save();
            }
        }

        /// <summary>
        /// The name of the log file
        /// </summary>
        /// <Remarks>
        /// The log is always located at %userprofile%\appdata\local\thoughtworks
        /// </Remarks>
        public static string TraceLog
        {
            get { return Settings.TraceLog; }
        }

        /// <summary>
        /// Indicates whether method entry/exit points are logged
        /// </summary>
        public static bool TraceEntryExit
        {
            get { return Settings.TraceEntryExit; }
            set
            {
                Settings.TraceEntryExit = value; 
                Settings.Save();
            }
        }

        /// <summary>
        /// Sets all Trace logging options at once
        /// </summary>
        /// <param name="traceOn"></param>
        /// <param name="logName"></param>
        /// <param name="entryExit"></param>
        public static void Set(bool traceOn, string logName, bool entryExit)
        {
            Settings["Trace"] = traceOn;
            Settings["TraceLog"] = logName;
            Settings["TraceEntryAndExit"] = entryExit;
            Settings.Save();
        }
    }
}
