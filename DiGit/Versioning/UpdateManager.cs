using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DiGit.Commands;
using DiGit.Helpers;
using DiGit.Model;
using DiGit.Properties;

namespace DiGit.Versioning
{
    public delegate void UpdateInfoChangedEventHandler(object sender, EventArgs arg);
    public delegate void UpdateRequiredEventHandler(object sender, EventArgs arg);

    internal static class UpdateManager
    {
        private static Exception _lastReadError;
        private static string _localSetupFile;

        public static event UpdateInfoChangedEventHandler OnUpdateInfoChanged;
        public static event UpdateRequiredEventHandler OnUpdateRequired;
        public static DiGitVersionInfo VersionInfo { get; private set; }

        internal static void CheckUpdateAsync()
        {
            Task.Factory.StartNew(CheckUpdate);
        }

        internal static void CheckUpdate()
        {
            if (Working) return;
            Working = true;
            //UserManager.AddLog("Check Update", "Start");
            try
            {
                LastReadError = null;
                GetVersionInfo();
                //NotificationHelper.ShowNotification("DiGit is checking for updates...", $"Current version: {AppInfo.AppVersionString}.\nClick to open the Update window.", new ShowSingleViewCommand(typeof(UpdateView)));
                
                //UserManager.AddLog("Check Update", "Success");
            }
            catch (FileNotFoundException ex1)
            {
                LastReadError = new Exception("Version information not found.", ex1);
            }
            catch (Exception ex)
            {
                LastReadError = ex;
            }
            finally
            {
                if (LastReadError != null)
                {
                    ResetVars();
                    UserManager.AddLog("Check Update", "Error", LastReadError.Message);
                }
            }
        }

        private static void GetVersionInfo()
        {
            try
            {
                using (var client = new WebClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    client.DownloadStringCompleted += Client_DownloadStringCompleted;
                    client.DownloadStringAsync(new Uri(Settings.Default.VersionInfoUrl));
                }
            }
            catch (Exception ex)
            {
                LastReadError = ex;
                Working = false;
            }
        }

        private static void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(DiGitVersionInfo));
                if (e.Error != null)
                {
                    // log...
                    LastReadError = e.Error;
                    ResetVars();
                    return;
                }

                VersionInfo = (DiGitVersionInfo)serializer.Deserialize(new StringReader(e.Result));

                ReadVersionInfo();
            }
            catch (Exception ex)
            {
                LastReadError = ex;
            }
            finally
            {
                Working = false;
            }

            if (UpdateRequired)
            {
                DownloadSetupFile();
            }
        }

        private static void ReadVersionInfo()
        {
            LatestVersionInfo = VersionInfo.Version.FirstOrDefault(v => v.version.Equals(VersionInfo.Version.Max(v1 => v1.version)));
            LastReadDateTime = DateTime.Now;
            if (LatestVersionInfo != null)
                UpdateRequired = Version.Parse(LatestVersionInfo.version).IsHigherThan(AppInfo.AppVersion);
            else
                UpdateRequired = false;

            VersionInfoUpdated = true;
            OnUpdateInfoChanged?.Invoke(null, new EventArgs());
        }

        private static void DownloadSetupFile()
        {
            try
            {
                _localSetupFile = Path.Combine(PathHelper.GetFullPath(Path.GetTempPath(), "DiGit", true), $"DiGitSetup v{LatestVersionInfo.version}.msi");
                if (VerifyLocalSetupFile())
                    return;
                
                Working = true;
                using (var client = new WebClient())
                {
                    // start download
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    client.DownloadFileCompleted += Client_DownloadFileCompleted;
                    client.DownloadFileAsync(new Uri(VersionInfo.Setup.URI), _localSetupFile);
                }
            }
            catch (Exception ex)
            {
                LastReadError = ex;
                Working = false;
            }
        }

        private static void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    // log...
                    LastReadError = e.Error;
                    ResetVars();
                    return;
                }

                VerifyLocalSetupFile();
            }
            catch (Exception ex)
            {
                LastReadError = ex;
            }
            finally
            {
                Working = false;
            }
        }

        private static bool VerifyLocalSetupFile()
        {
            SetupFileFound = File.Exists(_localSetupFile) && new FileInfo(_localSetupFile).Length > 500;

            if (UpdateAvailable)
            {
                OnUpdateInfoChanged?.Invoke(null, new EventArgs());
                OnUpdateRequired?.Invoke(null, new EventArgs());
                ShowUpdateAvailableMessage();
            }

            return SetupFileFound;
        }

        


        private static void ResetVars()
        {
            VersionInfoUpdated = false;
            UpdateRequired = false;
            LatestVersionInfo = null;
            SetupFileFound = false;
            Working = false;
        }

        private static void ShowUpdateAvailableMessage()
        {
            NotificationHelper.ShowUpdateNotification(LatestVersionInfo.version);
        }

        public static List<DiGitVersionInfoVersion> GetGreaterOrEqualVersions()
        {
            Version versionToCompare = Version.Parse(AppInfo.AppVersion.ToString(3));
            var versions =
                VersionInfo.Version.Where(v => Version.Parse(v.version).IsHigherThan(versionToCompare)).ToList();

            if (!versions.Any())
            {
                // return a list with a single item - the same version
                versions = VersionInfo.Version.Where(v => Version.Parse(v.version).Equals(versionToCompare)).ToList();
            }

            return versions;
        }

        public static void RunUpdate()
        {
            try
            {
                if (!VerifyLocalSetupFile()) throw new Exception($"File '{_localSetupFile}' could not be verified");
                if (Msg.ShowQ("DiGit will be closed and restarted after setup is complete. Your settings will be saved.\nDo you want to continue?"))
                {
                    Process p = new Process();
                    p.StartInfo.FileName = _localSetupFile;
                    p.Start();
                    new ExitCommand().Execute(null);
                }
            }
            catch (Exception ex)
            {
                Msg.ShowE(ex.Message);
            }
        }

        /// <summary>
        /// UpdateAvailable = true means we're all set
        /// </summary>
        public static bool UpdateAvailable
        {
            get { return UpdateRequired && SetupFileFound; }
        }

        public static bool Working { get; private set; }

        public static bool VersionInfoUpdated { get; private set; }

        public static bool UpdateRequired { get; private set; }

        public static bool SetupFileFound { get; private set; }

        public static DiGitVersionInfoVersion LatestVersionInfo { get; private set; }

        public static Exception LastReadError
        {
            get { return _lastReadError; }
            private set
            {
                _lastReadError = value;
                if (value != null) ErrorHandler.Handle(value, false);
            }
        }

        public static DateTime LastReadDateTime { get; private set; }

    }
}
