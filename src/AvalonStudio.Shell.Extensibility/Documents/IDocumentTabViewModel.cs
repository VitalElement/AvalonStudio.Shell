using Dock.Model.Controls;

namespace AvalonStudio.Documents
{
	public interface IDocumentTabViewModel : IDocumentTab
	{
		void OnDeselected();

		void Close();
	}
}
