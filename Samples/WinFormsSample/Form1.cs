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

            foreach (var knownTileServer in Enum.GetNames(typeof(KnownTileServers)))
            {
                listBox1.Items.Add(knownTileServer);
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
                (KnownTileServers) Enum.Parse(typeof (KnownTileServers), (string) listBox1.SelectedItem);
        }

    }
}
