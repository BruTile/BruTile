// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace BruTile.Wmts;

/// <summary>
/// A repository of well known scale sets.
/// </summary>
/// <remarks>
/// These scale sets are defined in the WMTS specification (OGC 07-057r7),
/// Chapter 6.2 and Annex E
/// <para>
/// Since a WMTS server will serve its data in a limited number of coordinate systems and
/// scales (because, unlike a WMS, it serves only pre-defined tiles), and since some simple
/// WMTS client will be unable to perform coordinate-system transformations or rescaling
/// of tiles, the ability for a WMTS client to overlay tiles from one server on top of tiles from
/// other servers will be limited unless there are some general agreements among WMTS
/// servers as to what constitutes a common coordinate reference system and a common set
/// of scales. Thus, this standard defines the concept of well-known scale sets. In order to
/// increase interoperability between clients and servers it is recommended that many layers
/// use a common set of scales in the same CRS that the target community agree to use.
/// </para>
/// <para>
/// A well-known scale set is a well-known combination of a coordinate reference system
/// and a set of scales that a tile matrix set declares support for. Each tile matrix set
/// references one well-known scale set. A client application can confirm that tiles from one
/// WMTS server are compatible with tiles from another WMTS server merely by verifying
/// that they declare a common well-known scale set. It may also be the case that a client
/// application is limited to supporting a particular coordinate system and set of scales (e.g.,
/// an application that overlays WMTS tiles on top of Google Maps tiles). In this situation, a
/// client application can accept or reject a WMTS as being compatible merely by verifying
/// the declared well-known scale set. Furthermore, the existence of well-known scale sets
/// provides incentive for WMTS servers to support a well-known scale set, increasing the
/// odds of compatibility with other WMTS sources. The informative Annex E provides
/// several well-known scale sets and others could be incorporated in the future.
/// </para>
/// <para>
/// A tile matrix set conforms to a particular well-known scale set when it uses the same
/// CRS and defines all scale denominators ranging from the largest scale denominator in the
/// well-known scale set to some low scale denominator (in other words, it is not necessary
/// to define all the lower scale denominators to conform to a well-known scale set).
/// </para>
/// <para>
/// NOTE: Well-known scale sets are technically not necessary for interoperability, since a client
/// application can always examine the actual list of coordinate systems and scales available for each layer of a
/// WMTS server in order to determine its level of compatibility with other WMTS servers. Well-known scale
/// sets are merely a convenience mechanism.
/// </para>
/// </remarks>
internal class WellKnownScaleSets
{
    private static readonly Dictionary<string, ScaleSet> Definitions;
    public static readonly Dictionary<CrsIdentifier, int[]> OrdinateOrder;

    /// <summary>
    /// Static constructor
    /// </summary>
    static WellKnownScaleSets()
    {
        Definitions = new Dictionary<string, ScaleSet>
        {
            {"GlobalCRS84Scale", CreateGlobalCRS84Scale()},
            {"GlobalCRS84Pixel", CreateGlobalCRS84Pixel()},
            {"GoogleCRS84Quad)", CreateGoogleCRS84Quad()},
            {"GoogleMapsCompatible", CreateGoogleMapsCompatible()}
        };

        OrdinateOrder = new Dictionary<CrsIdentifier, int[]>
        {
            {CRS84, new[] {0, 1}},
            {EPSG3857, new[] {0, 1}},
            {EPSG4326, new[] {1, 0}},
        };
    }

    /// <summary>
    /// Get a scale set by its key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public ScaleSet this[string key] => Definitions.TryGetValue(key, out var result) ? result : null;

    private static ScaleSet CreateGoogleMapsCompatible()
    {
        return new ScaleSet("GoogleMapsCompatible", EPSG3857,
        [
            new ScaleSetItem(559082264.0287178, 156543.0339280410),
            new ScaleSetItem(279541132.0143589, 78271.51696402048),
            new ScaleSetItem(139770566.0071794, 39135.75848201023),
            new ScaleSetItem(69885283.00358972, 19567.87924100512),
            new ScaleSetItem(34942641.50179486, 9783.939620502561),
            new ScaleSetItem(17471320.75089743, 4891.969810251280),
            new ScaleSetItem(8735660.375448715, 2445.984905125640),
            new ScaleSetItem(4367830.187724357, 1222.992452562820),
            new ScaleSetItem(2183915.093862179, 611.4962262814100),
            new ScaleSetItem(1091957.546931089, 305.7481131407048),
            new ScaleSetItem(545978.7734655447, 152.8740565703525),
            new ScaleSetItem(272989.3867327723, 76.43702828517624),
            new ScaleSetItem(136494.6933663862, 38.21851414258813),
            new ScaleSetItem(68247.34668319309, 19.10925707129406),
            new ScaleSetItem(34123.67334159654, 9.554628535647032),
            new ScaleSetItem(17061.83667079827, 4.777314267823516),
            new ScaleSetItem(8530.918335399136, 2.388657133911758),
            new ScaleSetItem(4265.459167699568, 1.194328566955879),
            new ScaleSetItem(2132.729583849784, 0.5971642834779395)
        ]);
    }

