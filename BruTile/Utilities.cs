﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BruTile
{
    public static class Utilities
    {
        /// <summary>
        ///   Reads data from a stream until the end is reached. The
        ///   data is returned as a byte array. An IOException is
        ///   thrown if any of the underlying IO calls fail.
        /// </summary>
        /// <param name = "stream">The stream to read data from</param>
        public static byte[] ReadFully(Stream stream)
        {
            //thanks to: http://www.yoda.arachsys.com/csharp/readbinary.html
            var buffer = new byte[32768];
            using (var ms = new MemoryStream())
            {
                while (true)
                {
                    var read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                    {
                        return ms.ToArray();
                    }
                    ms.Write(buffer, 0, read);
                }
            }
        }

        public static int GetNearestLevel(IDictionary<int, Resolution> resolutions, double resolution)
        {
            if (resolutions.Count == 0)
            {
                throw new ArgumentException("No tile resolutions");
            }

            //smaller than smallest
            if (resolutions.Last().Value.UnitsPerPixel > resolution) return resolutions.Last().Key;

            //bigger than biggest
            if (resolutions.First().Value.UnitsPerPixel < resolution) return resolutions.First().Key;

            int result = 0;
            double resultDistance = double.MaxValue;
            foreach (var key in resolutions.Keys)
            {
                double distance = Math.Abs(resolutions[key].UnitsPerPixel - resolution);
                if (distance < resultDistance)
                {
                    result = key;
                    resultDistance = distance;
                }
            }
            return result;
        }

        public static string Version
        {
            get
            {
                string name = typeof(Utilities).Assembly.FullName;
                var asmName = new AssemblyName(name);
                return asmName.Version.Major + "." + asmName.Version.Minor;
            }
        }

        public static string DefaultUserAgent { get { return "BruTile/" + Version; } }

        public static string DefaultReferer { get { return string.Empty; } }
    }
}