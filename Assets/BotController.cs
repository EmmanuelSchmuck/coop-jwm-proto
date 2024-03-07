using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BotController : MonoBehaviour
{
    [SerializeField] private PlayerBoard target;
    // Start is called before the first frame update
    void Start()
    {
        target.StimulusDisplayed += OnStimulusDisplayed;
    }

    void OnStimulusDisplayed(RoundInfo roundInfo)
	{
        StartCoroutine(AfterStimulusDisplayRoutine(roundInfo));
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

            playerB_coinAmountSequenceFloat[i] = correctProbability * gameConfig.coinPerRound / gameConfig.sequenceLength;
        }

        int[] playerB_coinAmountSequence = ComputeCoinSequence(playerB_coinAmountSequenceFloat, gameConfig);

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < roundInfo.gameConfig.sequenceLength; i++)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));

            target.ResponsePanel.SetSymbolInColumn(playerB_indices[i], i);
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < roundInfo.gameConfig.sequenceLength; i++)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 1f));

            int coinAmount = playerB_coinAmountSequence[i];
            target.ResponsePanel.AddCoinsInColumn(coinAmount, i);
            target.CoinCounter.RemoveCoin(coinAmount);
        }
        // playerB_ResponsePanel.SetCoversVisible(true);
        target.ResponsePanel.SetValidated();
    }

    private int[] ComputeCoinSequence(float[] coinSequenceFloat, JWMGameConfig gameConfig) // to do: refactor + fix, also compute coinSequenceFloat here
    {

        float normalizationFactor = gameConfig.coinPerRound / coinSequenceFloat.Sum();

        coinSequenceFloat = coinSequenceFloat.Select(x => Mathf.Min(gameConfig.maxCoinPerSymbol, x * normalizationFactor)).ToArray();

        string s = "";
        foreach (var f in coinSequenceFloat)
        {
            s += f + " - ";
        }
        Debug.Log($"float sequence = {s}, sum = {coinSequenceFloat.Sum()}");

        int[] coinSequenceInt = new int[gameConfig.sequenceLength];
        int remainingCoins = gameConfig.coinPerRound;

        float avgFloat = coinSequenceFloat.Average();

        // var sortedFloats = coinSequenceFloat.OrderByDescending(x => x).ToList();

        // indices of elements in coinSequenceFloat, sorted by descending value
        // int[] sortedFloatIndices = coinSequenceFloat.Select(x => sortedFloats.IndexOf(x)).ToArray();


        //int[] sortedFloatIndices = coinSequenceFloat.NewIndicesIfSortedDescending().ToArray(); // DOES NOT WORK :'(

        //s = "";
        //foreach (var f in sortedFloatIndices)
        //{
        //    s += f + " - ";
        //}
        //Debug.Log("sorted float indices = " + s);

        do
        {
            // iterate over coinSequenceFloat, in descending value order; assign coins to highest value in priority
            // if we still have coins, repeat process

            Debug.Log($"remaining coins = {remainingCoins}");

            for (int i = 0; i < gameConfig.sequenceLength; i++)
            {
                //int floatIndex = sortedFloatIndices[i];
                float floatValue = coinSequenceFloat[i];

                //Debug.Log($"floatIndex = {floatIndex}, floatValue = {floatValue}");

                int amount = Mathf.Min(remainingCoins, floatValue > avgFloat ? Mathf.FloorToInt(floatValue) : Mathf.FloorToInt(floatValue));
                amount = Mathf.Min(gameConfig.maxCoinPerSymbol - coinSequenceInt[i], amount);
                amount = Mathf.Max(0, amount);
                coinSequenceInt[i] += amount;
                remainingCoins -= amount;
                if (remainingCoins == 0) break;

            }

        } while (false); // remainingCoins > 0);

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
