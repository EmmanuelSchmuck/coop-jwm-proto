using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBoard : MonoBehaviour
{
    [SerializeField] private bool DEBUG_isHumanPlayer;
    [SerializeField] private ResponsePanel responsePanel;
    [SerializeField] private SymbolKeyboard symbolKeyboard;
    [SerializeField] private CoinCounter coinCounter;
    [SerializeField] private ScoreCounter scoreCounter;
    [SerializeField] private TMPro.TextMeshProUGUI instructionText;

    public ResponsePanel ResponsePanel => responsePanel;
    public SymbolKeyboard SymbolKeyboard => symbolKeyboard;
    public CoinCounter CoinCounter => coinCounter;
    private int score;
    public JWMGameConfig GameConfig { get; private set; }
    public int? SelectedSymbolIndex => symbolKeyboard.SelectedSymbolIndex;

    public event System.Action ResponseSumbitted;
    public event System.Action StartRoundButtonClicked;
    public event System.Action<RoundInfo> RoundStarted;
    public event System.Action<RoundInfo> StimulusDisplayed; // rename: symbol pick phase started
    public event System.Action<RoundInfo> CoinBettingStarted;
    public event System.Action<ResponseColumn> ResponseSymbolPicked;

    private const string EMPTY = "";
    private const string STIMULUS_DISPLAY_INSTRUCTION = "Remember each symbol.";
    private const string STIMULUS_RESPONSE_INSTRUCTION = "Use the keyboard on left to assign symbols to each card.";
    private const string COIN_BETTING_INSTRUCTION = "Right-click below each symbol to bet coins.";

    public void Initialize(int symbolPoolSize)
	{
        symbolKeyboard.Initialize(symbolPoolSize);
        SetScore(0, animate: false);
        SetInstructionText(EMPTY);
	}
    public void OnResponseValidated()
	{
        ResponseSumbitted?.Invoke();
    }

    private void SetInstructionText(string text)
    {
        if (!DEBUG_isHumanPlayer) text = "";
        instructionText.text = text;
    }

    public void OnRoundStart(int symbolPoolSize, RoundInfo roundInfo, bool isFirstRound)
	{
        this.GameConfig = roundInfo.gameConfig;

        symbolKeyboard.ResetSelection();

        responsePanel.Initialize(GameConfig.sequenceLength, this);

        coinCounter.SetCoin(GameConfig.CoinPerRound);

        if(isFirstRound && DEBUG_isHumanPlayer)
		{
            responsePanel.SetStartRoundButtonVisible(true);
        }

        SetInteractable(false);

        RoundStarted?.Invoke(roundInfo);
    }

    public void OnStimulusDisplayStart()
    {
        SetInstructionText(STIMULUS_DISPLAY_INSTRUCTION);
    }

    public void OnCoinBettingPhaseStart(RoundInfo roundInfo)
	{
        SetInstructionText(COIN_BETTING_INSTRUCTION);
        CoinBettingStarted?.Invoke(roundInfo);
        // enable coin zones
        responsePanel.SetCoinZoneInteractable(true);
        responsePanel.SetValidated(false);
	}

    public void OnStimulusDisplayCompleted(RoundInfo roundInfo)
	{
        SetInstructionText(STIMULUS_RESPONSE_INSTRUCTION);
        responsePanel.SetSymbolsInteractable(true);
        symbolKeyboard.Interactable = true;

        //responsePanel.set

        StimulusDisplayed?.Invoke(roundInfo);
    }

    public IEnumerator ShowFeedback(RoundInfo roundInfo)
    {
        SetInstructionText(EMPTY);
        //responsePanel.ShowCorrectFeedback(roundInfo.correctIndexSequence);

        SetInteractable(false);

        for (int i = 0; i < responsePanel.Columns.Count; i++)
        {
            yield return new WaitForSeconds(0.3f);
            responsePanel.Columns[i].ShowCorrectFeedback(responsePanel.Columns[i].SymbolIndex == roundInfo.correctIndexSequence[i]);
        }
    }

    public void OnRoundEnd(RoundInfo roundInfo)
	{
        for (int i = 0; i < responsePanel.Columns.Count; i++)
        {
            responsePanel.Columns[i].Cleanup();
        }

        if (DEBUG_isHumanPlayer) responsePanel.SetStartRoundButtonVisible(true);   
    }

    public int ComputeRawRoundScore(RoundInfo roundInfo)
    {
        int round_score = 0;

        foreach (var column in responsePanel.GetCorrectColumns(roundInfo.correctIndexSequence))
        {
            round_score += (1 + column.CoinCount);
        }

        return round_score;
    }

    public void WIP_OnStartRoundButtonClick()
    {
        responsePanel.SetStartRoundButtonVisible(false);

        StartRoundButtonClicked?.Invoke();
    }

    public void OnResponseSymbolPickAttempted(ResponseColumn column) // rename this !
	{
        int? selectedSymbolIndex = SelectedSymbolIndex; // this needs to work for the bot as well; bot should use the keyboard

        if (selectedSymbolIndex == null) return;

        column.SetSymbol((int)selectedSymbolIndex);

        
        bool canValidate = responsePanel.AllSymbolsPicked;
        responsePanel.SetCanValidate(canValidate);
        SetInstructionText(canValidate ? EMPTY : STIMULUS_RESPONSE_INSTRUCTION);
        //responsePanel.CheckIfCanValidate(GameConfig.CoinPerRound);

        ResponseSymbolPicked?.Invoke(column);
    }

    public void WIP_OnResponseColumnAddCoinClicked(ResponseColumn column)
    {
        if (column.CoinCount >= GameConfig.maxCoinPerSymbol) return;

        if (coinCounter.CoinCount <= 0) return;

        coinCounter.RemoveCoin();

        column.AddCoin();

        bool canValidate = GameConfig.CoinPerRound == responsePanel.CoinsInColumns;
        responsePanel.SetCanValidate(canValidate);

        SetInstructionText(canValidate ? EMPTY : COIN_BETTING_INSTRUCTION);

    }

    public void WIP_OnResponseColumnRemoveCoinClicked(ResponseColumn column)
    {
        if (column.CoinCount <= 0) return;

        coinCounter.AddCoin();

        column.RemoveCoin();

        bool canValidate = GameConfig.CoinPerRound == responsePanel.CoinsInColumns;
        responsePanel.SetCanValidate(canValidate);

        SetInstructionText(canValidate ? EMPTY : COIN_BETTING_INSTRUCTION);
    }

    public void SetInteractable(bool interactable)
    {
        if (!DEBUG_isHumanPlayer) interactable = false; // dirty, refactor at some point!

        symbolKeyboard.Interactable = interactable;

        responsePanel.SetCoinZoneInteractable(interactable);
        responsePanel.SetSymbolsInteractable(interactable);
    }

    public void IncrementScore(int value)
    {
        SetScore(score + value);
        SoundManager.Instance.PlaySound(SoundType.GainPoints);
    }

    public void SetScore(int value, bool animate = true)
    {
        score = value;
        scoreCounter.SetScore(value, animate);  
    }
}
