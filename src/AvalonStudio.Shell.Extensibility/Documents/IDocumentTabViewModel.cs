using AvalonStudio.MVVM;
using Dock.Model.Controls;

namespace AvalonStudio.Documents
{
	public interface IDocumentTabViewModel : IDockableViewModel
	{
		void OnDeselected();

        bool OnClose();

		void Close();
	}
}
