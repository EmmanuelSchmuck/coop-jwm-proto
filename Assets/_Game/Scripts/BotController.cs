using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Toolbox;

public class BotController : MonoBehaviour
{
    [SerializeField] private bool fastMode;
    [SerializeField] private PlayerBoard target;
    private int[] playerB_indices;
    private float[] playerB_coinAmountSequenceFloat;

    // Start is called before the first frame update
    void Start()
    {
        target.ResponsePhaseStarted += OnResponsePhaseStarted;
        target.CoinBettingStarted += OnCoinBettingStarted;
        target.LockResponseTurnStarted += OnLockResponseTurnStarted;
        target.SymbolResponseTurnStarted += OnSymbolResponseTurnStarted;
        target.StimulusDisplayed += OnStimulusDisplayed;
    }

    void OnStimulusDisplayed(RoundInfo roundInfo)
	{
        GameConfig gameConfig = roundInfo.gameConfig;

        playerB_indices = new int[gameConfig.sequenceLength];
        playerB_coinAmountSequenceFloat = new float[gameConfig.sequenceLength];

        for (int i = 0; i < gameConfig.sequenceLength; i++)
        {
            // get the proba of being correct from sampling the recall curve
            float correctProbability = Mathf.Clamp01(gameConfig.recallCurve.Evaluate((float)i / Mathf.Max(1, gameConfig.sequenceLength - 1)));

            // pick random value to see if symbol index is correct or not; if not correct, pick a random index
            playerB_indices[i] = Random.value < correctProbability ? roundInfo.correctIndexSequence[i] : Random.Range(0, 9);

            // assign coin amount value based on correctProbability 
            //playerB_coinAmountSequenceFloat[i] = correctProbability * gameConfig.CoinPerRound / gameConfig.sequenceLength;
        }
    }

    void OnResponsePhaseStarted(RoundInfo roundInfo)
    {
        StartCoroutine(PickSymbolsRoutine(roundInfo));
    }

    void OnCoinBettingStarted(RoundInfo roundInfo)
    {
        StartCoroutine(CoinBettingRoutine(roundInfo));
    }

    void OnSymbolResponseTurnStarted(RoundInfo roundInfo)
	{
        // pick a single symbol
        StartCoroutine(PickSingleSymbolRoutine(roundInfo));
    }

    private IEnumerator PickSingleSymbolRoutine(RoundInfo roundInfo)
    {
        if(!fastMode) yield return new WaitForSeconds(Random.Range(0.8f, 1.2f));

        int columnIndex = target.ResponsePanel.Columns.First(x => x.SymbolIndex == null && !x.IsLocked).ColumnIndex;

        target.SymbolKeyboard.SetSelectedSymbolIndex(playerB_indices[columnIndex]);

        target.OnResponseSymbolPickAttempted(target.ResponsePanel.Columns[columnIndex]); //.SetSymbolInColumn(playerB_indices[lastPickedSymbolIndex], lastPickedSymbolIndex);

        //if (!fastMode) yield return new WaitForSeconds(0.5f);
        //      if (!fastMode) yield return new WaitForSeconds(0.25f);

        //      yield return null;

        //      if(target.ResponsePanel.AllColumnsPickedOrLocked)
        //{
        //          target.ResponsePanel.SetValidated(true);
        //      }

        // playerB_ResponsePanel.SetCoversVisible(true);

    }

    void OnLockResponseTurnStarted(RoundInfo roundInfo)
    {
        // pick a single symbol
        StartCoroutine(LockOrUnlockOppositeSymbol(roundInfo));
    }

