﻿using DiGit.Configuration;
using DiGit.Helpers;
using DiGit.ViewModel.Base;

namespace DiGit.ViewModel.Settings
{
    public class BaseSettingsViewModel : BaseViewModel
    {
        protected readonly DiGitConfig _config;

        public BaseSettingsViewModel()
        {
            _config = ConfigurationHelper.Configuration;
        }
    }
}
