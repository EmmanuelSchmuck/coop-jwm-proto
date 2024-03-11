using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JWMGameConfig
{
    public GameMode gameMode;
    public int sequenceLength = 6;
    public float displayDurationPerSymbol = 1f;
    public bool allowSymbolRepetition = false;
    public int maxCoinPerSymbol = 3;
    public const int SCORE_MULTIPLIER = 1;
    public AnimationCurve recallCurve;
    public const int SYMBOL_POOL_SIZE = 9;
    public int CoinPerRound => sequenceLength * 2;
    public Dependency RewardDependency => gameMode switch
    {
        GameMode.PositiveReward | GameMode.PositiveRewardAction => Dependency.Positive,
		GameMode.NegativeReward | GameMode.NegativeRewardAction => Dependency.Negative,
        _ => Dependency.None
    };
    public Dependency ActionDependency => gameMode switch
    {
        GameMode.PositiveAction | GameMode.PositiveRewardAction => Dependency.Positive,
        GameMode.NegativeAction | GameMode.NegativeRewardAction => Dependency.Negative,
        _ => Dependency.None
    };

    public int ClampSequenceLength(int value) => Mathf.Clamp(value, 4, 10);
    public float ClampDisplayDurationPerSymbol(float value) => Mathf.Clamp(value, 0.5f, 3f);

    public int ClampCoinPerRound(int value) => Mathf.Clamp(value, 3, 15);
    public int ClampMaxCoinPerSymbol(int value) => Mathf.Clamp(value, 2, 4);
}
