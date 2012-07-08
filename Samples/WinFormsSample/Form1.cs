using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using BruTile;
using BruTile.Web;

namespace WinFormsSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            foreach (var knownOsmTileServer in Enum.GetNames(typeof(KnownOsmTileServers)))
            {
                if (knownOsmTileServer == "Custom")
                    continue;
                listBox1.Items.Add(knownOsmTileServer);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            osmImage1.ApiKey = textBox1.Text;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
                return;

            osmImage1.OsmServer =
                (KnownOsmTileServers) Enum.Parse(typeof (KnownOsmTileServers), (string) listBox1.SelectedItem);
        }

    }
}
