using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace BruTile.Metro
{
    static class Utilities
    {
        public static async Task<BitmapImage> TileToImage(byte[] tile)
        {
            var ims = await ByteArrayToRandomAccessStream(tile);
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(ims);
            return bitmapImage;
        }

        private static async Task<InMemoryRandomAccessStream> ByteArrayToRandomAccessStream(byte[] tile)
        {
            var stream = new InMemoryRandomAccessStream();
            DataWriter dataWriter = new DataWriter(stream);
            dataWriter.WriteBytes(tile);
            await dataWriter.StoreAsync();
            stream.Seek(0);
            return stream;
        }
    }
}
