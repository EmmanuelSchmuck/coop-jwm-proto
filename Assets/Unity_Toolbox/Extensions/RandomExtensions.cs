using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
	public static class RandomExtensions
	{
		public static Vector2 InsideUnitCircle(this System.Random rng)
		{
			float alpha = rng.NextFloat() * 2f * Mathf.PI;
			return rng.NextFloat() * new Vector2(Mathf.Cos(alpha), Mathf.Sin(alpha));
		}

		public static Vector2 OnUnitCircle(this System.Random rng)
		{
			float alpha = rng.NextFloat() * 2f * Mathf.PI;
			return new Vector2(Mathf.Cos(alpha), Mathf.Sin(alpha));
		}
		public static int IntInRange(this System.Random rng, int minInclusive, int maxInclusive)
		{
			return rng.Next(minInclusive, maxInclusive + 1);
		}

		public static float NextFloat(this System.Random rng)
		{
			return (float)rng.NextDouble();
		}
		public static float NextFloat(this System.Random rng, float min, float max)
		{
			return (float)(rng.NextDouble() * ((max - min) + min));
		}
	}
}