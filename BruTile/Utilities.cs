using System;
using System.Collections.Generic;
using System.IO;

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

        public static int GetNearestLevel(IList<Resolution> resolutions, double resolution)
        {
            if (resolutions.Count == 0)
            {
                throw new ArgumentException("No tile resolutions");
            }

            //smaller than smallest
            if (resolutions[(resolutions.Count - 1)].UnitsPerPixel > resolution) return resolutions.Count - 1;

            //bigger than biggest
            if (resolutions[0].UnitsPerPixel < resolution) return 0;

            int result = 0;
            double resultDistance = double.MaxValue;
            for (int i = 0; i < resolutions.Count; i++)
            {
                double distance = Math.Abs(resolutions[i].UnitsPerPixel - resolution);
                if (distance < resultDistance)
                {
                    result = i;
                    resultDistance = distance;
                }
            }
            return result;
        }
    }
}