// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace BruTile.Samples.Common;

// Todo: Use frameworks HashSet which I think is now available
public class HashSet<T> : ICollection<T>
{
    private readonly Dictionary<T, short> _dictionary = [];

    public int Count => _dictionary.Keys.Count;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        // We don't care for the value in dictionary, Keys matter.
        _dictionary.Add(item, 0);
    }

    public void Clear()
    {
        _dictionary.Clear();
    }

    public bool Contains(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        return _dictionary.ContainsKey(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        return _dictionary.Remove(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
