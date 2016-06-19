using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DiGit.Commands;
using DiGit.Helpers;
using DiGit.Model;

namespace DiGit.ViewModel
{
    public class PathClassViewModel
    {
        public ICommand SetFavorite { get; set; }

        public PathClassViewModel(PathClass pathClass)
        {
            this.PathClass = pathClass;
            SetFavorite = new RelayCommand(() => Msg.Show("Hello test"));

        }

        public PathClass PathClass { get; set; }
    }
}
