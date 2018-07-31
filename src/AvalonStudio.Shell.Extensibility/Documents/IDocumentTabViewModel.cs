using AvalonStudio.MVVM;
using Dock.Model.Controls;

namespace AvalonStudio.Documents
{
	public interface IDocumentTabViewModel : IDockableViewModel
	{
        bool IsDirty { get; set; }

		void Close();
	}
}
