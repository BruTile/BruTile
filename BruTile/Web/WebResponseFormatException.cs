// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Net;

namespace BruTile.Web
{
    public class WebResponseFormatException : WebException
    {
        public WebResponseFormatException() { }
        public WebResponseFormatException(string message) : base(message) { }
        public WebResponseFormatException(string message, Exception innerException) : base(message, innerException) { }
    }
}
