// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

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