using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace GetVersionFromAssembly
{
    static class Program
    {
        static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                try
                {
                    var path = Path.GetFullPath(arg);
                    var assembly = Assembly.LoadFile(path);
                    var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                    Console.Out.WriteLine(fileVersionInfo.ProductVersion);
                }
                catch (Exception exception)
                {
                    Console.Out.WriteLine($"{arg}: {exception.Message}");
                }
            }
        }
    }
}
