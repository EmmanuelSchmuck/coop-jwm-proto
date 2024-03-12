using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Toolbox;

public class BotController : MonoBehaviour
{
    [SerializeField] private bool fastMode;
    [SerializeField] private PlayerBoard target;
    [SerializeField] private PlayerBoard otherPlayer;
    private bool canPickSymbol;
    // Start is called before the first frame update
    void Start()
    {
        target.StimulusDisplayed += OnStimulusDisplayed;
        otherPlayer.ResponseSymbolPicked += OnOtherPlayerSymbolPicked;
    }

    void OnStimulusDisplayed(RoundInfo roundInfo)
	{
        StartCoroutine(AfterStimulusDisplayRoutine(roundInfo));
	}

    void OnOtherPlayerSymbolPicked(ResponseColumn column)
	{
        canPickSymbol = true;
    }

    private IEnumerator AfterStimulusDisplayRoutine(RoundInfo roundInfo)
	{
        JWMGameConfig gameConfig = roundInfo.gameConfig;

        int[] playerB_indices = new int[gameConfig.sequenceLength];
        float[] playerB_coinAmountSequenceFloat = new float[gameConfig.sequenceLength];

        for (int i = 0; i < gameConfig.sequenceLength; i++)
        {
            float correctProbability = Mathf.Clamp01(gameConfig.recallCurve.Evaluate((float)i / Mathf.Max(1, gameConfig.sequenceLength - 1)));

            playerB_indices[i] = Random.value < correctProbability ? roundInfo.correctIndexSequence[i] : Random.Range(0, 9);

            playerB_coinAmountSequenceFloat[i] = correctProbability * gameConfig.CoinPerRound / gameConfig.sequenceLength;
        }

        int[] playerB_coinAmountSequence = ComputeCoinSequence(playerB_coinAmountSequenceFloat, gameConfig);

        if (!fastMode) yield return new WaitForSeconds(1f);

        while(!target.ResponsePanel.AllSymbolsPickedOrLocked) // pick symbols, waiting between each symbol; either a small delay or until it's our turn
		{
            if (!fastMode) yield return new WaitForSeconds(Random.Range(0.5f, 1f));

            int columnIndex = target.ResponsePanel.Columns.First(c => c.SymbolIndex == null && !c.IsLocked).ColumnIndex;

            target.ResponsePanel.SetSymbolInColumn(playerB_indices[columnIndex], columnIndex);
        }

        //for (int i = 0; i < roundInfo.gameConfig.sequenceLength; i++)
        //{
        //    if(!fastMode) yield return new WaitForSeconds(Random.Range(0.5f, 1f));

        //    target.ResponsePanel.SetSymbolInColumn(playerB_indices[i], i);
        //}

        if (!fastMode) yield return new WaitForSeconds(1f);

        for (int i = 0; i < roundInfo.gameConfig.sequenceLength; i++)
        {
            if (!fastMode) yield return new WaitForSeconds(Random.Range(0.5f, 1f));

            int coinAmount = playerB_coinAmountSequence[i];
            for(int c = 0; c < coinAmount; c++)
			{
                if (!fastMode) yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
                target.ResponsePanel.AddCoinsInColumn(1, i);
                target.CoinCounter.RemoveCoin(1);
            }
            
        }
        // playerB_ResponsePanel.SetCoversVisible(true);
        target.ResponsePanel.SetValidated();
    }

    private int[] ComputeCoinSequence(float[] coinSequenceFloat, JWMGameConfig gameConfig) // to do: refactor + fix, also compute coinSequenceFloat here
    {

        float normalizationFactor = gameConfig.CoinPerRound / coinSequenceFloat.Sum();

        coinSequenceFloat = coinSequenceFloat.Select(x => Mathf.Min(gameConfig.maxCoinPerSymbol, x * normalizationFactor)).ToArray();

        string s = "";
        foreach (var f in coinSequenceFloat)
        {
            s += f + " - ";
        }
        Debug.Log($"float sequence = {s}, sum = {coinSequenceFloat.Sum()}");

        int[] coinSequenceInt = new int[gameConfig.sequenceLength];
        int remainingCoins = gameConfig.CoinPerRound;

        float avgFloat = coinSequenceFloat.Average();

        //var sorted = coinSequenceFloat.Select(x, i =>)

        // var sortedFloats = coinSequenceFloat.OrderByDescending(x => x).ToList();

        // indices of elements in coinSequenceFloat, sorted by descending value
        // int[] sortedFloatIndices = coinSequenceFloat.Select(x => sortedFloats.IndexOf(x)).ToArray();


        int[] sortedFloatIndices = coinSequenceFloat.OldIndicesIfSortedDescending().ToArray(); // DOES NOT WORK :'(

		s = "";
		foreach (var f in sortedFloatIndices)
		{
			s += f + " - ";
		}
		Debug.Log("sorted float indices = " + s);

        int loops = 0;
		do
		{
            // iterate over coinSequenceFloat, in descending value order; assign coins to highest value in priority
            // if we still have coins, repeat process

            for (int i = 0; i < gameConfig.sequenceLength; i++)
            {
                int floatIndex = sortedFloatIndices[i];
                float floatValue = coinSequenceFloat[floatIndex];

                int amount = Mathf.Min(remainingCoins, Mathf.RoundToInt(floatValue));
                amount = Mathf.Min(gameConfig.maxCoinPerSymbol - coinSequenceInt[floatIndex], amount);
                amount = Mathf.Max(0, amount);
                coinSequenceInt[floatIndex] += amount;
                remainingCoins -= amount;

                Debug.Log($"floatIndex = {floatIndex}, floatValue = {floatValue}, amount = {amount}");

                if (remainingCoins == 0) break;

            }

            Debug.Log($"remaining coins = {remainingCoins}");

            loops++;

        } while (remainingCoins > 0 && loops < 100);

        for (int i = 0; i < remainingCoins; i++)
        {
            bool success = false;
            int attempts = 0;
            int maxAttempts = 100;

            while (!success && attempts < maxAttempts)
            {
                attempts++;
                int index = Random.Range(0, coinSequenceInt.Length);
                if (coinSequenceInt[index] < gameConfig.maxCoinPerSymbol)
                {
                    success = true;
                    coinSequenceInt[index]++;

                }
            }

            if (attempts == maxAttempts) Debug.LogError("Cannot place coins!");

        }

        return coinSequenceInt;
    }
}
