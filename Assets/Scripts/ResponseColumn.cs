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
	[SerializeField] private SymbolCard symbolCard;
	[SerializeField] private GameObject DEBUG_lock;
	[SerializeField] private Button symbolButton;
	[SerializeField] private CanvasGroup canvasGroup;
	//[SerializeField] private CoinRepository coinRepo;
	[SerializeField] private AnimationCurve fadeCurve;
	private CoinZone coinZone;
	private List<GameObject> coins;
	private ResponsePanel responsePanel;
	public bool IsLocked { get; private set; }
	public bool IsPickedOrLocked => IsLocked || SymbolIndex != null;
	public int CoinCount => coinZone.CoinCount;
	public bool CoinZoneInteractable {
		get => coinZoneInteractable;
		set
		{
			coinZoneInteractable = value;
			coinZone.Interactable = value;
		}
	}
	public bool CoinZoneHighlighted
	{
		set => coinZone.Highlighted = value; // coinZone.Highlighted = value;
	}
	public bool SymbolHighlighted
	{
		set => symbolCard.Highlighted = value;
	}
	private bool coinZoneInteractable;
	public bool SymbolInteractable
	{
		get => symbolInteractable;
		set
		{
			symbolButton.gameObject.SetActive(value);
			symbolInteractable = value;
		}
	}
	private bool symbolInteractable;
	public bool AllowInteractionIfLocked;

	public void Initialize(ResponsePanel responsePanel, int columnIndex)
	{
		this.ColumnIndex = columnIndex;
		this.responsePanel = responsePanel;
		SymbolIndex = null;
		symbolCard.Initialize(null);
		
		check.Hide();
		SetCoverVisible(false);
		SetCoinButtonsVisible(false);
		coinZone = GetComponentInChildren<CoinZone>();
		coinZone.Initialize(this);

		symbolCard.SetEmpty();

		coins = new List<GameObject>();

		Cleanup();
	}

	public void SetCoinZoneVisible(bool visible, bool animate = false)
	{
		//return;

		coinZone.gameObject.SetActive(visible);
		if(animate)
		{
			coinZone.gameObject.SetActive(true);
			coinZone.FadeToVisible(visible);
		}
		else
		{
			coinZone.gameObject.SetActive(visible);
		}
	}

	private void FadeToVisible(bool visible)
	{
		float startAlpha = visible ? 0f : 1f;
		float targetAlpha = visible ? 1f : 0f;
		StartCoroutine(CoroutineTools.Tween(startAlpha, targetAlpha, 0.7f, t => canvasGroup.alpha = fadeCurve.Evaluate(t)));
	}

	public void SetVisible(bool visible, bool animate = false)
	{
		//this.gameObject.SetActive(visible);
		if (animate)
		{
			this.gameObject.SetActive(true);
			this.FadeToVisible(visible);
		}
		else
		{
			this.gameObject.SetActive(visible);
		}
	}

	public void SetLocked(bool isLocked = true, bool playSound = false)
	{
		this.IsLocked = isLocked;

		symbolCard.SetLocked(isLocked);

		if(playSound)
		{
			SoundManager.Instance.PlaySound(isLocked? SoundType.Lock : SoundType.Unlock);
		}
	}

	public void SetCoinButtonsVisible(bool visible)
	{
		// for now, keep buttons hidden, we dont use them
		coinButtons.gameObject.SetActive(false);
	}

	public void AddCoin(Coin coin)
	{
		coinZone.AddCoin(coin);
	}

	public void SetCoins(int amount) // to do: refactor this + add & remove coin
	{
		foreach(var coin in coins)
		{
			Destroy(coin);
		}

		coins.Clear();

		//CoinCount = amount;
		for (int i = 0; i < amount; i++)
		{
			coins.Add(Instantiate(coinPrefab, coinContainer));
		}
	}

	//public void RemoveCoin()
	//{
	//	if (coins.Count <= 0) throw new System.Exception("No coin to remove !");

	//	GameObject coin = coins[0];
	//	coins.RemoveAt(0);
	//	Destroy(coin);
	//	//CoinCount--;
	//	SoundManager.Instance.PlaySound(SoundType.RemoveCoin);
	//}

	public void FadeCoins()
	{
		foreach (var coin in coins)
		{
			coin.GetComponent<CanvasGroup>().alpha = 0.2f;
		}
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
		foreach(var coin in coinContainer.GetComponentsInChildren<Coin>())
		{
			Destroy(coin.gameObject);
		}
		//for (int i = 0; i < coinContainer.childCount; i++)
		//{
		//	Destroy(coinContainer.GetChild(i).gameObject);
		//}
		symbolCard.Initialize(null);
		SetLocked(false);
		check.Hide();

		//SetCoverVisible(true);

		SetCoins(0);	
	}

	public void SetCoverVisible(bool visible) // change this!
	{
		symbolCard.SetVisible(!visible);
	}

	public void ShowCorrectFeedback(bool isCorrect)
	{
		check.Show(isCorrect);
		SoundManager.Instance.PlaySound(isCorrect ? SoundType.FeedbackCorrect : SoundType.FeedbackError);
		if(!isCorrect)
		{
			FadeCoins();
		}
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
		if (!symbolInteractable || IsLocked && !AllowInteractionIfLocked) return;

		// if symbolkeyboard has non-null selectedSymbolIndex, set this column symbolIndex and update the symbol icon
		// then reset symbolkeyboard (set selectedSymbolIndex to null)

		responsePanel.WIP_OnResponseColumnSymbolClicked(this);
	}

	public void SetSymbol(int symbolIndex, bool canStillInteract = false)
	{
		symbolCard.Initialize(symbolIndex, animate: true);

		symbolCard.SetBaseColor();

		SoundManager.Instance.PlaySound(SoundType.SetSymbol);

		this.SymbolIndex = symbolIndex;

		SymbolInteractable = canStillInteract;
	}
}
