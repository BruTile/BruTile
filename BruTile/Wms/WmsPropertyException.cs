// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile.Wms;

public class WmsPropertyException : System.Exception
{
    public WmsPropertyException()
    { }

    public WmsPropertyException(string message)
        : base(message)
    { }

    public WmsPropertyException(string message, System.Exception inner)
        : base(message, inner)
    { }

    public static WmsPropertyException PositiveInteger(string property, int value)
    {
        return new WmsPropertyException($"{property} requires a positive integer value. You tried to assign {value}");
    }

    public static System.Exception NonNegativeInteger(string property, int value)
    {
        return new WmsPropertyException(
            $"{property} requires a non-negative integer value. You tried to assign {value}");
    }
}
