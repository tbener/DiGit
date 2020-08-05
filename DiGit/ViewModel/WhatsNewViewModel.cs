using DiGit.Versioning;
using DiGit.ViewModel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiGit.ViewModel
{
    public class WhatsNewViewModel : BaseWindowViewModel
    {
        public WhatsNewViewModel()
        {
            UpdateManager.OnUpdateInfoChanged += (s, e) =>
            {
                OnPropertyChanged("VersionInfo");
            };
        }

        public DiGitVersionInfo VersionInfo => UpdateManager.VersionInfo;
    }
}
