using System;

#if !SILVERLIGHT

using System.Runtime.Serialization;

#endif

namespace BruTile.Web.Wms
{
#if !SILVERLIGHT

    [Serializable]
#endif
    public class WmsPropertyException : System.Exception
    {
        public WmsPropertyException()
        {
        }

        public WmsPropertyException(string message)
            : base(message)
        {
        }

        public WmsPropertyException(string message, System.Exception inner)
            : base(message, inner)
        {
        }

#if !SILVERLIGHT

        protected WmsPropertyException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

#endif

        public static WmsPropertyException PositiveInteger(string property, int value)
        {
            return new WmsPropertyException(string.Format("{0} requires a positve integer value. You tried to assign {1}", property, value));
        }

        public static System.Exception NonNegativeInteger(string property, int value)
        {
            return new WmsPropertyException(string.Format("{0} requires a non-negative integer value. You tried to assign {1}", property, value));
        }
    }
}