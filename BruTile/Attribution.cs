namespace BruTile
{
    public class Attribution
    {
        public Attribution(string text = null, string url = null)
        {
            Text = text ?? "";
            Url = url ?? "";
        }

        public string Text { get; }
        public string Url { get; }
    }
}