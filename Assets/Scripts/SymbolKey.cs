using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbolKey : MonoBehaviour
{
	[SerializeField] private StimulusCard card;
	[SerializeField] private Button button;
	public int SymbolIndex { get; private set; }
	public event System.Action<SymbolKey> Clicked;
    public void Initialize(int symbolIndex, string symbolIcon)
	{
		//iconImage.sprite = symbolIcon;
		card.Initialize(symbolIcon);
		this.SymbolIndex = symbolIndex;
	}

	public void SetInteractable(bool interactable)
	{
		button.interactable = interactable;
	}

	public void OnButtonClick()
	{
		Clicked?.Invoke(this);
	}
}
