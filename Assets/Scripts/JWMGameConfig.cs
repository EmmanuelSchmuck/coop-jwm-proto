using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JWMGameConfig
{
    public int sequenceLength = 6;
    public float displayDurationPerSymbol = 1f;
    public bool allowSymbolRepetition = false;
    public int coinPerRound = 10;
    public int maxCoinPerSymbol = 3;

    public int ClampSequenceLength(int value) => Mathf.Clamp(value, 4, 10);
    public float ClampDisplayDurationPerSymbol(float value) => Mathf.Clamp(value, 0.5f, 3f);

    public int ClampCoinPerRound(int value) => Mathf.Clamp(value, 3, 15);
    public int ClampMaxCoinPerSymbol(int value) => Mathf.Clamp(value, 2, 4);
}