    private IEnumerator LockOrUnlockOppositeSymbol(RoundInfo roundInfo)
    {
        bool shouldLock = roundInfo.gameConfig.ActionDependency == Dependency.Negative; // else, should unlock

        if (!fastMode) yield return new WaitForSeconds(Random.Range(0.8f, 1.2f));

        // pick first column that is "empy" (not picked or locked) if we should lock; else pick first locked column if we should unlock
        int columnIndex = target.OppositeBoard.ResponsePanel.Columns.First(x => shouldLock ? !x.IsPickedOrLocked : x.IsLocked).ColumnIndex;
        ResponseColumn column = target.OppositeBoard.ResponsePanel.Columns[columnIndex];
        //column.SetLocked(shouldLock);
        //target.OppositeBoard.SymbolKeyboard.ResetSelection();
        target.OppositeBoard.OnResponseSymbolPickAttempted(column);
        
        //if (!fastMode) yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator PickSymbolsRoutine(RoundInfo roundInfo)
	{
        if (!fastMode) yield return new WaitForSeconds(1f);

		for (int i = 0; i < roundInfo.gameConfig.sequenceLength; i++)
		{
			if (!fastMode) yield return new WaitForSeconds(Random.Range(0.5f, 1f));

			target.ResponsePanel.SetSymbolInColumn(playerB_indices[i], i);
		}

		if (!fastMode) yield return new WaitForSeconds(0.5f);

        yield return null;

        // playerB_ResponsePanel.SetCoversVisible(true);
        target.SetValidated(true);
    }

    private IEnumerator CoinBettingRoutine(RoundInfo roundInfo)
	{
        GameConfig gameConfig = roundInfo.gameConfig;

        int[] playerB_indices = new int[gameConfig.sequenceLength];
        float[] playerB_coinAmountSequenceFloat = new float[gameConfig.sequenceLength];

        for (int i = 0; i < gameConfig.sequenceLength; i++)
        {
            float correctProbability = Mathf.Clamp01(gameConfig.recallCurve.Evaluate((float)i / Mathf.Max(1, gameConfig.sequenceLength - 1)));

            playerB_indices[i] = Random.value < correctProbability ? roundInfo.correctIndexSequence[i] : Random.Range(0, 9);

            //playerB_coinAmountSequenceFloat[i] = correctProbability * gameConfig.CoinPerRound / gameConfig.sequenceLength;
        }

        //int[] playerB_coinAmountSequence = ComputeCoinSequence(playerB_coinAmountSequenceFloat, gameConfig);


        for (int i = 0; i < roundInfo.gameConfig.sequenceLength; i++)
        {
            if (!fastMode) yield return new WaitForSeconds(Random.Range(0.5f, 1f));

            //if (!fastMode) yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
            Coin coin = target.CoinRepositories.Where(r => r.CoinCount > 0).Random().TakeLastCoin();
            target.ResponsePanel.AddCoinsInColumn(coin, i);
            

            //int coinAmount = playerB_coinAmountSequence[i];
            //for (int c = 0; c < coinAmount; c++)
            //{
                
            //}
        }

        yield return null;

        // playerB_ResponsePanel.SetCoversVisible(true);
        target.SetValidated(true);
    }

  //  private int[] ComputeCoinSequence(float[] coinSequenceFloat, JWMGameConfig gameConfig) // to do: refactor + fix, also compute coinSequenceFloat here
  //  {

  //      float normalizationFactor = gameConfig.CoinPerRound / coinSequenceFloat.Sum();

  //      coinSequenceFloat = coinSequenceFloat.Select(x => Mathf.Min(gameConfig.maxCoinPerSymbol, x * normalizationFactor)).ToArray();

  //      string s = "";
  //      foreach (var f in coinSequenceFloat)
  //      {
  //          s += f + " - ";
  //      }
  //      Debug.Log($"float sequence = {s}, sum = {coinSequenceFloat.Sum()}");

  //      int[] coinSequenceInt = new int[gameConfig.sequenceLength];
  //      int remainingCoins = gameConfig.CoinPerRound;

  //      float avgFloat = coinSequenceFloat.Average();

  //      //var sorted = coinSequenceFloat.Select(x, i =>)

  //      // var sortedFloats = coinSequenceFloat.OrderByDescending(x => x).ToList();

  //      // indices of elements in coinSequenceFloat, sorted by descending value
  //      // int[] sortedFloatIndices = coinSequenceFloat.Select(x => sortedFloats.IndexOf(x)).ToArray();


  //      int[] sortedFloatIndices = coinSequenceFloat.OldIndicesIfSortedDescending().ToArray(); // DOES NOT WORK :'(

		//s = "";
		//foreach (var f in sortedFloatIndices)
		//{
		//	s += f + " - ";
		//}
		//Debug.Log("sorted float indices = " + s);

  //      int loops = 0;
		//do
		//{
  //          // iterate over coinSequenceFloat, in descending value order; assign coins to highest value in priority
  //          // if we still have coins, repeat process

  //          for (int i = 0; i < gameConfig.sequenceLength; i++)
  //          {
  //              int floatIndex = sortedFloatIndices[i];
  //              float floatValue = coinSequenceFloat[floatIndex];

  //              int amount = Mathf.Min(remainingCoins, Mathf.RoundToInt(floatValue));
  //              amount = Mathf.Min(gameConfig.maxCoinPerSymbol - coinSequenceInt[floatIndex], amount);
  //              amount = Mathf.Max(0, amount);
  //              coinSequenceInt[floatIndex] += amount;
  //              remainingCoins -= amount;

  //              Debug.Log($"floatIndex = {floatIndex}, floatValue = {floatValue}, amount = {amount}");

  //              if (remainingCoins == 0) break;

  //          }

  //          Debug.Log($"remaining coins = {remainingCoins}");

  //          loops++;

  //      } while (remainingCoins > 0 && loops < 100);

  //      for (int i = 0; i < remainingCoins; i++)
  //      {
  //          bool success = false;
  //          int attempts = 0;
  //          int maxAttempts = 100;

  //          while (!success && attempts < maxAttempts)
  //          {
  //              attempts++;
  //              int index = Random.Range(0, coinSequenceInt.Length);
  //              if (coinSequenceInt[index] < gameConfig.maxCoinPerSymbol)
  //              {
  //                  success = true;
  //                  coinSequenceInt[index]++;

  //              }
  //          }

  //          if (attempts == maxAttempts) Debug.LogError("Cannot place coins!");

  //      }

  //      return coinSequenceInt;
  //  }
}
