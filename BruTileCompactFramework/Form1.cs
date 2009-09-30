// Copyright 2009 - Paul den Dulk (Geodan)
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
using System.Windows.Forms;
using System.Drawing;
using BruTileMap;
using BruTileForms;
using DemoConfig;
using BruTile;

namespace BruTileCompactFramework
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
      this.Load += new EventHandler(Form1_Load);
    }

    void Form1_Load(object sender, EventArgs e)
    {
      IConfig config = new ConfigOsm();
      mapControl1.RootLayer = new TileLayer<Bitmap>(new FetchTileWeb(config.RequestBuilder), config.TileSchema, new TileFactory());
    }

    private void zoomIn_Click(object sender, EventArgs e)
    {
      mapControl1.ZoomIn();
    }

    private void ZoomOut_Click(object sender, EventArgs e)
    {
      mapControl1.ZoomOut();
    }

    private void osmMenu_Click(object sender, EventArgs e)
    {
      IConfig config = new ConfigOsm();
      mapControl1.RootLayer = new TileLayer<Bitmap>(
        new FetchTileWeb(config.RequestBuilder), 
        config.TileSchema, new TileFactory());
    }

    private void bingMenu_Click(object sender, EventArgs e)
    {
      IConfig config = new ConfigVE();
      mapControl1.RootLayer = new TileLayer<Bitmap>(
          new FetchTileWeb(config.RequestBuilder), 
          config.TileSchema, 
          new TileFactory());
    }
  }
}