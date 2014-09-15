using System.Runtime.Serialization;

namespace BruTile.Cache
{
    internal class NullCacheSurrogate : ISerializationSurrogate
    {
        #region Implementation of ISerializationSurrogate

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            // Nothing to do
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            // Again, nothing to do
            return obj;
        }

        #endregion
    }
}