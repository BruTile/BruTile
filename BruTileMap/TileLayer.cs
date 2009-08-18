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
using System.ComponentModel;
using Tiling;

namespace BruTileMap
{
  public class TileLayer<T> : IDisposable
  {
    ITileSchema schema;
    TileFetcher<T> tileFetcher;
    public event AsyncCompletedEventHandler DataUpdated;
    MemoryCache<T> memoryCache = new MemoryCache<T>(40, 60);
    const int maxRetries = 3;
    ITileFactory<T> tileFactory;

    public ITileSchema Schema
    {
      //TODO: check if we need realy need this property
      get { return schema; }
      set { schema = value; }
    }

    public TileLayer(ITileProvider tileProvider, ITileSchema schema, ITileFactory<T> tileFactory)
        : this(tileProvider, schema, tileFactory, new NullCache())
    {
    }

    public TileLayer(ITileProvider tileProvider, ITileSchema schema, ITileFactory<T> tileFactory, ITileCache<byte[]> fileCache)
    {
      this.schema = schema;
      tileFetcher = new TileFetcher<T>(tileProvider, memoryCache, schema, fileCache);
      this.tileFactory = tileFactory;
      RegisterEventHandlers();
    }

    private void RegisterEventHandlers()
    {
        tileFetcher.FetchCompleted += new FetchCompletedEventHander(tileFetcher_FetchCompleted);
    }

    private void UnRegisterEventHandlers()
    {
        tileFetcher.FetchCompleted -= new FetchCompletedEventHander(tileFetcher_FetchCompleted);
    }

    public MemoryCache<T> MemoryCache
    {
      get { return memoryCache; }
    }

    public void UpdateData(Extent extent, double resolution)
    {
      tileFetcher.UpdateData(extent, resolution);
    }

    void tileFetcher_FetchCompleted(object sender, FetchCompletedEventArgs e)
    {
      if (!e.Cancelled && e.Error == null)
      {
        TileInfo tile = e.TileInfo;
        byte[] image = e.Image;
        System.Exception error = e.Error;
        if (memoryCache.Find(tile.Key) == null)
        {
          try
          {
            T bitmap = tileFactory.GetTile(image);
            memoryCache.Add(tile.Key, bitmap);
          }
          catch (Exception ex)
          {
            error = ex;
          }
        }
        OnDataUpdated(new AsyncCompletedEventArgs(error, e.Cancelled, null));
      }
      else if ((e.Error is System.Net.WebException) && (e.TileInfo.Retries < maxRetries))
      {
        e.TileInfo.Retries++;
        //todo: implement retries
      }
      else
      {
        OnDataUpdated(new AsyncCompletedEventArgs(e.Error, e.Cancelled, null));
      }
    }

    protected void OnDataUpdated(AsyncCompletedEventArgs e)
    {
      if (DataUpdated != null)
        DataUpdated(this, e);
    }

    private class NullCache : ITileCache<byte[]>
    {
        public NullCache()
        {
        }

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

    #region IDisposable Members

    public void Dispose()
    {
        UnRegisterEventHandlers();
    }

    #endregion
  }
}
