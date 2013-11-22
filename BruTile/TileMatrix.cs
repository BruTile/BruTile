namespace BruTile
{
    public class TileMatrix
    {
        public string Id { get; set; } 
        public double ScaleDenominator { get; set; }
        public double Top { get; set; }
        public double Left { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public int MatrixWidth { get; set; }
        public int MatrixHeight { get; set; }
    }
}
