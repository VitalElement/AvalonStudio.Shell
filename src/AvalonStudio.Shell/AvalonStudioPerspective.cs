using AvalonStudio.MVVM;
using Dock.Model;
using Dock.Model.Controls;
using System;
using System.Collections.Generic;

namespace AvalonStudio.Shell
{
    public class AvalonStudioPerspective : IPerspective
    {
        private List<IToolViewModel> _tools;
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
        }

        public void AddTool(IToolViewModel tool)
        {
            if (!_tools.Contains(tool))
            {
                _tools.Add(tool);
            }

            DockOrCreate(tool);
        }

        public void RemoveTool(IToolViewModel tool)
        {
            if (tool.Parent is IDock dock)
            {
                dock.Views.Remove(tool);
                dock.Factory.Update(tool, tool, dock);
            }

            _tools.Remove(tool);
        }

        private IDock DockOrCreate(IToolViewModel view)
        {
            switch (view.DefaultLocation)
            {
                case Location.Left:
                    if (_left != null)
                    {
                        _left.Dock(view);
                        return _left;
                    }
                    break;

                case Location.Right:
                    if (_right != null)
                    {
                        _right.Dock(view);
                        return _right;
                    }
                    break;

                case Location.Bottom:
                    if (_bottom != null)
                    {
                        _bottom.Dock(view);
                        return _bottom;
                    }
                    break;
            }

            var orientation = Orientation.Horizontal;
            var dockOperation = DockOperation.Left;

            switch (view.DefaultLocation)
            {
                case Location.Bottom:
                    orientation = Orientation.Vertical;
                    break;
            }

            switch (view.DefaultLocation)
            {
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

            if (index < 0 || index >= parentDock.Views.Count)
            {
                var factory = CenterPane.Factory;
                var toolDock = factory.CreateToolDock();
                toolDock.Id = nameof(IToolDock);
                toolDock.Title = nameof(IToolDock);
                toolDock.CurrentView = view;
                toolDock.Views = factory.CreateList<IView>();
                toolDock.Views.Add(view);

                factory.Split(CenterPane, toolDock, dockOperation);
                toolDock.Proportion = 0.15;

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
                }

                return toolDock;
            }

            throw new NotSupportedException();
        }
    }
}
