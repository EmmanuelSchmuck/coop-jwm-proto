using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;
using System.Linq;

public class JWMGameController : MonoBehaviourSingleton<JWMGameController>
{
    [Header("Config")]
    [SerializeField] private JWMGameConfig debugConfig;
    [SerializeField] private List<Sprite> cardShapePool;
    [SerializeField] private AnimationCurve recallCurve;
    [SerializeField] private float roundStartDelay;
    [SerializeField] private int scoreMultiplier;
    [Header("References")]
    [SerializeField] private PlayerBoard playerA_Board;
    [SerializeField] private PlayerBoard playerB_Board;
    [SerializeField] private StimulusDisplay stimulusDisplay;

    private bool roundStarted;
    
    private int[] correctIndexSequence;

    private const string parentSceneName = "Config";

    private JWMGameConfig gameConfig;

    private void Start()
    {
        gameConfig = AppState.GameConfig ?? debugConfig; // WIP;

        playerA_Board.Initialize(cardShapePool);
        playerB_Board.Initialize(cardShapePool);

        playerA_Board.ResponseValidated += CheckForRoundEnd;
        playerB_Board.ResponseValidated += CheckForRoundEnd;

        StartCoroutine(StartRound(isFirstRound: true));
    }

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
            UnityEngine.SceneManagement.SceneManager.LoadScene(parentSceneName);
		}
	}

	private void SetBoardInteractable(bool interactable)
	{
        playerA_Board.SetInteractable(interactable);
        playerB_Board.SetInteractable(interactable);
    }

    private IEnumerator StartRound(bool isFirstRound = false)
    {
        playerA_Board.OnRoundStart(cardShapePool, gameConfig, isFirstRound);
        playerB_Board.OnRoundStart(cardShapePool, gameConfig, isFirstRound);

        SetBoardInteractable(false);

        List<Sprite> shapeSequence = new List<Sprite>();
        correctIndexSequence = new int[gameConfig.sequenceLength];

        Sprite lastShape = null;

        for (int i = 0; i < gameConfig.sequenceLength; i++)
        {
            Sprite shape;

            do
            {
                shape = cardShapePool.Random();
            }
            while (!gameConfig.allowSymbolRepetition && lastShape == shape);

            shapeSequence.Add(shape);

            correctIndexSequence[i] = cardShapePool.IndexOf(shape);

            lastShape = shape;
        }

        stimulusDisplay.Initialize(shapeSequence);

        if(isFirstRound)
		{
            roundStarted = false;

            yield return new WaitUntil(() => roundStarted); // wait for human player to click on the start button
        }

        yield return new WaitForSeconds(roundStartDelay);

        stimulusDisplay.DoDisplayAnimation(gameConfig.displayDurationPerSymbol);

        int[] playerB_indices = new int[gameConfig.sequenceLength];
        float[] playerB_coinAmountSequenceFloat = new float[gameConfig.sequenceLength];

        for (int i = 0; i < gameConfig.sequenceLength; i++)
        {
            float correctProbability = Mathf.Clamp01(recallCurve.Evaluate((float)i / Mathf.Max(1, gameConfig.sequenceLength - 1)));

            playerB_indices[i] = Random.value < correctProbability ? correctIndexSequence[i] : Random.Range(0, 9);

            playerB_coinAmountSequenceFloat[i] = correctProbability * gameConfig.coinPerRound / gameConfig.sequenceLength;
        }

        int[] playerB_coinAmountSequence = ComputeCoinSequence(playerB_coinAmountSequenceFloat);

        yield return new WaitForSeconds(gameConfig.sequenceLength * gameConfig.displayDurationPerSymbol);

        // end of stimulus display

        SetBoardInteractable(true);

        playerB_Board.ResponsePanel.SetSymbols(playerB_indices);

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < gameConfig.sequenceLength; i++)
        {
            int coinAmount = playerB_coinAmountSequence[i];
            playerB_Board.ResponsePanel.AddCoinsInColumn(coinAmount, i);
            playerB_Board.CoinCounter.RemoveCoin(coinAmount);
        }
        // playerB_ResponsePanel.SetCoversVisible(true);
        playerB_Board.ResponsePanel.SetValidated();
    }

    private void CheckForRoundEnd()
    {
        bool bothPlayersHaveValidated = playerA_Board.ResponsePanel.IsValidated && playerB_Board.ResponsePanel.IsValidated;

        if (bothPlayersHaveValidated)
        {
            StartCoroutine(EndRound());
        }
    }

    private IEnumerator EndRound() // to do: move more code from here to board.OnRoundEnd
    {
        Debug.Log("Round ended, showing correct / incorrect feedback");

        SetBoardInteractable(false);

        playerA_Board.ResponsePanel.ShowCorrectFeedback(correctIndexSequence);
        playerB_Board.ResponsePanel.ShowCorrectFeedback(correctIndexSequence);

        stimulusDisplay.ShowStimulus();

        yield return new WaitForSeconds(1f);

        playerA_Board.OnRoundEnd(correctIndexSequence, scoreMultiplier);
        playerB_Board.OnRoundEnd(correctIndexSequence, scoreMultiplier);

        yield return new WaitForSeconds(2f);

        roundStarted = false;

        playerA_Board.ResponsePanel.SetStartRoundButtonVisible(true);

        yield return new WaitUntil(() => roundStarted); // wait for human player to click on the start button

        StartCoroutine(StartRound());
    }

    private int[] ComputeCoinSequence(float[] coinSequenceFloat) // to do: refactor + fix, also compute coinSequenceFloat here
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

        for(int i = 0; i < remainingCoins; i++)
		{
            bool success = false;
            int attempts = 0;
            int maxAttempts = 100;

            while(! success && attempts < maxAttempts)
			{
                attempts++;
                int index = Random.Range(0, coinSequenceInt.Length);
                if(coinSequenceInt[index] < gameConfig.maxCoinPerSymbol)
				{
                    success = true;
                    coinSequenceInt[index]++;

                }
			}

            if (attempts == maxAttempts) Debug.LogError("Cannot place coins!");

        }

        return coinSequenceInt;
    }

    public void WIP_OnStartRoundButtonClick()
    {
        roundStarted = true;
    }
}
