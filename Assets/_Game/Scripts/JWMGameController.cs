using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;
using System.Linq;
using PlayerProfileSystem;

public class JWMGameController : MonoBehaviourSingleton<JWMGameController>
{
    [Header("Config")]
    [SerializeField] private JWMGameConfig debugConfig;
    [SerializeField] private float interTurnDelay = 0.3f;
    [SerializeField] private PlayerInfo botPlayerInfo;
    //[SerializeField] private Sprite[] cardShapePool;
    [Header("References")]
    [SerializeField] private PlayerBoard playerA_Board;
    [SerializeField] private PlayerBoard playerB_Board;
    [SerializeField] private StimulusDisplay stimulusDisplay;
    [SerializeField] private TMPro.TextMeshProUGUI gameModeText;
    [SerializeField] private PlayerInfo debugHumanPlayerInfo;

    private bool roundStarted;
    
    private int[] correctIndexSequence;

    private const string parentSceneName = "Config";

    private JWMGameConfig gameConfig;
    private RoundInfo roundInfo;

    private PlayerBoard activePlayer;
    private PlayerBoard inactivePlayer;
    private PlayerBoard firstPlayer;
    private int turnCount;
    private bool playerA_lastTurnIsSuccess;
    private bool BothPlayersHaveValidated => playerA_Board.IsValidated && (playerB_Board.IsValidated || playerB_Board.IsDisabled);

    private void Start()
    {
        gameConfig = AppState.GameConfig ?? debugConfig; // WIP;

        gameConfig.recallCurve = debugConfig.recallCurve; // WIP

        firstPlayer = playerA_Board; // WIP

        gameModeText.text = gameConfig.gameMode.ToString();

        PlayerInfo humanPlayerInfo = AppState.HumanPlayerInfo ?? debugHumanPlayerInfo;

        playerA_Board.Initialize(JWMGameConfig.SYMBOL_POOL_SIZE, humanPlayerInfo.playerName, humanPlayerInfo.playerAvatar.avatarPortrait);
        playerB_Board.Initialize(JWMGameConfig.SYMBOL_POOL_SIZE, botPlayerInfo.playerName, botPlayerInfo.playerAvatar.avatarPortrait);

        playerA_Board.StartRoundButtonClicked += CheckForRoundStart;

        stimulusDisplay.SetVisible(false);

        StartCoroutine(StartRound(isFirstRound: true));
    }

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
            ExitToMenu();
		}
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
        turnCount = 0;
        correctIndexSequence = GenerateCorrectIndicesSequence(JWMGameConfig.SYMBOL_POOL_SIZE);

        roundInfo = new RoundInfo() { gameConfig = this.gameConfig, correctIndexSequence = this.correctIndexSequence };
        
        playerA_Board.OnRoundStart(JWMGameConfig.SYMBOL_POOL_SIZE, roundInfo, isFirstRound);
        playerB_Board.OnRoundStart(JWMGameConfig.SYMBOL_POOL_SIZE, roundInfo, isFirstRound);

        stimulusDisplay.Initialize(correctIndexSequence);

        if (true)
		{
            roundStarted = false;

            yield return new WaitUntil(() => roundStarted); // wait for human player to click on the start button
        }

        yield return stimulusDisplay.AnimateVisible(true);

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

            yield return new WaitUntil(() => BothPlayersHaveValidated);
        }
        else if (gameConfig.ActionDependency == Dependency.Positive)
        {
            playerA_Board.ResponsePanel.SetAllColumnsLocked();
            playerB_Board.ResponsePanel.SetAllColumnsLocked();

            do
            {
                if (turnCount > 0) yield return playerA_Board.SymbolPickResponseTurn();
                
                if (BothPlayersHaveValidated) break;

                yield return playerA_Board.LockResponseTurn();

                yield return new WaitForSeconds(interTurnDelay);

                yield return playerB_Board.SymbolPickResponseTurn();
                yield return playerB_Board.LockResponseTurn();

                yield return new WaitForSeconds(interTurnDelay);

                turnCount++;

            } while (!BothPlayersHaveValidated);
        }
        else // negative
        {
            do
            {
                yield return playerA_Board.SymbolPickResponseTurn();
                yield return playerA_Board.LockResponseTurn();

                yield return new WaitForSeconds(interTurnDelay);

                if (BothPlayersHaveValidated) break;

                yield return playerB_Board.SymbolPickResponseTurn();
                yield return playerB_Board.LockResponseTurn();

                yield return new WaitForSeconds(interTurnDelay);

            } while (!BothPlayersHaveValidated);
        }

        playerA_Board.OnCoinBettingPhaseStart(roundInfo);
        playerB_Board.OnCoinBettingPhaseStart(roundInfo);

        yield return new WaitUntil(() => BothPlayersHaveValidated);

        playerA_Board.OnCoinBettingEnd();
        playerB_Board.OnCoinBettingEnd();

        StartCoroutine(EndRound());
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

        stimulusDisplay.HideCards();

        yield return playerB_Board.OnRoundEnd(roundInfo);
        yield return playerA_Board.OnRoundEnd(roundInfo);

        yield return stimulusDisplay.AnimateVisible(false);

        // adaptive procedure start =====

        if (gameConfig.enable2Up1DDownStaircase)
		{
            bool playerASuccess = playerA_Board.AllSymbolsAreCorrect(roundInfo);

            if (!playerASuccess) // round failed: difficulty down
            {
                gameConfig.sequenceLength--;
            }
            else if (playerASuccess && playerA_lastTurnIsSuccess) // two consecutive successes: difficulty up
            {
                gameConfig.sequenceLength++;

            }

            gameConfig.ClampSequenceLength(gameConfig.sequenceLength);

            playerA_lastTurnIsSuccess = playerASuccess;
        }   

        // adaptive procedure end =====


        // to do: add & clarify "phases" such as feedback & score display, round end (show "next round" button) etc..;
        // should have: feedback then score then end round, all in here

        //roundStarted = false;

        // to do: move this into StartRound ?
        //yield return new WaitUntil(() => roundStarted); // wait for human player to click on the start button 

        StartCoroutine(StartRound());
    }

    enum EndResult
    {
        BestPlayerIsA,
        BestPlayerIsB,
        Tie
    }

    // returns scores for player A and player B
    private (int, int) UpdatePlayerScores()
	{
        int playerA_Score = playerA_Board.ComputeRawRoundScore(roundInfo);
        int playerB_Score = playerB_Board.ComputeRawRoundScore(roundInfo);

        EndResult result = playerA_Score == playerB_Score ? EndResult.Tie
            : playerA_Score > playerB_Score ? EndResult.BestPlayerIsA
            : EndResult.BestPlayerIsB;

        int sumScore = playerA_Score + playerB_Score;

        return gameConfig.RewardDependency switch
        {
            Dependency.None => (playerA_Score, playerB_Score),
            Dependency.Positive => (sumScore, sumScore),
            Dependency.Negative => result switch
            {
                EndResult.Tie => (playerA_Score, playerB_Score),
                EndResult.BestPlayerIsA => (sumScore, 0),
                EndResult.BestPlayerIsB => (0, sumScore),
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
