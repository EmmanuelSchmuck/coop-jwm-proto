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
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    
    public ResponsePanel ResponsePanel => responsePanel;
    public SymbolKeyboard SymbolKeyboard => symbolKeyboard;
    public CoinCounter CoinCounter => coinCounter;
    private int score;
    public JWMGameConfig GameConfig { get; private set; }
    public int? SelectedSymbolIndex => symbolKeyboard.SelectedSymbolIndex;

    public event System.Action ResponseValidated;
    public event System.Action StartRoundButtonClicked;
    public event System.Action<RoundInfo> RoundStarted;

    public void Initialize(List<Sprite> cardShapePool)
	{
        symbolKeyboard.Initialize(cardShapePool);
        SetScore(0);
	}
    public void OnResponseValidated()
	{
        ResponseValidated?.Invoke();
    }

    public void OnRoundStart(List<Sprite> cardShapePool, RoundInfo roundInfo, bool isFirstRound)
	{
        this.GameConfig = roundInfo.gameConfig;

        symbolKeyboard.ResetSelection();

        responsePanel.Initialize(GameConfig.sequenceLength, cardShapePool, this);

        coinCounter.SetCoin(GameConfig.coinPerRound);

        if(isFirstRound && DEBUG_isHumanPlayer)
		{
            responsePanel.SetStartRoundButtonVisible(true);
        }

        RoundStarted?.Invoke(roundInfo);
    }

    public void OnRoundEnd(int[] correctIndexSequence, int scoreMultiplier)
	{
        int round_score = 0;

        foreach (var column in responsePanel.GetCorrectColumns(correctIndexSequence))
        {
            round_score += (1 + column.CoinCount) * scoreMultiplier;
        }

        IncrementScore(round_score);
    }

    public void WIP_OnStartRoundButtonClick()
    {
        responsePanel.SetStartRoundButtonVisible(false);

        StartRoundButtonClicked?.Invoke();
    }

    public void WIP_OnResponseColumnAddCoinClicked(ResponseColumn column)
    {
        if (column.CoinCount >= GameConfig.maxCoinPerSymbol) return;

        if (coinCounter.CoinCount <= 0) return;

        coinCounter.RemoveCoin();

        column.AddCoin();

        responsePanel.CheckIfCanValidate(GameConfig.coinPerRound);

    }

    public void WIP_OnResponseColumnRemoveCoinClicked(ResponseColumn column)
    {
        if (column.CoinCount <= 0) return;

        coinCounter.AddCoin();

        column.RemoveCoin();

        responsePanel.CheckIfCanValidate(GameConfig.coinPerRound);
    }

    public void SetInteractable(bool interactable)
    {
        symbolKeyboard.Interactable = interactable;

        responsePanel.SetInteractable(interactable);
    }

    public void IncrementScore(int value) => SetScore(score + value);

    public void SetScore(int value)
    {
        score = value;
        scoreText.text = $"Score: {score}";
    }
}