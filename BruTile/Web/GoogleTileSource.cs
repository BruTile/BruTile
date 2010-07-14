/*  
 *  This code is based on information provided by http://greatmaps.codeplex.com
 *  
 */
using BruTile.PreDefined;
using BruTile.Cache;

namespace BruTile.Web
{
    public class GoogleTileSource : ITileSource
    {
        private readonly SphericalMercatorInvertedWorldSchema _tileSchema;
        private readonly WebTileProvider _tileProvider;
        private const string UserAgent = @"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.1.7) Gecko/20091221 Firefox/3.5.7";
        private const string Referer = "http://maps.google.com/";

        public GoogleTileSource(GoogleMapType mapType)
            : this(new GoogleRequest(mapType), new NullCache())
        {
        }
        public GoogleTileSource(GoogleRequest request)
            : this(request, new NullCache())
        {
        }
        
        public GoogleTileSource(GoogleRequest request, ITileCache<byte[]> fileCache)
        {
            _tileSchema = new SphericalMercatorInvertedWorldSchema();
            _tileProvider = new WebTileProvider(request, fileCache, UserAgent, Referer, true);
        }

        #region Implementation of ITileSource

        public ITileProvider Provider
        {
            get { return _tileProvider; }
        }

        public ITileSchema Schema
        {
            get { return _tileSchema; }
        }

        #endregion
    }
}
