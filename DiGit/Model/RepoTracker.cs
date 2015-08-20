using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DiGit.Model
{

    internal class RepoTracker
    {
        FileSystemWatcher _fileWatcher;
        RepositoryInformation _repoInfo;
        Timer _timer;

        public event EventHandler OnLockFileCreated;
        public event EventHandler OnLockFileDeleted;
        public event EventHandler OnBranchChanged;
        public event EventHandler OnStatusChanged;

        public RepoTracker(Repository repo)
        {
            _repoInfo = repo.Info;

            _fileWatcher = new FileSystemWatcher();
            _fileWatcher.Path = _repoInfo.Path;
            _fileWatcher.Filter = "*.*";
            _fileWatcher.Created += fileWatcher_Created;
            _fileWatcher.Deleted += fileWatcher_Deleted;
            _fileWatcher.Changed += fileWatcher_Changed;
            _fileWatcher.EnableRaisingEvents = true;

            _timer = new Timer(500);
            _timer.Elapsed += timer_Elapsed;
            //_timer.Start();
        }

        void fileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.Name == "HEAD")
                if (OnBranchChanged != null)
                    OnBranchChanged(this, e);
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Debug.Print(_repoInfo.CurrentOperation.ToString());
            if (OnStatusChanged != null)
                OnStatusChanged(this, e);
        }

        void fileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (e.Name == "index.lock")
                if (OnLockFileDeleted != null)
                    OnLockFileDeleted(this, e);
        }

        void fileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            if (e.Name == "index.lock")
                if (OnLockFileCreated != null)
                    OnLockFileCreated(this, e);
        }
    }
}
