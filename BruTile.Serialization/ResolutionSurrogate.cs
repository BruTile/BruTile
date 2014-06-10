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
            info.AddValue("sd", res.ScaleDenominator);
            info.AddValue("t", res.Top);
            info.AddValue("l", res.Left);
            info.AddValue("tw", res.TileWidth);
            info.AddValue("th", res.TileHeight);
            info.AddValue("mw", res.MatrixWidth);
            info.AddValue("mh", res.MatrixHeight);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var res = (Resolution)obj;
            res.Id = info.GetString("id");
            res.UnitsPerPixel = info.GetDouble("upp");
            res.ScaleDenominator = info.GetDouble("sd");
            res.Top = info.GetDouble("t");
            res.Left = info.GetDouble("l");
            res.TileWidth = info.GetInt32("tw");
            res.TileHeight = info.GetInt32("th");
            res.MatrixWidth = info.GetInt32("mw");
            res.MatrixHeight = info.GetInt32("mh");
            return res;
        }

        #endregion
    }
}