// Copyright 2008 - Paul den Dulk (Geodan)
// 
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using BruTileMap;

namespace BruTileWindows
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
