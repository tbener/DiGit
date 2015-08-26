﻿using System;
using System.Windows.Input;
using DiGit.Model;
using DiGit.Properties;

namespace DiGit.Helpers
{
    public static class ConfigurationHelper
    {

        private static DiGitConfig _configRoot = null;

        public static string ConfigFile
        {
            get { return PathHelper.GetFullPath(Settings.Default.ConfigFilePath); }
            set { Settings.Default.ConfigFilePath = value; }
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

        public static bool Load(string file = "", bool showErr = true)
        {
            if (file == "") file = ConfigFile;
            try
            {
                _configRoot = SerializeHelper.Load(typeof(DiGitConfig), file) as DiGitConfig;
                Upgrade();
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
            if (_configRoot.Settings == null) 
                _configRoot.Settings = new DiGitConfigSettings();
            if (_configRoot.Settings.ShowHideHotkey == null)
                _configRoot.Settings.ShowHideHotkey = new DiGitConfigSettingsShowHideHotkey(ModifierKeys.Control | ModifierKeys.Shift, "G");
            else
                _configRoot.Settings.ShowHideHotkey.Apply();

            if (_configRoot.Settings.VisualSettings == null) 
                _configRoot.Settings.VisualSettings = new DiGitConfigSettingsVisualSettings();

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
            if (file == "") file = ConfigFile;
            if (_configRoot == null) _configRoot = new DiGitConfig();
            try
            {
                _configRoot.Repositories = RepositoriesManager.Repos.ToArray();
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