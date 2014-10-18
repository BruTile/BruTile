// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Xml.Linq;

namespace BruTile.Wms
{
    public class Get : DCPTypeElement
    {
        public Get()
            : base("Get")
        {
        }

        public Get(XElement node, string @namespace)
            : base("Get", node, @namespace)
        { }
    }
}