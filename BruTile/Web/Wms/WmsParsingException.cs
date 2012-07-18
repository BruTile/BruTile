// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile.Web.Wms
{
    public class WmsParsingException : System.Exception
    {
        public WmsParsingException()
        { }

        public WmsParsingException(string message)
            : base(message)
        {
        }

        public WmsParsingException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

        public static WmsParsingException AttributeNotFound(string attribute)
        {
            return new WmsParsingException(string.Format("'{0}' attribute not found", attribute));
        }

        public static WmsParsingException ElementNotFound(string element)
        {
            return new WmsParsingException(string.Format("'<{0}>' element not found", element));
        }
    }
}