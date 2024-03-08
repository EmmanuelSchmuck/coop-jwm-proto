using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;
using System.Linq;

public class JWMGameController : MonoBehaviourSingleton<JWMGameController>
{
    [Header("Config")]
    [SerializeField] private JWMGameConfig debugConfig;
    //[SerializeField] private Sprite[] cardShapePool;
    [Header("References")]
    [SerializeField] private PlayerBoard playerA_Board;
    [SerializeField] private PlayerBoard playerB_Board;
    [SerializeField] private StimulusDisplay stimulusDisplay;

    private bool roundStarted;
    
    private int[] correctIndexSequence;

    private const string parentSceneName = "Config";

    private JWMGameConfig gameConfig;
    private RoundInfo roundInfo;

    private void Start()
    {
        gameConfig = AppState.GameConfig ?? debugConfig; // WIP;

        gameConfig.recallCurve = debugConfig.recallCurve; // WIP

        playerA_Board.Initialize(JWMGameConfig.SYMBOL_POOL_SIZE);
        playerB_Board.Initialize(JWMGameConfig.SYMBOL_POOL_SIZE);

        playerA_Board.StartRoundButtonClicked += CheckForRoundStart;

        playerA_Board.ResponseValidated += CheckForRoundEnd;
        playerB_Board.ResponseValidated += CheckForRoundEnd;

        StartCoroutine(StartRound(isFirstRound: true));
    }

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
            ExitToMenu();
		}
	}

    private void ExitToMenu()
	{
        UnityEngine.SceneManagement.SceneManager.LoadScene(parentSceneName);
    }

    private int[] GenerateCorrectIndicesSequence(int symbolPoolSize)
	{
        int[] correctIndexSequence = new int[gameConfig.sequenceLength];

        int lastShapeIndex = -1;

        for (int i = 0; i < gameConfig.sequenceLength; i++)
        {
            int shapeIndex;

            do
            {
                shapeIndex = Random.Range(0, symbolPoolSize);
            }
            while (!gameConfig.allowSymbolRepetition && shapeIndex == lastShapeIndex);

            correctIndexSequence[i] = shapeIndex;

            lastShapeIndex = shapeIndex;
        }

        return correctIndexSequence;
    }

    private IEnumerator StartRound(bool isFirstRound = false)
    {
        correctIndexSequence = GenerateCorrectIndicesSequence(JWMGameConfig.SYMBOL_POOL_SIZE);

        roundInfo = new RoundInfo() { gameConfig = this.gameConfig, correctIndexSequence = this.correctIndexSequence };
        
        playerA_Board.OnRoundStart(JWMGameConfig.SYMBOL_POOL_SIZE, roundInfo, isFirstRound);
        playerB_Board.OnRoundStart(JWMGameConfig.SYMBOL_POOL_SIZE, roundInfo, isFirstRound);

        stimulusDisplay.Initialize(correctIndexSequence);

        if(isFirstRound)
		{
            roundStarted = false;

            yield return new WaitUntil(() => roundStarted); // wait for human player to click on the start button
        }

        yield return new WaitForSeconds(1f);

        stimulusDisplay.DoDisplayAnimation(gameConfig.displayDurationPerSymbol);

        yield return new WaitForSeconds(gameConfig.sequenceLength * gameConfig.displayDurationPerSymbol);

        playerA_Board.OnStimulusDisplayCompleted(roundInfo);
        playerB_Board.OnStimulusDisplayCompleted(roundInfo);

        // end of stimulus display
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
        yield return stimulusDisplay.RevealAnimation(2f);

        yield return new WaitForSeconds(1f);

        playerA_Board.OnRoundEnd(roundInfo);

        yield return new WaitForSeconds(1f);

        playerB_Board.OnRoundEnd(roundInfo);

        yield return new WaitForSeconds(1f);

        UpdatePlayerScores();

        // to do: add & clarify "phases" such as feedback & score display, round end (show "next round" button) etc..;
        // should have: feedback then score then end round, all in here

        roundStarted = false;

        // to do: move this into StartRound ?
        yield return new WaitUntil(() => roundStarted); // wait for human player to click on the start button 

        StartCoroutine(StartRound());
    }

    private void UpdatePlayerScores()
	{
        int playerA_Score = playerA_Board.ComputeRawRoundScore(roundInfo);
        int playerB_Score = playerB_Board.ComputeRawRoundScore(roundInfo);

        PlayerBoard bestPlayer = playerA_Score > playerB_Score ? playerA_Board : playerB_Board;
        PlayerBoard worstPlayer = playerA_Score > playerB_Score ? playerB_Board : playerA_Board;
        int bestScore = Mathf.Max(playerA_Score, playerB_Score);

        switch (gameConfig.gameMode)
		{
            case GameMode.ActivePresence:
                playerA_Board.IncrementScore(playerA_Score);
                playerB_Board.IncrementScore(playerB_Score);
                break;
            case GameMode.NegativeReward:
                bestPlayer.IncrementScore(bestScore);
                worstPlayer.IncrementScore(0); // to display animation even if 0
                break;
            case GameMode.PositiveReward:
                bestPlayer.IncrementScore(bestScore);
                worstPlayer.IncrementScore(bestScore);
                break;
            default: break;
		}

    }

    public void CheckForRoundStart()
    {
        bool readyToStart = true; // to do: check for both players to be ready;

        if(readyToStart)
		{
            roundStarted = true;
        }  
    }
}
