using System;
using System.IO;
using System.Reflection;

namespace BruTile.Tests.Utilities
{
    public static class Paths
    {
        public static string AssemblyDirectory
        {
            get
            {
                var asm = typeof(Paths).GetTypeInfo().Assembly;
                var codeBase = asm.CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
