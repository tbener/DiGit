using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DiGit.Commands;
using DiGit.Configuration;
using DiGit.Helpers;
using DiGit.ViewModel.Base;
using LibGit2Sharp;

namespace DiGit.ViewModel
{
    /// <summary>
    /// Class FolderViewModel
    /// - Has a property of type DiGitConfigFolder and a Repository, which together combines a full path.
    /// A few FolderViewModel classes can be attached to a singel DiGitConfigFolder (with different repositories).
    /// - A static List of DiGitConfigFolder which comes from the configuration is shared between all
    /// the FolderViewModel classes. 
    /// Using the 2 properties above, if a DiGitConfigFolder instance is changed (isFavorite true/false, add or remove from the list)
    /// all other instances are affected.
    /// </summary>
    public class FolderViewModel : BaseRepoViewModel
    {
        public ICommand OpenFolderCommand { get; set; }

        public static event EventHandler OnChange;

        private static List<DiGitConfigFolder> _folderList;
        private static readonly Dictionary<Repository, ObservableCollection<FolderViewModel>> ListByRepo;

        protected virtual DiGitConfigFolder ConfigFolder { get; set; }

        static FolderViewModel()
        {
            ListByRepo = new Dictionary<Repository, ObservableCollection<FolderViewModel>>();
        }

        protected FolderViewModel(Repository repo, DiGitConfigFolder digitConfigFolder) : base(repo)
        {
            DisplayLength = Properties.Settings.Default.MenuPathLength;
            ConfigFolder = digitConfigFolder;
            
            OpenFolderCommand = new RelayCommand(OpenFolder, () => Exists);
        }

        private void OpenFolder()
        {
            Process.Start(FullPath);
            ConfigFolder.lastUsage = DateTime.Now;
            AddConfigFolderToList();
        }

        #region Static lists management

        public static ObservableCollection<FolderViewModel> GetListByRepo(Repository repo)
        {
            if (!ListByRepo.ContainsKey(repo))
            {
                var listDiGitConfigFolder =
                    FolderList.Where(f => PathHelper.Exists(repo.Info.WorkingDirectory, f.path)).ToList();
                var listViewModels = listDiGitConfigFolder.Select(f => new FolderViewModel(repo, f)).ToList();

                ListByRepo[repo] = new ObservableCollection<FolderViewModel>(listViewModels);
            }
            return ListByRepo[repo];
        }

        public static List<DiGitConfigFolder> FolderList
        {
            get { return _folderList ?? (_folderList = ConfigurationHelper.Configuration.Folders.ToList()); }
        }

        private void AddConfigFolderToList()
        {
            if (!_folderList.Contains(ConfigFolder)) return;
            _folderList.Add(ConfigFolder);
            foreach (KeyValuePair<Repository, ObservableCollection<FolderViewModel>> keyValuePair in ListByRepo)
            {
                if (PathHelper.Exists(keyValuePair.Key.Info.WorkingDirectory, ConfigFolder.path))
                    // rebuild the list for this Repo
                    keyValuePair.Value.Add(new FolderViewModel(keyValuePair.Key, ConfigFolder));
            }
            OnChange(null, null);
        }

        #endregion

        #region Display

        public virtual string DisplayText
        {
            get { return DisplayPath; }
        }

        public string DisplayPath
        {
            get
            {
                string display = ConfigFolder.path;
                try
                {
                    if (DisplayLength > 0)
                        display = PathHelper.ShortDisplay(display, DisplayLength);
                }
                catch (Exception ex)
                {
                    ErrorHandler.Handle(ex, false);
                    display += " (*)";
                }
                return display.Replace("_", "__");
            }
        }

        public int DisplayLength { get; set; }

        #endregion



        #region Properties

        public virtual bool IsFavorite
        {
            get { return ConfigFolder != null && ConfigFolder.isFavorite; }
            set
            {
                ConfigFolder.isFavorite = value;
                if (value) AddConfigFolderToList();
                if (OnChange != null) OnChange(this, null);
            }
        }

        public string FullPath
        {
            get { return PathHelper.GetFullPath(Repo.Info.WorkingDirectory, ConfigFolder.path); }
        }

        public virtual bool Exists
        {
            get { return Directory.Exists(FullPath); }
        }

        

        #endregion
    }
}
