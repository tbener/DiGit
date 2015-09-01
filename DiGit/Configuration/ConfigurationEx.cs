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

namespace DiGit.Configuration
{
    

    public partial class DiGitConfigRepository
    {
        private Repository _repository;
        private Window _view;

        [XmlIgnore]
        public Window View
        {
            get { return _view; }
            set
            {
                _view = value;
                _view.Visibility = this.isActive ? Visibility.Visible : Visibility.Hidden;
            }
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

        
    }

    public partial class DiGitConfig
    {
        public DiGitConfig()
        {
            
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

    public partial class DiGitConfigSettingsVisualSettings : INotifyPropertyChanged
    {

        [XmlIgnore]
        public double BubblesOpacity
        {
            get { return this._bubblesOpacity; }
            set
            {
                this._bubblesOpacity = Math.Round(value, 2, MidpointRounding.AwayFromZero);
                OnPropertyChanged("BubbleOpacity");
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

        public void Apply()
        {
            HotkeyHelper.RegisterHotkey(ModifierKeys, key);
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

    public partial class DiGitConfigCommand : ICommand
    {

        public void Execute(object parameter)
        {
            string args = arguments;
            Repository repo = parameter as Repository;
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = fileName;
                if (!String.IsNullOrEmpty(arguments))
                {
                    if (repo != null) 
                        args = arguments.Replace("{rep_path}", "\"" + repo.Info.WorkingDirectory + "\"");
                    p.StartInfo.Arguments = args;
                }
                if (!String.IsNullOrEmpty(windowStyle))
                    p.StartInfo.WindowStyle = (ProcessWindowStyle)Enum.Parse(typeof(ProcessWindowStyle), windowStyle);
                p.Start();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex, "An error ocurred occurred while trying to run command: {0} {1}", fileName, args == "{rep_path}" ? arguments : args);
            }
        }

        public override string ToString()
        {
            return header;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

    }

    
}
