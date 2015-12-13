using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DiGit.Configuration;
using DiGit.Helpers;
using LibGit2Sharp;

namespace DiGit.Model
{
    internal static class RepositoriesManager
    {

        private static Dictionary<string, DiGitConfigRepository> _repos;

        static RepositoriesManager()
        {
            if (_repos == null)
            {
                ReadRepositories(false);
            }
        }

        internal static List<DiGitConfigRepository> Repos
        {
            get
            {
                if (_repos == null)
                    ReadRepositories(false);
                return _repos.Values.ToList();
            }
        }

        internal static bool Add(string path, DiGitConfigRepository configRepository)
        {
            try
            {
                Repository repo = new Repository(path);
                return Add(repo, configRepository);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "Could not load repository: {0}.", path);
                return false;
            }
        }

        internal static bool Add(Repository repo, DiGitConfigRepository configRepository)
        {
            try
            {
                if (configRepository == null)
                    configRepository = new DiGitConfigRepository();
                configRepository.Repository = repo;
                _repos.Add(configRepository.path, configRepository);
                // don't remove the config saving from here. it's required for first init
                ConfigurationHelper.Save();
                return true;
            }
            catch (Exception ex)
            {
                Msg.ShowE(ex, "Could not add repository.");
                return false;
            }
        }

        internal static bool Exists(string path)
        {
            return _repos.ContainsKey(path);
        }

        internal static bool ReadRepositories(bool force)
        {
            if (!force && _repos != null) return false;
            
            _repos = new Dictionary<string, DiGitConfigRepository>();
            var config = ConfigurationHelper.Configuration;

            if (File.Exists(ConfigurationHelper.ConfigFile))
            {
                if (config.Repositories != null)
                    foreach (DiGitConfigRepository configRepository in config.Repositories)
                    {
                        Add(configRepository.path, configRepository);
                    }
            }

            return _repos.Count > 0;
           
        }
    }
}
