using System;
using System.Threading;
using System.Threading.Tasks;
using DiGit.Helpers;
using System.Windows;
using DiGit.Commands;
using DiGit.Model;
using DiGit.Versioning;

namespace DiGit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            string step = "Load configuration";
            try
            {
                ConfigurationHelper.Load("", false);

                step = "Load repositories";
                if (RepositoriesManager.Repos.Count == 0)
                    new AddRepositoryCommand().Execute(null);

                step = "Show Bubbles";
                BubblesManager.Refresh(true);

                step = "App base startup";
                base.OnStartup(e);

                step = "Init Update Manager";
                BackgroundTasksManager.Start();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An unexpected error ocurred while starting DiGit ({0}).", step), ex);
            }
        }

    }
}
