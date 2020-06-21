using System;
using DiGit.Helpers;
using System.Windows;
using DiGit.Commands;
using DiGit.Model;
using DiGit.Versioning;
using DiGit.Properties;
using DiGit.View;

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
                // need this to keep user settings from previous version
                if (Settings.Default.SettingsUpgradeRequired)
                {
                    Settings.Default.Upgrade();
                    Settings.Default.SettingsUpgradeRequired = false;
                    Settings.Default.Save();
                }

                ConfigurationHelper.LoadOrCreate();

                step = "Read repositories";
                RepositoriesManager.ReadRepositories(false);

                step = "Check if no repository exists";
                if (ConfigurationHelper.Configuration.Repositories == null || ConfigurationHelper.Configuration.Repositories.Length == 0)
                    new AddRepositoryCommand().Execute(null);

                step = "Show Bubbles";
                BubblesManager.Refresh();

                step = "App base startup";
                base.OnStartup(e);

                step = "Init Update Manager";
                BackgroundTasksManager.Start();

                NotificationHelper.ShowNotification("DiGit", $"DiGit {AppInfo.AppVersionString} has started successfully.", typeof(TipsView));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An unexpected error ocurred while starting DiGit ({0}).", step), ex);
            }
        }

    }
}
