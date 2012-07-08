// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile.Web.Wms
{
    public class WmsPropertyException : System.Exception
    {
        public WmsPropertyException()
        {
        }

        public WmsPropertyException(string message)
            : base(message)
        {
        }

        public WmsPropertyException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

        public static WmsPropertyException PositiveInteger(string property, int value)
        {
            return new WmsPropertyException(string.Format("{0} requires a positve integer value. You tried to assign {1}", property, value));
        }

        public static System.Exception NonNegativeInteger(string property, int value)
        {
            return new WmsPropertyException(string.Format("{0} requires a non-negative integer value. You tried to assign {1}", property, value));
        }
    }
}