using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DiGit.Configuration;
using DiGit.Helpers;
using DiGit.Model;
using LibGit2Sharp;

namespace DiGit.ViewModel
{
    /// <summary>
    /// This is a Folder Path class that monitors the Clipboard.
    /// There is one instance for each repository.
    /// Every Clipboard change the class instance checks against its repository.
    /// All instances share the same DiGitConfigFolder so any change (usage, favorite) applies to all.
    /// </summary>
    public class FolderCbViewModel : FolderViewModel
    {
        // The DiGitConfigFolder class that is shared between all FolderCBViewModel instances
        private static DiGitConfigFolder _cpConfigFolder;

        public new static event EventHandler OnChange;

        /// <summary>
        /// Static constructor to initialize the shared DiGitConfigFolder
        /// </summary>
        static FolderCbViewModel()
        {
            CheckClipboard(null, null);
            ClipboardNotification.ClipboardUpdate += CheckClipboard;
        }

        /// <summary>
        /// Override base contructor
        /// </summary>
        /// <param name="repo"></param>
        public FolderCbViewModel(Repository repo) : base(repo, _cpConfigFolder)
        {
            DisplayLength = Properties.Settings.Default.MenuPathLength;
            OnChange += (sender, args) => Refresh();
            FolderViewModel.OnChange += (sender, args) => Refresh();
        }

        public void Refresh()
        {
            OnPropertyChanged("DisplayText");
            OnPropertyChanged("Exists");
            OnPropertyChanged("IsFavorite");
        }

        private static void SetConfigFolder(string relPath)
        {
            _cpConfigFolder = FolderList.Find(f => f.path.Equals(relPath, StringComparison.OrdinalIgnoreCase)) ?? new DiGitConfigFolder(){path=relPath};
            if (OnChange != null) OnChange(null, null);
        }

        private static void CheckClipboard(object sender, EventArgs args)
        {
            try
            {
                string text = Clipboard.GetText();
                string relPath = string.Empty;
                DiGitConfigRepository foundRepo;

                if (Path.IsPathRooted(text))
                {
                    // Path rooted
                    // If exists - check if it is in one of the repositories
                    if (PathHelper.Exists(text))
                    {
                        foundRepo = ConfigurationHelper.Configuration.RepositoryList.Find(r => text.StartsWith(r.path) && text.Length > r.path.Length);
                        if (foundRepo != null) relPath = text.Substring(foundRepo.path.Length);
                    }
                }
                else
                {
                    // Relative path
                    // Check is at least one repository has it
                    foundRepo = ConfigurationHelper.Configuration.RepositoryList.Find(r => PathHelper.Exists(DiGit.Helpers.PathHelper.GetFullPath(r.path, text)));
                    if (foundRepo != null) relPath = text;
                }

                SetConfigFolder(relPath);
            }
            catch
            {
                if (_cpConfigFolder == null) 
                    _cpConfigFolder = new DiGitConfigFolder() { path = "" };
            }
        }

        public override DiGitConfigFolder ConfigFolder
        {
            get { return _cpConfigFolder; } 
            set { _cpConfigFolder = value; }
        }

        public override string DisplayText
        {
            get
            {
                return (Exists) ? string.Format("Open {0}", DisplayPath) : "No path found in Clipboard";
            }
        }

        public override bool Exists
        {
            get
            {
                if (ConfigFolder == null) return false;
                return ConfigFolder.path != string.Empty && base.Exists;
            }
        }
        
    }
}
