using AvalonStudio.Extensibility;
using AvalonStudio.Shell;
using Dock.Model;
using Dock.Model.Controls;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace AvalonStudio.MVVM
{
    public interface IToolViewModel : IDockableViewModel
    {
        Location DefaultLocation { get; }
    }

    public interface IDockableViewModel
    {
        void OnSelected();

        void OnDeselected();

        bool OnClose();

        string Title { get; }
    }

    public abstract class ToolViewModel : ViewModel, IToolViewModel
    {
        private bool _isVisible;
        private bool _isSelected;

        // TODO This should use ToolControl
        private string _title;

        protected ToolViewModel() : this(null)
        {

        }

        protected ToolViewModel(string title)
        {
            _isVisible = true;

            _title = title;

            IsVisibleObservable = this.ObservableForProperty(x => x.IsVisible).Select(x => x.Value);         
        }

        public Action OnSelect { get; set; }

        public IObservable<bool> IsVisibleObservable { get; }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { this.RaiseAndSetIfChanged(ref _isVisible, value); }
        }        

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;

                this.RaisePropertyChanged();

                if (value)
                {
                    IoC.Get<IShell>().Select(this);
                }

                if(value && OnSelect != null)
                {
                    OnSelect();
                }
            }
        }

        public abstract Location DefaultLocation { get; }

        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        public bool OnClose()
        {
            IoC.Get<IShell>().CurrentPerspective.RemoveTool(this);
            return true;
        }

        public virtual void OnSelected()
        {
        }

        public void OnDeselected()
        {
            //IsSelected = false;
        }
    }
}