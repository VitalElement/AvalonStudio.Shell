using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AvalonStudio.Extensibility.Utils
{
    public class AppDomain
    {
        public static AppDomain CurrentDomain { get; private set; }

        static AppDomain()
        {
            CurrentDomain = new AppDomain();
        }

        public Assembly[] GetAssemblies()
        {
            var assemblies = new List<Assembly>();

            var dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var files = Directory.EnumerateFiles(dir).ToList();

            //var compileDependencies = DependencyContext.Default.CompileLibraries;

            foreach (var library in files.Where(x=>Path.GetExtension(x) == ".dll"))
            {
                if (IsCandidateCompilationLibrary(Path.GetFileNameWithoutExtension(library)))
                {
                    try
                    {
                        var assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(library)));
                        assemblies.Add(assembly);
                    }
                    catch(Exception)
                    {

                    }
                }
            }

            return assemblies.ToArray();
        }

        private static bool IsCandidateCompilationLibrary(string name)
        {
            return name.ToLower() == "avalonStudio"
                || name.ToLower().StartsWith("avalonstudio");
        }
    }
}