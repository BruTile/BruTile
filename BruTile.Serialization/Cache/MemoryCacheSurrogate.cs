using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace BruTile.Cache
{
    internal class MemoryCacheSurrogate<T> : ISerializationSurrogate
    {
        #region Implementation of ISerializationSurrogate

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var ms = (MemoryCache<T>) obj;
            info.AddValue("min", Utility.GetFieldValue(ms, "_minTiles", BindingFlags.NonPublic | BindingFlags.Instance, 100));
            info.AddValue("max", Utility.GetFieldValue(ms, "_maxTiles", BindingFlags.NonPublic | BindingFlags.Instance, 200));
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Utility.SetFieldValue(ref obj, "_minTiles", BindingFlags.NonPublic | BindingFlags.Instance, info.GetInt32("min"));
            Utility.SetFieldValue(ref obj, "_maxTiles", BindingFlags.NonPublic | BindingFlags.Instance, info.GetInt32("max"));

            Utility.SetFieldValue(ref obj, "_syncRoot", BindingFlags.NonPublic | BindingFlags.Instance, new object());
            Utility.SetFieldValue(ref obj, "_bitmaps", BindingFlags.NonPublic | BindingFlags.Instance, new Dictionary<TileIndex, byte[]>());
            Utility.SetFieldValue(ref obj, "_touched", BindingFlags.NonPublic | BindingFlags.Instance, new Dictionary<TileIndex, DateTime>());
            return obj;
        }

        #endregion
    }
}