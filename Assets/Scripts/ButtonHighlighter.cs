using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Toolbox;

public class ButtonHighlighter : MonoBehaviour
{
	[SerializeField] private bool activeWithGameobject;
	[SerializeField] private float loopDuration;
	[SerializeField] private AnimationCurve animCurve;
	[SerializeField] private Image image;
	[SerializeField] private Color baseColor;

	private bool isEnabled;

	private void OnEnable()
	{
		if(activeWithGameobject) SetEnabled(true);
	}

	private void OnDisable()
	{
		SetEnabled(false, cancelAnim: true);
	}

	public void SetEnabled(bool enabled, bool cancelAnim = false)
	{
		bool wasEnabled = this.isEnabled;

		this.isEnabled = enabled;

		if (enabled && wasEnabled) return;

		if (enabled && !wasEnabled) StartCoroutine(Animate());

		else if (!enabled && cancelAnim) StopAllCoroutines();	
	}

	private IEnumerator Animate()
	{
		while(isEnabled)
		{
			yield return CoroutineTools.Tween01(loopDuration, t => image.color = baseColor.WithAlphaMultiplied(t), animCurve.Evaluate);
		}
	}
}
