using AvaloniaDemo.ViewModels.Views;
using AvalonStudio.Shell;
using Dock.Avalonia.Controls;
using Dock.Model;
using Dock.Model.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AvalonStudio.Docking
{
    public class AvalonStudioDocumentDock : DocumentDock
    {
        public override bool OnClose()
        {
            ShellViewModel.Instance.RemoveDock(this);
            return base.OnClose();
        }
    }

    public class AvalonStudioToolDock : ToolDock
    {
        public override bool OnClose()
        {
            ShellViewModel.Instance.RemoveDock(this);
            return base.OnClose();
        }
    }

    /// <inheritdoc/>
    public class DefaultLayoutFactory : DockFactory
    {
        public DocumentDock DocumentDock { get; private set; }
        public ToolDock RightDock { get; private set; }
        public ToolDock BottomDock { get; private set; }

        public LayoutDock RightPane { get; private set; }
        public LayoutDock CenterPane { get; private set; }

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
            // Documents
            DocumentDock = new DocumentDock
            {
                Id = "DocumentsPane",
                Proportion = double.NaN,
                Title = "DocumentsPane",
                CurrentView = null,
                IsCollapsable = false,
                Views = new ObservableCollection<IView>()
            };

            CenterPane = new LayoutDock
            {
                Id = "CenterPane",
                Proportion = double.NaN,
                Orientation = Orientation.Vertical,
                Title = "LeftPane",
                CurrentView = null,
                Views = CreateList<IView>
               (
                   DocumentDock
               )
            };

            // Main
            var mainLayout = new LayoutDock
            {
                Id = "MainLayout",
                Proportion = double.NaN,
                Orientation = Orientation.Horizontal,
                Title = "MainLayout",
                CurrentView = null,
                Views = new ObservableCollection<IView>
                {
                    CenterPane
                }
            };

            var mainView = new MainView
            {
                Id = "Main",
                Title = "Main",
                CurrentView = mainLayout,
                Views = new ObservableCollection<IView>
                {
                   mainLayout
                }
            };

            // Root

            var root = new RootDock
            {
                Id = "Root",
                Title = "Root",
                CurrentView = mainView,
                DefaultView = mainView,
                Views = new ObservableCollection<IView>
                {
                    mainView,
                }
            };

            return root;
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
                [nameof(RightDock)] = () => RightDock,
                [nameof(BottomDock)] = () => BottomDock,
                [nameof(DocumentDock)] = () => DocumentDock
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
