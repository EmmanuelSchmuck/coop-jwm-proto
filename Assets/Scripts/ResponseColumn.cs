using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Toolbox;

public class ResponseColumn : MonoBehaviour
{
	public int? SymbolIndex { get; private set; }
	public int ColumnIndex { get; private set; }

	[SerializeField] private Check check;

	[SerializeField] private Transform coinContainer;
	[SerializeField] private GameObject coinPrefab;
	[SerializeField] private Transform coinButtons;
	[SerializeField] private StimulusCard card;
	[SerializeField] private GameObject DEBUG_lock;
	private CoinZone coinZone;
	private List<GameObject> coins;
	private ResponsePanel responsePanel;
	public bool IsLocked { get; private set; }
	public int CoinCount { get; private set; }
	public bool CoinZoneInteractable {
		get => coinZoneInteractable;
		set
		{
			coinZoneInteractable = value;
			coinZone.Interactable = value;
		}
	}
	private bool coinZoneInteractable;
	public bool SymbolInteractable
	{
		get => symbolInteractable;
		set
		{
			symbolInteractable = value;
		}
	}
	private bool symbolInteractable;

	public void Initialize(ResponsePanel responsePanel, int columnIndex)
	{
		this.ColumnIndex = columnIndex;
		this.responsePanel = responsePanel;
		SymbolIndex = null;
		card.Initialize(null);
		check.Hide();
		SetCoverVisible(false);
		SetCoinButtonsVisible(false);
		coinZone = GetComponentInChildren<CoinZone>();
		coinZone.Initialize(this);

		coins = new List<GameObject>();

		Cleanup();
	}

	public void SetLocked(bool isLocked = true)
	{
		this.IsLocked = isLocked;
		DEBUG_lock.SetActive(isLocked);
		//if (IsLocked) Interactable = false;
	}

	public void SetCoinButtonsVisible(bool visible)
	{
		// for now, keep buttons hidden, we dont use them
		coinButtons.gameObject.SetActive(false);
	}

	public void AddCoin(int amount = 1)
	{
		CoinCount += amount;
		for(int i = 0; i<amount;i++)
		{
			GameObject coin = Instantiate(coinPrefab, coinContainer);
			coins.Add(coin);
		}
		SoundManager.Instance.PlaySound(SoundType.AddCoin);

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
		SoundManager.Instance.PlaySound(SoundType.RemoveCoin);
	}

	public void OnAddCoinButtonClick()
	{
		if (!coinZoneInteractable) return;

		responsePanel.OnResponseColumnAddCoin(this);
	}

	public void OnRemoveCoinButtonClick()
	{
		if (!coinZoneInteractable) return;

		responsePanel.OnResponseColumnRemoveCoin(this);
	}

	public void Cleanup()
	{
		for (int i = 0; i < coinContainer.childCount; i++)
		{
			Destroy(coinContainer.GetChild(i).gameObject);
		}
		card.Initialize(null);
		SetLocked(false);
		check.Hide();

		//SetCoverVisible(true);

		SetCoins(0);	
	}

	public void SetCoverVisible(bool visible) // change this!
	{
		card.SetVisible(!visible);
	}

	public void ShowCorrectFeedback(bool isCorrect)
	{
		check.Show(isCorrect);
		SoundManager.Instance.PlaySound(isCorrect ? SoundType.FeedbackCorrect : SoundType.FeedbackError);
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
		if (!symbolInteractable || IsLocked) return;

		// if symbolkeyboard has non-null selectedSymbolIndex, set this column symbolIndex and update the symbol icon
		// then reset symbolkeyboard (set selectedSymbolIndex to null)

		responsePanel.WIP_OnResponseColumnSymbolClicked(this);
	}

	public void SetSymbol(int symbolIndex)
	{
		card.Initialize(symbolIndex, animate: true);

		SoundManager.Instance.PlaySound(SoundType.SetSymbol);

		this.SymbolIndex = symbolIndex;

		SymbolInteractable = false;
	}
}
