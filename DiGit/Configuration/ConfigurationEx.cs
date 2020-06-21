using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using DiGit.Helpers;
using DiGit.Model;
using LibGit2Sharp;
using Microsoft.Win32;
using System.Collections.Generic;

namespace DiGit.Configuration
{


    public partial class DiGitConfigRepository : IComparable
    {
        private Repository _repository;
        private Window _view;
        private bool _isNew = false;

        [XmlIgnore]
        public Window View
        {
            get { return _view; }
            set
            {
                _view = value;
                _view.Visibility = this.isActive ? Visibility.Visible : Visibility.Hidden;
                _view.Left = left;
                _view.Top = top;
            }
        }

        public void UpdateLocation()
        {
            if (_view != null)
            {
                left = _view.Left;
                top = _view.Top;
            }
        }

        [XmlIgnore]
        public bool IsNew
        {
            get
            {
                if (_isNew)
                {
                    _isNew = false;
                    return true;
                }
                return false;
            }
            set { _isNew = value; }

        }

        [XmlIgnore]
        public Repository Repository
        {
            get { return _repository; }
            set
            {
                _repository = value;
                this.path = _repository.Info.WorkingDirectory;
            }
        }

        public int CompareTo(object obj)
        {
            var otherRepo = (DiGitConfigRepository)obj;
            return View.Left.CompareTo(otherRepo.View.Left);
        }
    }

    public partial class DiGitConfig
    {
        private List<DiGitConfigRepository> _repoList;

        public DiGitConfig()
        {

        }

        [XmlIgnore]
        public List<DiGitConfigRepository> RepositoryList
        {
            get
            {
                if (_repoList == null)
                {
                    if (Repositories == null)
                        _repoList = new List<DiGitConfigRepository>();
                    else
                        _repoList = new List<DiGitConfigRepository>(Repositories);

                }

                return _repoList;
            }
            set
            {
                _repoList = value;
            }
        }


        [XmlIgnore]
        public bool StartWithWindows
        {
            get
            {
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                string val = rkApp.GetValue("DiGit", "").ToString();
                return val.Length > 0;
            }
            set
            {
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (value)
                    rkApp.SetValue("DiGit", AppInfo.AppExeFullPath);
                else
                    try
                    {
                        rkApp.DeleteValue("DiGit");
                    }
                    catch { }
            }
        }

    }

    public partial class DiGitConfigSettingsBubbles : INotifyPropertyChanged
    {
        private HorizontalAlignment anchor = HorizontalAlignment.Right;

        [XmlIgnore]
        public double Opacity
        {
            get { return this._opacity; }
            set
            {
                this._opacity = Math.Round(value, 2, MidpointRounding.AwayFromZero);
                OnPropertyChanged("Opacity");
            }
        }

        [XmlIgnore]
        public HorizontalAlignment PositionAnchor
        {
            get
            {
                Enum.TryParse(this._positionAnchor, out anchor);
                return anchor;
            }
            set
            {
                anchor = value;
                _positionAnchor = anchor.ToString();
            }
        }

        #region INotifyPropertyChanged Members and helper

        readonly NotifyPropertyChangedHelper _propertyChangeHelper = new NotifyPropertyChangedHelper();

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChangeHelper.Add(value); }
            remove { _propertyChangeHelper.Remove(value); }
        }

        protected void SetValue<T>(ref T field, T value, params string[] propertyNames)
        {
            _propertyChangeHelper.SetValue(this, ref field, value, propertyNames);
        }

        public void OnPropertyChanged(string propertyName)
        {
            _propertyChangeHelper.NotifyPropertyChanged(this, propertyName);
        }

        public void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            OnPropertyChanged(ExtractPropertyName(propertyExpression));
        }

        protected static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpression = propertyExpression.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");
            }

            var property = memberExpression.Member as PropertyInfo;

            if (property == null)
            {
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");
            }

            return memberExpression.Member.Name;
        }


        #endregion
    }

    public partial class DiGitConfigSettingsShowHideHotkey
    {
        private ModifierKeys _modifier;

        public DiGitConfigSettingsShowHideHotkey()
        {

        }

        public DiGitConfigSettingsShowHideHotkey(ModifierKeys modifiersKeys, string key)
        {
            Set(modifiersKeys, key);
        }

        public void Set(ModifierKeys modifiersKeys, string key)
        {
            this.ModifierKeys = modifiersKeys;
            this.key = key;
            Apply();
        }

        public void Apply(bool showNotification = true)
        {
            try
            {
                HotkeyHelper.RegisterHotkey(ModifierKeys, key);
                if (showNotification)
                    NotificationHelper.ShowNotification("DiGit Hotkey", $"Hotkey set successfully.\nClick [{HotkeyHelper.HotkeyShortcut}] to show/hide bubbles.");
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, false);
                if (showNotification)
                    NotificationHelper.ShowNotification("DiGit Hotkey Error", "Show/hide hotkey not set");
            }
        }

        private ModifierKeys ModifierKeys
        {
            get
            {
                if (_modifier == ModifierKeys.None)
                    _modifier = (ModifierKeys)new ModifierKeysConverter().ConvertFromString(this.modifiers);
                return _modifier;
            }
            set
            {
                _modifier = value;
                this.modifiers = new ModifierKeysConverter().ConvertToString(_modifier);
            }
        }
    }


}
