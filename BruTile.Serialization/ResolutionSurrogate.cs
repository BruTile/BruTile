using System.Runtime.Serialization;

namespace BruTile
{
    internal class ResolutionSurrogate : ISerializationSurrogate
    {
        #region Implementation of ISerializationSurrogate

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var res = (Resolution)obj;
            info.AddValue("id", res.Id);
            info.AddValue("upp", res.UnitsPerPixel);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var res = (Resolution)obj;
            res.Id = info.GetString("id");
            res.UnitsPerPixel = info.GetDouble("upp");
            return res;
        }

        #endregion
    }
}