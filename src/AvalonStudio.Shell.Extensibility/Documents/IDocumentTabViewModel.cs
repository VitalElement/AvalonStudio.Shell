using Dock.Model.Controls;

namespace AvalonStudio.Documents
{
	public interface IDocumentTabViewModel : IDocumentTab
	{
		void OnSelected();

		void OnDeselected();

		void Close();
	}
}
