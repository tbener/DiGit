using DiGit.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using LibGit2Sharp;

namespace DiGit.ViewModel
{
    public abstract class BaseViewModel : DependencyObject, INotifyPropertyChanged
    {
        private Repository _repo;
        public event CancelEventHandler RequestClose;

        protected BaseViewModel()
        {
            
        }

        protected BaseViewModel(Repository repo) :this()
        {
            _repo = repo;
        }

        public Repository Repo
        {
            get { return _repo; }
            set { _repo = value; }
        }

        public double BubbleOpacity
        {
            get { return ConfigurationHelper.Configuration.Settings.VisualSettings.BubblesOpacity; }
            set
            {
                ConfigurationHelper.Configuration.Settings.VisualSettings.BubblesOpacity = value;
            }
        }


        protected virtual void OnRequestClose(bool cancel)
        {
            if (RequestClose != null)
                RequestClose(this, new CancelEventArgs(cancel));
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
}
