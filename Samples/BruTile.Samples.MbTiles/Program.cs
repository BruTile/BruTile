using System;
using System.Windows.Forms;

namespace BruTile.Samples.MbTiles
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MbTilesTileSource.SetPlatform(new SQLite.Net.Platform.Win32.SQLitePlatformWin32());
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MbTilesForm());
        }
    }
}
