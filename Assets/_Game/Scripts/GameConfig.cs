using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameConfig
{
    public GameMode gameMode;
    public int sequenceLength = 6;
    public float displayDurationPerSymbol = 1f;
    public bool allowSymbolRepetition = false;
    public bool enable2Up1DDownStaircase;
    public bool isTutorial;
    public AnimationCurve recallCurve;
    public const int SYMBOL_POOL_SIZE = 9;
    public const int MAX_SEQUENCE_LENGTH = 9;
    public const int MIN_SEQUENCE_LENGTH = 3;
    //public int CoinPerRound => ActionDependency == Dependency.Negative ? sequenceLength : sequenceLength * 2;
    public (int, int, int) CoinsPerRound => ComputeCoinsPerRound();

    private (int, int, int) ComputeCoinsPerRound()
	{
        int bronzeCoins = 0, silverCoins = 0, goldCoins = 0;

        int coinsToShare = sequenceLength;

        for(int i = 0; i < coinsToShare; i++)
		{
            if (i % 3 == 0) bronzeCoins++;
            else if (i % 3 == 1) silverCoins++;
            else if (i % 3 == 2) goldCoins++;
        }

        return (bronzeCoins, silverCoins, goldCoins);
	}

    public Dependency RewardDependency => gameMode switch
    {
        GameMode.PositiveReward or GameMode.PositiveRewardAction => Dependency.Positive,
		GameMode.NegativeReward or GameMode.NegativeRewardAction => Dependency.Negative,
        _ => Dependency.None
    };
    // Positive ActionDependency is not defined for this game, we can only return Dependency.Negative or Dependency.None
    public Dependency ActionDependency => gameMode switch
    {
        GameMode.PositiveAction or GameMode.PositiveRewardAction => Dependency.Positive,
        GameMode.NegativeAction or GameMode.NegativeRewardAction => Dependency.Negative,
        _ => Dependency.None
    };

    public int ClampSequenceLength(int value) => Mathf.Clamp(value, MIN_SEQUENCE_LENGTH, MAX_SEQUENCE_LENGTH);
    //public float ClampDisplayDurationPerSymbol(float value) => Mathf.Clamp(value, 0.5f, 3f);

    //public int ClampCoinPerRound(int value) => Mathf.Clamp(value, 3, 15);
    //public int ClampMaxCoinPerSymbol(int value) => Mathf.Clamp(value, 2, 4);
}
