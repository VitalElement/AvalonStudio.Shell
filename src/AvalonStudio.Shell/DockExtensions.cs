using AvalonStudio.Documents;
using AvalonStudio.MVVM;
using Dock.Model;
using Dock.Model.Controls;
using System.Linq;
using ReactiveUI;
using System;

namespace AvalonStudio.Shell
{
    abstract class AvalonStudioTab<T> : ViewModel, ITab where T : IDockableViewModel
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

        public IView Parent { get; set; }

        public bool OnClose()
        {
            return _model.OnClose();
        }

        public void OnSelected()
        {
            
        }
    }

    class AvalonStudioDocumentTab : AvalonStudioTab<IDocumentTabViewModel>, IDocumentTab
    {
        public AvalonStudioDocumentTab(IDocumentTabViewModel model) : base(model)
        {
        }
    }

    class AvalonStudioToolTab : AvalonStudioTab<IToolViewModel>, IToolTab
    {
        public AvalonStudioToolTab(IToolViewModel model) : base (model)
        {
        }
    }

    public static class DockExtensions
    {
        public static IView Dock(this IDock dock, IDockableViewModel model, bool add = true)
        {
            IView currentTab = null;

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

                dock.Views.Add(currentTab);
                dock.Factory.Update(currentTab, dock);
            }
            else
            {
                currentTab = dock.Views.FirstOrDefault(v => v.Context == model);

                if (currentTab != null)
                {
                    dock.Factory.Update(currentTab, currentTab.Parent);
                }
            }

            return currentTab;
        }
    }
}
