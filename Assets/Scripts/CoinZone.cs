using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Toolbox;

public class CoinZone : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] private ButtonHighlighter highlight;
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private AnimationCurve fadeCurve;
	public bool Highlighted
	{
		set
		{
			highlight.gameObject.SetActive(value);
		}
	}
	private ResponseColumn parent;

	public bool Interactable { get; set; }


	public void Initialize(ResponseColumn parent)
	{
		this.parent = parent;
	}

	public void FadeToVisible(bool visible)
	{
		float startAlpha = visible ? 0f : 1f;
		float targetAlpha = visible ? 1f : 0f;
		StartCoroutine(CoroutineTools.Tween(startAlpha, targetAlpha, 0.7f, t => canvasGroup.alpha = fadeCurve.Evaluate(t)));
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!Interactable) return;

		if (eventData.button == PointerEventData.InputButton.Left)
			parent.OnAddCoinButtonClick();

		else if (eventData.button == PointerEventData.InputButton.Right)
			parent.OnRemoveCoinButtonClick();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!Interactable) return;

		parent.OnCoinZoneHoverEnter();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		parent.OnCoinZoneHoverLeave();
	}
}
