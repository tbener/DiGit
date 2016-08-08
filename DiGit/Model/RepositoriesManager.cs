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


        internal static bool Add(Repository repo, DiGitConfigRepository configRepository)
        {
            try
            {
                if (configRepository == null)
                {
                    configRepository = new DiGitConfigRepository() { IsNew = true, Repository = repo };
                    ConfigurationHelper.Configuration.RepositoryList.Add(configRepository);
                    ConfigurationHelper.Save();
                }
                else
                {
                    configRepository.Repository = repo;
                }
                _repos.Add(configRepository.path, configRepository);
                

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

        internal static bool Delete(Repository repo)
        {
            if (_repos.ContainsKey(repo.Info.WorkingDirectory))
            {
                _repos.Remove(repo.Info.WorkingDirectory);
                return true;
            }

            return false;
        }

        
        internal static DiGitConfigRepository Get(Repository repo)
        {
            DiGitConfigRepository configRepo = null;
            _repos.TryGetValue(repo.Info.WorkingDirectory, out configRepo);
            return configRepo;
        }

        /// <summary>
        /// Initialize repository dictionary according to Configuration
        /// </summary>
        /// <param name="force"></param>
        /// <returns></returns>
        internal static bool ReadRepositories(bool force)
        {
            if (!force && _repos != null) return false;
            
            _repos = new Dictionary<string, DiGitConfigRepository>();
            var config = ConfigurationHelper.Configuration;

            List<DiGitConfigRepository> reposToDelete = new List<DiGitConfigRepository>();
            bool hadError = false;

            if (File.Exists(ConfigurationHelper.ConfigFile))
            {
                if (config.Repositories != null)
                {
                    foreach (DiGitConfigRepository configRepository in config.RepositoryList)
                    {
                        try
                        {
                            Repository repo = new Repository(configRepository.path);
                            Add(repo, configRepository);
                        }
                        catch (Exception ex)
                        {
                            hadError = true;

                            if (string.IsNullOrEmpty(configRepository.path))
                                ErrorHandler.Handle(new Exception("Empty path saved for repository - deleting."), false);
                            else
                                ErrorHandler.Handle(ex, "Could not load repository: {0}.", configRepository.path);

                            reposToDelete.Add(configRepository);
                        }
                    }

                    if (hadError)
                    {
                        config.RepositoryList.RemoveAll(reposToDelete.Contains);
                        ConfigurationHelper.Save();
                    }
                }
            }


            return _repos.Count > 0;
           
        }

        
    }
}
