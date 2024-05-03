// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile.Samples.Common.Samples;

public static class GoogleMapsTerrainSample
{
    public static ITileSource Create()
    {
        return GoogleMapsSample.CreateGoogleTileSource("http://mt{s}.google.com/vt/lyrs=t@125,r@130&hl=en&x={x}&y={y}&z={z}");
    }
}
