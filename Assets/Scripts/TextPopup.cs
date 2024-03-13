using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;

public class TextPopup : MonoBehaviour
{
    [SerializeField] private float animationDuration;
    [SerializeField] private AnimationCurve opacityCurve;
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] private TMPro.TextMeshProUGUI textElement;
    private RectTransform rectTransform;
    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        textElement.alpha = 0f;
    }

    public void Animate(string text)
	{
        textElement.text = text;
        StopAllCoroutines();
        StartCoroutine(CoroutineTools.Tween01(animationDuration, t =>
        {
            textElement.alpha = opacityCurve.Evaluate(t);
            rectTransform.anchoredPosition = rectTransform.anchoredPosition.WithY(heightCurve.Evaluate(t));
        }));
    }
}
