using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BruTile.Cache;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace BruTile.Metro
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
                    Task.Factory.StartNew(() =>
                        {
                            taskCompletionSource.SetResult(tileSource.Provider.GetTile(info));
                        });
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
            var memoryStream = new MemoryStream(tile);
            var memoryRandomAccessStream = new MemoryRandomAccessStream(memoryStream);
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(memoryRandomAccessStream);
            var image = new Image();
            image.Source = bitmapImage;
            return image;
        }
    }
}
