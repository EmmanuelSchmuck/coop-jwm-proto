using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StimulusCard : MonoBehaviour
{
	[SerializeField] private Image iconImage;
    public void Initialize(Sprite icon)
	{
		iconImage.sprite = icon;
	}

	public void SetVisible(bool visible)
	{
		iconImage.enabled = visible;
	}
}
