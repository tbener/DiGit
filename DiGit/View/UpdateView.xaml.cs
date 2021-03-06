﻿using DiGit.ViewModel;
using FirstFloor.ModernUI.Windows.Controls;

namespace DiGit.View
{
    /// <summary>
    /// Interaction logic for UpdateView.xaml
    /// </summary>
    public partial class UpdateView : ModernWindow
    {
        public UpdateView()
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

            this.DataContext = new UpdateViewModel();

        }
    }
}
