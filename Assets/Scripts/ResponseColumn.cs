using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResponseColumn : MonoBehaviour
{
	public int? SymbolIndex { get; private set; }
	private List<Sprite> cardShapes;
	[SerializeField] private Image symbolImage;

	public void Initialize(List<Sprite> cardShapes)
	{
		SymbolIndex = null;
		this.cardShapes = cardShapes;
	}

	public void OnButtonClick()
	{
		// if symbolkeyboard has non-null selectedSymbolIndex, set this column symbolIndex and update the symbol icon
		// then reset symbolkeyboard (set selectedSymbolIndex to null)

		JWMGameController.Instance.WIP_OnResponseColumnClicked(this);

		
	}

	public void SetSymbol(int symbolIndex)
	{
		symbolImage.sprite = cardShapes[symbolIndex];
		this.SymbolIndex = symbolIndex;
	}
}
