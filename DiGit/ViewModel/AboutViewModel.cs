using System.Windows.Input;
using DiGit.Commands;
using DiGit.Model;
using DiGit.Versioning;
using DiGit.View;
using DiGit.ViewModel.Base;
using DiGit.Helpers;
using System.Windows;

namespace DiGit.ViewModel
{
    public class AboutViewModel : BaseDialogViewModel
    {
        public ICommand ShowUpdateCommand { get; set; }
        public ICommand OpenConfigFolderCommand { get; set; }
        public ICommand EmailCommand { get; set; }
        public ICommand CopyEmailCommand { get; set; }

        public AboutViewModel()
        {
            ShowUpdateCommand = new ShowSingleViewCommand(typeof(UpdateView));

            UpdateManager.OnUpdateInfoChanged += delegate
            {
                OnPropertyChanged("UpdateInfo");
                OnPropertyChanged("UpdateLink");
            };

            OpenConfigFolderCommand = new RelayCommand(
                delegate
                {
                    Utils.OpenContainingFolder(ConfigurationHelper.ConfigFile);
                });

            EmailCommand = new RelayCommand(SendEmail);
            CopyEmailCommand = new RelayCommand(CopyEmail);
        }

        public void SendEmail()
        {
            System.Diagnostics.Process.Start($"mailto:{AuthorEmail}");
        }

        public void CopyEmail()
        {
            Clipboard.SetText(AuthorEmail);
        }

        public string AuthorEmail
        {
            get { return AppInfo.AppAuthorEmail; }
        }

        public string Version
        {
            get { return AppInfo.AppVersionFull(); }
        }

        public string ConfigFilePathDisplay
        {
            get { return PathHelper.ShortDisplay(ConfigurationHelper.ConfigFile, Properties.Settings.Default.MenuPathLength); }
        }
        

        public string ConfigFilePathToolTip
        {
            get { return string.Format("{0} (Click to open containing folder)", ConfigurationHelper.ConfigFile); }
        }

        public string Description
        {
            get { return AppInfo.AppDescription; }
        }

        public string Author
        {
            get { return AppInfo.AppAuthor; }
        }

        public string UpdateInfo
        {
            get
            {
                if (UpdateManager.VersionInfoUpdated)
                {
                    if (UpdateManager.UpdateRequired)
                        return "New version available.";
                    return "Your DiGit version is up to date.";
                }

                return "";
            }
        }

        public string UpdateLink
        {
            get
            {
                if (UpdateManager.UpdateRequired)
                    return "Click to update";
                return "Check for update";
            }
        }
    }
}
