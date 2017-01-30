using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        public virtual DiGitConfigFolder ConfigFolder { get; set; }

        public string FullPath => PathHelper.GetFullPath(Repo.Info.WorkingDirectory, ConfigFolder.path);
        public virtual bool Exists => Directory.Exists(FullPath);

        static FolderViewModel()
        {
            ListByRepo = new Dictionary<Repository, ObservableCollection<FolderViewModel>>();
            SizeFoldersMru();   // first call - this will only resize the main list
        }

        protected FolderViewModel(Repository repo, DiGitConfigFolder digitConfigFolder) : base(repo)
        {
            DisplayLength = Properties.Settings.Default.MenuPathLengthWide;
            ConfigFolder = digitConfigFolder;
            
            OpenFolderCommand = new RelayCommand(OpenFolder, () => Exists);
        }

        private void OpenFolder()
        {
            Process.Start(FullPath);
            ConfigFolder.lastUsage = DateTime.Now;
            ConfigFolder.lastUsageSpecified = true;
            AddConfigFolderToList();
            OnChange?.Invoke(null, null);
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
            SizeFoldersMru(repo);
            return ListByRepo[repo];
        }

        public static List<DiGitConfigFolder> FolderList
        {
            get
            {
                if (ConfigurationHelper.Configuration.Folders != null)
                    return _folderList ?? (_folderList = ConfigurationHelper.Configuration.Folders.ToList());
                return new List<DiGitConfigFolder>();
            }
        }

        private void AddConfigFolderToList()
        {
            if (_folderList.Contains(ConfigFolder)) return;
            // add to main list
            _folderList.Add(ConfigFolder);
            // add to every repo that this path exists in
            foreach (KeyValuePair<Repository, ObservableCollection<FolderViewModel>> keyValuePair in ListByRepo)
            {
                if (PathHelper.Exists(keyValuePair.Key.Info.WorkingDirectory, ConfigFolder.path))
                    keyValuePair.Value.Add(new FolderViewModel(keyValuePair.Key, ConfigFolder));
            }
            // the mru is limitted (by default to 10)
            if (!ConfigFolder.isFavorite)
                SizeFoldersMru();
        }

        /// <summary>
        /// This method resizes the mru folder list for the given repo of for all existing ones.
        /// Mru list can hold the maximum items as configured in FoldersMRU value (10 by default).
        /// Notes:
        ///   1. Every such list is created only on first use.
        ///   2. There is only one main list which is actually saved: FolderList.
        /// Since keeping FolderList contains only the necessary items is too complicated for this matter,
        /// we're just keeping it not more than 100 (hard coded).
        /// (In order to really keep FolderList without redundant folders we must generate the lists for all repos
        ///   and resize them all, and then check which folders none of them contain. These ones will be deleted
        ///   from FolderList)
        /// </summary>
        /// <param name="repo">Optional. Call it with a repo when a list is created. When a folder is added we need to check all repos.</param>
        private static void SizeFoldersMru(Repository repo = null)
        {
            // get the list of folder that aren't in favorites
            // var mruList = FolderList.Where(dcf => !dcf.isFavorite).ToList();
            int max = Math.Max(3, Properties.Settings.Default.FoldersMRU);

            // check every repo's list
            foreach (ObservableCollection<FolderViewModel> repoFolderViewModels in ListByRepo.Where(kv => repo == null || kv.Key == repo).Select(kv => kv.Value))
            {
                // get the list of folder that aren't in favorites
                var repoMruList = repoFolderViewModels.Where(fvm => !fvm.IsFavorite) ;
                // if the list is too big
                if (repoMruList.Count() > max)
                {
                    // get the folders to remove (the least used)
                    List<FolderViewModel> toRemove = repoMruList.OrderByDescending(dcf => dcf.ConfigFolder.lastUsage).ToList().GetRange(max, repoMruList.Count() - max);

                    // remove the folders
                    foreach (var vm in toRemove.ToList())
                    {
                        repoFolderViewModels.Remove(vm);
                    }
                }
            }

            if (repo == null)
            {
                // fix main folder list
                max = 100;
                int countNotFavorites = FolderList.Count(dcf => !dcf.isFavorite);
                if (countNotFavorites > max)
                {
                    List<DiGitConfigFolder> toRemove =
                        FolderList.OrderByDescending(dcf => dcf.lastUsage)
                            .ToList()
                            .GetRange(max, countNotFavorites - max);

                    FolderList.RemoveAll(toRemove.Contains);
                }
            }

            //if (mruList.Count() > max)
            //{
            //    List<DiGitConfigFolder> toRemove = mruList.OrderByDescending(dcf => dcf.lastUsage).ToList().GetRange(max, mruList.Count() - max);
            //    FolderList.RemoveAll(toRemove.Contains);
            //    // loop on list of every repo
            //    foreach (KeyValuePair<Repository, ObservableCollection<FolderViewModel>> keyValuePair in ListByRepo)
            //    {

            //        foreach (var vm in keyValuePair.Value.ToList())
            //        {
            //            if (!FolderList.Contains(vm.ConfigFolder)) keyValuePair.Value.Remove(vm);
            //        }

            //    }
            //}
        }

        #endregion

        #region Display

        public virtual string DisplayText => DisplayPath;

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
                return display; //.Replace("_", "__");
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
                OnChange?.Invoke(this, null);
            }
        }

       

        #endregion
    }
}