    private static ScaleSet CreateGoogleCRS84Quad()
    {
        return new ScaleSet("GoogleCRS84Quad", CRS84,
        [
            new ScaleSetItem(559082264.0287178, 1.40625000000000),
            new ScaleSetItem(279541132.0143589, 0.703125000000000),
            new ScaleSetItem(139770566.0071794, 0.351562500000000),
            new ScaleSetItem(69885283.00358972, 0.175781250000000),
            new ScaleSetItem(34942641.50179486, 8.78906250000000E-2),
            new ScaleSetItem(17471320.75089743, 4.39453125000000E-2),
            new ScaleSetItem(8735660.375448715, 2.19726562500000E-2),
            new ScaleSetItem(4367830.187724357, 1.09863281250000E-2),
            new ScaleSetItem(2183915.093862179, 5.49316406250000E-3),
            new ScaleSetItem(1091957.546931089, 2.74658203125000E-3),
            new ScaleSetItem(545978.7734655447, 1.37329101562500E-3),
            new ScaleSetItem(272989.3867327723, 6.86645507812500E-4),
            new ScaleSetItem(136494.6933663862, 3.43322753906250E-4),
            new ScaleSetItem(68247.34668319309, 1.71661376953125E-4),
            new ScaleSetItem(34123.67334159654, 8.58306884765625E-5),
            new ScaleSetItem(17061.83667079827, 4.29153442382812E-5),
            new ScaleSetItem(8530.918335399136, 2.14576721191406E-5),
            new ScaleSetItem(4265.459167699568, 1.07288360595703E-5),
            new ScaleSetItem(2132.729583849784, 5.36441802978516E-6)
        ]);
    }

    private static ScaleSet CreateGlobalCRS84Pixel()
    {
        return new ScaleSet("GlobalCRS84Pixel", CRS84,
        [
            new ScaleSetItem(795139219.9519541, 2), // 240000
            new ScaleSetItem(397569609.9759771, 1), // 120000
            new ScaleSetItem(198784804.9879885, 0.5), // (30') 60000
            new ScaleSetItem(132523203.3253257, 0.333333333333333), // (20') 40000
            new ScaleSetItem(66261601.66266284, 0.166666666666667), // (10') 20000
            new ScaleSetItem(33130800.83133142, 8.333333333333333E-2), // (5') 10000
            new ScaleSetItem(13252320.33253257, 3.333333333333333E-2), // (2') 4000
            new ScaleSetItem(6626160.166266284, 1.666666666666667E-2), // (1') 2000
            new ScaleSetItem(3313080.083133142, 8.333333333333333E-3), // (30") 1000
            new ScaleSetItem(1656540.041566571, 4.166666666666667E-3), // (15") 500
            new ScaleSetItem(552180.0138555236, 1.388888888888889E-3), // (5") 166
            new ScaleSetItem(331308.0083133142, 8.333333333333333E-4), // (3") 100
            new ScaleSetItem(110436.0027711047, 2.777777777777778E-4), // (1") 33
            new ScaleSetItem(55218.00138555237, 1.388888888888889E-4), // (0.5") 16
            new ScaleSetItem(33130.80083133142, 8.333333333333333E-5), // (0.3") 10
            new ScaleSetItem(11043.60027711047, 2.777777777777778E-5), // (0.1") 3
            new ScaleSetItem(3313.080083133142, 8.333333333333333E-6), // (0.03") 1
            new ScaleSetItem(1104.360027711047, 2.777777777777778E-6)  // (0.01") 0.33
        ]);
    }

    internal static CrsIdentifier CRS84 => new("OGC", "1.3", "CRS84");

    internal static CrsIdentifier EPSG3857 => new("EPSG", string.Empty, "3857");

    internal static CrsIdentifier EPSG4326 => new("EPSG", string.Empty, "4326");

    private static ScaleSet CreateGlobalCRS84Scale()
    {
        return new ScaleSet("GlobalCRS84Scale", CRS84,
        [
            new ScaleSetItem(500E6, 1.25764139776733),
            new ScaleSetItem(250E6, 0.628820698883665),
            new ScaleSetItem(100E6, 0.251528279553466),
            new ScaleSetItem(50E6, 0.125764139776733),
            new ScaleSetItem(25E6, 6.28820698883665E-2),
            new ScaleSetItem(10E6, 2.51528279553466E-2),
            new ScaleSetItem(5E6, 1.25764139776733E-2),
            new ScaleSetItem(2.5E6, 6.28820698883665E-3),
            new ScaleSetItem(1E6, 2.51528279553466E-3),
            new ScaleSetItem(500E3, 1.25764139776733E-3),
            new ScaleSetItem(250E3, 6.28820698883665E-4),
            new ScaleSetItem(100E3, 2.51528279553466E-4),
            new ScaleSetItem(50E3, 1.25764139776733E-4),
            new ScaleSetItem(25E3, 6.28820698883665E-5),
            new ScaleSetItem(10E3, 2.51528279553466E-5),
            new ScaleSetItem(5E3, 1.25764139776733E-5),
            new ScaleSetItem(2.5E3, 6.28820698883665E-6),
            new ScaleSetItem(1E3, 2.51528279553466E-6),
            new ScaleSetItem(500, 1.25764139776733E-6),
            new ScaleSetItem(250, 6.28820698883665E-7),
            new ScaleSetItem(100, 2.51528279553466E-7)
        ]);
    }

}
