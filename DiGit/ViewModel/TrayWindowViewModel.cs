using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using DiGit.Commands;
using DiGit.Helpers;
using DiGit.Model;
using DiGit.Versioning;
using DiGit.View;
using DiGit.Versioning;
using DiGit.ViewModel.Base;
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
        public ICommand OpenConfigFolderCommand { get; set; }
        public ICommand ReloadConfigurationCommand { get; set; }
        public ICommand SaveConfigurationCommand { get; set; }
        public ICommand SaveAsConfigurationCommand { get; set; }
        public ICommand ShowTipsCommand { get; set; }
        public ICommand ResetToDefaultPositionCommand { get; set; }

        private TaskbarIcon _taskbarIcon;

        public TrayWindowViewModel(TaskbarIcon taskbarIcon)
        {
            _taskbarIcon = taskbarIcon;

            ExitCommand = new ExitCommand();
            ShowCommand = new ShowAllCommand();
            HideCommand = new HideAllCommand();
            AddRepoCommand = new AddRepositoryCommand();
            ResetToDefaultPositionCommand = new RelayCommand(BubblesManager.ResetPositionsToDefault);

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

            #region TEMP - Configuration handling

            OpenConfigFolderCommand = new RelayCommand(
                delegate
                {
                    ConfigurationHelper.Save();
                    Utils.OpenContainingFolder(ConfigurationHelper.ConfigFile);
                });
            ReloadConfigurationCommand = new RelayCommand(delegate
            {
                ConfigurationHelper.Load();
                Msg.Show("Loaded. Note that the Load (Beta) might not reload all the information. For full refresh restart DiGit.");
            });
            SaveConfigurationCommand = new RelayCommand(delegate
            {
                ConfigurationHelper.Save();
                Msg.Show("Settings saved to {0}", ConfigurationHelper.ConfigFile);
            });
            SaveAsConfigurationCommand = new RelayCommand(delegate
            {
                string file = ConfigurationHelper.ConfigFile;// Path.GetFileName(ConfigurationHelper.ConfigFile);
                if (DialogHelper.BrowseSaveFileByExtensions(new[]{"xml"}, true, ref file))
                {
                    ConfigurationHelper.Save();
                }
            });
            
            #endregion

            ShowTipsCommand = new RelayCommand(() => new ShowSingleViewCommand(typeof(TipsView)).Execute(null));

            HotkeyHelper.OnHotkeyChanged += (sender, args) => OnPropertyChanged("ShowHideHeader");
            // todo: currently the only balloon is for update, so no distinction about what to do.
            _taskbarIcon.TrayBalloonTipClicked += (sender, args) => ShowUpdateCommand.Execute(null);
            UpdateManager.OnUpdateRequired += UpdateManager_OnUpdateRequired;
            UpdateManager.OnUpdateInfoChanged += delegate(object sender, EventArgs args)
            {
                OnPropertyChanged("CheckUpdateHeader");
                OnPropertyChanged("ToolTipText");
#if DEBUG
                _taskbarIcon.ShowBalloonTip("Checking for updates...", "", BalloonIcon.Info);
#endif
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

        public double BubbleOpacity
        {
            get { return ConfigurationHelper.Configuration.Settings.Bubbles.Opacity; }
            set
            {
                ConfigurationHelper.Configuration.Settings.Bubbles.Opacity = value;
            }
        }

        public bool AutoArrange
        {
            get { return ConfigurationHelper.Configuration.Settings.Bubbles.autoArrange; }
            set
            {
                ConfigurationHelper.Configuration.Settings.Bubbles.autoArrange = value;
                if (value) BubblesManager.Arrange();
            }
        }
    }
}
