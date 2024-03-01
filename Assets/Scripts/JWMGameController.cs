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
    [SerializeField] private StimulusDisplay stimulusDisplay;
    [SerializeField] private SymbolKeyboard playerA_Keyboard;
    [SerializeField] private SymbolKeyboard playerB_Keyboard;
    [SerializeField] private ResponsePanel playerA_ResponsePanel;
    [SerializeField] private ResponsePanel playerB_ResponsePanel;
    [SerializeField] private CoinCounter playerA_CoinCounter;
    [SerializeField] private CoinCounter playerB_CoinCounter;
    [SerializeField] private TMPro.TextMeshProUGUI playerA_ScoreText;
    [SerializeField] private TMPro.TextMeshProUGUI playerB_ScoreText;

    private bool roundStarted;
    
    private int[] correctIndexSequence;

    private int playerA_score, playerB_score;

    private const string parentSceneName = "Config";

    private JWMGameConfig gameConfig;

    private void Start()
    {
        gameConfig = AppState.GameConfig ?? debugConfig; // WIP;

        playerA_Keyboard.Initialize(cardShapePool);
        playerB_Keyboard.Initialize(cardShapePool);

        SetPlayerAScore(0);
        SetPlayerBScore(0);

        playerA_ResponsePanel.ResponseValidated += CheckForRoundEnd;
        playerB_ResponsePanel.ResponseValidated += CheckForRoundEnd;

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
        playerA_Keyboard.Interactable = interactable;
        playerB_Keyboard.Interactable = interactable;

        playerA_ResponsePanel.SetInteractable(interactable);
        playerB_ResponsePanel.SetInteractable(interactable);

    }

    private IEnumerator StartRound(bool isFirstRound = false)
    {
        playerA_Keyboard.ResetSelection();
        playerB_Keyboard.ResetSelection();

        playerA_ResponsePanel.Initialize(gameConfig.sequenceLength, cardShapePool);
        playerB_ResponsePanel.Initialize(gameConfig.sequenceLength, cardShapePool);

        playerA_CoinCounter.SetCoin(gameConfig.coinPerRound);
        playerB_CoinCounter.SetCoin(gameConfig.coinPerRound);

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

            playerA_ResponsePanel.SetStartRoundButtonVisible(true);

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

        playerB_ResponsePanel.SetSymbols(playerB_indices);

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < gameConfig.sequenceLength; i++)
        {
            int coinAmount = playerB_coinAmountSequence[i];
            playerB_ResponsePanel.AddCoinsInColumn(coinAmount, i);
            playerB_CoinCounter.RemoveCoin(coinAmount);
        }
        // playerB_ResponsePanel.SetCoversVisible(true);
        playerB_ResponsePanel.SetValidated();
    }

    private void CheckForRoundEnd()
    {
        bool bothPlayersHaveValidated = playerA_ResponsePanel.IsValidated && playerB_ResponsePanel.IsValidated;

        if (bothPlayersHaveValidated)
        {
            StartCoroutine(EndRound());
        }
    }

    private IEnumerator EndRound()
	{
        Debug.Log("Round ended, showing correct / incorrect feedback");

        SetBoardInteractable(false);

        playerA_ResponsePanel.ShowCorrectFeedback(correctIndexSequence);
        playerB_ResponsePanel.ShowCorrectFeedback(correctIndexSequence);

        stimulusDisplay.ShowStimulus();

        yield return new WaitForSeconds(1f);

        //int playerA_score = 0, playerB_score = 0;

        foreach (var column in playerA_ResponsePanel.GetCorrectColumns(correctIndexSequence))
        {
            playerA_score += (1 + column.CoinCount) * scoreMultiplier;
        }

        foreach (var column in playerB_ResponsePanel.GetCorrectColumns(correctIndexSequence))
        {
            playerB_score += (1 + column.CoinCount) * scoreMultiplier;
        }

        SetPlayerAScore(playerA_score);
        SetPlayerBScore(playerB_score);

        yield return new WaitForSeconds(2f);

        roundStarted = false;

        playerA_ResponsePanel.SetStartRoundButtonVisible(true);

        yield return new WaitUntil(() => roundStarted); // wait for human player to click on the start button

        StartCoroutine(StartRound());
    }

    private void SetPlayerAScore(int value)
	{
        playerA_ScoreText.text = $"Score: {value}";
	}

    private void SetPlayerBScore(int value)
    {
        playerB_ScoreText.text = $"Score: {value}";
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


    public void WIP_OnResponseColumnAddCoinClicked(ResponseColumn column)
    {
        if (column.CoinCount >= gameConfig.maxCoinPerSymbol) return;

        if (playerA_CoinCounter.CoinCount <= 0) return;

        playerA_CoinCounter.RemoveCoin();

        column.AddCoin();

        playerA_ResponsePanel.CheckIfCanValidate(gameConfig.coinPerRound);

    }

    public void WIP_OnResponseColumnRemoveCoinClicked(ResponseColumn column)
    {
        if (column.CoinCount <= 0) return;

        playerA_CoinCounter.AddCoin();

        column.RemoveCoin();
    }

    public void WIP_OnResponseColumnSymbolClicked(ResponseColumn column)
    {
        // assume this is from player A

        int? selectedSymbolIndex = playerA_Keyboard.SelectedSymbolIndex;

        if (selectedSymbolIndex == null) return;

        column.SetSymbol((int)selectedSymbolIndex);

        playerA_ResponsePanel.CheckIfCanValidate(gameConfig.coinPerRound);

        // playerA_Keyboard.ResetSelection();

    }

    public void WIP_OnResponseColumnCoinZoneMouseEnter(ResponseColumn column)
    {
        playerA_ResponsePanel.OnColumnHoverEnter(column);
    }

    public void WIP_OnResponseColumnCoinZoneMouseLeave(ResponseColumn column)
    {
        playerA_ResponsePanel.OnColumnHoverLeave(column);
    }

    public void WIP_OnStartRoundButtonClick()
    {
        playerA_ResponsePanel.SetStartRoundButtonVisible(false);

        roundStarted = true;
    }
}
