using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Toolbox
{
    public static class CollectionExtensions
    {
        public static T Random<T>(this IEnumerable<T> source, System.Random rng = null)
        {
            rng ??= new System.Random();
            return source.ElementAt(rng.Next(0, source.Count()));
        }

        public static IEnumerable<T> RandomRange<T>(this IEnumerable<T> source, int count, bool mustbeDifferent = false, System.Random rng = null)
        {
            rng ??= new System.Random();

            T[] result = new T[count];

            int attempts;
            int maxAttempts = 50;

            for(int i = 0; i < count; i++)
			{
                T element;

                if(mustbeDifferent)
				{
                    attempts = 0;

                    do
                    {
                        element = source.ElementAt(rng.Next(0, source.Count()));
                    }
                    while (++attempts < maxAttempts && result.WithoutNullValues().Any(x => x.Equals(element))); 
                }
                else
				{
                    element = source.ElementAt(rng.Next(0, source.Count()));
                }
                
                result[i] = element;
            }

            return result;
        }

        public static IEnumerable<T> WithoutNullValues<T>(this IEnumerable<T> source)
		{
            return source.Where(x => x != null);
		}


        public static IEnumerable<T> PickRandomRange<T>(this List<T> source, int count, bool different = false, System.Random rng = null)
        {
            IEnumerable<T> picked = source.RandomRange(count, different, rng);

            foreach(var element in picked)
			{
                source.Remove(element);
			}

            return picked;
        }

        /// <summary>
        /// Removes a random element from the collection and returns it.
        /// </summary>
        public static T PickRandom<T>(this ICollection<T> source, System.Random rng = null)
        {
            rng ??= new System.Random();
            T item = source.ElementAt(rng.Next(0, source.Count()));
            source.Remove(item);
            return item;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, System.Random rng = null)
        {
            rng ??= new System.Random();
            return source.OrderBy((item) => rng.Next());
        }

        public static List<T> PickRange<T>(this List<T> source, int index, int count)
        {
            List<T> picked = source.Skip(index).Take(count).ToList();
            source.RemoveRange(index, count);
            return picked;
        }

        public static T Pick<T>(this List<T> source, int index)
        {
            T picked = source.Skip(index).FirstOrDefault();
            source.RemoveAt(index);
            return picked;
        }

        public static T PickLast<T>(this List<T> source) => source.Pick(source.Count - 1);
        public static T PickFirst<T>(this List<T> source) => source.Pick(0);

    }
}