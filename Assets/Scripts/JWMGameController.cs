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

        gameConfig.recallCurve = debugConfig.recallCurve; // WIP

        playerA_Board.Initialize(cardShapePool);
        playerB_Board.Initialize(cardShapePool);

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

	private void SetBoardInteractable(bool interactable)
	{
        playerA_Board.SetInteractable(interactable);
        playerB_Board.SetInteractable(interactable);
    }

    private IEnumerator StartRound(bool isFirstRound = false)
    {
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

        RoundInfo roundInfo = new RoundInfo() { gameConfig = this.gameConfig, correctIndexSequence = this.correctIndexSequence };
        
        playerA_Board.OnRoundStart(cardShapePool, roundInfo, isFirstRound);
        playerB_Board.OnRoundStart(cardShapePool, roundInfo, isFirstRound);

        SetBoardInteractable(false);

        stimulusDisplay.Initialize(shapeSequence);

        if(isFirstRound)
		{
            roundStarted = false;

            yield return new WaitUntil(() => roundStarted); // wait for human player to click on the start button
        }

        yield return new WaitForSeconds(roundStartDelay);

        stimulusDisplay.DoDisplayAnimation(gameConfig.displayDurationPerSymbol);

        yield return new WaitForSeconds(gameConfig.sequenceLength * gameConfig.displayDurationPerSymbol);

        // end of stimulus display

        SetBoardInteractable(true);
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

        // to do: refactor; should not have to call this here
        playerA_Board.ResponsePanel.SetStartRoundButtonVisible(true);

        // to do: move this into StartRound ?
        yield return new WaitUntil(() => roundStarted); // wait for human player to click on the start button 

        StartCoroutine(StartRound());
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
