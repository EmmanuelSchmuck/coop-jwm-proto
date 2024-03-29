using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbolKey : MonoBehaviour
{
	[SerializeField] private SymbolCard card;
	[SerializeField] private Button button;
	public bool Highlighted
	{
		set
		{
			card.Highlighted = value;
		}
	}
	public int SymbolIndex { get; private set; }
	public event System.Action<SymbolKey> Clicked;
    public void Initialize(int symbolIndex)
	{
		//iconImage.sprite = symbolIcon;
		card.Initialize(symbolIndex);
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
