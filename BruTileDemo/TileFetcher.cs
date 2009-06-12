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

using System;
using System.Collections.Generic;
using Tiling;
using Toub.Threading;

namespace BruTileDemo
{
  public delegate void FetchCompletedEventHander(object sender, FetchCompletedEventArgs e);

  class TileFetcher
  {
    public event FetchCompletedEventHander FetchCompleted;
    ITileCache<byte[]> cache;
    List<TileKey> queued = new List<TileKey>();
    ITileProvider tileProvider;
    object syncRoot = new object();

    public TileFetcher(ITileProvider tileProvider)
      : this(tileProvider, new NullCache())
    {
    }

    public TileFetcher(ITileProvider tileProvider, ITileCache<byte[]> cache)
    {
      this.cache = cache;
      this.tileProvider = tileProvider;
    }

    public void Fetch(TileInfo tile, int priority)
    {
      lock (syncRoot)
      {
        if (queued.Contains(tile.Key)) return;
        queued.Add(tile.Key);
      }
      
      PriorityThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(FetchOnThread),
        new FetchOnThreadEventArgs(tile, new FetchCompletedEventHander(FetchCompleted)), priority);
    }

    private void FetchOnThread(object state)
    {
      System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.BelowNormal;

      FetchOnThreadEventArgs args = (FetchOnThreadEventArgs)state;
      TileInfo tile = args.TileInfo;
      Exception error = null;
      byte[] image = null;

      try
      {
        try
        {
          if (cache != null)
            image = cache.Find(tile.Key);
          if (image == null) //if not on disk get from web
          {
            image = tileProvider.GetTile(tile);
            if (cache != null)
             cache.Add(tile.Key, image); //now cache on disk for next time.
          }
        }
        catch (Exception ex) //This is a bit weird. I need to catch the exception before passing it to FetchCompleted.
        {
          error = ex;
        }

        args.FetchCompleted(this, new FetchCompletedEventArgs(error, false, tile, image));
      }
      finally
      {
        lock (syncRoot)
        {
          if (queued.Contains(tile.Key))
            queued.Remove(tile.Key); //I need to Remove the tile from the queue after calling FetchCompleted otherwise the caller could do double requests.
        }
      }
    }

    public void ReportBadTile(TileInfo tile)
    {
      cache.Remove(tile.Key);
    }

    public void ClearQueue()
    {
      PriorityThreadPool.EmptyQueue();
      queued.Clear();
    }

    private class FetchOnThreadEventArgs
    {
      public FetchOnThreadEventArgs(TileInfo tileInfo, FetchCompletedEventHander fetchCompleted)
      {
        this.TileInfo = tileInfo;
        this.FetchCompleted = fetchCompleted;
      }

      public TileInfo TileInfo;
      public FetchCompletedEventHander FetchCompleted;
    }

    private class NullCache : ITileCache<byte[]>
    {
      public void Add(TileKey key, byte[] image)
      {
        //do nothing
      }

      public void Remove(TileKey key)
      {
        throw new NotImplementedException(); //and should not
      }

      public byte[] Find(TileKey key)
      {
        return null; 
      }
    }
  }

  public class FetchCompletedEventArgs
  {
    public FetchCompletedEventArgs(Exception error, bool cancelled, TileInfo tileInfo, byte[] image)
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
