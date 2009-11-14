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
using System.Net;
using BruTile;

namespace BruTileMap
{
    public class WebTileProvider : ITileProvider
    {
        IRequestBuilder requestBuilder;
        ITileCache<byte[]> fileCache;

        public WebTileProvider(IRequestBuilder requestBuilder)
            : this(requestBuilder, new NullCache())
        {
        }

        public WebTileProvider(IRequestBuilder requestBuilder, ITileCache<byte[]> fileCache)
        {
            if (requestBuilder == null) throw new ArgumentException("RequestBuilder can not be null");
            this.requestBuilder = requestBuilder;

            if (fileCache == null) throw new ArgumentException("File can not be null");
            this.fileCache = fileCache;
        }

        #region TileProvider Members

#if PocketPC
        //This if def is very ugly but it is a lot simpler compared to the dependency injection 
        //solution I had before. PDD.
            public void GetTile(TileInfo tileInfo, FetchCompletedEventHandler fetchCompleted)
        {
          Exception error = null;
          byte[] bytes = null;

          try
          {
            bytes = fileCache.Find(tileInfo.Key);
            if (bytes == null)
            {
              bytes = ImageRequest.GetImageFromServer(requestBuilder.GetUri(tileInfo));
              if (bytes != null)
                fileCache.Add(tileInfo.Key, bytes);
            }
          }
          catch (Exception ex) //This may seem a bit weird. We catch the exception to pass it as an argument. This is because we are on a worker thread here, we cannot just let it fall through. 
          {
            error = ex;
          }
          fetchCompleted(this, new FetchCompletedEventArgs(error, false, tileInfo, bytes)); 

        }
#else

        public void GetTile(TileInfo tileInfo, FetchCompletedEventHandler fetchCompleted)
        {
            WebClient webClient = new WebClient();

#if !SILVERLIGHT
            //proxy patch by Starnuto Di Topo:
            IWebProxy proxy = WebRequest.GetSystemWebProxy();
            proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            webClient.Proxy = proxy;
#endif

            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);
            webClient.OpenReadAsync(requestBuilder.GetUri(tileInfo), new AsyncEventArgs() { TileInfo = tileInfo, FetchCompleted = fetchCompleted });
        }

        private void webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            //TODO rewrite this
            AsyncEventArgs state = (AsyncEventArgs)e.UserState;
            Exception exception = null;
            byte[] bytes = null;

            if (e.Error != null || e.Cancelled)
            {
                exception = e.Error;
            }
            else
            {
                try
                {
                    bytes = BruTile.Utilities.ReadFully(e.Result);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            FetchCompletedEventArgs args = new FetchCompletedEventArgs(exception, e.Cancelled, state.TileInfo, bytes);
            state.FetchCompleted(this, args);
        }

#endif

        private class AsyncEventArgs
        {
            public TileInfo TileInfo { get; set; }
            public FetchCompletedEventHandler FetchCompleted { get; set; }
        }

        #endregion

        #region Private classes

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

        #endregion
    }
}
