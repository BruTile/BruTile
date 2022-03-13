using System.Windows.Forms;

namespace BruTile.Samples.MbTiles
{
    partial class MbTilesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslPosition = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslExtent = new System.Windows.Forms.ToolStripStatusLabel();
            this.picMap = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getSampleFileFromInternetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMap)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslPosition,
            this.tsslExtent});
            this.statusStrip1.Location = new System.Drawing.Point(0, 416);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(700, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslPosition
            // 
            this.tsslPosition.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.tsslPosition.Name = "tsslPosition";
            this.tsslPosition.Size = new System.Drawing.Size(0, 17);
            // 
            // tsslExtent
            // 
            this.tsslExtent.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.tsslExtent.Name = "tsslExtent";
            this.tsslExtent.Size = new System.Drawing.Size(0, 17);
            // 
            // picMap
            // 
            this.picMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picMap.Location = new System.Drawing.Point(0, 24);
            this.picMap.Name = "picMap";
            this.picMap.Size = new System.Drawing.Size(700, 392);
            this.picMap.TabIndex = 1;
            this.picMap.TabStop = false;
            this.picMap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picMap_MouseClick);
            this.picMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picMap_MouseMove);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(700, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.getSampleFileFromInternetToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.openToolStripMenuItem.Text = "&Open ...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItemClick);
            // 
            // getSampleFileFromInternetToolStripMenuItem
            // 
            this.getSampleFileFromInternetToolStripMenuItem.Name = "getSampleFileFromInternetToolStripMenuItem";
            this.getSampleFileFromInternetToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.getSampleFileFromInternetToolStripMenuItem.Text = "Get sample file from internet";
            this.getSampleFileFromInternetToolStripMenuItem.Click += new System.EventHandler(this.GetSampleFileFromInternetToolStripMenuItem_Click);
            // 
            // MbTilesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 438);
            this.Controls.Add(this.picMap);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MbTilesForm";
            this.Text = "MbTiles Sample";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picMap)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsslPosition;
        private System.Windows.Forms.ToolStripStatusLabel tsslExtent;
        private System.Windows.Forms.PictureBox picMap;

        private void picMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mapTransform == null) return;
            
            var pt = _mapTransform.MapToWorld(e.X, e.Y);
            tsslPosition.Text = string.Format("({0:N} East/{1:N} North", pt.X, pt.Y);
        }

        private async void picMap_MouseClick(object sender, MouseEventArgs e)
        {
            if (_mapTransform == null) return;
            
            if (e.Button == MouseButtons.Left)
            {
                var pt = _mapTransform.MapToWorld(e.X, e.Y);
                _mapTransform = new MapTransform(pt, _mapTransform.UnitsPerPixel, picMap.Width, picMap.Height);
                await RenderToBuffer();
            }
        }

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem getSampleFileFromInternetToolStripMenuItem;
    }
}