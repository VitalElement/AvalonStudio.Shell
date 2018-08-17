using AvalonStudio.MVVM;
using Dock.Model;
using Dock.Model.Controls;
using System;
using System.Collections.Generic;
using ReactiveUI;

namespace AvalonStudio.Shell
{
    public class AvalonStudioPerspective : IPerspective
    {
        private List<IToolViewModel> _tools;
        private Dictionary<IToolViewModel, IView> _tabTools;
        private IDock _left;
        private IDock _right;
        private IDock _top;
        private IDock _bottom;

        public AvalonStudioPerspective (IView root, ILayoutDock centerPane, IDocumentDock documentDock)
        {
            DocumentDock = documentDock;
            CenterPane = centerPane;
            Root = root;
            _tools = new List<IToolViewModel>();
            _tabTools = new Dictionary<IToolViewModel, IView>();

            CenterPane.WhenAnyValue(l => l.FocusedView).Subscribe(focused =>
            {
                if (focused?.Context is IToolViewModel tool)
                {
                    SelectedTool = tool;
                }
                else
                {
                    SelectedTool = null;
                }
            });
        }

        public IReadOnlyList<IToolViewModel> Tools => _tools.AsReadOnly();

        public IView Root { get; }

        public ILayoutDock CenterPane { get; }

        public IDocumentDock DocumentDock { get; }

        public void RemoveDock(IDock dock)
        {
            if (dock == _left)
            {
                _left = null;
            }
            else if (dock == _right)
            {
                _right = null;
            }
            else if (dock == _bottom)
            {
                _bottom = null;
            }
            else if(dock == _top)
            {
                _top = null;
            }
        }

        public void AddTool(IToolViewModel tool)
        {
            if(!_tabTools.ContainsKey(tool))
            {
                _tabTools.Add(tool, DockOrCreate(tool));
                _tools.Add(tool);
            }
            else
            {
                DockOrCreate(tool);
            }

            CenterPane.Factory.SetCurrentView(_tabTools[tool]);
        }

        public void RemoveTool(IToolViewModel tool)
        {
            if(_tabTools.ContainsKey(tool))
            {
                if(_tabTools[tool].Parent is IDock dock)
                {
                    dock.Views.Remove(_tabTools[tool]);
                    dock.Factory.Update(_tabTools[tool], dock);
                }

                _tabTools.Remove(tool);
                _tools.Remove(tool);
            }
        }

        private IToolViewModel _selectedTool;

        public IToolViewModel SelectedTool
        {
            get => _selectedTool;
            set
            {
                if (value != null && _tabTools.ContainsKey(value))
                {
                    _selectedTool?.OnDeselected();
                    CenterPane.Factory.SetCurrentView(_tabTools[value]);
                }

                _selectedTool = value;

                _selectedTool?.OnSelected();

                //this.RaisePropertyChanged(nameof(SelectedTool));
            }
        }

        private IView DockOrCreate(IToolViewModel view)
        {
            switch (view.DefaultLocation)
            {
                case Location.Left:
                    if (_left != null)
                    {
                        return _left.Dock(view);
                    }
                    break;

                case Location.Right:
                    if (_right != null)
                    {
                        return _right.Dock(view);
                    }
                    break;

                case Location.Bottom:
                    if (_bottom != null)
                    {
                        return _bottom.Dock(view);
                    }
                    break;

                case Location.Top:
                    if(_top != null)
                    {
                        return _top.Dock(view);
                    }
                    break;
            }

            var orientation = Orientation.Horizontal;
            var dockOperation = DockOperation.Left;

            switch (view.DefaultLocation)
            {
                case Location.Top:
                case Location.Bottom:
                    orientation = Orientation.Vertical;
                    break;
            }

            switch (view.DefaultLocation)
            {
                case Location.Top:
                    dockOperation = DockOperation.Top;
                    break;

                case Location.Right:
                    dockOperation = DockOperation.Right;
                    break;

                case Location.Bottom:
                    dockOperation = DockOperation.Bottom;
                    break;

            }

            var parentDock = CenterPane as ILayoutDock;
            var containedElement = DocumentDock as IDock;

            while (true)
            {
                if (parentDock.Orientation == orientation)
                {
                    break;
                }

                containedElement = parentDock;
                parentDock = parentDock.Parent as ILayoutDock;
            }

            var index = parentDock.Views.IndexOf(containedElement);

            void advanceIndex()
            {
                switch (view.DefaultLocation)
                {
                    case Location.Top:
                    case Location.Left:
                        index--;
                        break;

                    case Location.Right:
                    case Location.Bottom:
                        index++;
                        break;

                }
            }

            advanceIndex();

            if (index <= 0 || index >= parentDock.Views.Count)
            {
                var factory = CenterPane.Factory;
                var toolDock = factory.CreateToolDock();
                toolDock.Id = nameof(IToolDock);
                toolDock.Title = nameof(IToolDock);
                toolDock.Views = factory.CreateList<IView>();
                toolDock.Factory = factory;

                var currentView = toolDock.Dock(view);
                //toolDock.CurrentView = view;
                //toolDock.Views.Add(view);

                factory.Split(CenterPane, toolDock, dockOperation);
                toolDock.Proportion = 0.2;

                switch (view.DefaultLocation)
                {
                    case Location.Left:
                        if (_left == null)
                        {
                            _left = toolDock;
                        }
                        break;

                    case Location.Right:
                        if (_right == null)
                        {
                            _right = toolDock;
                        }
                        break;

                    case Location.Bottom:
                        if (_bottom == null)
                        {
                            _bottom = toolDock;
                        }
                        break;

                    case Location.Top:
                        if(_top == null)
                        {
                            _top = toolDock;
                        }
                        break;
                }

                return currentView;
            }

            throw new NotSupportedException();
        }
    }
}
