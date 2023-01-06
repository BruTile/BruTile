// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System.Xml.Linq;

namespace BruTile.Wms
{
    public class Post : DCPTypeElement
    {
        public Post()
            : base("Post")
        {
        }

        public Post(XElement node, string @namespace)
            : base("Post", node, @namespace)
        { }
    }
}
