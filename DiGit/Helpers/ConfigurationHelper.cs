using System;
using System.Windows.Input;
using DiGit.Commands;
using DiGit.Configuration;
using DiGit.Model;
using DiGit.Properties;
using DiGit.View;
using DiGit.ViewModel;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace DiGit.Helpers
{

    public delegate void ConfigurationLoadedEventHandler(object sender, EventArgs arg);

    public static class ConfigurationHelper
    {
        // My Documents\DiGit\DiGit.xml
        private static readonly string DEFAULT_FILE = PathHelper.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.Personal), @"DiGit\DiGit.xml");

        private static DiGitConfig _configRoot = null;

        public static event ConfigurationLoadedEventHandler OnConfigurationLoaded;

        public static string ConfigFile
        {
            get { return PathHelper.GetFullPath(Settings.Default.ConfigFilePath); }
            set
            {
                Settings.Default.ConfigFilePath = value;
                Settings.Default.Save();
            }
        }

        public static DiGitConfig Configuration
        {
            get
            {
                if (_configRoot == null)
                    Load("", false);

                return _configRoot ?? new DiGitConfig();
            }
        }

        public static void LoadOrCreate()
        {
            // try loading the last saved file
            if (TryLoad())
                return;

            // try loading the default file
            ConfigFile = DEFAULT_FILE;
            if (TryLoad())
                return;

            // create new file and save as default file
            CreateDefault();
            Save();
        }

        public static void CreateDefault()
        {
            _configRoot = new DiGitConfig();
            Upgrade();
        }

        private static bool TryLoad()
        {
            if (!string.IsNullOrEmpty(ConfigFile) && File.Exists(ConfigFile))
            {
                return Load(ConfigFile, false);
            }
            return false;
        }

        public static bool Load(string file = "", bool showErr = true)
        {
            if (file == "")
                file = ConfigFile;
            try
            {
                _configRoot = SerializeHelper.Load(typeof(DiGitConfig), file) as DiGitConfig;
                Upgrade();
                OnConfigurationLoaded?.Invoke(null, new EventArgs());
                return true;
            }
            catch (Exception ex)
            {
                if (showErr)
                    ErrorHandler.Handle(ex, "Error while loading configuration file '{0}'", file);
                return false;
            }
        }

        // Fill in missing entries from older versions
        private static void Upgrade()
        {
            bool isUpgrade = false;
            bool isNew = _configRoot.ver == null;

            Version prevVer;
            if (!isNew && Version.TryParse(_configRoot.ver, out prevVer))
            {
                if (AppInfo.AppVersion.Build > Version.Parse(_configRoot.ver).Build)
                {
                    isUpgrade = true;
                    new ShowSingleViewCommand(typeof(TipsView)).Execute(null);
                }
            }

            if (isNew || isUpgrade || !_configRoot.versionUpdatedSpecified)
            {
                _configRoot.versionUpdated = DateTime.Now;
                _configRoot.versionUpdatedSpecified = true;
            }

            _configRoot.ver = AppInfo.AppVersion.ToString();

            if (_configRoot.Settings == null)
                _configRoot.Settings = new DiGitConfigSettings();
            if (_configRoot.Settings.ShowHideHotkey == null)
                _configRoot.Settings.ShowHideHotkey = new DiGitConfigSettingsShowHideHotkey(ModifierKeys.Control | ModifierKeys.Shift, "G");
            else
                _configRoot.Settings.ShowHideHotkey.Apply();

            if (_configRoot.Commands == null)
            {
                _configRoot.Commands = new[]
                {
                    GetUserCommand("Switch", "switch"),
                    GetUserCommand("Push", "push"),
                    GetUserCommand("Pull", "pull"),
                    GetUserCommand("Commit", "commit"),
                    GetUserCommand("Merge", "merge"),
                    GetUserCommand("Show log", "log"),
                    GetUserCommand("Sync", "sync")
                };
            }

            if (isNew || isUpgrade)
            {
                // v1.0.218
                // add gated push

                // Add Gated Checkin if not exists
                if (!_configRoot.Commands.Any(cmd => cmd.header.StartsWith("Gated", StringComparison.CurrentCultureIgnoreCase)))
                {
                    List<DiGitConfigCommand> listCommands = new List<DiGitConfigCommand>(_configRoot.Commands);
                    listCommands.Insert(2, new DiGitConfigCommand()
                    {
                        header = "Gated Push",
                        arguments = "{rep_path}",
                        fileName = @"C:\Program Files\Gated Updater\Gated Updater.exe"
                    });
                    _configRoot.Commands = listCommands.ToArray();
                }

                // v1.0.220
                // this is where we first add ID to command
                // in addtion, the gated push command path has changed

                // if there is no command with id (if at least one with ID we assume it's been handled)
                if (!_configRoot.Commands.Any(cmd => cmd.id != null))
                {
                    SetDiGitID("Switch");
                    SetDiGitID("Push");
                    SetDiGitID("Pull");
                    SetDiGitID("Commit");
                    SetDiGitID("Merge");
                    SetDiGitID("Show log", "showlog");
                    SetDiGitID("Sync");

                    DiGitConfigCommand cmdGated = SetDiGitID("Gated");
                    if (cmdGated != null)
                        cmdGated.fileName = @"C:\Program Files\Gated Updater\Gated Updater.exe";
                }
            }


            if (_configRoot.Folders == null)
            {
                _configRoot.Folders = new[]
                {
                    new DiGitConfigFolder() {path=@"dbm_System\Updates\Resources"},
                };
            }

            if (_configRoot.Settings.Bubbles == null)
                _configRoot.Settings.Bubbles = new DiGitConfigSettingsBubbles();

            // Settings.VisualSettings is going obsolete as of ver > 1.0.209
            if (_configRoot.Settings.VisualSettings != null)
            {
                _configRoot.Settings.Bubbles.Opacity = _configRoot.Settings.VisualSettings._bubblesOpacity;
                _configRoot.Settings.VisualSettings = null;
            }
        }

        private static DiGitConfigCommand SetDiGitID(string headerKey, string id = "")
        {
            if (id == "") id = headerKey.ToLower();
            DiGitConfigCommand cmd = _configRoot.Commands.First(c => c.header.StartsWith(headerKey));
            if (cmd != null) cmd.id = string.Format(Settings.Default.DiGitCommandID_Template, id);

            return cmd;
        }

        private static DiGitConfigCommand GetUserCommand(string header, string cmd)
        {
            return new DiGitConfigCommand()
            {
                header = header,
                arguments = string.Format("/command:{0} /path:{1}", cmd, "{rep_path}"),
                fileName = @"C:\Program Files\TortoiseGit\bin\TortoiseGitProc.exe"
            };
        }


        public static bool Save(string file = "")
        {
            if (file == "")
                file = ConfigFile;
            if (!Directory.Exists(Path.GetDirectoryName(file)))
                Directory.CreateDirectory(Path.GetDirectoryName(file));
            if (_configRoot == null)
            {
                CreateDefault();
            }
            try
            {
                _configRoot.RepositoryList.ForEach(r => r.UpdateLocation());
                _configRoot.Repositories = _configRoot.RepositoryList.ToArray();
                _configRoot.Folders = FolderViewModel.FolderList.ToArray();
                _configRoot.lastUpdated = DateTime.Now;
                _configRoot.lastUpdatedSpecified = true;
                return SerializeHelper.Save(_configRoot, file);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Error while saving configuration file '{0}'", file);
                return false;
            }
        }
    }
}
