
using DiGit.ViewModel;
using FirstFloor.ModernUI.Windows.Controls;

namespace DiGit.View
{
    /// <summary>
    /// Interaction logic for TipsView.xaml
    /// </summary>
    public partial class TipsView : ModernWindow
    {
        public TipsView()
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

            DataContext = new TipsViewModel();
        }
    }
}
