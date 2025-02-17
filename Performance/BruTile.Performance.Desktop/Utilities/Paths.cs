// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.IO;
namespace BruTile.Performance.Desktop.Utilities;

public static class Paths
{
    public static string AssemblyDirectory => Path.GetDirectoryName(System.AppContext.BaseDirectory)
                ?? throw new System.Exception("AssemblyName was null");
}
