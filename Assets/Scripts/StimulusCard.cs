using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StimulusCard : MonoBehaviour
{
	[SerializeField] private Image iconImage;
	[SerializeField] private Image coverImage;
    public void Initialize(Sprite icon)
	{
		iconImage.sprite = icon;
	}

	public void SetVisible(bool visible)
	{
		coverImage.gameObject.SetActive(!visible);
	}
}
