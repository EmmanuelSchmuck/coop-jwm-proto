using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StimulusCard : MonoBehaviour
{
	[SerializeField] private TMPro.TextMeshProUGUI symbolText;
	[SerializeField] private Image coverImage;
	[SerializeField] private GameObject questionMark;
	private readonly char[] fontSymbolChars = new char[] {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };
    public void Initialize(int? symbolIndex)
	{
		questionMark.SetActive(symbolIndex == null);
		symbolText.text = symbolIndex == null ? "" : fontSymbolChars[(int)symbolIndex].ToString();
	}

	public void SetVisible(bool visible)
	{
		coverImage.gameObject.SetActive(!visible);
	}
}
