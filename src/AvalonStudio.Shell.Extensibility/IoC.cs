using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;

namespace AvalonStudio.Extensibility
{
    public static class IoC
    {
        private static CompositionHost s_compositionHost;

        public static object Get(Type t, string contract = null)
        {
            if (s_compositionHost != null)
            {
                return s_compositionHost.GetExport(t, contract);
            }

            return default;
        }

        public static T Get<T>(string contract)
        {
            if (s_compositionHost != null)
            {
                return s_compositionHost.GetExport<T>(contract);
            }

            return default;
        }

        public static T Get<T>()
        {
            if (s_compositionHost != null)
            {
                return s_compositionHost.GetExport<T>();
            }

            return default;
        }

        public static IEnumerable<T> GetInstances<T>()
        {
            if (s_compositionHost != null)
            {
                return s_compositionHost.GetExports<T>();
            }

            return Enumerable.Empty<T>();
        }

        public static IEnumerable<T> GetInstances<T>(string contract)
        {
            if (s_compositionHost != null)
            {
                return s_compositionHost.GetExports<T>(contract);
            }

            return Enumerable.Empty<T>();
        }

        public static void Initialise(CompositionHost host)
        {
            s_compositionHost = host;
        }
    }
}