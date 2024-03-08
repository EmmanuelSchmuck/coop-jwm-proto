using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimulusDisplay : MonoBehaviour
{
	[SerializeField] private StimulusCard stimulusCardPrefab;
	private List<StimulusCard> cards;

	public void Initialize(int[] correctIndices)
	{
		Cleanup();

		cards = new List<StimulusCard>();

		for (int i = 0; i < correctIndices.Length; i++)
		{
			StimulusCard card = Instantiate(stimulusCardPrefab, this.transform);
			card.Initialize(correctIndices[i]);
			card.SetVisible(false);
			cards.Add(card);
		}

	}

	public void DoDisplayAnimation(float displayDurationPerSymbol)
	{
		StartCoroutine(DisplayAnimation(displayDurationPerSymbol));
	}

	public void ShowStimulus()
	{
		foreach (var card in cards)
		{
			card.SetVisible(true);
		}
	}

	private IEnumerator DisplayAnimation(float displayDurationPerSymbol)
	{
		//yield return new WaitForSeconds(1f);

		foreach(var card in cards)
		{
			card.SetVisible(true);

			yield return new WaitForSeconds(displayDurationPerSymbol);

			card.SetVisible(false);
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
