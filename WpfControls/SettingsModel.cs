//
// Copyright © ThoughtWorks Studios 2011
//

using System.Security;
using Settings = WpfControls.Properties.Settings;

namespace WpfControls
{
    /// <summary>
    /// This class supports persistance of user and application settings. 
    /// </summary>
    public class SettingsModel 
    {
        private readonly Settings _settings = new Settings();

        #region Mingle Settings
        /// <summary>
        /// The host URL to use for Mingle
        /// </summary>
        public string MingleHost { get; set; }

        /// <summary>
        /// Mingle login name
        /// </summary>
        public string MingleLogin { get; set; }

        /// <summary>
        /// Password encoded as a SecureString
        /// </summary>
        public SecureString MinglePassword { get; set; }

        /// <summary>
        /// Mingle project id (not name)
        /// </summary>
        public string MingleProjectId { get; set; }

        /// <summary>
        /// Saves all settings to persistent storage
        /// </summary>
        public void Save()
        {
            _settings.Save();
        }

        /// <summary>
        /// reloads all settings from persistent storage
        /// </summary>
        public void Reload()
        {
            _settings.Reload();
        }

        /// <summary>
        /// Sets all Mingle settings at once
        /// </summary>
        /// <param name="host"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="projectId"></param>
        public void SetMingleSettings(string host, string login, SecureString password, string projectId)
        {
            MingleHost = host;
            MingleLogin = login;
            MinglePassword = password;
            MingleProjectId = projectId;
        }
        #endregion

        #region GO Settings
        /// <summary>
        /// GO host URL string
        /// </summary>
        public string GoHost { get; set; }

        /// <summary>
        /// GO login name
        /// </summary>
        public string GoLogin { get; set; }

        /// <summary>
        /// GO password encoded as a SecureString
        /// </summary>
        public SecureString GoPassword { get; set; }

        /// <summary>
        /// Sets all GO settings at once
        /// </summary>
        /// <param name="host"></param>
        /// <param name="login"></param>
        /// <param name="password"></param>
        public void SetGoSettings(string host, string login, SecureString password)
        {
            GoHost = host;
            GoLogin = login;
            GoPassword = password;
        }
        #endregion

        #region Trace Logging Settings
        /// <summary>
        /// Indicates whether trace logging is on or off
        /// </summary>
        public bool Trace { get; set; }

        /// <summary>
        /// The name of the log file
        /// </summary>
        /// <Remarks>
        /// The log is always located at %userprofile%\appdata\local\thoughtworks
        /// </Remarks>
        public string TraceLog { get; set; }

        /// <summary>
        /// Indicates whether method entry/exit points are logged
        /// </summary>
        public bool TraceEntryAndExit { get; set; }

        /// <summary>
        /// Sets all Trace logging options at once
        /// </summary>
        /// <param name="traceOn"></param>
        /// <param name="logName"></param>
        /// <param name="entryExit"></param>
        public void SetTraceSettings(bool traceOn, string logName, bool entryExit)
        {
            _settings["Trace"] = traceOn;
            _settings["TraceLog"] = logName;
            _settings["TraceEntryAndExit"] = entryExit;
        }
        #endregion
    }
}
