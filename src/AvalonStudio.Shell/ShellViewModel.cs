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

namespace AvalonStudio.Shell
{
	[Export(typeof(ShellViewModel))]
    [Export(typeof(IShell))]
    [Shared]
    public class ShellViewModel : ViewModel, IShell
    {
        public static ShellViewModel Instance { get; set; }
        private List<KeyBinding> _keyBindings;
        private IPerspective _currentPerspective;

        private IDocumentTabViewModel _selectedDocument;

        private IEnumerable<Lazy<IExtension>> _extensions;
        private IEnumerable<Lazy<ToolViewModel>> _toolControls;
        private CommandService _commandService;
        private Dictionary<IDocumentTabViewModel, IDockable> _documentViews;
        private List<IDocumentTabViewModel> _documents;
        private List<IPerspective> _perspectives;

        private Lazy<StatusBarViewModel> _statusBar;

        private ModalDialogViewModelBase modalDialog;

        private IFactory _factory;
        private IRootDock _root;
        private IDock _layout;

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
            StandardToolbar = toolbars.SingleOrDefault(t => t.Key == "Standard").Value;

            _statusBar = statusBar;

            _keyBindings = new List<KeyBinding>();

            ModalDialog = new ModalDialogViewModelBase("Dialog");

            _documents = new List<IDocumentTabViewModel>();
            _documentViews = new Dictionary<IDocumentTabViewModel, IDockable>();
            _perspectives = new List<IPerspective>();

            this.WhenAnyValue(x => x.CurrentPerspective).Subscribe(perspective =>
            {
                if (perspective != null)
                {
                    //Root.Navigate(perspective.Root);
                    ApplyPerspective(perspective.Root);
                }
            });
        }

        public void RemoveDock(IDock dock)
        {
            CurrentPerspective.RemoveDock(dock);
        }

        public void Initialise(IFactory layoutFactory = null)
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

            _layout.WhenAnyValue(l => l.ActiveDockable).Subscribe(focused =>
			{
				if (focused?.Context is IDocumentTabViewModel doc)
				{
					SelectedDocument = doc;
				}
				else
				{
					SelectedDocument = null;
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

            _layout = Factory.CreateLayout();
            Factory.InitLayout(_layout);

            Factory.SetActiveDockable(_layout.VisibleDockables.First());

            Root = _layout as IRootDock;

            MainPerspective = CreateInitialPerspective();

            _documentDock = Root.Factory.FindDockable(Root, x => x.Id == "DocumentsPane") as IDock;

            CurrentPerspective = MainPerspective;
        }

        public IPerspective MainPerspective { get; private set; }

        public IFactory Factory
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

        public IRootDock CreatePerspective(IDock dock)
        {
            if (dock != null && dock.Owner is IDock owner)
            {
                var clone = (IRootDock)dock.Clone();
                
                if (clone != null)
                {
                    owner.Factory.AddDockable(owner, clone);
                    //ApplyPerspective(clone);
                }

                return clone;
            }

            throw new Exception();
        }

        public void ApplyPerspective(IRootDock dock)
        {
            if (dock != null)
            {
                if (Root is IDock root)
                {
                    root.Navigate(dock);
                    root.Factory.SetFocusedDockable(root, dock);
                    root.DefaultDockable = dock;
                }
            }
        }

        public IPerspective CreateInitialPerspective()
        {
            var result = new AvalonStudioPerspective(Root.ActiveDockable as IRootDock);

            _perspectives.Add(result);

            return result;
        }

        public IPerspective CreatePerspective()
        {
            var currentLayout = Root.ActiveDockable as IRootDock;
            var root = CreatePerspective(currentLayout);
            //ApplyPerspective(root);

            var result = new AvalonStudioPerspective(root);

            _perspectives.Add(result);

            return result;
        }

        public IPerspective CurrentPerspective
        {
            get => _currentPerspective;
            set => this.RaiseAndSetIfChanged(ref _currentPerspective, value);
        }

        private IDock _documentDock;

        public void AddDocument(IDocumentTabViewModel document, bool temporary = false, bool select = true)
        {
			if (!_documentViews.ContainsKey(document))
			{
				var view = _documentDock.Dock(document, !Documents.Contains(document));

				_documents.Add(document);

				_documentViews.Add(document, view);
			}

			if (select)
			{
				Factory.SetActiveDockable(_documentViews[document]);

				document.OnOpen();
			}
        }

        public void RemoveDocument(IDocumentTabViewModel document)
        {
            if (document == null)
            {
                return;
            }

			if(_documentViews[document].Owner is IDock dock)
			{
				dock.VisibleDockables.Remove(_documentViews[document]);
				dock.Factory.UpdateDockable(_documentViews[document], dock);
			}

            _documentViews.Remove(document);
			_documents.Remove(document);

			if(SelectedDocument == document)
			{
				SelectedDocument = null;
			}

			GC.Collect();
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

                if (value != null && _documentViews.ContainsKey(value))
                {
                    foreach(var perspective in _perspectives)
                    {
                        if(_documentViews[value].Owner is IDock dock)
                        {
                            if(dock.VisibleDockables.Contains(_documentViews[value]))
                            {
                                dock.ActiveDockable = _documentViews[value];
                            }
                        }
                    }
                }

                _selectedDocument = value;

                _selectedDocument?.OnSelected();

                this.RaisePropertyChanged(nameof(SelectedDocument));
            }
        }

        public void Select(object view)
        {
            if (view is IDocumentTabViewModel doc)
            {
                SelectedDocument = doc;
            }
            else if (view is IToolViewModel tool)
            {
                CurrentPerspective.SelectedTool = tool;
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
