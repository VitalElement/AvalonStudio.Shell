using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Composition;

namespace AvalonStudio.Extensibility
{
    public static class IoC
    {
        private static ExportProvider s_exportProvider;

        public static object Get(Type t, string contract = null)
        {
            if (s_exportProvider != null)
            {
                return s_exportProvider.AsExportProvider().GetExports(t, null, contract).SingleOrDefault();
            }

            return default;
        }

        public static T Get<T>(string contract)
        {
            if (s_exportProvider != null)
            {
                return s_exportProvider.GetExportedValue<T>(contract);
            }

            return default;
        }

        public static T Get<T>()
        {
            if (s_exportProvider != null)
            {
                return s_exportProvider.GetExportedValue<T>();
            }

            return default;
        }

        public static IEnumerable<T> GetInstances<T>()
        {
            if (s_exportProvider != null)
            {
                return s_exportProvider.GetExportedValues<T>();
            }

            return Enumerable.Empty<T>();
        }

        public static IEnumerable<T> GetInstances<T>(string contract)
        {
            if (s_exportProvider != null)
            {
                return s_exportProvider.GetExportedValues<T>(contract);
            }

            return Enumerable.Empty<T>();
        }

        public static void Initialise(ExportProvider exportProvider)
        {
            s_exportProvider = exportProvider;
        }
    }
}