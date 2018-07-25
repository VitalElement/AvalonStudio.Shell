using Avalonia.Input;
using AvalonStudio.Commands;
using AvalonStudio.Docking;
using AvalonStudio.Documents;
using AvalonStudio.Extensibility;
using AvalonStudio.Extensibility.Dialogs;
using AvalonStudio.Extensibility.Shell;
using AvalonStudio.MainMenu;
using AvalonStudio.Menus.ViewModels;
using AvalonStudio.MVVM;
using AvalonStudio.Shell.Controls;
using AvalonStudio.Shell.Extensibility.Platforms;
using AvalonStudio.Toolbars;
using AvalonStudio.Toolbars.ViewModels;
using Dock.Model;
using Dock.Model.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace AvalonStudio.Shell
{
    [Export(typeof(ShellViewModel))]
    [Export(typeof(IShell))]
    [Shared]
    public class ShellViewModel : ViewModel, IShell
    {
        public static ShellViewModel Instance { get; set; }
        private List<KeyBinding> _keyBindings;

        private IDocumentTabViewModel _selectedDocument;

        private IEnumerable<Lazy<IExtension>> _extensions;
        private IEnumerable<Lazy<ToolViewModel>> _toolControls;
        private CommandService _commandService;
        private List<IDocumentTabViewModel> _documents;

        private Lazy<StatusBarViewModel> _statusBar;

        private ModalDialogViewModelBase modalDialog;

        private IDockFactory _factory;
        private IRootDock _root;

        [ImportingConstructor]
        public ShellViewModel(
            CommandService commandService,
            Lazy<StatusBarViewModel> statusBar,
            MainMenuService mainMenuService,
            ToolbarService toolbarService,
            [ImportMany] IEnumerable<Lazy<IExtension>> extensions,
            [ImportMany] IEnumerable<Lazy<ToolViewModel>> toolControls)
        {
            _extensions = extensions;
            _toolControls = toolControls;

            _commandService = commandService;

            MainMenu = mainMenuService.GetMainMenu();

            var toolbars = toolbarService.GetToolbars();
            StandardToolbar = toolbars.Single(t => t.Key == "Standard").Value;

            _statusBar = statusBar;

            _keyBindings = new List<KeyBinding>();

            ModalDialog = new ModalDialogViewModelBase("Dialog");

            _documents = new List<IDocumentTabViewModel>();

            this.WhenAnyValue(x => x.CurrentPerspective).Subscribe(perspective =>
            {
                if (perspective != null)
                {
                    Root.Navigate(perspective.Root);
                }
            });
        }

        public void RemoveDock(IDock dock)
        {
            CurrentPerspective.RemoveDock(dock);
        }

        public void Initialise(IDockFactory layoutFactory = null)
        {
            if (layoutFactory == null)
            {
                Factory = new DefaultLayoutFactory();
            }
            else
            {
                Factory = layoutFactory;
            }

            LoadLayout();

            foreach (var extension in _extensions)
            {
                if (extension.Value is IActivatableExtension activatable)
                {
                    activatable.BeforeActivation();
                }
            }

            Root.WhenAnyValue(l => l.FocusedView).Subscribe(focused =>
			{
				if (focused is IDocumentTabViewModel doc)
				{
					SelectedDocument = doc;
				}
				else
				{
					SelectedDocument = null;
				}

                if(focused is IToolViewModel tool)
                {
                    CurrentPerspective.SelectedTool = tool;
                }
                else
                {
                    CurrentPerspective.SelectedTool = null;
                }
			});

            foreach (var extension in _extensions)
            {
                if (extension.Value is IActivatableExtension activatable)
                {
                    activatable.Activation();
                }
            }

            foreach (var command in _commandService.GetKeyGestures())
            {
                foreach (var keyGesture in command.Value)
                {
                    _keyBindings.Add(new KeyBinding { Command = command.Key.Command, Gesture = KeyGesture.Parse(keyGesture) });
                }
            }

            IoC.Get<IStatusBar>().ClearText();
        }

        public string Title => Platform.AppName;

        public IReadOnlyList<IDocumentTabViewModel> Documents => _documents.AsReadOnly();

        public IRootDock Root
        {
            get => _root;
            set => this.RaiseAndSetIfChanged(ref _root, value);
        }

        public void LoadLayout()
        {
            //string path = System.IO.Path.Combine(Platform.SettingsDirectory, "Layout.json");

            //if (DockSerializer.Exists(path))
            //{
            //    //Layout = DockSerializer.Load<RootDock>(path);
            //}

            var layout = Factory.CreateLayout();
            Factory.InitLayout(layout, this);

            Root = layout as IRootDock;

            MainPerspective = CreatePerspective();

            CurrentPerspective = MainPerspective;
        }

        public IPerspective MainPerspective { get; private set; }

        public IDockFactory Factory
        {
            get => _factory;
            set => this.RaiseAndSetIfChanged(ref _factory, value);
        }

        public void CloseLayout()
        {
            //Layout.Close();
        }

        public void SaveLayout()
        {
            //string path = System.IO.Path.Combine(Platform.SettingsDirectory, "Layout.json");
            //DockSerializer.Save(path, Layout);
        }

        public MenuViewModel MainMenu { get; }

        public StatusBarViewModel StatusBar => _statusBar.Value;

        private ToolbarViewModel StandardToolbar { get; }

        public IEnumerable<KeyBinding> KeyBindings => _keyBindings;

        public IPerspective CreatePerspective()
        {
            var newPerspectiveLayout = (Root.Factory as DefaultLayoutFactory).CreatePerspectiveLayout("Name");
            Root.Factory.AddView(Root, newPerspectiveLayout.root, null);

            return new AvalonStudioPerspective(newPerspectiveLayout.root, newPerspectiveLayout.centerPane, newPerspectiveLayout.documentDock);
        }

        private IPerspective _currentPerspective;
        public IPerspective CurrentPerspective
        {
            get => _currentPerspective;
            set => this.RaiseAndSetIfChanged(ref _currentPerspective, value);
        }

        public void AddDocument(IDocumentTabViewModel document, bool temporary = false)
        {
            CurrentPerspective.DocumentDock.Dock(document, !Documents.Contains(document));

            _documents.Add(document);
        }

        public void RemoveDocument(IDocumentTabViewModel document)
        {
            if (document == null)
            {
                return;
            }

            // TODO implement save on close.

            /*if (document.Parent is IDock dock)
            {
                dock.Views.Remove(document);
                Factory.Update(document, document, dock);
            }*/

            _documents.Remove(document);
        }

        public ModalDialogViewModelBase ModalDialog
        {
            get { return modalDialog; }
            set { this.RaiseAndSetIfChanged(ref modalDialog, value); }
        }

        public IDocumentTabViewModel SelectedDocument
        {
            get => _selectedDocument;
            set
            {
                (_selectedDocument as IDocumentTabViewModel)?.OnDeselected();

                if (value != null)
                {
                    //Factory.SetCurrentView(value);
                }

                _selectedDocument = value;

                //(_selectedDocument as IDocumentTabViewModel)?.OnSelected();

                this.RaisePropertyChanged(nameof(SelectedDocument));
            }
        }

        public void Select(object view)
        {
            if (view is IDocumentTabViewModel doc)
            {
                SelectedDocument = doc;
            }
            else if (view is ToolViewModel tool)
            {
                //Root.Factory.SetCurrentView(tool);
            }
        }

        public void AddOrSelectDocument<T>(T document) where T : IDocumentTabViewModel
        {
            IDocumentTabViewModel doc = Documents.FirstOrDefault(x => x.Equals(document));

            if (doc != null)
            {
                SelectedDocument = doc;
            }
            else
            {
                AddDocument(document);
            }
        }

        public void AddOrSelectDocument<T>(Func<T> factory) where T : IDocumentTabViewModel
        {
            IDocumentTabViewModel doc = Documents.FirstOrDefault(x => x is T);

            if (doc != default)
            {
                SelectedDocument = doc;
            }
            else
            {
                AddDocument(factory());
            }
        }

        public T GetOrCreate<T>() where T : IDocumentTabViewModel, new()
        {
            T document = default;

            IDocumentTabViewModel doc = Documents.FirstOrDefault(x => x is T);

            if (doc != default)
            {
                document = (T)doc;
                SelectedDocument = doc;
            }
            else
            {
                document = new T();
                AddDocument(document);
            }
            return document;
        }

        public Avalonia.Controls.IPanel Overlay { get; internal set; }
    }
}
