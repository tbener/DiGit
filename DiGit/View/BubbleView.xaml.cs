using DiGit.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace DiGit.View
{
    /// <summary>
    /// Interaction logic for BubbleView.xaml
    /// </summary>
    public partial class BubbleView : Window
    {
        public BubbleView()
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

            Deactivated += delegate
            {
                try
                {
                    this.Topmost = true;
                    this.Activate();
                }
                catch
                { }
            };
        }

        private void CommandsContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            var cm = sender as ContextMenu;
            if (cm != null)
            {
                cm.PlacementTarget = MainBorder;
                cm.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            }
        }


        private void CommandsContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as BubbleViewModel;
            if (vm != null)
                vm.IsShowMenu = false;
        }
    }
}
