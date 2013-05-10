using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BruTile.Cache;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BruTile.Demo
{
    class Fetcher 
    {
        public event EventHandler DataChanged;

        protected void OnDataChanged()
        {
            if (DataChanged != null)
            {
                DataChanged(this, EventArgs.Empty);
            }
        }

        public void Fetch(Viewport viewport, ITileSource tileSource, ITileCache<Image> tileCache)
        {
            var level = Utilities.GetNearestLevel(tileSource.Schema.Resolutions, viewport.Resolution);
            var tileInfos = tileSource.Schema.GetTilesInView(viewport.Extent, level);
            foreach (var tileInfo in tileInfos)
            {
                var image = tileCache.Find(tileInfo.Index);
                if (image == null)
                {
                    TileInfo info = tileInfo;

                    var taskCompletionSource = new TaskCompletionSource<byte[]>();
                    
                    // Start a background task that will complete tcs1.Task
                    Task.Factory.StartNew(() => taskCompletionSource.SetResult(tileSource.Provider.GetTile(info)));
                    tileCache.Add(info.Index, TileToImage(taskCompletionSource.Task.Result));
                    OnDataChanged();
                    //Async.Call(
                    //    () => tileSource.Provider.GetTile(info), 
                    //    bytes => tileCache.Add(info.Index, TileToImage(bytes)));
                }
            }
        }
   
        private Image TileToImage(byte[] tile)
        {
            var stream = new MemoryStream(tile);
            var bitmapImage = new BitmapImage();
            stream.Position = 0;
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            var image = new Image();
            image.BeginInit();
            image.Source = bitmapImage;
            image.EndInit();
            return image;
        }
    }
}
