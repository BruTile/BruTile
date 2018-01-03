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
            foreach (string arg in args)
            {
                try
                {
                    string path = Path.GetFullPath(arg);
                    var assembly = Assembly.LoadFile(path);
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                    Console.Out.WriteLine(fileVersionInfo.ProductVersion);
                }
                catch (Exception exception)
                {
                    Console.Out.WriteLine(string.Format("{0}: {1}", arg, exception.Message));
                }
            }
        }
    }
}
