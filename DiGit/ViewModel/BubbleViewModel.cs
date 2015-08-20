using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DiGit.Properties;
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
    public class BubbleViewModel : BaseViewModel
    {
        private bool _lockFileExists = false;
        private string _currentBranch = string.Empty;
        private string _status = string.Empty;
        private bool _flagMoved = false;
        private readonly CommandsListModel _cmdModel;

        public ICommand HideCommand { get; set; }
        public ICommand HideAllCommand { get; set; }
        public ICommand ShowAllCommand { get; set; }
        public ICommand ClickCommand { get; set; }
        public ICommand DblClkCommand { get; set; }
        public ICommand ShowMenu { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand ShowHideMenuCommand { get; set; }
        public ICommand OpenFolderCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        public ObservableCollection<MenuCommand> CommandsList { get; set; }

        // TODO: add refresh command

        public BubbleViewModel(Repository repo)
            : base(repo)
        {


            ClickCommand = new RelayCommand(ShowHideMenu);
            DblClkCommand = new OpenFolderCommand();
            DblClkCommandParam = repo.Info.WorkingDirectory;
            RefreshCommand = new RelayCommand(Refresh);

            // try compacting path
            string shortPath = repo.Info.WorkingDirectory;
            try
            {
                if (Settings.Default.MenuPathLength > 0)
                    shortPath = PathHelper.ShortDisplay(repo.Info.WorkingDirectory, Settings.Default.MenuPathLength);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, false);
                shortPath += " (*)";
            }

            FolderOpenMenuHeader = string.Format("Open {0}", shortPath);
            OpenFolderCommand = new OpenFolderCommand();
            OpenFolderCommandParameter = repo.Info.WorkingDirectory;

            HideAllCommand = new RelayCommand(() => BubblesManager.ShowAll(false), () => { return RepositoriesManager.Repos.Count() > 0; });
            ShowAllCommand = new RelayCommand(() => BubblesManager.ShowAll(true), () => { return RepositoriesManager.Repos.Count() > 0; });
            ExitCommand = new ExitCommand();

            ShowHideMenuCommand = new RelayCommand(ShowHideMenu);

            CurrentBranch = repo.Head.Name;
            var repoTracker = new RepoTracker(repo);
            repoTracker.OnLockFileCreated += (s, e) => { LockExists = true; };
            repoTracker.OnLockFileDeleted += (s, e) => { LockExists = false; };
            repoTracker.OnBranchChanged += (s, e) => { Refresh(); };
            //_repoTracker.OnStatusChanged += repoTracker_OnStatusChanged;

            CommandsListViewModel = new CommandsListViewModel(repo);

            _cmdModel = new CommandsListModel(repo);
            CommandsList = _cmdModel.GetCommandsList;

            ConfigurationHelper.Configuration.Settings.VisualSettings.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "BubbleOpacity")
                    OnPropertyChanged("BubbleOpacity");
            };

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
                catch (Exception ex)
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


        //void repoTracker_OnStatusChanged(object sender, EventArgs e)
        //{
        //    Status = string.Format("{0}: {1}", DateTime.Now.ToLongTimeString(), _repo.Info.Message);
        //    if (!_repo.Head.Name.Equals(CurrentBranch))
        //        CurrentBranch = _repo.Head.Name;
        //}

        public CommandsListViewModel CommandsListViewModel { get; set; }


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
    }
}
