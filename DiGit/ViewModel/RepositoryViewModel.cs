using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;
using DiGit.Commands;
using DiGit.Helpers;
using DiGit.Model;
using DiGit.ViewModel.Base;
using LibGit2Sharp;

namespace DiGit.ViewModel
{
    public class RepositoryViewModel : BaseRepoViewModel
    {
        private string _repositoryPath;
        public ICommand BrowseCommand { get; set; }
        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand CheckCommand { get; set; }

        private readonly bool _isAdding;

        public event CancelEventHandler RequestClose;

        public RepositoryViewModel(Repository repo)
            : base(repo)
        {
            if (repo != null)
            {
                RepositoryPath = repo.Info.WorkingDirectory;
                OkButtonText = "Set";
                WindowTitle = "Change Repository";
                _isAdding = false;
            }
            else
            {
                OkButtonText = "Add";
                WindowTitle = "Add Repository";
                _isAdding = true;
            }

            OkCommand = new RelayCommand(() => OnCloseCommand(true), IsValidRepository);
            CancelCommand = new RelayCommand(() => OnCloseCommand(false));
            BrowseCommand = new RelayCommand(OnOpenRepository);
            CheckCommand = new RelayCommand(CheckRepository);
        }

        private void OnOpenRepository()
        {
            var dlg = new FolderBrowser2();
            if (dlg.ShowDialog(null) == DialogResult.OK)
            {
                RepositoryPath = dlg.DirectoryPath;
                CheckRepository();
                if (IsValidRepository())
                    OnCloseCommand(true);
            }

        }

        public string RepositoryPath
        {
            get { return _repositoryPath; }
            set
            {
                _repositoryPath = value;
                OnPropertyChanged("RepositoryPath");
            }
        }

        public void CheckRepository()
        {
            Repo = null;
            try
            {
                Repo = new Repository(RepositoryPath);
                if (RepositoriesManager.Exists(Repo.Info.WorkingDirectory))
                {
                    Repo = null;
                    SetStatus("Repository exists");
                }
                else
                    SetStatus("Repository OK");
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, false);
                SetStatus("No Git repository found");
            }

            //IsValidRepository();
            
        }

        public bool IsValidRepository()
        {
            return Repo != null;

        }

        private void SetRepository()
        {
            if (Repo != null)
            {
                if (_isAdding)
                {
                    RepositoriesManager.Add(Repo, null);
                    BubblesManager.Refresh();
                    ConfigurationHelper.Save();
                }
                else
                {
                    // TODO: set existing repo
                }
            }
        }

        public string Status { get; set; }
        public string OkButtonText { get; set; }
        public string WindowTitle { get; set; }

        private void SetStatus(string text)
        {
            Status = text;
            OnPropertyChanged("Status");
        }

        protected void OnCloseCommand(bool ok)
        {
            if (ok)
            {
                if (!IsValidRepository()) return;
                SetRepository();
            }

            OnRequestClose(!ok);
        }

        protected virtual void OnRequestClose(bool cancel)
        {
            if (RequestClose != null)
                RequestClose(this, new CancelEventArgs(cancel));
        }
    }
}
