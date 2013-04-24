using System;
using System.Net;
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
            info.AddValue("RequestType", wp.Request.GetType());
            info.AddValue("Request", wp.Request);

            Func<Uri, HttpWebRequest> defaultWebRequestFactory = (uri => (HttpWebRequest) WebRequest.Create(uri));
            var webRequestFactory = Utility.GetFieldValue(wp, "_webRequestFactory", BindingFlags.NonPublic | BindingFlags.Instance, defaultWebRequestFactory);
            info.AddValue("_webRequestFactoryType", webRequestFactory.GetType());
            info.AddValue("_webRequestFactory", webRequestFactory);

            ITileCache<byte[]> defaultCache = new NullCache();
            var cache = Utility.GetFieldValue(wp, "PersistentCache", BindingFlags.Public | BindingFlags.Instance, defaultCache);
            if (cache == null) cache = new NullCache();
            info.AddValue("PersistentCacheType", cache.GetType());
            info.AddValue("PersistentCache", cache);

            info.AddValue("userAgent", Utility.GetPropertyValue(obj, "UserAgent", BindingFlags.NonPublic | BindingFlags.Instance, string.Empty));
            info.AddValue("referer", Utility.GetPropertyValue(obj, "Referer", BindingFlags.NonPublic | BindingFlags.Instance, string.Empty));
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var wp = (WebTileProvider)obj;
            var type = (Type)info.GetValue("RequestType", typeof(Type));
            Utility.SetPropertyValue(ref obj, "Request", BindingFlags.Public | BindingFlags.Instance, (IRequest)info.GetValue("Request", type));

            type = (Type)info.GetValue("_webRequestFactoryType", typeof(Type));
            Utility.SetFieldValue(ref obj, "_webRequestFactory", BindingFlags.NonPublic | BindingFlags.Instance, info.GetValue("_webRequestFactory", type));

            type = (Type)info.GetValue("PersistentCacheType", typeof(Type));
            Utility.SetPropertyValue(ref obj, "PersistentCache", BindingFlags.Public | BindingFlags.Instance, info.GetValue("PersistentCache", type));

            Utility.SetFieldValue(ref obj, "_userAgent", newValue: info.GetString("userAgent"));
            Utility.SetFieldValue(ref obj, "_referer", newValue: info.GetString("referer"));

            return wp;
        }

        #endregion
    }
}