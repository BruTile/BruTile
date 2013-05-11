using BruTile.Web;
using System;
using System.Windows.Forms;

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

        private void TextBox1TextChanged(object sender, EventArgs e)
        {
            osmImage1.ApiKey = textBox1.Text;
        }

        private void ListBox1SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
                return;

            osmImage1.OsmServer =
                (KnownOsmTileServers) Enum.Parse(typeof (KnownOsmTileServers), (string) listBox1.SelectedItem);
        }

    }
}
