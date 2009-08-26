using System.IO;
using BruTileMap;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BruTileDemo
{
    class TileFactory : ITileFactory<Image>
    {
        #region ITileFactory<Bitmap> Members

        public Image GetTile(byte[] bytes)
        {
          BitmapImage bitmapImage = new BitmapImage();
          bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
          bitmapImage.SetSource(new MemoryStream(bytes));

          Image image = new Image();
          image.Opacity = 0; //TODO: think of a better place to initialize this.
          image.Source = bitmapImage;
          return image;
        }

        #endregion
    }
}
