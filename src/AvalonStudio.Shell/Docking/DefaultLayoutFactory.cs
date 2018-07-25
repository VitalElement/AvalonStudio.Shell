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
        private DocumentDock _documentDock;

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
            var documents = new ObservableCollection<IView>();

            // Documents
            _documentDock = new DocumentDock
            {
                Id = "DocumentsPane",
                Proportion = double.NaN,
                Title = "DocumentsPane",
                CurrentView = null,
                IsCollapsable = false,
                Views = documents
            };

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
            var centerPane = new LayoutDock
            {
                Id = $"{identifier}CenterPane",
                Proportion = double.NaN,
                Orientation = Orientation.Vertical,
                Title = $"{identifier}CenterPane",
                CurrentView = null,
                Views = CreateList<IView>
                (
                    _documentDock
                )
            };

            var debugLayout = new LayoutDock
            {
                Id = $"{identifier}Layout",
                Proportion = double.NaN,
                Orientation = Orientation.Horizontal,
                Title = $"{identifier}Layout",
                CurrentView = null,
                Views = new ObservableCollection<IView>
                {
                    centerPane
                }
            };

            return (new MainView
            {
                Id = identifier,
                Title = identifier,
                CurrentView = debugLayout,
                Views = new ObservableCollection<IView>
                {
                    debugLayout
                }
            }, centerPane, _documentDock);
        }

        /// <inheritdoc/>
        public override void InitLayout(IView layout, object context)
        {
            this.ContextLocator = new Dictionary<string, Func<object>>
            {
                // Defaults
                [nameof(IRootDock)] = () => context,
                [nameof(ILayoutDock)] = () => context,
                [nameof(IDocumentDock)] = () => context,
                [nameof(IToolDock)] = () => context,
                [nameof(ISplitterDock)] = () => context,
                [nameof(IDockWindow)] = () => context,
                // Documents
                ["Document1"] = () => context,
                ["Document2"] = () => context,
                ["Document3"] = () => context,
                // Tools
                ["Editor"] = () => layout,
                ["LeftTop1"] = () => context,
                ["LeftTop2"] = () => context,
                ["LeftTop3"] = () => context,
                ["LeftBottom1"] = () => context,
                ["LeftBottom2"] = () => context,
                ["LeftBottom3"] = () => context,
                ["RightTop1"] = () => context,
                ["RightTop2"] = () => context,
                ["RightTop3"] = () => context,
                ["RightBottom1"] = () => context,
                ["RightBottom2"] = () => context,
                ["RightBottom3"] = () => context,
                ["LeftPane"] = () => context,
                ["LeftPaneTop"] = () => context,
                ["LeftPaneTopSplitter"] = () => context,
                ["LeftPaneBottom"] = () => context,
                ["RightPane"] = () => context,
                ["RightPaneTop"] = () => context,
                ["RightPaneTopSplitter"] = () => context,
                ["RightPaneBottom"] = () => context,
                ["DocumentsPane"] = () => context,
                ["MainLayout"] = () => context,
                ["LeftSplitter"] = () => context,
                ["RightSplitter"] = () => context,
                // Layouts
                ["MainLayout"] = () => context,
                // Views
                ["Home"] = () => layout,
                ["Main"] = () => context
            };

            this.HostLocator = new Dictionary<string, Func<IDockHost>>
            {
                [nameof(IDockWindow)] = () => new HostWindow()
            };

            this.ViewLocator = new Dictionary<string, Func<IView>>
            {
                //[nameof(DebugCenterPane)] = () => DebugCenterPane,
                //[nameof(MainCenterPane)] = () => MainCenterPane,
            };

            this.Update(layout, context, null);

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
