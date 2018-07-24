using Avalonia.Controls;
using AvalonStudio.Documents;
using AvalonStudio.Extensibility.Dialogs;
using AvalonStudio.MVVM;
using Dock.Model;
using System.Collections.Generic;

namespace AvalonStudio.Shell
{
	public interface IShell
	{
		IDocumentTabViewModel SelectedDocument { get; set; }

        IToolViewModel SelectedTool { get; set; }

        void Select(object view);

        ModalDialogViewModelBase ModalDialog { get; set; }

		void AddDocument(IDocumentTabViewModel document, bool temporary = true);

		void RemoveDocument(IDocumentTabViewModel document);

        void AddTool(IToolViewModel tool);

        void RemoveTool(IToolViewModel tool);

		IReadOnlyList<IDocumentTabViewModel> Documents { get; }

        IReadOnlyList<IToolViewModel> Tools { get; }

		IDock Layout { get; }

		IPanel Overlay { get; }
	}
}
