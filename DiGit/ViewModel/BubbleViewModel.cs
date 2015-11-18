using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DiGit.ViewModel.Base;
using LibGit2Sharp;
using DiGit.Model;
using System.Windows.Input;
using DiGit.Commands;
using DiGit.Helpers;
using DiGit.View;
using System.Collections.ObjectModel;
using System.Threading;


namespace DiGit.ViewModel
{
    public class BubbleViewModel : BaseRepoViewModel
    {
        private bool _lockFileExists;
        private string _currentBranch = string.Empty;
        private string _status = string.Empty;
        private bool _flagMoved;

        public ICommand HideCommand { get; set; }
        public ICommand HideAllCommand { get; set; }
        public ICommand ShowAllCommand { get; set; }
        public ICommand ClickCommand { get; set; }
        public ICommand DblClkCommand { get; set; }
        public ICommand ShowMenu { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand ShowHideMenuCommand { get; set; }
        public ICommand RootPathCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand ContextMenuOpen { get; set; }
        public ICommand ClipboardPathCommand { get; set; }
        public ICommand OpenFavFolderCommand { get; set; }
        public ICommand SetFavorite { get; set; }

        public ObservableCollection<ICommand> CommandsList { get; set; }

        public BubbleViewModel(Repository repo)
            : base(repo)
        {


            ClickCommand = new RelayCommand(ShowHideMenu);
            DblClkCommand = new OpenFolderCommand(new PathClass(repo));
            RefreshCommand = new RelayCommand(Refresh);




            HideAllCommand = new RelayCommand(() => BubblesManager.ShowAll(false), () => RepositoriesManager.Repos.Any());
            ShowAllCommand = new RelayCommand(() => BubblesManager.ShowAll(true), () => RepositoriesManager.Repos.Any());
            ExitCommand = new ExitCommand();

            ShowHideMenuCommand = new RelayCommand(ShowHideMenu);

            CurrentBranch = repo.Head.Name;
            var repoTracker = new RepoTracker(repo);
            repoTracker.OnLockFileCreated += (s, e) => { LockExists = true; };
            repoTracker.OnLockFileDeleted += (s, e) => { LockExists = false; };
            repoTracker.OnBranchChanged += (s, e) => Refresh();

            //CommandsList = CreateCommandList();

            ConfigurationHelper.Configuration.Settings.VisualSettings.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "BubbleOpacity")
                    OnPropertyChanged("BubbleOpacity");
            };

            // todo: dispose use of PathClass
            PathClass rootPathClass = new PathClass(repo);
            rootPathClass.DisplayLength = Properties.Settings.Default.MenuPathLength;
            RootPathCommand = new OpenFolderCommand(rootPathClass);
            RootPath = string.Format("Open {0}", rootPathClass.DisplayPath);

            ClipboardFolder = new FolderCBViewModel(repo);

            ConfigurationHelper.OnConfigurationLoaded += (sender, args) =>
            {
                OnPropertyChanged("UserCommandList");
                OnPropertyChanged("FavoriteFoldersViewModels");
                OnPropertyChanged("RecentFoldersViewModels");
            };

            FolderViewModel.OnChange += (sender, args) =>
            {
                OnPropertyChanged("FavoriteFoldersViewModels");
                OnPropertyChanged("RecentFoldersViewModels");
            };

        }

        public string RootPath { get; set; }


        public FolderCBViewModel ClipboardFolder{ get; set;}
        

        public ObservableCollection<FolderViewModel> FavoriteFoldersViewModels
        {
            get
            {
                var list = FolderViewModel.GetListByRepo(Repo).Where(f => f.IsFavorite);
                var obs = new ObservableCollection<FolderViewModel>(list);
                return obs;
            }
        }

        public ObservableCollection<FolderViewModel> RecentFoldersViewModels
        {
            get
            {
                var list = FolderViewModel.GetListByRepo(Repo).Where(f => !f.IsFavorite);
                var obs = new ObservableCollection<FolderViewModel>(list.OrderByDescending(f => f.ConfigFolder.lastUsage));
                return obs;
            }
        }

        

        public List<ICommand> UserCommandList
        {
            get
            {
                return ConfigurationHelper.Configuration.Commands.Select(c => new UserCommand(c, Repository)).Cast<ICommand>().ToList();
            }

        }


        public void Refresh()
        {
            int counter = 3;
            do
            {
                try
                {
                    CurrentBranch = Repo.Head.Name;
                    break;
                }
                catch (Exception)
                {
                    if (counter > 0)
                    {
                        CurrentBranch = "[Reading...]";
                        Thread.Sleep(1000);
                    }
                    else
                        CurrentBranch = "[Error]";
                }
            } while (counter-- > 0);

        }

        public void Start(Window view)
        {
            //BubblesManager.ShowView(view, true);
            HideCommand = new RelayCommand(() => BubblesManager.ShowView(view, false));
            view.LocationChanged += (s, e) =>
            {
                if (((BubbleView)s).IsLoaded)
                    _flagMoved = true;
                //BubbleView view1 = s as BubbleView;
                //if (view1 != null) Status = string.Format("{0}, {1}", view1.Left.ToString(), view1.Top.ToString());
            };

            _flagMoved = false;
        }

        private void ShowHideMenu()
        {
            if (_flagMoved)
            {
                _flagMoved = false;
                return;
            }
            //CommandsListViewModel.IsShowMenu = !CommandsListViewModel.IsShowMenu;

            IsShowMenu = !IsShowMenu;

        }


        public string CurrentBranch
        {
            get { return _currentBranch; }
            set
            {
                _currentBranch = value;
                OnPropertyChanged("CurrentBranch");
            }
        }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        public bool LockExists
        {
            get { return _lockFileExists; }
            set
            {
                _lockFileExists = value;
                OnPropertyChanged("LockExists");
            }
        }

        public string FolderOpenMenuHeader { get; set; }
        public string OpenFolderCommandParameter { get; set; }

        public string DblClkCommandParam { get; set; }
        public bool BubbleIsMoving { get; set; }

        public static readonly DependencyProperty IsShowMenuProperty = DependencyProperty.Register(
            "IsShowMenu", typeof(bool), typeof(BubbleViewModel), new PropertyMetadata(default(bool)));





        public bool IsShowMenu
        {
            get { return (bool)GetValue(IsShowMenuProperty); }
            set { SetValue(IsShowMenuProperty, value); }
        }


        public double BubbleOpacity
        {
            get { return ConfigurationHelper.Configuration.Settings.VisualSettings.BubblesOpacity; }
            set
            {
                ConfigurationHelper.Configuration.Settings.VisualSettings.BubblesOpacity = value;
            }
        }
    }
}
