using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Toolbox;

public class SymbolCard : MonoBehaviour
{
	[SerializeField] private float animDuration;
	[SerializeField] private AnimationCurve animSizeCurve;
	[SerializeField] private TMPro.TextMeshProUGUI symbolText;
	[SerializeField] private Image coverImage;
	[SerializeField] private GameObject questionMark;
	[SerializeField] private ButtonHighlighter highlight;
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
    public void Initialize(int? symbolIndex, bool animate = false)
	{
		questionMark.SetActive(symbolIndex == null);
		symbolText.text = symbolIndex == null ? "" : fontSymbolChars[(int)symbolIndex].ToString();
		if (animate) DoAppearAnimation();

	}

	private void DoAppearAnimation()
	{
		symbolText.transform.localScale = Vector3.zero;
		StartCoroutine(CoroutineTools.Tween01(animDuration, t => symbolText.transform.localScale = Vector3.one * animSizeCurve.Evaluate(t)));
	}

	public void SetVisible(bool visible, bool animate = false)
	{
		coverImage.gameObject.SetActive(!visible);
		if (animate) DoAppearAnimation();
	}
}
