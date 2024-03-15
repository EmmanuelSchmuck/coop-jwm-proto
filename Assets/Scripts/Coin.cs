using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;

public class Coin : MonoBehaviour
{
    [SerializeField] private float animDuration;
    [SerializeField] private AnimationCurve animSizeCurve;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CoroutineTools.Tween01(animDuration, t => transform.localScale = Vector3.one * animSizeCurve.Evaluate(t)));
    }
}
