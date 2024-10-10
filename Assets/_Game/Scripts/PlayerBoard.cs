using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Toolbox;
using System.Linq;

public class PlayerBoard : MonoBehaviour
{
    [SerializeField] private GameObject disabledCover;
    [SerializeField] private SymbolCard selectedCard;
    [SerializeField] private bool isMainPlayer;
    [SerializeField] private PlayerBoard oppositeBoard;
    [SerializeField] private ResponsePanel responsePanel;
    [SerializeField] private SymbolKeyboard symbolKeyboard;
    [SerializeField] private CoinRepository[] coinRepositories;
    [SerializeField] private ScoreCounter scoreCounter;
    [SerializeField] private TMPro.TextMeshProUGUI instructionText;
    [SerializeField] private TMPro.TextMeshProUGUI playerNameText;
    [SerializeField] private Image avatarImage;
    [SerializeField] private GameObject validateButton;
    
    [SerializeField] private GameObject activePlayerIndicator;
    [SerializeField] private Coin bronzeCoinPrefab, silverCoinPrefab, goldCoinPrefab;

    public PlayerBoard OppositeBoard => oppositeBoard;
    public CoinRepository[] CoinRepositories => coinRepositories;

    public ResponsePanel ResponsePanel => responsePanel;
    public SymbolKeyboard SymbolKeyboard => symbolKeyboard;
    public bool IsValidated { get; private set; }
    private int score;
    public GameConfig GameConfig { get; private set; }
    public int? SelectedSymbolIndex => symbolKeyboard.SelectedSymbolIndex;

    public event System.Action<RoundInfo> RoundStarted;
    public event System.Action<RoundInfo> StimulusDisplayed;
    public event System.Action<RoundInfo> ResponsePhaseStarted; // rename: symbol pick phase started
    public event System.Action<RoundInfo> CoinBettingStarted;
    public event System.Action<RoundInfo> LockResponseTurnStarted;
    public event System.Action<RoundInfo> SymbolResponseTurnStarted;
    public event System.Action<ResponseColumn> ResponseSymbolClicked;

    private const string EMPTY = "";
    private const string STIMULUS_DISPLAY_INSTRUCTION = "Remember the sequence of symbols.";
    private const string STIMULUS_RESPONSE_INSTRUCTION = "Drag & drop symbols from the bottom to replicate the original sequence.";
    private const string COIN_BETTING_INSTRUCTION = "Click below each card to place coins. Right-click to remove coins.";
    private const string TURN_START_INSTRUCTION = "Use cards on the left to replicate the hidden sequence.";
    private const string TURN_END_INSTRUCTION = "Waiting for the other player to pick a card...";
    private const string LOCK_SYMBOL_INSTRUCTION = "Click on an opposite card slot to lock it.";
    private const string UNLOCK_SYMBOL_INSTRUCTION = "Click on an opposite card slot to unlock it.";
    private const string ASSIGN_SYMBOL_INSTRUCTION = "Use cards on the left to replicate the hidden sequence.";

    private RoundInfo roundInfo;
    private int symbolPickedCount;
    private int turnCount;
    private bool oppositeBoardSymbolClicked;
    private bool symbolClicked;
    public bool IsDisabled { get; private set; }
    private Coin grabbedCoin;
    private CoinRepository lastClickedRepo;
    public void Initialize(int symbolPoolSize, string playerName, Sprite avatar)
	{
        activePlayerIndicator.SetActive(false);
        IsValidated = false;
        SetCanValidate(false);
        SetPlayerName(playerName);
        avatarImage.sprite = avatar;
        symbolKeyboard.Initialize(symbolPoolSize);
        SetScore(0, animate: false);
        SetInstructionText(EMPTY);

        responsePanel.Initialize(0, this);

        SetCoinReposVisible(false);
        symbolKeyboard.SetVisible(false);

        if (isMainPlayer)  selectedCard.gameObject.SetActive(false);

        if (isMainPlayer) symbolKeyboard.SymbolSelected += OnKeyboardSymbolSelected;

    }

