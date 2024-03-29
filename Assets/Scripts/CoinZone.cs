using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoinZone : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] private ButtonHighlighter highlight;
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
