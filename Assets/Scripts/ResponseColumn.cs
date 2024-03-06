using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResponseColumn : MonoBehaviour // IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public int? SymbolIndex { get; private set; }
	private List<Sprite> cardShapes;
	[SerializeField] private Image symbolImage;
	[SerializeField] private Image checkImage;
	[SerializeField] private Image coverImage;
	[SerializeField] private Transform coinContainer;
	[SerializeField] private GameObject coinPrefab;
	[SerializeField] private Transform coinButtons;
	[SerializeField] private Sprite questionMarkSymbol;
	private CoinZone coinZone;
	private List<GameObject> coins;
	private ResponsePanel responsePanel;
	public int CoinCount { get; private set; }
	public bool Interactable {
		get => interactable;
		set
		{
			interactable = value;
			coinZone.Interactable = value;
		}
	}
	private bool interactable;

	public void Initialize(List<Sprite> cardShapes, ResponsePanel responsePanel)
	{
		this.responsePanel = responsePanel;
		SymbolIndex = null;
		this.cardShapes = cardShapes;
		checkImage.enabled = false;
		SetCoverVisible(false);
		SetCoinButtonsVisible(false);
		coinZone = GetComponentInChildren<CoinZone>();
		coinZone.Initialize(this);

		coins = new List<GameObject>();

		Cleanup();
	}

	public void SetCoinButtonsVisible(bool visible)
	{
		coinButtons.gameObject.SetActive(visible);
	}

	public void AddCoin(int amount = 1)
	{
		CoinCount += amount;
		for(int i = 0; i<amount;i++)
		{
			coins.Add(Instantiate(coinPrefab, coinContainer));
		}
		
	}

	public void SetCoins(int amount) // to do: refactor this + add & remove coin
	{
		foreach(var coin in coins)
		{
			Destroy(coin);
		}

		coins.Clear();

		CoinCount = amount;
		for (int i = 0; i < amount; i++)
		{
			coins.Add(Instantiate(coinPrefab, coinContainer));
		}
	}

	public void RemoveCoin()
	{
		if (coins.Count <= 0) throw new System.Exception("No coin to remove !");
		GameObject coin = coins[0];
		coins.RemoveAt(0);
		Destroy(coin);
		CoinCount--;
	}

	public void OnAddCoinButtonClick()
	{
		if (!Interactable) return;

		responsePanel.OnResponseColumnAddCoin(this);
	}

	public void OnRemoveCoinButtonClick()
	{
		if (!Interactable) return;

		responsePanel.OnResponseColumnRemoveCoin(this);
	}

	private void Cleanup()
	{
		for (int i = 0; i < coinContainer.childCount; i++)
		{
			Destroy(coinContainer.GetChild(i).gameObject);
		}

		ShowCorrectFeedback(false);

		SetSymbol(null);

		SetCoins(0);	
	}

	public void SetCoverVisible(bool visible)
	{
		coverImage.enabled = visible;
	}

	public void ShowCorrectFeedback(bool isCorrect)
	{
		checkImage.enabled = isCorrect;
	}

	public void OnCoinZoneHoverEnter()
	{
		responsePanel.OnColumnHoverEnter(this);
	}

	public void OnCoinZoneHoverLeave()
	{
		responsePanel.OnColumnHoverLeave(this);
	}


	public void OnSymbolButtonClick()
	{
		if (!Interactable) return;

		// if symbolkeyboard has non-null selectedSymbolIndex, set this column symbolIndex and update the symbol icon
		// then reset symbolkeyboard (set selectedSymbolIndex to null)

		responsePanel.WIP_OnResponseColumnSymbolClicked(this);
	}

	public void SetSymbol(int? symbolIndex)
	{
		symbolImage.sprite = symbolIndex == null ? questionMarkSymbol : cardShapes[(int)symbolIndex];
		this.SymbolIndex = symbolIndex;
	}
}
