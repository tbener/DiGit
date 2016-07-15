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
using LibGit2Sharp;
using DiGit.Model;

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

    public class DeleteRepositoryCommand : ICommand
    {

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            // Since it doesn't affect the enabled\disabled of a menu item at runtime, we leave it as true
            return true;
            //return ConfigurationHelper.Configuration.RepositoryList.Any() && ConfigurationHelper.Configuration.RepositoryList.Count > 1;
        }


        public void Execute(object parameter)
        {
            Repository repo = parameter as Repository;

            if (repo != null)
            {
                var configRepo = RepositoriesManager.Get(repo);
                if (configRepo != null)
                {
                    if (!Msg.ShowQ("Do you want to delete this repository?\nFile system will not be affected."))
                        return;
                    // remove from repository manager
                    RepositoriesManager.Delete(repo);
                    // close bubble window
                    BubblesManager.Close(configRepo.View);
                    // remove from repository main list
                    ConfigurationHelper.Configuration.RepositoryList.Remove(configRepo);
                    // refresh bubbles
                    BubblesManager.Refresh();
                    // notify
                    CanExecuteChanged?.Invoke(this, new EventArgs());
                }
            }
        }
    }

}
