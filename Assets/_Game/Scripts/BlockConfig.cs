using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockConfig
{
    public int trialCount;
    public bool enableStaircase;
    public bool useLastStaircaseValue;
    public bool isTutorial;
    public int sequenceLength;
    public int gameMode;

    // gamemMode values:
    // SinglePlayer = 0, (1p alone / baseline)
    // PassivePresence = 1, (2p independent)
    // PositiveReward = 2, (2p collaboration)
    // NegativeReward = 3, (2p competition)
}
