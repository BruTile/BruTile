using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DemoConfig;
using BruTile.UI.Forms;
using BruTile.UI;
using BruTile;

namespace BruTile.UI.WinForms
{
  public partial class Form1 : Form
  {
    //WARNING: This WinForms implementation is very basic. 
    //Contact me if you actually use BruTileWinforms I might make some improvements. PDD

    public Form1()
    {
      InitializeComponent();
      InitTransform();
      this.Load += new EventHandler(Form1_Load);
    }

    private void InitTransform()
    {
      mapControl1.Transform.Center = new PointF(629816, 6805085);
      mapControl1.Transform.Resolution = 1222.992452344;
      mapControl1.Transform.Width = (float)this.Width;
      mapControl1.Transform.Height = (float)this.Height;
    }

    void Form1_Load(object sender, EventArgs e)
    {
        mapControl1.RootLayer = new TileLayer(new ConfigOsm().CreateTileSource());
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
      mapControl1.RootLayer = new TileLayer(new ConfigOsm().CreateTileSource());
    }

    private void bingMenu_Click(object sender, EventArgs e)
    {
      mapControl1.RootLayer = new TileLayer(new ConfigVE().CreateTileSource());
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.mapControl1.ZoomIn();
    }

    private void button2_Click(object sender, EventArgs e)
    {
      this.mapControl1.ZoomOut();
    }
  }
}
