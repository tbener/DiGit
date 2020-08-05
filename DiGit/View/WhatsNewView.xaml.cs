using DiGit.ViewModel;
using FirstFloor.ModernUI.Windows.Controls;

namespace DiGit.View
{
    /// <summary>
    /// Interaction logic for WhatsNewView.xaml
    /// </summary>
    public partial class WhatsNewView : ModernWindow
    {
        public WhatsNewView()
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

            this.DataContext = new WhatsNewViewModel();
        }
    }
}
