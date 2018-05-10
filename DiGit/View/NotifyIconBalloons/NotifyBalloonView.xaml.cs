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
using System.Windows.Shapes;

namespace DiGit.View.NotifyIconBalloons
{
    /// <summary>
    /// Interaction logic for NotifyBalloonView.xaml
    /// </summary>
    public partial class NotifyBalloonView : Window
    {
        public NotifyBalloonView()
        {
            InitializeComponent();

            MouseDown += delegate
            {
                try { DragMove(); }
                catch { }
            };
        }


        public Action OnFadeComplete { get; set; }
        private void FadeOut_Completed(object sender, EventArgs e)
        {
            OnFadeComplete();
        }
    }
}
