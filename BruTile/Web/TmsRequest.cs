#region License

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
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BruTile.Web
{
    public class TmsRequest : IRequest
    {
        private readonly string _baseUrl;
        private readonly IDictionary<string, Uri> _baseUrls;
        private readonly string _imageFormat;
        private readonly Dictionary<string, string> _customParameters;
        private readonly IList<string> _serverNodes;
        private readonly Random _random = new Random();
        private const string ServerNodeTag = "{S}";

        public TmsRequest(string baseUrl, string imageFormat, IList<string> serverNodes = null,
         Dictionary<string, string> customParameters = null)
            : this(imageFormat, serverNodes, customParameters)
        {
            _baseUrl = baseUrl;
            if (_baseUrl.Contains(ServerNodeTag))
            {
                if (serverNodes == null || serverNodes.Count == 0)
                    throw new Exception("The '" + ServerNodeTag + "' tag was set but no server nodes were specified");
            }
            if (serverNodes != null && serverNodes.Count > 0)
            {
               if (!_baseUrl.Contains(ServerNodeTag))
                    throw new Exception("Server nodes were specified but no '" + ServerNodeTag + "' tag was set");
            }
        }

        public TmsRequest(Uri baseUrl, string imageFormat, Dictionary<string, string> customParameters = null)
            : this(imageFormat, null, customParameters)
        {
            _baseUrl = baseUrl.ToString();
        }

        public TmsRequest(IDictionary<string, Uri> baseUrls, string imageFormat, 
            Dictionary<string, string> customParameters = null)
            : this(imageFormat, null, customParameters)
        {
            _baseUrls = baseUrls;
        }

        private TmsRequest(string imageFormat, IList<string> serverNodes = null, 
            Dictionary<string, string> customParameters = null)
        {
            _imageFormat = imageFormat;
            _serverNodes = serverNodes;
            _customParameters = customParameters;
        }

        /// <summary>
        /// Generates a URI at which to get the data for a tile.
        /// </summary>
        /// <param name="info">Information about a tile.</param>
        /// <returns>The URI at which to get the data for the specified tile.</returns>
        public Uri GetUri(TileInfo info)
        {
            var url = new StringBuilder(GetUrlForLevel(info.Index.LevelId));
            InsertRandomServerNode(url, _serverNodes, _random);
            AppendXY(url, info);
            AppendImageFormat(url, _imageFormat);
            AppendCustomParameters(url, _customParameters);
            return new Uri(url.ToString());
        }

        private string GetUrlForLevel(string levelId)
        {
            // if a single url is specified for all levels return that one plus the level id
            if (_baseUrl != null)
            {
                if (!_baseUrl.EndsWith("/")) _baseUrl += "/";
                return string.Format(CultureInfo.InvariantCulture, "{0}{1}/", _baseUrl, levelId);
            }
            // else return the url that was defined for the specific level
            if (!_baseUrls[levelId].EndsWith("/")) _baseUrls[levelId] += "/";
            return _baseUrls[levelId].ToString();
        }

        private static void InsertRandomServerNode(StringBuilder baseUrl, IList<string> serverNodes, Random random)
        {
            if (serverNodes != null)
            {
                baseUrl.Replace(ServerNodeTag, serverNodes[random.Next(serverNodes.Count)]);
            }
        }

        private static void AppendImageFormat(StringBuilder url, string imageFormat)
        {
            if (!string.IsNullOrEmpty(imageFormat))
            {
                url.AppendFormat(CultureInfo.InvariantCulture, ".{0}", imageFormat);
            }
        }

        private static void AppendXY(StringBuilder url, TileInfo info)
        {
            url.AppendFormat(CultureInfo.InvariantCulture, "{0}/{1}", info.Index.Col, info.Index.Row);
        }

        private static void AppendCustomParameters(StringBuilder url, Dictionary<string, string> customParameters)
        {
            if (customParameters == null) return;
            
            bool first = true;
            foreach (string name in customParameters.Keys)
            {
                string value = customParameters[name];
                url.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}={2}", first ? "?" : "&", name, value);
                first = false;
            }
        }
    }
}
