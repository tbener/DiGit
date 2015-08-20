using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DiGit.Model;
using LibGit2Sharp;

namespace DiGit.ViewModel
{
    public class CommandsListViewModel : BaseViewModel
    {
        private readonly CommandsListModel _cmdModel;

        public CommandsListViewModel(Repository repo) : base(repo)
        {
            _cmdModel = new CommandsListModel(repo);
            CommandsList = _cmdModel.GetCommandsList;
        }

        public ObservableCollection<MenuCommand> CommandsList { get; set; }

        public MenuCommand SelectedItem
        {
            get { return null; }
            set { //_cmdModel.ExecuteCommand(value);
            }
        }



        //public bool IsShowMenu
        //{
        //    get { return (bool)GetValue(IsShowMenuProperty); }
        //    set { SetValue(IsShowMenuProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for IsShowMenu.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty IsShowMenuProperty =
        //    DependencyProperty.Register("IsShowMenu", typeof(bool), typeof(CommandsListViewModel), new PropertyMetadata(false));


    }
}
