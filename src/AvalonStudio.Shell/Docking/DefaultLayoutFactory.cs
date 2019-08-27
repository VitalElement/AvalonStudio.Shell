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
    public class DefaultLayoutFactory : Factory
    {
        private ObservableCollection<IDockable> _documents;
        private IDocumentDock _documentDock;
        private IDocumentDock _centerPane;

        public DefaultLayoutFactory()
        {
            _documents = new ObservableCollection<IDockable>();

            _documentDock = new DocumentDock
            {
                Id = "DocumentsPane",
                Proportion = double.NaN,
                Title = "DocumentsPane",
                ActiveDockable = null,
                IsCollapsable = false,
                VisibleDockables = _documents
            };

            _centerPane = new DocumentDock
            {
                Id = $"CenterPane",
                Proportion = double.NaN,                
                Title = $"CenterPane",
                ActiveDockable = null,
                VisibleDockables = CreateList<IDockable>
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
                VisibleDockables = new ObservableCollection<IDockable>
                {
                    
                }
            };

            return Root;
        }

        public (DockBase root, IProportionalDock centerPane, IDocumentDock documentDock) CreatePerspectiveLayout(string identifier)
        {
            var debugLayout = new ProportionalDock
            {
                Id = $"{identifier}Layout",
                Proportion = double.NaN,
                Orientation = Orientation.Vertical,
                Title = $"{identifier}Layout",
                ActiveDockable = null,
                VisibleDockables = new ObservableCollection<IDockable>
                {
                    _centerPane
                }
            };

            var container = new ProportionalDock
            {
                Id = $"{identifier}Container",
                Proportion = double.NaN,
                Orientation = Orientation.Horizontal,
                Title = $"{identifier}Container",
                ActiveDockable = null,
                VisibleDockables = new ObservableCollection<IDockable>
                {
                    debugLayout
                }
            };

            return (new MainView
            {
                Id = identifier,
                Title = identifier,
                ActiveDockable = container,
                VisibleDockables = new ObservableCollection<IDockable>
                {
                    container
                }
            }, debugLayout, _documentDock);
        }

        public override void UpdateDockable(IDockable view, IDockable parent)
        {
            view.Owner = parent;

            if (view is IDock dock)
            {
                dock.Factory = this;

                if (dock.VisibleDockables != null)
                {
                    foreach (var child in dock.VisibleDockables)
                    {
                        UpdateDockable(child, view);
                    }
                }

                if (dock.Windows != null)
                {
                    foreach (var child in dock.Windows)
                    {
                        UpdateDockWindow(child, view);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void InitLayout(IDockable layout)
        {
            this.HostWindowLocator = new Dictionary<string, Func<IHostWindow>>
            {
                [nameof(IDockWindow)] = () => new HostWindow()
            };

            this.DockableLocator = new Dictionary<string, Func<IDockable>>
            {
                //[nameof(DebugCenterPane)] = () => DebugCenterPane,
                //[nameof(MainCenterPane)] = () => MainCenterPane,
            };

            this.UpdateDockable(layout, null);

            if (layout is IDock layoutWindowsHost)
            {
                layoutWindowsHost.ShowWindows();
                if (layout is IDock layoutViewsHost)
                {
                    layoutViewsHost.ActiveDockable = layoutViewsHost.DefaultDockable;
                    if (layoutViewsHost.ActiveDockable is IDock currentViewWindowsHost)
                    {
                        currentViewWindowsHost.ShowWindows();
                    }
                }
            }
        }
    }
}
