using System;
using System.Linq;
using BruTile.Predefined;
using BruTile.Web;
using BruTile.Wmsc;

namespace BruTile.Samples.Common
{
    public static class TileSourceForWmsSample
    {
        public static ITileSource Create()
        {
            const string url = "http://geodata.nationaalgeoregister.nl/ahn25m/wms?service=wms";
            // You need to know the schema. This can be a problem. Usally it is GlobalSphericalMercator
            var schema = new WkstNederlandSchema {Format = "image/png"};
            var request = new WmscRequest(new Uri(url), schema, new[] { "ahn25m" }.ToList(), new string[0].ToList());
            var provider = new HttpTileProvider(request);
            return new TileSource(provider, schema);
        }
    }
}
