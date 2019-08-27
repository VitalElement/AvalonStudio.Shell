using AvalonStudio.Documents;
using AvalonStudio.MVVM;
using Dock.Model;
using Dock.Model.Controls;
using ReactiveUI;
using System;
using System.Linq;

namespace AvalonStudio.Shell
{
	abstract class AvalonStudioTab<T> : ViewModel, IDockable where T : IDockableViewModel
    {
        T _model;

        public AvalonStudioTab(T model)
        {
            _model = model;
            Context = model;
            Id = "ASTab";

            model.WhenAnyValue(x => x.Title)
                .Subscribe(title => Title = title);
        }

        public string Id { get; set; }


        private string _title;

        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }


        private object _context;

        public object Context
        {
            get { return _context; }
            set { this.RaiseAndSetIfChanged(ref _context, value); }
        }

        public IDockable Owner { get; set; }

        public bool OnClose()
        {
            return _model.OnClose();
        }

		public void OnOpen()
		{
			_model.OnOpen();
		}

        public void OnSelected()
        {
            
        }

        public IDockable Clone()
        {
            throw new NotImplementedException();
        }
    }

    class AvalonStudioDocumentTab : AvalonStudioTab<IDocumentTabViewModel>, IDocument
    {
        public AvalonStudioDocumentTab(IDocumentTabViewModel model) : base(model)
        {
        }
    }

    class AvalonStudioToolTab : AvalonStudioTab<IToolViewModel>, ITool
    {
        public AvalonStudioToolTab(IToolViewModel model) : base (model)
        {
        }
    }

    public static class DockExtensions
    {
        public static IDockable Dock(this IDock dock, IDockableViewModel model, bool add = true)
        {
            IDockable currentTab = null;

            if (add)
            {
                if (model is IToolViewModel toolModel)
                {
                    currentTab = new AvalonStudioToolTab(toolModel);
                }
                else if(model is IDocumentTabViewModel documentModel)
                {
                    currentTab = new AvalonStudioDocumentTab(documentModel);
                }

                dock.VisibleDockables.Add(currentTab);
                dock.Factory.UpdateDockable(currentTab, dock);
            }
            else
            {
                currentTab = dock.VisibleDockables.FirstOrDefault(v => v.Context == model);

                if (currentTab != null)
                {
                    dock.Factory.UpdateDockable(currentTab, currentTab.Owner);
                }
            }

            return currentTab;
        }
    }
}
