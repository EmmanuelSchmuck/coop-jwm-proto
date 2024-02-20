using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Toolbox
{
	public class WeightedDistributionSampler<T>
	{

		private Dictionary<T, int> occurences;
		Dictionary<T, float> distribution;
		private float correctionFactor;

		public WeightedDistributionSampler(Dictionary<T, float> distribution, float correctionFactor = 0f)
		{
			this.distribution = new Dictionary<T, float>(distribution);
			this.correctionFactor = correctionFactor;
			occurences = new Dictionary<T, int>();
			foreach (T key in distribution.Keys)
			{
				occurences.Add(key, 0);
			}
		}
		public T GetSample(System.Random rng, params T[] resultsToAvoid)
		{
			Dictionary<T, float> actualDistribution = new Dictionary<T, float>(distribution);
			if (resultsToAvoid.Length > 0)
			{
				foreach (T key in resultsToAvoid)
				{
					actualDistribution.Remove(key);
				}
			}

			float totalOccurences = occurences.Where(kvp => actualDistribution.ContainsKey(kvp.Key)).Sum(kvp => kvp.Value);
			if (totalOccurences > 0)
			{
				foreach (T key in occurences.Keys.Where(k => actualDistribution.ContainsKey(k)))
				{
					float weight = actualDistribution.Values.Sum();
					float desiredProba = actualDistribution[key] / weight;
					float correction = correctionFactor * weight * (desiredProba - (occurences[key] / totalOccurences));
					// Debug.Log($"correction for {key.ToString()} is {correction}, final proba is {Mathf.Max(0,actualDistribution[key] + correction)}");
					actualDistribution[key] += correction;
					actualDistribution[key] = Mathf.Max(0, actualDistribution[key]);
				}
			}

			float totalWeight = actualDistribution.Values.Sum();
			float randomValue = rng.NextFloat(0, totalWeight);
			T selectedItem = default(T);
			float weightSum = 0f;

			foreach (T item in actualDistribution.Keys.OrderBy(k => actualDistribution[k]))
			{
				weightSum += actualDistribution[item];
				if (randomValue < weightSum)
				{
					selectedItem = item;
					break;
				}
			}
			if (!occurences.Keys.Contains(selectedItem))
			{
				occurences.Add(selectedItem, 0);
			}
			occurences[selectedItem]++;

			return selectedItem;
		}
	}
}