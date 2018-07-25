using AvalonStudio.Documents;
using AvalonStudio.MVVM;
using System;
using System.Linq;

namespace AvalonStudio.Shell
{
	public static class IShellExtensions
	{
		public static void AddOrSelectDocument<T>(this IShell me, T document) where T : IDocumentTabViewModel
		{
			var doc = me.Documents.FirstOrDefault(x => x.Equals(document));

			if (doc != null)
			{
				me.SelectedDocument = doc;
			}
			else
			{
				me.AddDocument(document);
			}
		}

        public static void AddOrSelectTool<T>(this IPerspective me, T tool) where T : IToolViewModel
        {
            var currentTool = me.Tools.FirstOrDefault(t => t.Equals(tool));

            if(currentTool != null)
            {
               me.SelectedTool = currentTool;
            }
            else
            {
                me.AddTool(tool);
            }
        }

        public static void AddOrSelectTool<T>(this IPerspective me, Func<T> factory) where T : IToolViewModel
        {
            var currentTool = me.Tools.FirstOrDefault(t => t is T);

            if (currentTool != default)
            {
                me.SelectedTool = currentTool;
            }
            else
            {
                me.AddTool(factory());
            }
        }

        public static void AddOrSelectDocument<T>(this IShell me, Func<T> factory) where T : IDocumentTabViewModel
		{
			var doc = me.Documents.FirstOrDefault(x => x is T);

			if (doc != default)
			{
				me.SelectedDocument = doc;
			}
			else
			{
				me.AddDocument(factory());
			}
		}

		public static T GetOrCreate<T>(this IShell me) where T : IDocumentTabViewModel, new()
		{
			T document = default;

			IDocumentTabViewModel doc = me.Documents.FirstOrDefault(x => x is T);

			if (doc != default)
			{
				document = (T)doc;
				me.SelectedDocument = doc;
			}
			else
			{
				document = new T();
				me.AddDocument(document);
			}

			return document;
		}
	}
}
