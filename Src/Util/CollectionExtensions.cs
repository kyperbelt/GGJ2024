using System;
using System.Collections.Generic;

namespace GGJ2024.Util;

public static class CollectionExtensions
{
    public static IList<T> Shuffle<T>(this IList<T> items)
    {      
        for(int i = 0; i < items.Count - 1; i++)
        {
            int pos = Random.Shared.Next(i, items.Count); 
            (items[i], items[pos]) = (items[pos], items[i]);
        }
        return items;
    }
}