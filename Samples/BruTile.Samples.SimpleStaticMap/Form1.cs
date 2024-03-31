// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using BruTile.Tms;

namespace BruTile.Samples.SimpleStaticMap;

public partial class Form1 : Form
{
    private readonly Bitmap _buffer;
    private readonly HttpClient _httpClient = Web.HttpClientBuilder.Build();

    //a list of resolutions in which the tiles are stored
    private readonly double[] _unitsPerPixelArray =
    [        
        156543.033900000, 78271.516950000, 39135.758475000, 19567.879237500, 9783.939618750,
        4891.969809375, 2445.984904688, 1222.992452344, 611.496226172, 305.748113086,
        152.874056543, 76.437028271, 38.218514136, 19.109257068, 9.554628534, 4.777314267,
        2.388657133, 1.194328567, 0.597164283
    ];

    public Form1()
    {
        InitializeComponent();

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("UserAgentOfBruTilesSimpleStaticMapSample");
        _buffer = new Bitmap(Width, Height);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.DrawImage(_buffer, 0, 0);
    }

    private async void Form1Load(object sender, EventArgs e)
    {
        var viewport = new Viewport(new PointF(629816f, 6805085f), 1222.992452344f, Width, Height);

        var schema = CreateTileSchema();
        var tiles = schema.GetTileInfos(viewport.Extent, Utilities.GetNearestLevel(schema.Resolutions, viewport.UnitsPerPixel));

        var requestBuilder = new TmsRequest(new Uri("https://tile.openstreetmap.org"), "png");

        var graphics = Graphics.FromImage(_buffer);
        foreach (var tile in tiles)
        {
            var url = requestBuilder.GetUri(tile);
            var bytes = await FetchTileAsync(url).ConfigureAwait(false);
            var bitmap = new Bitmap(new MemoryStream(bytes));
            var destination = viewport.WorldToScreen(tile.Extent.MinX, tile.Extent.MinY, tile.Extent.MaxX, tile.Extent.MaxY);
            graphics.DrawImage(bitmap, RoundToPixel(destination));
        }
        Invalidate();
    }

    private async Task<byte[]> FetchTileAsync(Uri arg)
    {
        return await _httpClient.GetByteArrayAsync(arg).ConfigureAwait(false);
    }

    private ITileSchema CreateTileSchema()
    {
        var schema = new TileSchema
        {
            Name = "OpenStreetMap",
            OriginX = -20037508.342789,
            OriginY = 20037508.342789,
            YAxis = YAxis.OSM,
            Extent = new Extent(-20037508.342789, -20037508.342789, 20037508.342789, 20037508.342789),
            Format = "png",
            Srs = "EPSG:900913"
        };

        var i = 0;
        foreach (var unitsPerPixel in _unitsPerPixelArray)
        {
            var level = i++;
            schema.Resolutions[level] = new Resolution
            (
                level,
                unitsPerPixel
            );
        }
        return schema;
    }

    private static Rectangle RoundToPixel(RectangleF dest)
    {
        // To get seamless aligning you need to round the locations
        // not the width and height
        var result = new Rectangle(
            (int)Math.Round(dest.Left),
            (int)Math.Round(dest.Top),
            (int)(Math.Round(dest.Right) - Math.Round(dest.Left)),
            (int)(Math.Round(dest.Bottom) - Math.Round(dest.Top)));
        return result;
    }
}
