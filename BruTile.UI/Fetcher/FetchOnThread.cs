using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BruTile;

namespace BruTile.UI.Fetcher
{
    class FetchOnThread
        //This class exists because in CF you can only pass arguments to a thread using a class constructor.
        //Once support for CF is dropped (replaced by SL on WinMo?) this class should be removed.
    {

        ITileProvider tileProvider;
        TileInfo tileInfo;
        FetchTileCompletedEventHandler fetchTileCompleted;

        public FetchOnThread(ITileProvider tileProvider, TileInfo tileInfo, FetchTileCompletedEventHandler fetchTileCompleted)
        {
            this.tileProvider = tileProvider;
            this.tileInfo = tileInfo;
            this.fetchTileCompleted = fetchTileCompleted;
        }

        public void FetchTile()
        {
            Exception error = null;
            byte[] image = null;

            try
            {
                image = tileProvider.GetTile(tileInfo);
            }
            catch (Exception ex) //This may seem a bit weird. We catch the exception to pass it as an argument. This is because we are on a worker thread here, we cannot just let it fall through. 
            {
                error = ex;
            }
            this.fetchTileCompleted(this, new FetchTileCompletedEventArgs(error, false, tileInfo, image));
        }
    }

    public delegate void FetchTileCompletedEventHandler(object sender, FetchTileCompletedEventArgs e);

    public class FetchTileCompletedEventArgs
    {
        public FetchTileCompletedEventArgs(Exception error, bool cancelled, TileInfo tileInfo, byte[] image)
        {
            this.Error = error;
            this.Cancelled = cancelled;
            this.TileInfo = tileInfo;
            this.Image = image;
        }

        public Exception Error;
        public bool Cancelled;
        public TileInfo TileInfo;
        public byte[] Image;
    }
}
