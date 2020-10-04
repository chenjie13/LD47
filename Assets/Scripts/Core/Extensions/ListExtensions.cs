using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CGDK.Core
{
    public static class ListExtensions
    {
        public static int RandomIndex<T>(this ICollection<T> collection)
        {
            return Random.Range(0, collection.Count);
        }

        public static T RandomItem<T>(this IList<T> collection)
        {
            var index = collection.RandomIndex();
            return collection[index];
        }

        public static void Shuffle<T>(this IList<T> collection)
        {
            if (collection.Count < 2) return;
            for (var i = collection.Count - 1; i > 0; i--)
            {
                collection.Swap(i, Random.Range(0, i + 1));
            }
        }

        public static void Swap<T>(this IList<T> collection, int i, int j)
        {
            var temp = collection[i];
            collection[i] = collection[j];
            collection[j] = temp;
        }
    }
}