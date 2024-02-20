using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimulusDisplay : MonoBehaviour
{
	[SerializeField] private StimulusCard stimulusCardPrefab;
	[SerializeField] private RectTransform cardContainer;

	public void Initialize(List<Sprite> cardIconSequence)
	{
		Cleanup();

		for (int i = 0; i < cardIconSequence.Count; i++)
		{
			StimulusCard card = Instantiate(stimulusCardPrefab, cardContainer);
			card.Initialize(cardIconSequence[i]);
		}

	}

	void Cleanup()
	{
		for (int i = 0; i < cardContainer.childCount; i++)
		{
			Destroy(cardContainer.GetChild(i).gameObject);
		}
	}
}
