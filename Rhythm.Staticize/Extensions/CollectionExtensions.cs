using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rhythm.Staticize
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this IList<T> source, IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            List<T> list = source as List<T>;
            if (list != null)
            {
                list.AddRange(collection);
            }
            else
            {
                foreach (var item in collection)
                {
                    source.Add(item);
                }
            }
        }
    }
}
