using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiGit.ViewModel.Base
{
    public class BaseDialogViewModel : BaseViewModel
    {
        public event CancelEventHandler RequestClose;

        protected virtual void OnRequestClose(bool cancel)
        {
            RequestClose?.Invoke(this, new CancelEventArgs(cancel));
        }

    }
}
