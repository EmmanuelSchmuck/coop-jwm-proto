using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;

public class Coin : MonoBehaviour
{
    [SerializeField] private float animDuration;
    [SerializeField] private AnimationCurve animSizeCurve;
    [SerializeField] private int value;
    public int Value => value;

    [SerializeField] private CoinType coinType;
    public CoinType CoinType => coinType;
    public void PlaySizeAnimation()
	{
        StopAllCoroutines();
        StartCoroutine(CoroutineTools.Tween01(animDuration, t => transform.localScale = Vector3.one * animSizeCurve.Evaluate(t)));
    }
}
