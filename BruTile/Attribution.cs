// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

namespace BruTile;

public class Attribution(string text = null, string url = null)
{
    public string Text { get; } = text ?? "";
    public string Url { get; } = url ?? "";
}
