// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Threading;

namespace BruTile.Extensions
{
    public static class HttpWebRequestExtensions
    {
        /// <summary>
        /// A blocking operation that does not continue until a response has been
        /// received for a given <see cref="HttpWebRequest"/>, or the request
        /// timed out.
        /// </summary>
        /// <param name="request">The request to be sent.</param>
        /// <param name="timeout">An optional timeout.</param>
        /// <returns>The response that was received for the request.</returns>
        /// <exception cref="TimeoutException">If the <paramref name="timeout"/>
        /// parameter was set, and no response was received within the specified
        /// time.</exception>
        /// <see href="http://www.hardcodet.net/2010/02/blocking-httpwebrequest-getresponse-for-silverlight"/>
        public static HttpWebResponse GetSyncResponse(this HttpWebRequest request,
                                                  int? timeout)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            /*
             * TODO:
             * Check if this method is usable when called from the UI thread in Silverlight
             * if not, see how we could throw an exception when this is the case (otherwise it just hangs)
             */
            var waitHandle = new AutoResetEvent(false);
            HttpWebResponse response = null;
            Exception exception = null;

            AsyncCallback callback = ar =>
            {
                try
                {
                    //get the response
                    response = (HttpWebResponse) request.EndGetResponse(ar);
                }
                catch (WebException we)
                {
                    if (we.Status != WebExceptionStatus.RequestCanceled)
                    {
                        exception = we;
                    }
                }
                catch (Exception e)
                {
                    exception = e;
                }
                finally
                {
                    //setting the handle unblocks the loop below
                    waitHandle.Set();
                }
            };

            //request response async
            var asyncResult = request.BeginGetResponse(callback, null);
            if (asyncResult.CompletedSynchronously) return response;

            var hasSignal = waitHandle.WaitOne(timeout ?? Timeout.Infinite);
            if (!hasSignal)
            {
                try
                {
                    if (response != null)
                        return response;
                    if (request != null)
                        request.Abort();
                }
                catch
                {
                    throw new TimeoutException("No response received in time.");
                }

                throw new TimeoutException("No response received in time.");
            }

            //bubble exception that occurred on worker thread
            if (exception != null) throw exception;

            return response;
        }
    }
}
