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
            SQLitePCL.Batteries.Init();
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MbTilesForm());
        }
    }
}
