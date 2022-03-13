// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile.Wms
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
            return new WmsParsingException($"'{attribute}' attribute not found");
        }

        public static WmsParsingException ElementNotFound(string element)
        {
            return new WmsParsingException($"'<{element}>' element not found");
        }
    }
}