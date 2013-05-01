using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace BruTile
{
    internal class TileSchemaSurrogate : ISerializationSurrogate
    {
        #region Implementation of ISerializationSurrogate

        public virtual void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var ts = (TileSchema) obj;
            info.AddValue("name", ts.Name);
            info.AddValue("srs", ts.Srs);
            info.AddValue("extent", ts.Extent);
            info.AddValue("originX", ts.OriginX);
            info.AddValue("originY", ts.OriginY);
            info.AddValue("width", ts.Width);
            info.AddValue("height", ts.Height);
            info.AddValue("format", ts.Format);
            info.AddValue("resolutionsType", ts.Resolutions.GetType());
            info.AddValue("resolutionsCount", ts.Resolutions.Count);
            for (var i = 0; i < ts.Resolutions.Count; i++ )
                info.AddValue(string.Format("resolution{0}", i), ts.Resolutions[i]);
            info.AddValue("axis", ts.Axis);
        }

        public virtual object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var ts = (TileSchema) obj;
            ts.Name = info.GetString("name");
            ts.Srs = info.GetString("srs");
            ts.Extent = (Extent)info.GetValue("extent", typeof(Extent));
            ts.OriginX = info.GetDouble("originX");
            ts.OriginY = info.GetDouble("originY");
            ts.Width = info.GetInt32("width");
            ts.Height = info.GetInt32("height");
            ts.Format = info.GetString("format");
            
            var type = (Type) info.GetValue("resolutionsType", typeof (Type));
            var list = (IList<Resolution>) Activator.CreateInstance(type);
            var count = info.GetInt32("resolutionsCount");
            for (var i = 0; i < count; i++)
                list.Add((Resolution)info.GetValue(string.Format("resolution{0}", i), typeof(Resolution)));    
            Utility.SetFieldValue(ref obj, "_resolutions", BindingFlags.NonPublic | BindingFlags.Instance, list);
            
            ts.Axis = (AxisDirection)info.GetInt32("axis");
            return ts;
        }

        #endregion
    }

    namespace Predefined
    {
        internal class BingSchemaSurrogate : TileSchemaSurrogate
        {
        }

        internal class GlobalMercatorSchemaSurrogate : TileSchemaSurrogate
        {
        }

        internal class WkstNederlandSchemaSurrogate : TileSchemaSurrogate
        {
        }

        internal class SphericalMercatorWorldSchemaSurrogate : TileSchemaSurrogate
        {
        }

        internal class SphericalMercatorWorldInvertedSchemaSurrogate : TileSchemaSurrogate
        {
        }
    }

}