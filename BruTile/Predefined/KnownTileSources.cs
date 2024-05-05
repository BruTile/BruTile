// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using BruTile.Cache;
using BruTile.Web;

namespace BruTile.Predefined;

/// <summary>
/// Known tile sources
/// </summary>
public enum KnownTileSource
{
    OpenStreetMap,
    OpenCycleMap,
    OpenCycleMapTransport,
    BingAerial,
    BingHybrid,
    BingRoads,
    BingAerialStaging,
    BingHybridStaging,
    BingRoadsStaging,
    StamenToner,
    StamenTonerLite,
    StamenWatercolor,
    StamenTerrain,
    EsriWorldTopo,
    EsriWorldPhysical,
    EsriWorldShadedRelief,
    EsriWorldReferenceOverlay,
    EsriWorldTransportation,
    EsriWorldBoundariesAndPlaces,
    EsriWorldDarkGrayBase,
    BKGTopPlusColor,
    BKGTopPlusGrey,
    HereNormal,
    HereSatellite,
    HereHybrid,
    HereTerrain
}

public static class KnownTileSources
{
    private static readonly Attribution OpenStreetMapAttribution = new(
        "© OpenStreetMap contributors", "https://www.openstreetmap.org/copyright");
    private static readonly string CurrentYear = DateTime.Today.Year.ToString();
    private static readonly Attribution BKGAttribution = new("© Bundesamt für Kartographie und Geodäsie (" + CurrentYear + ")",
                     "https://sg.geodatenzentrum.de/web_public/Datenquellen_TopPlus_Open.pdf");
    private static readonly Attribution HereAttribution = new("© HERE (" + CurrentYear + ")", "https://www.here.com");
    private static readonly Attribution StamenAttribution = new("© Stamen Design, under CC BY 3.0", "http://creativecommons.org/licenses/by/3.0");

