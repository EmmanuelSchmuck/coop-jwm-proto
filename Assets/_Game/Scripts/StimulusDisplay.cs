using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;

public class StimulusDisplay : MonoBehaviour
{
	[SerializeField] private SymbolCard stimulusCardPrefab;
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private AnimationCurve fadeCurve;
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

	public void HideCards()
	{
		foreach (var card in cards)
		{
			card.SetVisible(false);
		}
	}

	public void SetVisible(bool visible)
	{
		canvasGroup.alpha = visible ? 1f : 0f;
	}

	public IEnumerator AnimateVisible(bool visible)
	{
		float startAlpha = visible ? 0f : 1f;
		float targetAlpha = visible ? 1f : 0f;
		yield return CoroutineTools.Tween(startAlpha, targetAlpha, 0.7f, t => canvasGroup.alpha = fadeCurve.Evaluate(t));
	}

	void Cleanup()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
		}
	}
}
