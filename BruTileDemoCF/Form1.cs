using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BruTileDemoCF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void zoomIn_Click(object sender, EventArgs e)
        {
            mapControl1.ZoomIn();
        }

        private void ZoomOut_Click(object sender, EventArgs e)
        {
            mapControl1.ZoomOut();
        }
    }
}