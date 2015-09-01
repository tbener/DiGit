using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DiGit.Configuration;

namespace DiGit.ViewModel.Settings
{
    public class UserCommandEditViewModel : BaseSettingsViewModel
    {

        public DiGitConfigCommand UserCommand { get; set; }

        public string Header
        {
            get { return UserCommand.header; }
            set
            {
                UserCommand.header = value;
                OnPropertyChanged("Header");
            }
        }

        public string FileName
        {
            get { return UserCommand.fileName; }
            set
            {
                UserCommand.fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        public string Arguments
        {
            get { return UserCommand.arguments; }
            set
            {
                UserCommand.arguments = value;
                OnPropertyChanged("Arguments");
            }
        }
    }
}
