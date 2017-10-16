using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BruTile.Web;
using BruTile.Wms;
using BruTile.Wmsc;

namespace BruTile.Samples.Common
{
    public static class WorldHeritageSwedenTest
    {
        public static string NV_Natura2000_GetCapabilties_Url = "http://gis-services.metria.se/arcgis/rest/services/nv/InspireNV_N2K/MapServer/exts/InspireView/service?SERVICE=WMS&REQUEST=GETCAPABILITIES";
        public const string DefaultAuthorityCode = "EPSG:3006";
        public static ITileSource CreateNatura2000TileSource(string authorityCode = DefaultAuthorityCode)
        {
            Uri capabilitiesUri = new Uri(NV_Natura2000_GetCapabilties_Url);

            var layerNames = new List<string>() { "PS.N2K.Habitatdirektivet", "PS.N2K.Fageldirektivet" };

            var schema = new WmsSchemaFromCapabilities(capabilitiesUri, layerNames, 10, 20000, "EPSG:3006", 512);
            return TileSource(schema, layerNames, true);
        }

        public static ITileSource TileSource(WmsSchemaFromCapabilities schema, IList<string> layerNames, bool includeImages = false)
        {
            if (schema.OnlineResource != null)
            {
                schema.Transparent = true;
                var request = new WmscRequest(schema.OnlineResource, schema, layerNames, schema.StyleNames,
                    schema.CustomParameters, schema.Version.VersionString);
                var provider = new HttpTileProvider(request);
                return new TileSource(provider, schema);
            }
            else
            {
                return new EmptyTileSource(schema);
            }
        }

        public class EmptyTileSource : ITileSource
        {
            public EmptyTileSource(WmsSchemaFromCapabilities schema = null)
            {
                CrsSupported = new List<string>();
                if (schema != null)
                {
                    Message = schema.Message;
                    foreach (var crs in schema.CrsSupported)
                        CrsSupported.Add(crs);
                }
            }

            public IList<string> CrsSupported { get; set; }

            public byte[] GetTile(TileInfo tileInfo)
            {
                return null;
            }

            public ITileSchema Schema { get { return null; } }
            public string Name { get { return "Empty"; } }
            public Attribution Attribution { get; }
            public string Message { get; set; }
        }
    }
}
