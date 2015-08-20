using System.ComponentModel;
using System.Windows;
using BondTech.HotKeyManagement.WPF._4;
using DiGit.Commands;
using DiGit.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DiGit.View;

namespace DiGit.ViewModel
{
    internal class SettingsViewModel : DependencyObject
    {
        HotKeyManager MyHotKeyManager;
        private string _path;
        private int _length;
        private string _results;

        public ICommand GoCommand { get; set; }

        public SettingsViewModel()
        {
            _length = 30;
            GoCommand = new RelayCommand(Refresh);
        }

        public void Refresh()
        {
            try
            {
                results = PathHelper.ShortDisplay(P, _length);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
                results = "[Error]";
            }
        }

        public string P
        {
            get { return _path; }
            set { _path = value; }
        }

        public string L
        {
            get { return _length.ToString(); }
            set
            {
                _length = int.Parse(value);
            }
        }



        public string results
        {
            get { return (string)GetValue(resultsProperty); }
            set { SetValue(resultsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for results.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty resultsProperty =
            DependencyProperty.Register("results", typeof(string), typeof(SettingsViewModel), new PropertyMetadata(""));

        
    }
}
