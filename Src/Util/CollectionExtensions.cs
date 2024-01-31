using System;
using System.Collections.Generic;
using System.Linq;

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
    
    public static T RandomElement<T>(this IEnumerable<T> list)
    {
        // If there are no elements in the collection, return the default value
        if (!list.Any()) return default;

        return list.ElementAt(Random.Shared.Next(list.Count()));
    }
}