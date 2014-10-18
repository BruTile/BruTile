// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.
using System.Reflection;
using System.Runtime.Serialization;

namespace BruTile.Web
{
    internal class BasicRequestSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var br = (BasicRequest)obj;
            info.AddValue("urlFormatter", Utility.GetFieldValue(br, "_urlFormatter", BindingFlags.NonPublic | BindingFlags.Instance, string.Empty));
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var br = (BasicRequest)obj;
            Utility.SetFieldValue(ref obj, "_urlFormatter", BindingFlags.NonPublic | BindingFlags.Instance, info.GetString("urlFormatter"));
            return br;
        }
    }
}