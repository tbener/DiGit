using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DiGit.Helpers;
using DiGit.View;
using DiGit.ViewModel;

namespace DiGit.Commands
{

    public class AddRepositoryCommand : ICommand
    {

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var view = new AddRepositoryView();
            RepositoryViewModel viewModel = new RepositoryViewModel(null);
            viewModel.RequestClose += (s, e) =>
            {
                //view.DialogResult = !e.Cancel;
                view.Close();
                if (!e.Cancel)
                {
                    //RepositoriesManager.Add(viewModel.RepositoryPath, null);
                    //BubblesManager.Refresh();
                }
            };
            view.DataContext = viewModel;
            //view.Owner = Application.Current.MainWindow;
            view.Show();
        }
    }

}
