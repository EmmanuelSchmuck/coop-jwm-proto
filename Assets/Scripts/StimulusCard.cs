using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StimulusCard : MonoBehaviour
{
	[SerializeField] private TMPro.TextMeshProUGUI symbolText;
	[SerializeField] private Image coverImage;
	[SerializeField] private GameObject questionMark;
    public void Initialize(string symbol)
	{
		questionMark.SetActive(symbol == null);
		symbolText.text = symbol ?? "";
	}

	public void SetVisible(bool visible)
	{
		coverImage.gameObject.SetActive(!visible);
	}
}
