// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.IO;
using System.Reflection;

namespace BruTile.Tests.Utilities;

public static class Paths
{
    public static string AssemblyDirectory
    {
        get
        {
            var asm = typeof(Paths).GetTypeInfo().Assembly;
            return Path.GetDirectoryName(System.AppContext.BaseDirectory) ?? throw new System.Exception("AssemblyName was null");
        }
    }
}
