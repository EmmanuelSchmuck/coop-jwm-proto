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

		Debug.Log("highlight 1");

		if (enabled && wasEnabled) return;

		Debug.Log("highlight 2");

		if (enabled && !wasEnabled)
		{
			StartCoroutine(Animate());
			Debug.Log("highlight 3");
		}
		else if (!enabled && cancelAnim) StopAllCoroutines();	
	}

	private IEnumerator Animate()
	{
		while(isEnabled)
		{
			Debug.Log("highlight 4");
			yield return CoroutineTools.Tween01(loopDuration, t => image.color = baseColor.WithAlphaMultiplied(t), animCurve.Evaluate);
		}
	}
}
