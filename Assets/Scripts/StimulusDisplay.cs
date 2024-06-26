using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimulusDisplay : MonoBehaviour
{
	[SerializeField] private SymbolCard stimulusCardPrefab;
	private List<SymbolCard> cards;

	public void Initialize(int[] correctIndices)
	{
		Cleanup();

		cards = new List<SymbolCard>();

		for (int i = 0; i < correctIndices.Length; i++)
		{
			SymbolCard card = Instantiate(stimulusCardPrefab, this.transform);
			card.Initialize(correctIndices[i], animate: false);
			card.SetVisible(false);
			cards.Add(card);
		}

	}

	public IEnumerator RevealAnimation(float animationDuration)
	{
		float durationPerCard = animationDuration / cards.Count;
		foreach (var card in cards)
		{
			card.SetVisible(true, animate: true);

			SoundManager.Instance.PlaySound(SoundType.StimulusShow);

			yield return new WaitForSeconds(durationPerCard);
		}
	}

	public IEnumerator DisplayAnimation(float displayDurationPerSymbol, float initialDelay)
	{
		yield return new WaitForSeconds(initialDelay);

		foreach(var card in cards)
		{
			card.SetVisible(true, animate: true);

			SoundManager.Instance.PlaySound(SoundType.StimulusShow);

			yield return new WaitForSeconds(displayDurationPerSymbol);

			card.SetVisible(false);
		}
	}
	public void Hide()
	{
		foreach (var card in cards)
		{
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
