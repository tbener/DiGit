using System.ComponentModel;
using System.Windows;
using DiGit.Helpers;
using LibGit2Sharp;

namespace DiGit.ViewModel.Base
{
    public class BaseRepoViewModel : BaseViewModel
    {
        private Repository _repo;

        protected BaseRepoViewModel(Repository repo)
        {
            _repo = repo;
            Repository = repo;
        }


        public Repository Repository
        {
            get { return (Repository)GetValue(RepositoryProperty); }
            set { SetValue(RepositoryProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Repository.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RepositoryProperty =
            DependencyProperty.Register("Repository", typeof(Repository), typeof(BaseRepoViewModel), new PropertyMetadata(null));



        public Repository Repo
        {
            get { return _repo; }
            set { _repo = value; }
        }



    }
}
