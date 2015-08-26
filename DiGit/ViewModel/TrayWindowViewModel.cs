﻿using System;
using System.Windows;
using System.Windows.Input;
using DiGit.Commands;
using DiGit.Helpers;
using DiGit.View;
using DiGit.Updates;
using Hardcodet.Wpf.TaskbarNotification;

namespace DiGit.ViewModel
{


    class TrayWindowViewModel : BaseViewModel
    {

        public ICommand ExitCommand { get; set; }
        public ICommand ShowCommand { get; set; }
        public ICommand HideCommand { get; set; }
        public ICommand LeftClickCommand { get; set; }
        public ICommand DoubleClickCommand { get; set; }
        public ICommand AddRepoCommand { get; set; }
        public ICommand ShowAboutCommand { get; set; }
        public ICommand ShowSettingsCommand { get; set; }
        public ICommand CheckUpdateCommand { get; set; }
        public ICommand ShowUpdateCommand { get; set; }

        private TaskbarIcon _taskbarIcon;

        public TrayWindowViewModel(TaskbarIcon taskbarIcon)
        {
            _taskbarIcon = taskbarIcon;

            ExitCommand = new ExitCommand();
            ShowCommand = new ShowAllCommand();
            HideCommand = new HideAllCommand();
            AddRepoCommand = new AddRepositoryCommand();

            ShowAboutCommand = new ShowSingleViewCommand(typeof(AboutView));

            LeftClickCommand = new ToggleShowHideCommand();
            DoubleClickCommand = LeftClickCommand;

            ShowSettingsCommand = new ShowSingleViewCommand(typeof(SettingsView));
#if DEBUG
            ShowSettingsVisiblilty = "Visible";
#else
            ShowSettingsVisiblilty = "Collapsed";
#endif

            ShowUpdateCommand = new ShowSingleViewCommand(typeof(UpdateView));
            CheckUpdateCommand = ShowUpdateCommand;

            HotkeyHelper.OnHotkeyChanged += (sender, args) => OnPropertyChanged("ShowHideHeader");
            // todo: currently the only balloon is for update, so no distinction about what to do.
            _taskbarIcon.TrayBalloonTipClicked += (sender, args) => ShowUpdateCommand.Execute(null);
            UpdateManager.OnUpdateRequired += UpdateManager_OnUpdateRequired;
            UpdateManager.OnUpdateInfoChanged += delegate(object sender, EventArgs args)
            {
                OnPropertyChanged("CheckUpdateHeader");
                OnPropertyChanged("ToolTipText");
            };

        }

        public string ShowSettingsVisiblilty { get; set; }
        
        private void UpdateManager_OnUpdateRequired(object sender, System.EventArgs arg)
        {
            _taskbarIcon.ShowBalloonTip("New DiGit version found", "Click for details", BalloonIcon.Info);
        }


        public bool StartWithWindows
        {
            get { return ConfigurationHelper.Configuration.StartWithWindows; }
            set
            {
                ConfigurationHelper.Configuration.StartWithWindows = value;
                OnPropertyChanged("StartWithWindows");
            }
        }

        public string ShowHideHeader
        {
            get{ return string.Format("Show/hide ({0})", HotkeyHelper.HotkeyShortcut); }
        }

        public string CheckUpdateHeader
        {
            get { return UpdateManager.UpdateAvailable ? "*** Update..." : "Check update..."; }
        }

        public string ToolTipText
        {
            get
            {
                return string.Format("{0} {1}{2}", AppInfo.AppName, AppInfo.AppVersionString, UpdateManager.UpdateAvailable ? " (update available)" : "");
            }
        }
    }
}