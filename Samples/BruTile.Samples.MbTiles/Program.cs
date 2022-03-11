using System;
using System.Windows.Forms;
using BruTile.Samples.MbTiles;

namespace BruTile.Samples.MbTiles2
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MbTilesForm());
        }
    }
}
