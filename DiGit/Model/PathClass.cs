using System;
using System.IO;
using System.Windows;
using DiGit.Configuration;
using DiGit.Helpers;

namespace DiGit.Model
{
    public class PathClass
    {
        public event EventHandler OnChange;
        public static event EventHandler ClipboardPathFound;
        private bool _monitorClipboard;
        private string _relativePath;

        private static string _clipboardPath = "";

        static PathClass()
        {
            CheckClipboard(null, null);
            ClipboardNotification.ClipboardUpdate += CheckClipboard;
        }

        private static void CheckClipboard(object sender, EventArgs args)
        {
            try
            {
                string text = Clipboard.GetText();
                string relPath = "";
                DiGitConfigRepository foundRepo;

                if (Path.IsPathRooted(text))
                {
                    // Path rooted
                    // If exists - check if it is in one of the repositories
                    if (PathHelper.Exists(text))
                    {
                        foundRepo = RepositoriesManager.Repos.Find(r => text.StartsWith(r.path) && text.Length > r.path.Length);
                        if (foundRepo != null)
                            relPath = text.Substring(foundRepo.path.Length);
                    }
                }
                else
                {
                    // Relative path
                    // Check is at least one repository has it
                    foundRepo = RepositoriesManager.Repos.Find(r => PathHelper.Exists(DiGit.Helpers.PathHelper.GetFullPath(r.path, text)));
                    if (foundRepo != null)
                        relPath = text;
                }


                if (relPath.Length > 0)
                {
                    _clipboardPath = relPath;
                    if (ClipboardPathFound != null)
                        ClipboardPathFound(null, null);
                }
            }
            catch (Exception ex)
            {
                // do nothing
            }
        }

        public PathClass(string root, string path)
        {
            RelativePath = path;
            Root = root;
        }

        public PathClass(string root)
            : this(root, "")
        {
        }

        public PathClass(LibGit2Sharp.Repository repo)
            : this(repo.Info.WorkingDirectory)
        {
        }

        public PathClass(LibGit2Sharp.Repository repo, string path)
            : this(repo.Info.WorkingDirectory, path)
        {
        }

        public bool MonitorClipboard
        {
            get { return _monitorClipboard; }
            set
            {
                _monitorClipboard = value;
                if (value)
                {
                    SetClipboardPath(null, null);
                    ClipboardPathFound += SetClipboardPath;
                }
            }
        }

        private void SetClipboardPath(object sender, EventArgs args)
        {
            if (PathHelper.Exists(Root, _clipboardPath))
                RelativePath = _clipboardPath;
        }

        public string RelativePath
        {
            get { return _relativePath; }
            private set
            {
                _relativePath = value;
                if (OnChange != null) OnChange(this, null);
            }
        }

        public string Root { get; private set; }

        public string DisplayPath
        {
            get
            {
                string display = RelativePath.Length > 0 ? RelativePath : Root;
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

        public string FullPath
        {
            get { return PathHelper.GetFullPath(Root, RelativePath); }
        }

        public bool Exists
        {
            get { return Directory.Exists(FullPath); }
        }

        public bool Enabled
        {
            get { return MonitorClipboard ? RelativePath.Length > 0 : Exists; }
        }

        public bool IsFavorite { get; set; }

    }
}
