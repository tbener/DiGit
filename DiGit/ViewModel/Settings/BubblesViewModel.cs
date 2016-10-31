using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiGit.Model;
using DiGit.Configuration;
using System.Collections.ObjectModel;
using DiGit.Helpers;

namespace DiGit.ViewModel.Settings
{
    public class BubblesViewModel : BaseSettingsViewModel
    {
        //private ObservableCollection<DiGitConfigRepository> RepositoryList;

        public BubblesViewModel()
        {
            RepositoryList = new ObservableCollection<DiGitConfigRepository>(ConfigurationHelper.Configuration.RepositoryList);
            //RepositoriesManager.Repos[0].
        }

        public ObservableCollection<DiGitConfigRepository> RepositoryList { get; set; }
      
        

        public bool AttachToRight
        {
            get
            {
                return BubblesManager.Anchor == System.Windows.HorizontalAlignment.Right;
            }
            set
            {
                BubblesManager.Anchor = value ? System.Windows.HorizontalAlignment.Right : System.Windows.HorizontalAlignment.Left;
            }
        }
    }
}
