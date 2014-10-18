// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace BruTile.Web
{
    internal class TileSourceSurrogate : ISerializationSurrogate
    {
        #region Implementation of ISerializationSurrogate

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var ot = (OsmTileSource)obj;
            info.AddValue("providerType", ot.Provider.GetType());
            info.AddValue("provider", ot.Provider);
            info.AddValue("schemaType", ot.Schema.GetType());
            info.AddValue("schema", ot.Schema);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var type = (Type) info.GetValue("providerType", typeof (Type));
            Utility.SetPropertyValue(ref obj, "Provider", BindingFlags.Public | BindingFlags.Instance, (ITileProvider)info.GetValue("provider", type));
            type = (Type)info.GetValue("schemaType", typeof(Type));
            Utility.SetPropertyValue(ref obj, "Schema", BindingFlags.Public | BindingFlags.Instance, (ITileSchema)info.GetValue("schema", type));
            return obj;
        }

        #endregion
    }
}