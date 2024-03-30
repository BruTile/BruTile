// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile.Wmts;

/// <summary>
/// A precomputed pair of ScaleDenominator and PixelSize
/// </summary>
internal readonly struct ScaleSetItem
{
    /// <summary>
    /// Initializes a scale set item with the provided values
    /// </summary>
    /// <param name="scaleDenominator">The scale denominator</param>
    /// <param name="pixelSize">The pixel size</param>
    public ScaleSetItem(double scaleDenominator, double pixelSize)
    {
        ScaleDenominator = scaleDenominator;
        PixelSize = pixelSize;
    }

    /// <summary>
    /// Gets a value indicating the scale denominator (1:...)
    /// </summary>
    public double ScaleDenominator { get; }

    /// <summary>
    /// Gets a value indicating the pixel size
    /// </summary>
    public double PixelSize { get; }
}
