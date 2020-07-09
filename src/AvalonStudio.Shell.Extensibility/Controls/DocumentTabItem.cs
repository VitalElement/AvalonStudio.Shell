using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
using System;

namespace AvalonStudio.Controls
{
    public class DocumentTabItem : ContentControl
    {
        public DocumentTabItem()
        {
            this.GetObservable(DockPanel.DockProperty).Subscribe(dock =>
            {
                var parent = Parent;
            });
        }

        public static readonly StyledProperty<string> TitleProprty =
            AvaloniaProperty.Register<DocumentTabItem, string>(nameof(Title));

        public string Title
        {
            get { return GetValue(TitleProprty); }
            set { SetValue(TitleProprty, value); }
        }

        public static readonly StyledProperty<IBrush> HeaderBackgroundProperty =
            AvaloniaProperty.Register<DocumentTabItem, IBrush>(nameof(HeaderBackground), defaultValue: Brushes.WhiteSmoke);

        public IBrush HeaderBackground
        {
            get { return GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        private void UpdatePseudoClasses (bool? isFocused, Avalonia.Controls.Dock? dock)
        {
            if(isFocused.HasValue)
            {
                PseudoClasses.Set(":focused", isFocused.Value);
            }

            if(dock.HasValue)
            {
                PseudoClasses.Set(":dockright", dock.Value == Avalonia.Controls.Dock.Right);
                PseudoClasses.Set(":dockleft", dock.Value == Avalonia.Controls.Dock.Left);
                PseudoClasses.Set(":docktop", dock.Value == Avalonia.Controls.Dock.Top);
                PseudoClasses.Set(":dockbottom", dock.Value == Avalonia.Controls.Dock.Bottom);
            }
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);


            if (change.Property == IsFocusedProperty)
            {
                UpdatePseudoClasses(change.NewValue.GetValueOrDefault<bool>(), null);
            }
            else if (change.Property == DockPanel.DockProperty)
            {
                UpdatePseudoClasses(null, change.NewValue.GetValueOrDefault<Avalonia.Controls.Dock>());
            }
        }
    }
}