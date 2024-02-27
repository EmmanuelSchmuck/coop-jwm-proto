using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimulusDisplay : MonoBehaviour
{
	[SerializeField] private StimulusCard stimulusCardPrefab;

	public void Initialize(List<Sprite> cardIconSequence)
	{
		Cleanup();

		for (int i = 0; i < cardIconSequence.Count; i++)
		{
			StimulusCard card = Instantiate(stimulusCardPrefab, this.transform);
			card.Initialize(cardIconSequence[i]);
		}

	}

	void Cleanup()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
		}
	}
}
