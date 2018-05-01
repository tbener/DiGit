using LibGit2Sharp;
using System;
using System.IO;

namespace DiGit.Model
{

    internal class RepoTracker
    {
        FileSystemWatcher _fileWatcher;
        RepositoryInformation _repoInfo;

        public event EventHandler OnLockFileChanged;
        public event EventHandler OnBranchChanged;

        public RepoTracker(Repository repo)
        {
            _repoInfo = repo.Info;

            _fileWatcher = new FileSystemWatcher();
            _fileWatcher.Path = _repoInfo.Path;
            _fileWatcher.Filter = "*.*";
            _fileWatcher.Created += fileWatcher_Created;
            _fileWatcher.Deleted += fileWatcher_Deleted;
            _fileWatcher.Renamed += fileWatcher_Renamed;
            _fileWatcher.EnableRaisingEvents = true;
            
        }


        private bool CheckLockFilesChange(string fileName)
        {
            if (fileName.StartsWith("index"))
            {
                OnLockFileChanged?.Invoke(this, null);
                return true;
            }
            return false;
        }
        

        private void fileWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (e.Name == "HEAD")
                OnBranchChanged?.Invoke(this, e);
            else
                CheckLockFilesChange(e.Name);
        }               

        void fileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            CheckLockFilesChange(e.Name);
        }

        void fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            CheckLockFilesChange(e.Name);
        }
    }
}
