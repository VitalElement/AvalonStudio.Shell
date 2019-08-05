using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.LogicalTree;
using AvalonStudio.Utils;
using Avalonia.Controls.Generators;

namespace AvalonStudio.Controls
{
    public class DocumentTabControl : SelectingItemsControl
    {
        public DocumentTabControl()
        {
            SelectionMode = SelectionMode.AlwaysSelected;            
        }

        public static readonly StyledProperty<object> HeaderSeperatorContentProperty = AvaloniaProperty.Register<DocumentTabControl, object>(nameof(HeaderSeperatorContent));

        public object HeaderSeperatorContent
        {
            get { return GetValue(HeaderSeperatorContentProperty); }
            set { SetValue(HeaderSeperatorContentProperty, value); }
        }

        public static readonly StyledProperty<IDataTemplate> HeaderTemplateProperty = AvaloniaProperty.Register<DocumentTabControl, IDataTemplate>(nameof(HeaderTemplate));

        public IDataTemplate HeaderTemplate
        {
            get { return GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }                

        protected override void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            base.ItemsCollectionChanged(sender, e);
        }

        protected override void ItemsChanged(AvaloniaPropertyChangedEventArgs e)
        {
            base.ItemsChanged(e);

            if (Items.Count() > 0)
            {                
                SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Selects the content of a tab item.
        /// </summary>
        /// <param name="o">The tab item.</param>
        /// <returns>The content.</returns>
        private static object SelectContent(object o)
        {
            var content = o as IContentControl;

            if (content != null)
            {
                return content.Content;
            }
            else
            {
                return o;
            }
        }

        protected override IItemContainerGenerator CreateItemContainerGenerator()
        {
            return null;
        }
    }
}