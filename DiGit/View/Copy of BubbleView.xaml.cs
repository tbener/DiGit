using System.Windows;

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
    }
}
