using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiGit.ViewModel.Base;

namespace DiGit.ViewModel
{
    public class TipsViewModel : BaseWindowViewModel
    {
        public Version NewVersion { get; set; }
        public Version PreviousVersion { get; set; }
    }
}
