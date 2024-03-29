using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBoard : MonoBehaviour
{
    [SerializeField] private bool DEBUG_isHumanPlayer;
    [SerializeField] private bool isMainPlayer;
    [SerializeField] private ResponsePanel responsePanel;
    [SerializeField] private SymbolKeyboard symbolKeyboard;
    [SerializeField] private CoinCounter coinCounter;
    [SerializeField] private ScoreCounter scoreCounter;
    [SerializeField] private TMPro.TextMeshProUGUI instructionText;
    [SerializeField] private TMPro.TextMeshProUGUI playerNameText;

    public ResponsePanel ResponsePanel => responsePanel;
    public SymbolKeyboard SymbolKeyboard => symbolKeyboard;
    public CoinCounter CoinCounter => coinCounter;
    private int score;
    public JWMGameConfig GameConfig { get; private set; }
    public int? SelectedSymbolIndex => symbolKeyboard.SelectedSymbolIndex;

    public event System.Action ResponseSumbitted;
    public event System.Action StartRoundButtonClicked;
    public event System.Action<RoundInfo> RoundStarted;
    public event System.Action<RoundInfo> StimulusDisplayed;
    public event System.Action<RoundInfo> ResponsePhaseStarted; // rename: symbol pick phase started
    public event System.Action<RoundInfo> CoinBettingStarted;
    public event System.Action<RoundInfo> ResponseTurnStarted;
    public event System.Action<ResponseColumn> ResponseSymbolPicked;

    private const string EMPTY = "";
    private const string STIMULUS_DISPLAY_INSTRUCTION = "Remember each symbol.";
    private const string STIMULUS_RESPONSE_INSTRUCTION = "Use the keyboard on left to assign symbols to each card.";
    private const string COIN_BETTING_INSTRUCTION = "Right-click below each symbol to bet coins.";
    private const string TURN_START_INSTRUCTION = "Use the keyboard on left to assign a symbol to a card.";
    private const string TURN_END_INSTRUCTION = "Waiting for the other player to pick a symbol...";

    private RoundInfo roundInfo;
    private int symbolPickedCount;
    private int turnCount;

    public void Initialize(int symbolPoolSize, string playerName)
	{
        SetPlayerName(playerName);
        symbolKeyboard.Initialize(symbolPoolSize);
        SetScore(0, animate: false);
        SetInstructionText(EMPTY);
        SetCoinCounterVisible(false);

    }

    private void SetCoinCounterVisible(bool visible)
	{
        coinCounter.gameObject.SetActive(visible);
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

    private void SetPlayerName(string name)
	{
        playerNameText.text = name;
	}

    public void OnRoundStart(int symbolPoolSize, RoundInfo roundInfo, bool isFirstRound)
	{
        this.GameConfig = roundInfo.gameConfig;
        this.roundInfo = roundInfo;

        symbolKeyboard.ResetSelection();

        symbolPickedCount = 0;
        turnCount = 0;

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

    public void OnStimulusDisplayEnd(RoundInfo roundInfo)
    {
        StimulusDisplayed?.Invoke(roundInfo);
    }

    public void OnCoinBettingPhaseStart(RoundInfo roundInfo)
	{
        symbolKeyboard.Interactable = false;
        responsePanel.CoinZoneHighlighted = isMainPlayer;
        SetCoinCounterVisible(isMainPlayer);
        SetInstructionText(COIN_BETTING_INSTRUCTION);
        CoinBettingStarted?.Invoke(roundInfo);
        // enable coin zones
        responsePanel.SetCoinZoneInteractable(true);
        responsePanel.SetValidated(false);
	}

    public void OnResponseTurnStart(RoundInfo roundInfo)
	{
        turnCount++;
        
        if(turnCount == 1)
		{
            symbolKeyboard.Highlighted = isMainPlayer;
            symbolKeyboard.SymbolSelected += OnKeyboardFirstSelect;
        }

        SetInstructionText(TURN_START_INSTRUCTION);

        symbolKeyboard.Interactable = isMainPlayer;
        responsePanel.SetSymbolsInteractable(true, onlyNonPickedSymbols: true);
        ResponseTurnStarted?.Invoke(roundInfo);
    }

    public void OnResponseTurnEnd()
	{
        SetInstructionText(TURN_END_INSTRUCTION);
        symbolKeyboard.Interactable = false;
        responsePanel.SetSymbolsInteractable(false);
    }

    public void OnResponsePhaseStart(RoundInfo roundInfo)
	{
        SetInstructionText(STIMULUS_RESPONSE_INSTRUCTION);
        symbolKeyboard.Highlighted = isMainPlayer;
        symbolKeyboard.SymbolSelected += OnKeyboardFirstSelect;
        responsePanel.SetSymbolsInteractable(true);

        if(DEBUG_isHumanPlayer) symbolKeyboard.Interactable = true;


        //responsePanel.set

        ResponsePhaseStarted?.Invoke(roundInfo);
    }

    private void OnKeyboardFirstSelect()
	{
        symbolKeyboard.Highlighted = false;
        responsePanel.SymbolsHighlighted = isMainPlayer;
    }

    public IEnumerator ShowFeedback(RoundInfo roundInfo)
    {
        SetCoinCounterVisible(false);

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

        symbolPickedCount++;

        if(symbolPickedCount == 1)
		{
            responsePanel.SymbolsHighlighted = false;
            symbolKeyboard.SymbolSelected -= OnKeyboardFirstSelect;
        }

        

        column.SetSymbol((int)selectedSymbolIndex);

        bool canValidate = responsePanel.AllColumnsPickedOrLocked;

        if(GameConfig.ActionDependency == Dependency.None)
		{
            responsePanel.SetCanValidate(canValidate);
            SetInstructionText(canValidate ? EMPTY : STIMULUS_RESPONSE_INSTRUCTION);
        }
        else if(canValidate) // action dependency positive or negative; we validate without using the button
		{
            responsePanel.SetValidated(true);
		}
        
        ResponseSymbolPicked?.Invoke(column);
    }

    public void WIP_OnResponseColumnAddCoinClicked(ResponseColumn column)
    {
        if (column.CoinCount >= GameConfig.maxCoinPerSymbol) return;

        if (coinCounter.CoinCount <= 0) return;

        responsePanel.CoinZoneHighlighted = false;

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

        bool canValidate = GameConfig.ActionDependency == Dependency.Negative ? true : GameConfig.CoinPerRound == responsePanel.CoinsInColumns;
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
