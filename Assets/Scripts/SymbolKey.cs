using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbolKey : MonoBehaviour
{
	[SerializeField] private Image iconImage;
	private int symbolIndex;
    public void Initialize(int symbolIndex, Sprite symbolIcon)
	{
		iconImage.sprite = symbolIcon;
		this.symbolIndex = symbolIndex;
	}

	public void OnButtonClick()
	{

	}
}
