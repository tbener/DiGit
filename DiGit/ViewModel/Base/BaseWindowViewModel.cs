using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DiGit.Commands;

namespace DiGit.ViewModel.Base
{
    public class BaseWindowViewModel : BaseDialogViewModel
    {
        public ICommand CloseCommand { get; set; }

        public BaseWindowViewModel()
        {
            CloseCommand = new RelayCommand(() => OnRequestClose(true));
        }

    }
}
