using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DiGit.Commands;
using DiGit.Helpers;
using DiGit.Updates;

namespace DiGit.ViewModel
{
    class UpdateViewModel : BaseViewModel
    {
        public ICommand CheckUpdateCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand CloseCommand { get; set; }

        public UpdateViewModel()
        {
            UpdateManager.OnUpdateInfoChanged += (sender, args) => Refresh();
            Refresh();

            CheckUpdateCommand = new RelayCommand(UpdateManager.CheckRemoteAsync);
            UpdateCommand = new RelayCommand(UpdateManager.RunUpdate, CanUpdate);
            CloseCommand = new RelayCommand(() => OnRequestClose(true));

            if (!UpdateManager.Working && !UpdateManager.HasData)
                UpdateManager.CheckRemoteAsync();
        }

        private bool CanUpdate(object obj)
        {
            return UpdateManager.SetupFileFound;
        }

        public void Refresh()
        {
            CurrentVersion = AppInfo.AppVersionString;
            if (UpdateManager.Working)
            {
                NewVersion = "Reading...";
                WhatsNewList = null;
            }
            else
            {
                if (UpdateManager.LastReadError == null)
                {
                    NewVersion = UpdateManager.HasData ? UpdateManager.LastVersionInfo.version : "";
                }
                else
                    NewVersion = UpdateManager.LastReadError.Message;

                if (UpdateManager.LastVersionInfo != null)
                    WhatsNewList =new ObservableCollection<DiGitVersionInfoVersion>(UpdateManager.GetGreaterOrEqualVersions());

                //new ObservableCollection<DiGitVersionInfoVersionChange>(UpdateManager.LastVersionInfo.Change.ToList());
            }
            OnPropertyChanged("NewVersion");
            OnPropertyChanged("WhatsNewList");
            OnPropertyChanged("CanUpdate");
            OnPropertyChanged("UpdateCommandTooltip");
        }

        public string UpdateCommandTooltip
        {
            get
            {
                return UpdateManager.LastReadError != null ? UpdateManager.LastReadError.Message :
                    UpdateManager.SetupFileFound ? "Update to the latest version of DiGit" : "Setup file not found";
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
                UpdateManager.CheckRemoteAsync();
            }
        }
    }
}
