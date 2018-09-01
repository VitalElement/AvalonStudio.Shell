using AvalonStudio.Extensibility;
using AvalonStudio.Extensibility.Utils;
using Microsoft.VisualStudio.Composition;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AvalonStudio
{
    public static class CompositionRoot
    {
        public static async Task<ExportProvider> CreateExportProviderAsync(
            ExtensionManager extensionManager,
            CancellationToken cancellationToken = default)
        {
            var resolver = Resolver.DefaultInstance;

            ComposableCatalog catalog;

            var cachedCatalog = new CachedCatalog();

            var cacheIsValid = false;
            var cachePath = "";

            if (cacheIsValid)
            {
                using (var cacheStream = File.OpenRead(cachePath))
                {
                    catalog = await cachedCatalog.LoadAsync(cacheStream, resolver, cancellationToken);
                }
            }
            else
            {
                var discovery = new AttributedPartDiscovery(resolver, true);

                // TODO AppDomain here is a custom appdomain from namespace AvalonStudio.Extensibility.Utils. It is able
                // to load any assembly in the bin directory (so not really appdomain) we need to get rid of this
                // once all our default extensions are published with a manifest and copied to extensions dir.
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var extensionAssemblies = LoadMefComponents(extensionManager);

                catalog = ComposableCatalog.Create(resolver)
                    .AddParts(await discovery.CreatePartsAsync(assemblies).ConfigureAwait(false))
                    .AddParts(await discovery.CreatePartsAsync(extensionAssemblies).ConfigureAwait(false));

                // TODO: cache catalog
                //using (var cacheStream = File.OpenWrite(cachePath))
                //{
                //    await cachedCatalog.SaveAsync(catalog, cacheStream, cancellationToken);
                //}
            }

            var configuration = CompositionConfiguration.Create(catalog);

            // TODO: log errors to file

            var compositionErrors = configuration.CompositionErrors;

            while (!compositionErrors.IsEmpty)
            {
                var error = compositionErrors.Peek();

                System.Console.WriteLine();
                System.Console.WriteLine("Composition Error:");
                System.Console.WriteLine();

                foreach (var entry in error)
                {
                    System.Console.WriteLine(entry.Message);

                    foreach (var part in entry.Parts)
                    {
                        System.Console.WriteLine($"    Part: {part.Definition.Id}");
                    }
                }

                compositionErrors = compositionErrors.Pop();
            }

            var exportProviderFactory = configuration.CreateExportProviderFactory();
            var exportProvider = exportProviderFactory.CreateExportProvider();

            return exportProvider;
        }

        private static IEnumerable<Assembly> LoadMefComponents(ExtensionManager extensionManager)
        {
            var assemblies = new List<Assembly>();

            foreach (var extension in extensionManager.GetInstalledExtensions())
            {
                foreach (var mefComponent in extension.GetMefComponents())
                {
                    try
                    {
                        assemblies.Add(Assembly.LoadFrom(mefComponent));
                    }
                    catch (System.Exception e)
                    {
                        System.Console.WriteLine($"Failed to load MEF component from extension: '{mefComponent}'");
                        System.Console.WriteLine(e.ToString());
                    }
                }
            }

            return assemblies;
        }
    }
}