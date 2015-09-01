using System.Windows.Input;
using DiGit.ViewModel.Base;

namespace DiGit.ViewModel
{
	public abstract class DialogViewModel : BaseViewModel
	{
		public abstract string Title { get; }
        public ICommand ClosedCommand { get; protected set; }
	}
}
