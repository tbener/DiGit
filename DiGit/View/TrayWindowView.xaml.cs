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
using DiGit.ViewModel;

namespace DiGit.View
{
    /// <summary>
    /// Interaction logic for TrayWindowView.xaml
    /// </summary>
    public partial class TrayWindowView : Window
    {
        public TrayWindowView()
        {
            InitializeComponent();
            this.DataContext = new TrayWindowViewModel(this.TaskbarIcon);
        }
    }
}
