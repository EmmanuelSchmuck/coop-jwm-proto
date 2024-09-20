using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Toolbox;

public class SymbolCard : MonoBehaviour
{
	[SerializeField] private Color baseColor, emptyColor;
	[SerializeField] private Image lockImage;
	[SerializeField] private float animDuration;
	[SerializeField] private AnimationCurve animSizeCurve;
	[SerializeField] private TMPro.TextMeshProUGUI symbolText;
	[SerializeField] private Image coverImage;
	[SerializeField] private GameObject questionMark;
	[SerializeField] private ButtonHighlighter highlight;
	private int? symbolIndex;
	private bool highlighted;
	public bool Highlighted
	{
		get => highlighted;

		set
		{
			highlighted = value;
			highlight.gameObject.SetActive(highlighted);
		}
	}
	private readonly char[] fontSymbolChars = new char[] {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I' };
    public void Initialize(int? symbolIndex, bool animate = false) // if animate = false, no symbol is displayed !
	{
		this.symbolIndex = symbolIndex;
		questionMark.SetActive(false);
		symbolText.text = "";
		SetLocked(false);
		
		if (animate) DoAppearAnimation();

	}

	public void SetEmpty()
	{
		GetComponent<Image>().color = emptyColor;
	}

	public void SetBaseColor()
	{
		GetComponent<Image>().color = baseColor;
	}

	public void SetLocked(bool locked)
	{
		questionMark.SetActive(!locked);
		lockImage.enabled = locked;
	}

	private void DoAppearAnimation() // (to do: refactor)
	{
		questionMark.SetActive(symbolIndex == null);
		symbolText.text = symbolIndex == null ? "" : fontSymbolChars[(int)symbolIndex].ToString();
		symbolText.transform.localScale = Vector3.zero;
		StartCoroutine(CoroutineTools.Tween01(animDuration, t => symbolText.transform.localScale = Vector3.one * animSizeCurve.Evaluate(t)));
	}

	// to do: refactor this;
	public void SetVisible(bool visible, bool animate = false)
	{
		questionMark.SetActive(symbolIndex == null);
		symbolText.text = symbolIndex == null || !visible ? "" : fontSymbolChars[(int)symbolIndex].ToString();
		coverImage.gameObject.SetActive(!visible);
		if (animate) DoAppearAnimation();
	}
}
