// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile.Predefined
{
    public class BingSchema : SphericalMercatorInvertedWorldSchema
    {
        public BingSchema()
        {
            Format = "jpg";
            Name = "BingMaps";
            Resolutions.Remove("0"); //Bing does not have the single tile top level.
        }
    }
}