    private void Update()
    {
        if (!isMainPlayer) return;
        
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));

        selectedCard.transform.position = cursorPosition;
        
        if(grabbedCoin != null)
		{
            grabbedCoin.transform.position = cursorPosition;
		}
    }

    private void OnKeyboardSymbolSelected()
	{
        selectedCard.gameObject.SetActive(true);
        selectedCard.Initialize(symbolKeyboard.SelectedSymbolIndex);
        selectedCard.SetVisible(true);
    }

    private void SetCoinReposVisible(bool visible)
	{
        foreach(var rep in coinRepositories)
		{
            rep.gameObject.SetActive(visible);
		}
    }

    private void SetInstructionText(string text)
    {
        if (!isMainPlayer) text = "";
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

        if(!isMainPlayer)
		{
            IsDisabled = GameConfig.gameMode == GameMode.SinglePlayer;
            disabledCover.SetActive(IsDisabled);
        }

        IsValidated = false;
        SetCanValidate(false);

        symbolKeyboard.ResetSelection();

        symbolPickedCount = 0;
        turnCount = 0;

        responsePanel.Initialize(GameConfig.sequenceLength, this);
        var coinsPerRound = GameConfig.CoinsPerRound;
        InitializeCoinRepos(coinsPerRound.Item1, coinsPerRound.Item2, coinsPerRound.Item3);
        foreach (var coinRepo in GetComponentsInChildren<CoinRepository>(true))
        {
            coinRepo.Interacted += OnCoinRepoClicked;
        }

        SetInteractable(false);

        RoundStarted?.Invoke(roundInfo);
    }

    private void InitializeCoinRepos(int bronzeCount, int silverCount, int goldCount)
	{
        Coin[] bronzeCoins = new Coin[bronzeCount];
        for(int i = 0; i<bronzeCount;i++)
		{
            bronzeCoins[i] = Instantiate(bronzeCoinPrefab, this.transform);
        }
        Coin[] silverCoins = new Coin[silverCount];
        for (int i = 0; i < silverCount; i++)
        {
            silverCoins[i] = Instantiate(silverCoinPrefab, this.transform);
        }
        Coin[] goldCoins = new Coin[goldCount];
        for (int i = 0; i < goldCount; i++)
        {
            goldCoins[i] = Instantiate(goldCoinPrefab, this.transform);
        }
        coinRepositories[0].Initialize(bronzeCoins);
        coinRepositories[1].Initialize(silverCoins);
        coinRepositories[2].Initialize(goldCoins);

        //oinRepositories[3].Initialize(new Coin[1] { Instantiate(goldCoinPrefab, this.transform) });

    }



    public void OnStimulusDisplayStart()
    {
        if (IsDisabled) return;

        SetInstructionText(STIMULUS_DISPLAY_INSTRUCTION);
    }

    public void OnStimulusDisplayEnd(RoundInfo roundInfo)
    {
        if (IsDisabled) return;

        StimulusDisplayed?.Invoke(roundInfo);

        symbolKeyboard.SetVisible(true, animate: true);
        responsePanel.SetColumnsVisible(true, animate: true);
    }

    private void OnCoinRepoClicked(CoinRepository repo, bool isDrag)
	{
        if (repo == lastClickedRepo && isDrag) return;

        lastClickedRepo = repo;

        //Debug.Log("CoinRepoClicked");
        if(grabbedCoin != null && repo.CanAcceptCoinOfType(grabbedCoin.CoinType)) // deposit or swap
		{
            if(repo.CoinCount == repo.MaxCointCount) // repo is full, must swap
			{
                Coin swapCoin = repo.TakeLastCoin();
                swapCoin.transform.SetParent(this.transform);
                repo.AddCoin(grabbedCoin);
                grabbedCoin = swapCoin;
            }
            else // simple deposit
			{
                repo.AddCoin(grabbedCoin);
                grabbedCoin = null;
            }
		}
        //else if(//test for swap)
        
        else if (grabbedCoin == null && repo.CoinCount > 0) // take coin if possible
        {
            grabbedCoin = repo.TakeLastCoin();
            grabbedCoin.transform.SetParent(this.transform);
                //Debug.Log($"Taken coin {Time.frameCount}");      
		}
	}

    public void OnCoinBettingPhaseStart(RoundInfo roundInfo)
	{
        if (IsDisabled) return;
        if (isMainPlayer) selectedCard.gameObject.SetActive(false);
        symbolKeyboard.SetVisible(false, true);
        responsePanel.SetCoinZonesVisible(true, animate: true);
        activePlayerIndicator.SetActive(true);
        symbolKeyboard.Interactable = false;
        responsePanel.CoinZoneHighlighted = isMainPlayer;

        SetCoinReposVisible(isMainPlayer);

        SetInstructionText(COIN_BETTING_INSTRUCTION);
        CoinBettingStarted?.Invoke(roundInfo);
        // enable coin zones
        responsePanel.SetCoinZoneInteractable(true);

        

        IsValidated = false;
	}

    public IEnumerator LockResponseTurn()
	{
        activePlayerIndicator.SetActive(true);

        oppositeBoardSymbolClicked = false;

        bool isPositiveDependency = roundInfo.gameConfig.ActionDependency == Dependency.Positive;

        SetInstructionText(isPositiveDependency ? UNLOCK_SYMBOL_INSTRUCTION : LOCK_SYMBOL_INSTRUCTION);

        oppositeBoard.ResponsePanel.SetSymbolsInteractable(true, allowInteractIfLocked: true,
            onlyNonPickedOrLockedSymbols: !isPositiveDependency,
            onlyLocked: isPositiveDependency);

        oppositeBoard.SymbolKeyboard.ResetSelection();
        oppositeBoard.ResponseSymbolClicked += OnOppositeBoardSymbolClicked;

        LockResponseTurnStarted?.Invoke(roundInfo);

        if(isPositiveDependency)
		{
            oppositeBoard.ResponsePanel.LockedCardsHighlighted = isMainPlayer;
        }
        else
		{
            oppositeBoard.ResponsePanel.EmptyCardsHighlighted = isMainPlayer;
        }
        

        yield return new WaitUntil(() => oppositeBoardSymbolClicked);

        oppositeBoard.ResponseSymbolClicked -= OnOppositeBoardSymbolClicked;

        oppositeBoard.ResponsePanel.SetSymbolsInteractable(false);

        activePlayerIndicator.SetActive(false);

        oppositeBoard.ResponsePanel.EmptyCardsHighlighted = false;

        oppositeBoard.ResponsePanel.LockedCardsHighlighted = false;

        SetInstructionText(TURN_END_INSTRUCTION);

    }

    public IEnumerator SymbolPickResponseTurn()
    {

        SetInstructionText(ASSIGN_SYMBOL_INSTRUCTION);

        activePlayerIndicator.SetActive(true);
        symbolPickedCount = 0; // WIP hack
        turnCount++;
        symbolKeyboard.Highlighted = isMainPlayer;
        symbolKeyboard.SymbolSelected += OnKeyboardFirstSelect;

        symbolKeyboard.Interactable = isMainPlayer;

        ResponsePanel.SetSymbolsInteractable(true, onlyNonPickedOrLockedSymbols: true);

        SymbolResponseTurnStarted?.Invoke(roundInfo);

        symbolClicked = false;

        yield return new WaitUntil(() => symbolClicked);

        activePlayerIndicator.SetActive(false);
        SetInstructionText(TURN_END_INSTRUCTION);
        symbolKeyboard.Interactable = false;
        responsePanel.SetSymbolsInteractable(false);
        if (isMainPlayer) selectedCard.gameObject.SetActive(false);
    }

    private void OnOppositeBoardSymbolClicked(ResponseColumn column)
	{
        oppositeBoardSymbolClicked = true;
        column.SetLocked(roundInfo.gameConfig.ActionDependency == Dependency.Negative, playSound: true);
    }

    public void OnCoinBettingEnd()
    {
        if (IsDisabled) return;
        activePlayerIndicator.SetActive(false);
        SetCoinReposVisible(false);
        foreach (var coinRepo in GetComponentsInChildren<CoinRepository>(true))
        {
            coinRepo.Interacted -= OnCoinRepoClicked;
        }

        SetInstructionText(EMPTY);
        //responsePanel.ShowCorrectFeedback(roundInfo.correctIndexSequence);

        SetInteractable(false);
    }

    public void OnResponsePhaseStart(RoundInfo roundInfo)
	{
        if (IsDisabled) return;
        activePlayerIndicator.SetActive(true);
        SetInstructionText(STIMULUS_RESPONSE_INSTRUCTION);
        symbolKeyboard.Highlighted = isMainPlayer;
        symbolKeyboard.SymbolSelected += OnKeyboardFirstSelect;
        responsePanel.SetSymbolsInteractable(true);

        symbolKeyboard.Interactable = isMainPlayer;

        ResponsePhaseStarted?.Invoke(roundInfo);
    }

    private void OnKeyboardFirstSelect()
	{
        symbolKeyboard.Highlighted = false;
        responsePanel.EmptyCardsHighlighted = isMainPlayer;
    }

    public void OnValidateButtonClick()
    {
        SoundManager.Instance.PlaySound(SoundType.GenericClick);
        SetValidated(true);
    }


    public IEnumerator ShowFeedback(RoundInfo roundInfo)
    {
        if (IsDisabled) yield break;
        for (int i = 0; i < responsePanel.Columns.Count; i++)
        {
            yield return new WaitForSeconds(0.3f);
            responsePanel.Columns[i].ShowCorrectFeedback(responsePanel.Columns[i].SymbolIndex == roundInfo.correctIndexSequence[i]);
        }
    }

    public IEnumerator OnRoundEnd(RoundInfo roundInfo)
	{
        if (IsDisabled) yield break;
        responsePanel.SetCoinZonesVisible(false, true);
        //symbolKeyboard.SetVisible(false, true);
        responsePanel.SetColumnsVisible(false, true);
        yield return new WaitForSeconds(1.0f);
        for (int i = 0; i < responsePanel.Columns.Count; i++)
        {
            responsePanel.Columns[i].Cleanup();
        }
        
        //if (isMainPlayer) SetStartRoundButtonVisible(true);
        
    }

    public bool AllSymbolsAreCorrect(RoundInfo roundInfo)
	{
        return responsePanel.GetCorrectColumns(roundInfo.correctIndexSequence).Count() == roundInfo.gameConfig.sequenceLength;
    }


    public int ComputeRawRoundScore(RoundInfo roundInfo)
    {
        int round_score = 0;

        foreach (var column in responsePanel.GetCorrectColumns(roundInfo.correctIndexSequence))
        {
            //round_score += (1 + column.CoinCount);
            round_score += (1 + column.CoinValueSum);
        }

        return round_score;
    }
    
    public void OnResponseSymbolPickAttempted(ResponseColumn column) // rename this !
	{
        ResponseSymbolClicked?.Invoke(column);

        int? selectedSymbolIndex = SelectedSymbolIndex; // this needs to work for the bot as well; bot should use the keyboard

        if (selectedSymbolIndex != null)
		{
            symbolPickedCount++;

            if (symbolPickedCount == 1)
            {
                responsePanel.EmptyCardsHighlighted = false;
                symbolKeyboard.SymbolSelected -= OnKeyboardFirstSelect;
            }

            column.SetSymbol((int)selectedSymbolIndex, canStillInteract: roundInfo.gameConfig.ActionDependency == Dependency.None);

            if(isMainPlayer) selectedCard.gameObject.SetActive(false);

            symbolClicked = true;

            symbolKeyboard.ResetSelection();
        }

        // else, we are locking or unlocking a column; TO DO: refactor this

        bool canValidate = roundInfo.gameConfig.ActionDependency == Dependency.Negative ? responsePanel.AllColumnsPickedOrLocked : responsePanel.AllSymbolsPicked;

        //Debug.Log($"can validate : {canValidate}");
        if(roundInfo.gameConfig.ActionDependency == Dependency.None) SetCanValidate(canValidate);
        else if (canValidate) SetValidated(true);  
    }

    public void SetCanValidate(bool canValidate)
	{
        if (canValidate) SetInstructionText(EMPTY);
        validateButton?.gameObject.SetActive(canValidate);
	}

    public void WIP_OnResponseColumnAddCoinClicked(ResponseColumn column)
    {
        //if (column.CoinCount >= GameConfig.maxCoinPerSymbol) return;

        //if (coinCounter.CoinCount <= 0) return;

        responsePanel.CoinZoneHighlighted = false;

        //coinCounter.RemoveCoin();

        //column.AddCoin();

        bool canValidate = GameConfig.sequenceLength == responsePanel.CoinsInColumns;

        //Debug.Log($"coin count = {responsePanel.CoinsInColumns}");
        SetCanValidate(canValidate);

        SetInstructionText(canValidate ? EMPTY : COIN_BETTING_INSTRUCTION);

    }

    public void SetValidated(bool validated)
	{
        IsValidated = validated;
        validateButton?.gameObject.SetActive(false);
	}

    public void WIP_OnResponseColumnRemoveCoinClicked(ResponseColumn column)
    {
        //if (column.CoinCount <= 0) return;

        //coinCounter.AddCoin();

        //column.RemoveCoin();

        bool canValidate = GameConfig.ActionDependency == Dependency.Negative ? true : GameConfig.sequenceLength == responsePanel.CoinsInColumns;
        SetCanValidate(canValidate);

        SetInstructionText(canValidate ? EMPTY : COIN_BETTING_INSTRUCTION);
    }

    public void SetInteractable(bool interactable)
    {
        if (!isMainPlayer) interactable = false; // dirty, refactor at some point!

        symbolKeyboard.Interactable = interactable;

        responsePanel.SetCoinZoneInteractable(interactable);
        responsePanel.SetSymbolsInteractable(interactable);
    }

    public void IncrementScore(int value)
    {
        if (IsDisabled) return;

        SetScore(score + value);
        SoundManager.Instance.PlaySound(SoundType.GainPoints);
    }

    public void SetScore(int value, bool animate = true)
    {
        score = value;
        scoreCounter.SetScore(value, animate);  
    }
}
