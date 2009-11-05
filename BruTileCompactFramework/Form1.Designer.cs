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

using BruTileForms;

namespace BruTileCompactFramework
{
  partial class Form1
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.MainMenu mainMenu1;

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
        this.mainMenu1 = new System.Windows.Forms.MainMenu();
        this.menuItem1 = new System.Windows.Forms.MenuItem();
        this.osmMenu = new System.Windows.Forms.MenuItem();
        this.bingMenu = new System.Windows.Forms.MenuItem();
        this.mapControl1 = new BruTileForms.MapControl();
        this.ZoomOut = new System.Windows.Forms.Button();
        this.zoomIn = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // mainMenu1
        // 
        this.mainMenu1.MenuItems.Add(this.menuItem1);
        // 
        // menuItem1
        // 
        this.menuItem1.MenuItems.Add(this.osmMenu);
        this.menuItem1.MenuItems.Add(this.bingMenu);
        this.menuItem1.Text = "Layers";
        // 
        // osmMenu
        // 
        this.osmMenu.Text = "OSM";
        this.osmMenu.Click += new System.EventHandler(this.osmMenu_Click);
        // 
        // bingMenu
        // 
        this.bingMenu.Text = "Bing";
        this.bingMenu.Click += new System.EventHandler(this.bingMenu_Click);
        // 
        // mapControl1
        // 
        this.mapControl1.Location = new System.Drawing.Point(0, 0);
        this.mapControl1.Name = "mapControl1";
        this.mapControl1.RootLayer = null;
        this.mapControl1.Size = new System.Drawing.Size(240, 268);
        this.mapControl1.TabIndex = 0;
        this.mapControl1.Text = "mapControl1";
        // 
        // ZoomOut
        // 
        this.ZoomOut.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
        this.ZoomOut.Location = new System.Drawing.Point(8, 230);
        this.ZoomOut.Name = "ZoomOut";
        this.ZoomOut.Size = new System.Drawing.Size(30, 30);
        this.ZoomOut.TabIndex = 1;
        this.ZoomOut.Text = "-";
        this.ZoomOut.Click += new System.EventHandler(this.ZoomOut_Click);
        // 
        // zoomIn
        // 
        this.zoomIn.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
        this.zoomIn.Location = new System.Drawing.Point(43, 230);
        this.zoomIn.Name = "zoomIn";
        this.zoomIn.Size = new System.Drawing.Size(30, 30);
        this.zoomIn.TabIndex = 1;
        this.zoomIn.Text = "+";
        this.zoomIn.Click += new System.EventHandler(this.zoomIn_Click);
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
        this.AutoScroll = true;
        this.ClientSize = new System.Drawing.Size(240, 268);
        this.Controls.Add(this.zoomIn);
        this.Controls.Add(this.ZoomOut);
        this.Controls.Add(this.mapControl1);
        this.Menu = this.mainMenu1;
        this.MinimizeBox = false;
        this.Name = "Form1";
        this.Text = "Form1";
        this.ResumeLayout(false);

    }

    #endregion

    private MapControl mapControl1;
    private System.Windows.Forms.Button ZoomOut;
    private System.Windows.Forms.Button zoomIn;
    private System.Windows.Forms.MenuItem menuItem1;
    private System.Windows.Forms.MenuItem osmMenu;
    private System.Windows.Forms.MenuItem bingMenu;
  }
}

