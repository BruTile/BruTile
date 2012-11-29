using System;
using System.Reflection;
using System.Runtime.Serialization;
using BruTile.Cache;

namespace BruTile.Web
{
    internal class WebTileProviderSurrogate : ISerializationSurrogate
    {
        #region Implementation of ISerializationSurrogate

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var wp = (WebTileProvider)obj;
            info.AddValue("requestType", wp.Request.GetType());
            info.AddValue("request",  wp.Request);

            ITileCache<byte[]> defaultCache = new Cache.NullCache();
            var cache = Utility.GetFieldValue(wp, "_cache", BindingFlags.NonPublic | BindingFlags.Instance, defaultCache);
            if (cache == null) cache = new NullCache();
            info.AddValue("cacheType", cache.GetType());
            info.AddValue("cache", cache);

            info.AddValue("userAgent", Utility.GetPropertyValue(obj, "UserAgent", BindingFlags.NonPublic | BindingFlags.Instance, string.Empty));
            info.AddValue("referer", Utility.GetPropertyValue(obj, "Referer", BindingFlags.NonPublic | BindingFlags.Instance, string.Empty));
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var wp = (WebTileProvider)obj;
            var type = (Type) info.GetValue("requestType", typeof (Type));
            Utility.SetFieldValue(ref obj, "_request", Utility.PrivateInstance, (IRequest)info.GetValue("request", type));

            type = (Type)info.GetValue("cacheType", typeof(Type));
            Utility.SetFieldValue(ref obj, "_cache", Utility.PrivateInstance, info.GetValue("cache", type));

            Utility.SetFieldValue(ref obj, "_userAgent", newValue: info.GetString("userAgent"));
            Utility.SetFieldValue(ref obj, "_referer", newValue: info.GetString("referer"));

            return wp;
        }

        #endregion
    }
}