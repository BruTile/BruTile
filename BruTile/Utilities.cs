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

        public static string GetNearestLevel(IDictionary<string, Resolution> resolutions, double unitsPerPixel)
        {
            if (resolutions.Count == 0)
            {
                throw new ArgumentException("No tile resolutions");
            }

            var orderedResolutions = resolutions.OrderByDescending(r => r.Value.UnitsPerPixel);

            //smaller than smallest
            if (orderedResolutions.Last().Value.UnitsPerPixel > unitsPerPixel) return orderedResolutions.Last().Key;

            //bigger than biggest
            if (orderedResolutions.First().Value.UnitsPerPixel < unitsPerPixel) return orderedResolutions.First().Key;

            string result = null;
            double resultDistance = double.MaxValue;
            foreach (var current in orderedResolutions)
            {
                double distance = Math.Abs(current.Value.UnitsPerPixel - unitsPerPixel);
                if (distance < resultDistance)
                {
                    result = current.Key;
                    resultDistance = distance;
                }
            }
            if (result == null) throw new Exception("Unexpected error when calculating nearest level");
            return result;
        }

        public static string Version
        {
            get
            {
                var assembly = typeof(Utilities).GetTypeInfo().Assembly;
                var assemblyName = new AssemblyName(assembly.FullName);
                return assemblyName.Version.Major + "." + assemblyName.Version.Minor;
            }
        }

        public static string DefaultUserAgent { get { return "BruTile/" + Version; } }

        public static string DefaultReferer { get { return string.Empty; } }
    }
}