﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using BruTile.Web;

namespace BruTile.Tms;

public class TmsUrlBuilder : IUrlBuilder
{
    private readonly string? _baseUrl;
    private readonly IDictionary<int, Uri>? _baseUrls;
    private readonly string _imageFormat;
    private readonly Dictionary<string, string>? _customParameters;
    private readonly IList<string>? _serverNodes;
    private readonly Random _random = new();
    private const string ServerNodeTag = "{S}";

    public TmsUrlBuilder(string baseUrl, string imageFormat, IList<string>? serverNodes = null,
     Dictionary<string, string>? customParameters = null)
        : this(imageFormat, serverNodes, customParameters)
    {
        _baseUrl = baseUrl;

        if (_baseUrl.Contains(ServerNodeTag))
        {
            if (serverNodes == null || serverNodes.Count == 0)
                throw new Exception("The '" + ServerNodeTag + "' tag was set but no server nodes were specified");
        }
        else
        {
            if (serverNodes != null && serverNodes.Count > 0)
                throw new Exception("Server nodes were specified but no '" + ServerNodeTag + "' tag was set");
        }
    }

    public TmsUrlBuilder(Uri baseUrl, string imageFormat, Dictionary<string, string>? customParameters = null)
        : this(imageFormat, null, customParameters)
    {
        _baseUrl = baseUrl.ToString();
    }

    public TmsUrlBuilder(IDictionary<int, Uri> baseUrls, string imageFormat,
        Dictionary<string, string>? customParameters = null)
        : this(imageFormat, null, customParameters)
    {
        _baseUrls = baseUrls;
    }

    private TmsUrlBuilder(string imageFormat, IList<string>? serverNodes = null,
        Dictionary<string, string>? customParameters = null)
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
    public Uri GetUrl(TileInfo info)
    {
        var url = new StringBuilder(GetUrlForLevel(info.Index.Level));
        InsertRandomServerNode(url, _serverNodes, _random);
        AppendXY(url, info);
        AppendImageFormat(url, _imageFormat);
        AppendCustomParameters(url, _customParameters);
        return new Uri(url.ToString());
    }

    private string GetUrlForLevel(int level)
    {
        var url = new StringBuilder();
        // If a single url is specified for all levels return that one plus the level id
        if (_baseUrl != null)
        {
            url.Append(_baseUrl);
            if (!_baseUrl.EndsWith('/'))
                url.Append('/');
            url.Append(level).Append('/');
        }
        else if (_baseUrls != null)
        {
            url.Append(_baseUrls[level]);
            if (!_baseUrls[level].ToString().EndsWith('/'))
                url.Append('/');
        }
        else
        {
            throw new Exception("Both _baseUrl and _baseUrls where null. This should never happen.");
        }
        return url.ToString();
    }

    private static void InsertRandomServerNode(StringBuilder baseUrl, IList<string>? serverNodes, Random random)
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

    private static void AppendCustomParameters(StringBuilder url, Dictionary<string, string>? customParameters)
    {
        if (customParameters == null) return;

        var first = true;
        foreach (var name in customParameters.Keys)
        {
            var value = customParameters[name];
            url.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}={2}", first ? "?" : "&", name, value);
            first = false;
        }
    }
}

[Obsolete("Use TmsUrlBuilder instead.")]
public class TmsRequest : TmsUrlBuilder, IUrlBuilder
{
    public TmsRequest(string baseUrl, string imageFormat, IList<string>? serverNodes = null,
               Dictionary<string, string>? customParameters = null)
        : base(baseUrl, imageFormat, serverNodes, customParameters)
    { }

    public TmsRequest(Uri baseUrl, string imageFormat, Dictionary<string, string>? customParameters = null)
        : base(baseUrl, imageFormat, customParameters)
    { }

    public TmsRequest(IDictionary<int, Uri> baseUrls, string imageFormat, Dictionary<string, string>? customParameters = null)
        : base(baseUrls, imageFormat, customParameters)
    { }
}
