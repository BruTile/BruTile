// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tiling;
using System.Windows;

namespace BruTileDemo
{
  static class Util
  {
    public static string GetAppDir()
    {
      return System.IO.Path.GetDirectoryName(
        System.Reflection.Assembly.GetEntryAssembly().GetModules()[0].FullyQualifiedName);
    }

    public static double Distance(double x1, double y1, double x2, double y2)
    {
      return Math.Sqrt(Math.Pow(x1 - x2, 2f) + Math.Pow(y1 - y2, 2f));
    }

    public static string AppName
    {
      get { return System.Reflection.Assembly.GetEntryAssembly().GetName().Name.ToString(); }
    }

    public static string DefaultCacheDir
    {
      get { return BruTileDemo.Properties.Settings.Default.DefaultCacheDir; }
    }

    public static Extent ToExtent(Rect rect)
    {
      return new Extent(rect.Left, rect.Top, rect.Right, rect.Bottom);
    }

  }
}
