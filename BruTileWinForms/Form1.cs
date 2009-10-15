using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DemoConfig;
using BruTileForms;
using BruTileMap;

namespace WindowsFormsApplication1
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
