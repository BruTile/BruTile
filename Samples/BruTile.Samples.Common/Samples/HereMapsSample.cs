// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using BruTile.Predefined;
using BruTile.Web;

namespace BruTile.Samples.Common.Samples
{
    public static class HereMapsSample
    {
        public static ITileSource Create()
        {
            return new HttpTileSource(new GlobalSphericalMercator(0, 18),
                    "https://{s}.base.maps.cit.api.here.com/maptile/2.1/maptile/newest/normal.day/{z}/{x}/{y}/256/png8?app_id=xWVIueSv6JL0aJ5xqTxb&app_code=djPZyynKsbTjIUDOBcHZ2g",
                    new[] { "1", "2", "3", "4" }, name: "Here Maps Source");
        }
    }
}