    /// <summary>
    /// Static factory method for known tile services
    /// </summary>
    /// <param name="source">The source</param>
    /// <param name="apiKey">An (optional) API key</param>
    /// <param name="persistentCache">A place to permanently store tiles (file of database)</param>
    /// <param name="tileFetcher">Option to override the web request</param>
    /// <param name="userAgent">The 'User-Agent' http header added to the tile request</param>
    /// <param name="minZoomLevel">The minimum zoom level</param>
    /// <param name="maxZoomLevel">The maximum zoom level</param>
    /// <returns>The tile source</returns>
    public static HttpTileSource Create(KnownTileSource source = KnownTileSource.OpenStreetMap, string apiKey = null,
        IPersistentCache<byte[]> persistentCache = null, Func<Uri, CancellationToken, Task<byte[]>> tileFetcher = null, string userAgent = null,
        int minZoomLevel = 0, int maxZoomLevel = 20)
    {
        return source switch
        {
            KnownTileSource.OpenStreetMap => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(18, maxZoomLevel)),
                "https://tile.openstreetmap.org/{z}/{x}/{y}.png",
                name: source.ToString(),
                tileFetcher: tileFetcher,
                attribution: OpenStreetMapAttribution, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.OpenCycleMap => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(17, maxZoomLevel)),
                "http://{s}.tile.opencyclemap.org/cycle/{z}/{x}/{y}.png",
                ["a", "b", "c"], name: source.ToString(),
                tileFetcher: tileFetcher,
                attribution: OpenStreetMapAttribution, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.OpenCycleMapTransport => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(20, maxZoomLevel)),
                "http://{s}.tile2.opencyclemap.org/transport/{z}/{x}/{y}.png",
                ["a", "b", "c"], name: source.ToString(),
                tileFetcher: tileFetcher,
                attribution: OpenStreetMapAttribution, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.BingAerial => new HttpTileSource(new GlobalSphericalMercator(Math.Max(1, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "https://t{s}.tiles.virtualearth.net/tiles/a{quadkey}.jpeg?g=517&token={k}",
                ["0", "1", "2", "3", "4", "5", "6", "7"], apiKey, source.ToString(), null,
                new Attribution("© Microsoft"), userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.BingHybrid => new HttpTileSource(new GlobalSphericalMercator(Math.Max(1, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "https://t{s}.tiles.virtualearth.net/tiles/h{quadkey}.jpeg?g=517&token={k}",
                ["0", "1", "2", "3", "4", "5", "6", "7"], apiKey, source.ToString(), null,
                new Attribution("© Microsoft"), userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.BingRoads => new HttpTileSource(new GlobalSphericalMercator(Math.Max(1, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "https://t{s}.tiles.virtualearth.net/tiles/r{quadkey}.jpeg?g=517&token={k}",
                ["0", "1", "2", "3", "4", "5", "6", "7"], apiKey, source.ToString(), null,
                new Attribution("© Microsoft"), userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.BingAerialStaging => new HttpTileSource(new GlobalSphericalMercator(Math.Max(1, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "http://t{s}.staging.tiles.virtualearth.net/tiles/a{quadkey}.jpeg?g=517&token={k}",
                ["0", "1", "2", "3", "4", "5", "6", "7"], apiKey, source.ToString(),
                userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.BingHybridStaging => new HttpTileSource(new GlobalSphericalMercator(Math.Max(1, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "http://t{s}.staging.tiles.virtualearth.net/tiles/h{quadkey}.jpeg?g=517&token={k}",
                ["0", "1", "2", "3", "4", "5", "6", "7"], apiKey, source.ToString(),
                userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.BingRoadsStaging => new HttpTileSource(new GlobalSphericalMercator(Math.Max(1, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "http://t{s}.staging.tiles.virtualearth.net/tiles/r{quadkey}.jpeg?g=517&token={k}",
                ["0", "1", "2", "3", "4", "5", "6", "7"], apiKey, source.ToString(),
                userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.StamenToner => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "https://stamen-tiles-{s}.a.ssl.fastly.net/toner/{z}/{x}/{y}.png",
                ["a", "b", "c", "d"], name: source.ToString(),
                tileFetcher: tileFetcher,
                attribution: StamenAttribution, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.StamenTonerLite => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "https://stamen-tiles-{s}.a.ssl.fastly.net/toner-lite/{z}/{x}/{y}.png",
                ["a", "b", "c", "d"], name: source.ToString(),
                tileFetcher: tileFetcher,
                attribution: StamenAttribution, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.StamenWatercolor => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "https://stamen-tiles-{s}.a.ssl.fastly.net/watercolor/{z}/{x}/{y}.jpg",
                ["a", "b", "c", "d"], name: source.ToString(),
                tileFetcher: tileFetcher,
                attribution: StamenAttribution, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.StamenTerrain => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "https://stamen-tiles-{s}.a.ssl.fastly.net/terrain/{z}/{x}/{y}.jpg",
                ["a", "b", "c", "d"], name: source.ToString(),
                tileFetcher: tileFetcher,
                attribution: StamenAttribution, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.EsriWorldTopo => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "https://server.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}",
                name: source.ToString(), tileFetcher: tileFetcher, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.EsriWorldPhysical => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(8, maxZoomLevel)),
                "https://server.arcgisonline.com/ArcGIS/rest/services/World_Physical_Map/MapServer/tile/{z}/{y}/{x}",
                name: source.ToString(), tileFetcher: tileFetcher, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.EsriWorldShadedRelief => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(13, maxZoomLevel)),
                "https://server.arcgisonline.com/ArcGIS/rest/services/World_Shaded_Relief/MapServer/tile/{z}/{y}/{x}",
                name: source.ToString(), tileFetcher: tileFetcher, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.EsriWorldReferenceOverlay => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(13, maxZoomLevel)),
                "https://server.arcgisonline.com/ArcGIS/rest/services/Reference/World_Reference_Overlay/MapServer/tile/{z}/{y}/{x}",
                name: source.ToString(), tileFetcher: tileFetcher, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.EsriWorldTransportation => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "https://server.arcgisonline.com/ArcGIS/rest/services/Reference/World_Transportation/MapServer/tile/{z}/{y}/{x}",
                name: source.ToString(), tileFetcher: tileFetcher, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.EsriWorldBoundariesAndPlaces => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "https://server.arcgisonline.com/ArcGIS/rest/services/Reference/World_Boundaries_and_Places/MapServer/tile/{z}/{y}/{x}",
                name: source.ToString(), tileFetcher: tileFetcher, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.EsriWorldDarkGrayBase => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(16, maxZoomLevel)),
                "https://server.arcgisonline.com/arcgis/rest/services/Canvas/World_Dark_Gray_Base/MapServer/tile/{z}/{y}/{x}",
                name: source.ToString(), tileFetcher: tileFetcher, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.BKGTopPlusColor => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "https://sg.geodatenzentrum.de/wmts_topplus_open/tile/1.0.0/web_scale/default/WEBMERCATOR/{z}/{y}/{x}.png",
                name: source.ToString(), tileFetcher: tileFetcher,
                attribution: BKGAttribution, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.BKGTopPlusGrey => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "https://sg.geodatenzentrum.de/wmts_topplus_open/tile/1.0.0/web_scale_grau/default/WEBMERCATOR/{z}/{y}/{x}.png",
                name: source.ToString(), tileFetcher: tileFetcher,
                attribution: BKGAttribution, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.HereNormal => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "https://{s}.base.maps.ls.hereapi.com/maptile/2.1/maptile/newest/normal.day/{z}/{x}/{y}/256/png8?apiKey={k}",
                ["1", "2", "3", "4"], apiKey,
                name: source.ToString(), tileFetcher: tileFetcher,
                attribution: HereAttribution, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.HereSatellite => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "https://{s}.aerial.maps.ls.hereapi.com/maptile/2.1/maptile/newest/satellite.day/{z}/{x}/{y}/256/png8?apiKey={k}",
                ["1", "2", "3", "4"], apiKey,
                name: source.ToString(), tileFetcher: tileFetcher,
                attribution: HereAttribution, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.HereHybrid => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "https://{s}.aerial.maps.ls.hereapi.com/maptile/2.1/maptile/newest/hybrid.grey.day/{z}/{x}/{y}/256/png8?apiKey={k}",
                ["1", "2", "3", "4"], apiKey,
                name: source.ToString(), tileFetcher: tileFetcher,
                attribution: HereAttribution, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            KnownTileSource.HereTerrain => new HttpTileSource(new GlobalSphericalMercator(Math.Max(0, minZoomLevel), Math.Min(19, maxZoomLevel)),
                "https://{s}.aerial.maps.ls.hereapi.com/maptile/2.1/maptile/newest/terrain.day/{z}/{x}/{y}/256/png8?apiKey={k}",
                ["1", "2", "3", "4"], apiKey,
                name: source.ToString(), tileFetcher: tileFetcher,
                attribution: HereAttribution, userAgent: userAgent)
            { PersistentCache = persistentCache ?? new NullCache() },
            _ => throw new NotSupportedException("KnownTileSource not known"),
        };
    }
}
