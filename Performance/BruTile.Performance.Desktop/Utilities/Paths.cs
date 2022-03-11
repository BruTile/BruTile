using System;
using System.IO;
using System.Reflection;

namespace BruTile.Performance.Desktop.Utilities
{
    public static class Paths
    {
        public static string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase!;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
