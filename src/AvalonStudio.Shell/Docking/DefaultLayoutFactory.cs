using AvaloniaDemo.ViewModels.Views;
using Dock.Avalonia.Controls;
using Dock.Model;
using Dock.Model.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AvalonStudio.Docking
{
    /// <inheritdoc/>
    public class DefaultLayoutFactory : DockFactory
    {
        private ObservableCollection<IView> _documents;
        private IDocumentDock _documentDock;
        private ILayoutDock _centerPane;

        public DefaultLayoutFactory()
        {
            _documents = new ObservableCollection<IView>();

            _documentDock = new DocumentDock
            {
                Id = "DocumentsPane",
                Proportion = double.NaN,
                Title = "DocumentsPane",
                CurrentView = null,
                IsCollapsable = false,
                Views = _documents
            };

            _centerPane = new LayoutDock
            {
                Id = $"CenterPane",
                Proportion = double.NaN,
                Orientation = Orientation.Vertical,
                Title = $"CenterPane",
                CurrentView = null,
                Views = CreateList<IView>
                (
                    _documentDock
                )
            };
        }

        public RootDock Root { get; private set; }

        public override IToolDock CreateToolDock()
        {
            return new AvalonStudioToolDock();
        }

        public override IDocumentDock CreateDocumentDock()
        {
            return new AvalonStudioDocumentDock();
        }

        /// <inheritdoc/>
        public override IDock CreateLayout()
        {
          //  MainLayout = CreatePerspectiveLayout("Main").root;
            // Root

            Root = new RootDock
            {
                Id = "Root",
                Title = "Root",
                Views = new ObservableCollection<IView>
                {
                    
                }
            };

            return Root;
        }

        public (DockBase root, ILayoutDock centerPane, IDocumentDock documentDock) CreatePerspectiveLayout(string identifier)
        {
            var debugLayout = new LayoutDock
            {
                Id = $"{identifier}Layout",
                Proportion = double.NaN,
                Orientation = Orientation.Vertical,
                Title = $"{identifier}Layout",
                CurrentView = null,
                Views = new ObservableCollection<IView>
                {
                    _centerPane
                }
            };

            var container = new LayoutDock
            {
                Id = $"{identifier}Container",
                Proportion = double.NaN,
                Orientation = Orientation.Horizontal,
                Title = $"{identifier}Container",
                CurrentView = null,
                Views = new ObservableCollection<IView>
                {
                    debugLayout
                }
            };

            return (new MainView
            {
                Id = identifier,
                Title = identifier,
                CurrentView = container,
                Views = new ObservableCollection<IView>
                {
                    container
                }
            }, debugLayout, _documentDock);
        }

        public override void Update(IView view, IView parent)
        {
            view.Parent = parent;

            if (view is IDock dock)
            {
                dock.Factory = this;

                if (dock.Views != null)
                {
                    foreach (var child in dock.Views)
                    {
                        Update(child, view);
                    }
                }

                if (dock.Windows != null)
                {
                    foreach (var child in dock.Windows)
                    {
                        Update(child, view);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void InitLayout(IView layout)
        {
            this.HostLocator = new Dictionary<string, Func<IDockHost>>
            {
                [nameof(IDockWindow)] = () => new HostWindow()
            };

            this.ViewLocator = new Dictionary<string, Func<IView>>
            {
                //[nameof(DebugCenterPane)] = () => DebugCenterPane,
                //[nameof(MainCenterPane)] = () => MainCenterPane,
            };

            this.Update(layout, null);

            if (layout is IDock layoutWindowsHost)
            {
                layoutWindowsHost.ShowWindows();
                if (layout is IDock layoutViewsHost)
                {
                    layoutViewsHost.CurrentView = layoutViewsHost.DefaultView;
                    if (layoutViewsHost.CurrentView is IDock currentViewWindowsHost)
                    {
                        currentViewWindowsHost.ShowWindows();
                    }
                }
            }
        }
    }
}
