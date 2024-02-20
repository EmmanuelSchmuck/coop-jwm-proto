using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Toolbox
{
    public static class LinqExtensions
    {
        public static float Product(this IEnumerable<float> source)
        {
            return source.Aggregate(1f, (x, y) => x * y);
        }

        public static float Product<T>(this IEnumerable<T> source, System.Func<T, float> selector)
        {
            return source.Select(t => selector.Invoke(t)).Aggregate(1f, (x, y) => x * y);
        }
    }
}

