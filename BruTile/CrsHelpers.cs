using System.Linq;
using BruTile.Wmts;

namespace BruTile
{
    /// <summary>
    /// An enumeration of the CRS axis order 
    /// </summary>
    public enum CrsAxisOrder
    {
        Natural,
        Geographic
    }

    /// <summary>
    /// An enumeration of possibilities of how to interpret the axis order in &lt;ows:BoundingBox&gt; definitions
    /// </summary>
    public enum BoundingBoxAxisOrderInterpretation
    {
        /// <summary>
        /// Natural, first x, then y
        /// </summary>
        Natural,

        /// <summary>
        /// As defined in the definition of the coordinate reference system
        /// </summary>
        CRS,

        /// <summary>
        /// Geographic, first y (latitude), then x (longitude)
        /// </summary>
        Geographic

    }

    public static class CrsHelpers
    {
        public static int[] GetOrdinateOrderFromCrs(string srs,
            string wellKnownScaleSet = null)
        {
            // Get a set of well known scale sets. For these we don't need to have
            var wkss = new WellKnownScaleSets();
            var parts = srs.Split(':');
            var authority = parts[0];
            var identifier = parts.Last();
            var crsId = new CrsIdentifier(authority,null,identifier);
            // Axis order registry
            var crsAxisOrder = new CrsAxisOrderRegistry();
            // Unit of measure registry
            var crsUnitOfMeasure = new CrsUnitOfMeasureRegistry();

            // Check if a Well-Known scale set is used, either by Identifier or WellKnownScaleSet property
            var ss = wkss[identifier];
            if (ss == null && !string.IsNullOrEmpty(wellKnownScaleSet))
                ss = wkss[wellKnownScaleSet.Split(':').Last()];

            // Try to parse the Crs
            // Hack to fix broken crs spec
            //crsId = crsId.ToString().Replace("6.18:3", "6.18.3");

            //CrsIdentifier crs;
            //if (!CrsIdentifier.TryParse(crsId.ToString(), out crs))
            //{
            //    // If we cannot parse the crs, we cannot compute tile schema, thus ignore.
            //    // ToDo: Log this
            //    return null;
            //}

            // Get the ordinate order for the crs (x, y) or (y, x) aka (lat, long)
            return crsAxisOrder[crsId];

        }
    }
}
