using System;
using System.Collections.Generic;
using System.Linq;

namespace BruTile.Wmts
{
    /// <summary>
    /// A set of scales
    /// </summary>
    internal class ScaleSet
    {
        /// <summary>
        /// The items
        /// </summary>
        private readonly ScaleSetItem[] _items;

        /// <summary>
        /// Creates an instance for this class
        /// </summary>
        /// <param name="name"></param>
        /// <param name="crs"></param>
        /// <param name="items"></param>
        public ScaleSet(string name, CrsIdentifier crs, IEnumerable<ScaleSetItem> items)
        {
            Name = name;
            Crs = crs;
            _items = items.ToArray();
        }

        /// <summary>
        /// Gets the Crs identifier for this scale set
        /// </summary>
        public CrsIdentifier Crs
        {
            get; private set;
            
        }

        /// <summary>
        /// Gets a value indicating the name of the scale set
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public ScaleSetItem this[int level]
        {
            get { return _items[level]; }
        }

        /// <summary>
        /// Accessor to a pixel size
        /// </summary>
        /// <param name="scaleDenominator"></param>
        /// <returns></returns>
        public double? this[double scaleDenominator]
        {
            get
            {
                for (var i = 0; i < _items.Length; i++)
                {
                    if (Math.Abs(scaleDenominator - _items[i].ScaleDenominator) < 1e-7)
                    {
                        return _items[i].PixelSize;
                    }
                    if (_items[i].ScaleDenominator < scaleDenominator) break;
                }
                return null;
            }
        }
    }
}