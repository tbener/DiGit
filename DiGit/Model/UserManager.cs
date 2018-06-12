using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using DiGit.Helpers;
using DiGit.Properties;
using System.DirectoryServices.AccountManagement;
using SchedulerK;

namespace DiGit.Model
{
    public static class UserManager
    {
        public static void UpdateInfo(string additionalInfoKey, string additionalInfoValue)
        {
            try
            {
                UserPrincipal user = UserPrincipal.Current;
                string fileName = Path.Combine(Settings.Default.InfoUrl, $"{user.GivenName} {user.Surname}" + ".xml");

                // previous method named the file after Environment.UserName, which could hold a code number
                // instead of the real user name. If this file still exists, we'll save a new one.
                string oldFileName = Path.Combine(Settings.Default.InfoUrl, Environment.UserName + ".xml");
                bool needUpdate = !Settings.Default.UserRegistered || File.Exists(oldFileName);
                if (!needUpdate)
                {
                    Version configVersion;
                    bool firstRun = true;
                    if (Version.TryParse(ConfigurationHelper.Configuration.ver, out configVersion))
                        firstRun = configVersion != AppInfo.AppVersion;
                    needUpdate = firstRun;
                }
                if (needUpdate)
                {
                    var evt = SchedulerK.Scheduler.SharedInstance.GetNextEvent();
                    
                    XDocument xDoc = new XDocument();
                    xDoc.Add(new XElement("DiGit",
                        new XAttribute("appVersion", AppInfo.AppVersion.ToString()),
                        new XAttribute("versionInstalledDate", ConfigurationHelper.Configuration.versionUpdated.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("date", DateTime.Now.ToString(CultureInfo.InvariantCulture)),
                        new XAttribute("userName", Environment.UserName),
                        new XAttribute("userDisplayName", user.DisplayName),
                        new XAttribute("isBetaUser", ConfigurationHelper.Configuration.isBetaUser),
                        new XElement("AdditionalInfo",
                            new XAttribute("configFilePath", ConfigurationHelper.ConfigFile),
                            new XAttribute("appPath", AppInfo.AppPath),
                            new XAttribute(additionalInfoKey, additionalInfoValue)
                        )));
                    xDoc.Save(fileName);
                    Settings.Default.UserRegistered = false;
                    Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, false);
            }
        }

        class LogEntry
        {
            public string Key { get; set; }
            public string Log { get; set; }
            public string Status { get; set; }
            public string Message { get; set; }
            public int Counter { get; set; }
            public DateTime LastUpdate { get; set; }

            public override string ToString()
            {
                return string.Format("Log: [{4}] {0} - {1} (Counter={3}). Message: {2}", Log, Status, Message, Counter, LastUpdate);
            }
        }

        private static Dictionary<string, LogEntry> _logEntries = new Dictionary<string, LogEntry>();

        private static void UpdateLog(LogEntry logEntry)
        {
            i = 10;
            logEntry.Counter++;
            logEntry.LastUpdate = DateTime.Now;
            i++;
            _logEntries[logEntry.Key] = logEntry;
            i++;
            Debug.Print(logEntry.ToString());
        }

        static int i = 0;

        public static void AddLog(string action, string status, string message = "")
        {
            i = 0;
            try
            {
                string key = string.Format("{0}.{1}", action, status);
                i++;
                LogEntry log = _logEntries.ContainsKey(key) ? _logEntries[key] : new LogEntry()
                {
                    Key = key,
                    Log = action,
                    Status = status
                };
                i++;
                log.Message = message;
                i++;
                UpdateLog(log);
            }
            catch (Exception ex)
            {
                //
            }
        }

    }
}
