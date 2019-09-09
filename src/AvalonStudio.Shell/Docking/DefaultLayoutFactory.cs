using AvaloniaDemo.ViewModels.Views;
using Dock.Avalonia.Controls;
using Dock.Model;
using Dock.Model.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AvalonStudio.Docking
{
    /// <inheritdoc/>
    public class DefaultLayoutFactory : Factory
    {
        private ObservableCollection<IDockable> _documents;

        public DefaultLayoutFactory()
        {
            _documents = new ObservableCollection<IDockable>();
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
            var documentDock = new AvalonStudioDocumentDock
            {
                Id = "DocumentsPane",
                Proportion = double.NaN,
                Title = "DocumentsPane",
                ActiveDockable = null,
                IsCollapsable = false,
                VisibleDockables = _documents
            };

            var verticalContainer = new ProportionalDock
            {
                Id = "VerticalContainer",
                Proportion = double.NaN,
                Orientation = Orientation.Vertical,
                Title = "VerticalContainer",
                ActiveDockable = null,
                VisibleDockables = new ObservableCollection<IDockable>
                {
                    documentDock,
                }
            };

            var horizontalContainer = new ProportionalDock
            {
                Id = "HorizontalContainer",
                Proportion = double.NaN,
                Orientation = Orientation.Horizontal,
                Title = "HorizontalContainer",
                ActiveDockable = null,
                VisibleDockables = new ObservableCollection<IDockable>
                {
                    verticalContainer,
                }
            };

            var mainLayout = new RootDock
            {
                Id = "Perspective",
                Title = "Perspective",
                ActiveDockable = horizontalContainer,
                VisibleDockables = new ObservableCollection<IDockable>
                {
                    horizontalContainer
                }
            };

            Root = new RootDock
            {
                Id = "Root",
                Title = "Root",
                Top = new PinDock
                {
                    Alignment = Alignment.Top
                },
                Bottom = new PinDock
                {
                    Alignment = Alignment.Bottom
                },
                Left = new PinDock
                {
                    Alignment = Alignment.Left
                },
                Right = new PinDock
                {
                    Alignment = Alignment.Right
                },
                ActiveDockable = mainLayout,
                VisibleDockables = new ObservableCollection<IDockable>
                {
                    mainLayout
                }
            };

            Root.WhenAnyValue(x => x.VisibleDockables)
                .Subscribe(x =>
                {

                });

            (Root.VisibleDockables as ObservableCollection<IDockable>).CollectionChanged += DefaultLayoutFactory_CollectionChanged;

            return Root;
        }

        private void DefaultLayoutFactory_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
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
