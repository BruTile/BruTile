using System;
using System.IO;
using System.Net;
using System.Threading;

namespace BruTile.Web
{
    public static partial class RequestHelper
    {
        public static byte[] FetchImage(Uri uri, string userAgent, string referer, bool keepAlive)
        {
            var webClient = (HttpWebRequest)WebRequest.Create(uri);

            //it seems Silverlight has explicit exceptions built in for assigning user-agent and referer. 
            //I seems there is no way around this. PDD.
            //Todo: remove this overload from SL or throw exception.
            //!!!if (!String.IsNullOrEmpty(userAgent)) webClient.Headers["user-agent"] = userAgent;
            //!!!if (!String.IsNullOrEmpty(referer)) webClient.Headers["Referer"] = referer;

            //we use a waithandle to fake a synchronous call
            var waitHandle = new AutoResetEvent(false);
            IAsyncResult result = webClient.BeginGetResponse(WebClientOpenReadCompleted, waitHandle);

            //This trick works because the this is called on a worker thread. In SL it wont work if you call
            //it from the main thread because the main thead dispatches the worker threads and it starts waiting 
            //before it dispatches the worker thread.
            waitHandle.WaitOne();

            var response = (HttpWebResponse)webClient.EndGetResponse(result);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("An error occurred while fetching the tile");
            }
            if (!response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {
                string message = CreateErrorMessage(response, uri.AbsoluteUri);
                throw (new WebResponseFormatException(message, null));
            }
            using (Stream responseStream = response.GetResponseStream())
            {
                return Utilities.ReadFully(responseStream);
            }
        }

        private static void WebClientOpenReadCompleted(IAsyncResult e)
        {
            //Call Set() so that WaitOne can proceed.
            ((AutoResetEvent)e.AsyncState).Set();
        }

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
        public static HttpWebResponse GetResponse(this HttpWebRequest request,
                                                  int? timeout)
        {
            if (request == null) throw new ArgumentNullException("request");

            if (System.Windows.Deployment.Current.Dispatcher.CheckAccess())
            {
                const string msg = "Invoking this method on the UI thread is forbidden.";
                throw new InvalidOperationException(msg);
            }

            var waitHandle = new AutoResetEvent(false);
            HttpWebResponse response = null;
            Exception exception = null;

            AsyncCallback callback = ar =>
            {
                try
                {
                    //get the response
                    response = (HttpWebResponse)request.EndGetResponse(ar);
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

            bool hasSignal = waitHandle.WaitOne(timeout ?? System.Threading.Timeout.Infinite);
            if (!hasSignal)
            {
                throw new TimeoutException("No response received in time.");
            }

            //bubble exception that occurred on worker thread
            if (exception != null) throw exception;

            return response;
        }

    }
}