using AvalonStudio.MVVM;

namespace AvalonStudio.Documents
{
	public interface IDocumentTabViewModel : IDockableViewModel
	{
        bool IsDirty { get; set; }
	}
}
