// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;

namespace BruTile.Extensions;

public static class ObjectExtensions
{
    public static void DisposeIfDisposable(this object obj)
    {
        if (obj is IDisposable disposable)
            disposable.Dispose();
    }
}
