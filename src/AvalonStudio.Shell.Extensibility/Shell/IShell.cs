using Avalonia.Controls;
using AvalonStudio.Documents;
using AvalonStudio.Extensibility.Dialogs;
using AvalonStudio.MVVM;
using Dock.Model;
using Dock.Model.Controls;
using System.Collections.Generic;

namespace AvalonStudio.Shell
{
    public interface IPerspective
    {
        void AddTool(IToolViewModel tool);

        void RemoveTool(IToolViewModel tool);

        void RemoveDock(IDock dock);

        IReadOnlyList<IToolViewModel> Tools { get; }

        IView Root { get; }

        ILayoutDock CenterPane { get; }

        IDocumentDock DocumentDock { get; }

        IToolViewModel SelectedTool { get; set; }
    }

	public interface IShell
	{
		IDocumentTabViewModel SelectedDocument { get; set; }

        //IToolViewModel SelectedTool { get; set; }

        void Select(object view);

        ModalDialogViewModelBase ModalDialog { get; set; }

		void AddDocument(IDocumentTabViewModel document, bool temporary = true);

		void RemoveDocument(IDocumentTabViewModel document);

        IPerspective MainPerspective { get; }

        IPerspective CreatePerspective();

        IPerspective CurrentPerspective { get; set; }

        IReadOnlyList<IDocumentTabViewModel> Documents { get; }

		IPanel Overlay { get; }
	}
}
