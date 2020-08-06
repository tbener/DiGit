using System.Collections.ObjectModel;
using System.Windows.Input;
using DiGit.Commands;
using DiGit.Helpers;
using DiGit.Model;
using DiGit.Versioning;
using DiGit.ViewModel.Base;

namespace DiGit.ViewModel
{
    class UpdateViewModel : BaseWindowViewModel
    {
        public ICommand CheckUpdateCommand { get; set; }
        public ICommand UpdateCommand { get; set; }

        public UpdateViewModel()
        {
            UpdateManager.OnUpdateInfoChanged += (sender, args) => Refresh();
            UpdateManager.OnPropertyChanged += UpdateManager_OnPropertyChanged;
            Refresh();

            CheckUpdateCommand = new RelayCommand(CheckUpdate);
            UpdateCommand = new RelayCommand(UpdateManager.RunUpdate, CanUpdate);

            if (!UpdateManager.Working && !UpdateManager.VersionInfoUpdated)
                UpdateManager.CheckUpdateAsync();
        }

        private void UpdateManager_OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Status");
        }

        private bool CanUpdate(object obj)
        {
            return UpdateManager.SetupFileFound;
        }

        private void CheckUpdate()
        {
            WhatsNewList = null;
            UpdateManager.CheckUpdateAsync();
        }

        public void Refresh()
        {
            CurrentVersion = AppInfo.AppVersionString;
            NewVersion = UpdateManager.VersionInfoUpdated ? UpdateManager.LatestVersionInfo.version : "";

            if (UpdateManager.LatestVersionInfo != null)
                WhatsNewList =new ObservableCollection<DiGitVersionInfoVersion>(UpdateManager.GetGreaterOrEqualVersions());

            OnPropertyChanged("Status");
            OnPropertyChanged("NewVersion");
            OnPropertyChanged("WhatsNewList");
            OnPropertyChanged("CanUpdate");
            OnPropertyChanged("UpdateCommandTooltip");
        }

        public string UpdateCommandTooltip
        {
            get
            {
                return UpdateManager.SetupFileFound ? "Update to the latest version of DiGit" : "Setup file not found";
            }
        }

        public string Status {
            get
            {
                if (UpdateManager.LastReadError != null)
                    return UpdateManager.LastReadError.Message;
                return string.IsNullOrEmpty(UpdateManager.Status) ? $"Last read: {UpdateManager.LastReadDateTime}" : UpdateManager.Status;
            }
        }

        public string CurrentVersion { get; set; }
        public string NewVersion { get; set; }

        public ObservableCollection<DiGitVersionInfoVersion> WhatsNewList { get; set; }

        public bool IsBetaUser
        {
            get { return ConfigurationHelper.Configuration.isBetaUser; }
            set
            {
                ConfigurationHelper.Configuration.isBetaUser = value;
                UpdateManager.CheckUpdateAsync();
            }
        }
    }
}
