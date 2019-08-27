using AvalonStudio.MVVM;
using Dock.Model;
using Dock.Model.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace AvalonStudio.Shell
{
	public class AvalonStudioPerspective : IPerspective
    {
        private List<IToolViewModel> _tools;
        private Dictionary<IToolViewModel, IDockable> _tabTools;
        private IDock _left;
        private IDock _right;
        private IDock _top;
        private IDock _bottom;

        public AvalonStudioPerspective (IRootDock root)
        {
            Root = root;
            _tools = new List<IToolViewModel>();
            _tabTools = new Dictionary<IToolViewModel, IDockable>();

            //CenterPane.WhenAnyValue(l => l.FocusedDockable).Subscribe(focused =>
            //{
            //    if (focused?.Context is IToolViewModel tool)
            //    {
            //        SelectedTool = tool;
            //    }
            //    else
            //    {
            //        SelectedTool = null;
            //    }
            //});
        }

        public IReadOnlyList<IToolViewModel> Tools => _tools.AsReadOnly();

        public IRootDock Root { get; }

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

            Root.Factory.SetActiveDockable(_tabTools[tool]);

			tool.OnOpen();
        }

        public void RemoveTool(IToolViewModel tool)
        {
            if(_tabTools.ContainsKey(tool))
            {
                if(_tabTools[tool].Owner is IDock dock)
                {
                    dock.Factory.RemoveDockable(_tabTools[tool], true);

                    if(dock.VisibleDockables.Count == 0)
                    {
                        if(dock == _left)
                        {
                            _left = null;
                        }
                        else if(dock == _right)
                        {
                            _right = null;
                        }
                        else if(dock == _top)
                        {
                            _top = null;
                        }
                        else if(dock == _bottom)
                        {
                            _bottom = null;
                        }
                    }
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
                    Root.Factory.SetActiveDockable(_tabTools[value]);
                }

                _selectedTool = value;

                _selectedTool?.OnSelected();

                //this.RaisePropertyChanged(nameof(SelectedTool));
            }
        }

        private IDockable DockOrCreate(IToolViewModel view)
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
                    if (_top != null)
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

            IProportionalDock container = null;

            if (orientation == Orientation.Horizontal)
            {
                container = Root.Factory.FindDockable(Root, x => x.Id == "HorizontalContainer") as IProportionalDock;
            }
            else
            {
                container = Root.Factory.FindDockable(Root, x => x.Id == "VerticalContainer") as IProportionalDock;
            }

            var documentDock = Root.Factory.FindDockable(Root, x => x.Id == "DocumentsPane") as IDock;

            var factory = Root.Factory;
            var toolDock = factory.CreateToolDock();
            toolDock.Id = nameof(IToolDock);
            toolDock.Title = nameof(IToolDock);
            toolDock.VisibleDockables = factory.CreateList<IDockable>();
            toolDock.Factory = factory;

            var currentView = toolDock.Dock(view);
            //toolDock.CurrentView = view;
            //toolDock.Views.Add(view);

            
            factory.SplitToDock(documentDock, toolDock, dockOperation);
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
                    if (_top == null)
                    {
                        _top = toolDock;
                    }
                    break;
            }

            return currentView;
        }
    }
}
