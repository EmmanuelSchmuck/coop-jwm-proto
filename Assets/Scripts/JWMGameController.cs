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
    [SerializeField] private TMPro.TextMeshProUGUI gameModeText;


    private bool roundStarted;
    
    private int[] correctIndexSequence;

    private const string parentSceneName = "Config";

    private JWMGameConfig gameConfig;
    private RoundInfo roundInfo;
    private bool symbolsSubmitted_REFACTOR_ME;
    private PlayerBoard activePlayer;
    private PlayerBoard inactivePlayer;
    private PlayerBoard firstPlayer;

    private void Start()
    {
        gameConfig = AppState.GameConfig ?? debugConfig; // WIP;

        gameConfig.recallCurve = debugConfig.recallCurve; // WIP

        firstPlayer = playerA_Board; // WIP

        gameModeText.text = gameConfig.gameMode.ToString();

        playerA_Board.Initialize(JWMGameConfig.SYMBOL_POOL_SIZE, gameConfig.playerName);
        playerB_Board.Initialize(JWMGameConfig.SYMBOL_POOL_SIZE, "Bot Player");

        playerA_Board.StartRoundButtonClicked += CheckForRoundStart;

        playerA_Board.ResponseSumbitted += OnPlayerSubmittedResponse;
        playerB_Board.ResponseSumbitted += OnPlayerSubmittedResponse;

        playerA_Board.ResponseSymbolPicked += OnSymbolPicked;
        playerB_Board.ResponseSymbolPicked += OnSymbolPicked;

        StartCoroutine(StartRound(isFirstRound: true));
    }

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
            ExitToMenu();
		}
	}



    private void OnSymbolPicked(ResponseColumn responseColumn)
	{
        switch(gameConfig.ActionDependency)
		{
            default:
            case Dependency.None:
                return;
            case Dependency.Positive:
                inactivePlayer.ResponsePanel.SetSymbolInColumn((int)responseColumn.SymbolIndex, responseColumn.ColumnIndex);
                break;
            case Dependency.Negative:
                inactivePlayer.ResponsePanel.SetColumnLocked(responseColumn.ColumnIndex);
                //playerB_Board.ResponsePanel.SetColumnLocked(responseColumn.ColumnIndex);
                break;  
		}

        if(inactivePlayer.ResponsePanel.AllColumnsPickedOrLocked)
		{
            StartCoinBettingPhase();
        }
		else
		{
            NextPlayerResponseTurn();
        }
	}

    private void NextPlayerResponseTurn()
	{
        SetActivePlayer(activePlayer == null ? firstPlayer : inactivePlayer);
        activePlayer.OnResponseTurnStart(roundInfo);
        inactivePlayer.OnResponseTurnEnd();
    }

    private void SetActivePlayer(PlayerBoard player)
	{
        activePlayer = player;
        inactivePlayer = player == playerA_Board ? playerB_Board : playerA_Board;

        //Debug.Log($"Set active player to {player}");
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

        symbolsSubmitted_REFACTOR_ME = false;

        if (isFirstRound)
		{
            roundStarted = false;

            yield return new WaitUntil(() => roundStarted); // wait for human player to click on the start button
        }

        playerA_Board.OnStimulusDisplayStart();
        playerB_Board.OnStimulusDisplayStart();

        SoundManager.Instance.PlaySound(SoundType.RoundStart);

        yield return stimulusDisplay.DisplayAnimation(gameConfig.displayDurationPerSymbol, 1f);

        playerA_Board.OnStimulusDisplayEnd(roundInfo);
        playerB_Board.OnStimulusDisplayEnd(roundInfo);


        if (gameConfig.ActionDependency == Dependency.None)
		{
            playerA_Board.OnResponsePhaseStart(roundInfo);
            playerB_Board.OnResponsePhaseStart(roundInfo);
        }
        else
		{
            NextPlayerResponseTurn();
		}
    }

    private void StartCoinBettingPhase()
	{
        symbolsSubmitted_REFACTOR_ME = true; // refactor this
        playerA_Board.OnCoinBettingPhaseStart(roundInfo);
        playerB_Board.OnCoinBettingPhaseStart(roundInfo);
    }

    private void OnPlayerSubmittedResponse()
    {
        bool bothPlayersHaveValidated = playerA_Board.ResponsePanel.IsValidated && playerB_Board.ResponsePanel.IsValidated;

        if (bothPlayersHaveValidated)
        {
            if(!symbolsSubmitted_REFACTOR_ME)
			{
                StartCoinBettingPhase();
                // start coin betting
            }
            else
			{
                StartCoroutine(EndRound());
            }    
        }
    }

    private IEnumerator EndRound() // to do: move more code from here to board.OnRoundEnd
    {
        playerA_Board.SetInteractable(false);
        playerB_Board.SetInteractable(false);

        yield return new WaitForSeconds(1f);

        yield return stimulusDisplay.RevealAnimation(1.5f);

        yield return new WaitForSeconds(0.3f);

        // to do : feedback presentation phase
        yield return playerA_Board.ShowFeedback(roundInfo);
        yield return playerB_Board.ShowFeedback(roundInfo);

        yield return new WaitForSeconds(0.5f);

        var scores = UpdatePlayerScores();

        playerA_Board.IncrementScore(scores.Item1);

        yield return new WaitForSeconds(1.5f);

        playerB_Board.IncrementScore(scores.Item2);

        yield return new WaitForSeconds(1f);

        stimulusDisplay.Hide();

        playerA_Board.OnRoundEnd(roundInfo);
        playerB_Board.OnRoundEnd(roundInfo);

        // to do: add & clarify "phases" such as feedback & score display, round end (show "next round" button) etc..;
        // should have: feedback then score then end round, all in here

        roundStarted = false;

        // to do: move this into StartRound ?
        yield return new WaitUntil(() => roundStarted); // wait for human player to click on the start button 

        StartCoroutine(StartRound());
    }

    enum EndResult
    {
        BestPlayerIsA,
        BestPlayerIsB,
        Tie
    }

    private (int, int) UpdatePlayerScores()
	{
        int playerA_Score = playerA_Board.ComputeRawRoundScore(roundInfo);
        int playerB_Score = playerB_Board.ComputeRawRoundScore(roundInfo);

        EndResult result = playerA_Score == playerB_Score ? EndResult.Tie
            : playerA_Score > playerB_Score ? EndResult.BestPlayerIsA
            : EndResult.BestPlayerIsB;

        //int bestScore = Mathf.Max(playerA_Score, playerB_Score);
        int sumScore = playerA_Score + playerB_Score;

        return gameConfig.RewardDependency switch
        {
            Dependency.None => (playerA_Score, playerB_Score),
            Dependency.Positive => (sumScore, sumScore),
            Dependency.Negative => result switch
            {
                EndResult.Tie => (playerA_Score, playerB_Score),
                EndResult.BestPlayerIsA => (playerA_Score, 0),
                EndResult.BestPlayerIsB => (0, playerB_Score),
                _ => throw new System.NotImplementedException()
            },
            _ => throw new System.NotImplementedException()
        };
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
