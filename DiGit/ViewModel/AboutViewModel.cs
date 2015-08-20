using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Input;
using DiGit.Commands;
using DiGit.Helpers;
using DiGit.Updates;
using DiGit.View;

namespace DiGit.ViewModel
{
    public class AboutViewModel : BaseViewModel
    {
        public ICommand ShowUpdateCommand { get; set; }

        public AboutViewModel()
        {
            ShowUpdateCommand = ShowUpdateCommand = new ShowSingleViewCommand(typeof(UpdateView));

            UpdateManager.OnUpdateInfoChanged += delegate(object sender, EventArgs args)
            {
                OnPropertyChanged("UpdateInfo");
                OnPropertyChanged("UpdateLink");
            };
        }

        public string Version
        {
            get { return AppInfo.AppVersionFull(); }
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
                    else
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
