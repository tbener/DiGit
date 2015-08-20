using System.Windows.Input;
using DiGit.ViewModel;

namespace EhrInterceptionStudio.ViewModels
{
	public abstract class DialogViewModel : BaseViewModel
	{
		public abstract string Title { get; }
        public ICommand ClosedCommand { get; protected set; }
	}
}
