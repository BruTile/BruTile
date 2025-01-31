// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.IO;
using System.Reflection;

#pragma warning disable IL3000 // Avoid accessing Assembly file path when publishing as a single file. Suppressed because this file is only used in tests.

namespace BruTile.MbTiles.Tests.Utilities;

public static class Paths
{
    public static string AssemblyDirectory
    {
        get
        {
            var asm = typeof(Paths).GetTypeInfo().Assembly;
            return Path.GetDirectoryName(asm.Location) ?? throw new System.Exception("AssemblyName was null");
        }
    }
}
