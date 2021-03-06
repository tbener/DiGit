﻿using DiGit.ViewModel;
using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DiGit.View
{
    /// <summary>
    /// Interaction logic for ModernDialog1.xaml
    /// </summary>
    public partial class AboutView : ModernDialog
    {
        public AboutView()
        {
            InitializeComponent();

            MouseDown += delegate
            {
                try
                {
                    DragMove();
                }
                catch
                { }
            };

            // define the dialog buttons
            this.Buttons = new Button[] { this.CloseButton };
            this.CloseButton.Content = "Close";
            this.CloseButton.IsCancel = true;
            this.CloseButton.IsDefault = true;

            this.DataContext = new AboutViewModel();
        }
    }
}
