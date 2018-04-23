using System.Windows.Input;
using DiGit.Commands;
using DiGit.Model;
using DiGit.Versioning;
using DiGit.View;
using DiGit.ViewModel.Base;
using DiGit.Helpers;

namespace DiGit.ViewModel
{
    public class AboutViewModel : BaseDialogViewModel
    {
        public ICommand ShowUpdateCommand { get; set; }
        public ICommand OpenConfigFolderCommand { get; set; }

        public AboutViewModel()
        {
            ShowUpdateCommand = ShowUpdateCommand = new ShowSingleViewCommand(typeof(UpdateView));

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
                if (UpdateManager.HasData)
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
