using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResponseColumn : MonoBehaviour
{
	public int? SymbolIndex { get; private set; }
	private List<Sprite> cardShapes;
	[SerializeField] private Image symbolImage;
	[SerializeField] private Image checkImage;
	[SerializeField] private Image coverImage;
	[SerializeField] private Transform coinContainer;
	[SerializeField] private GameObject coinPrefab;
	private List<GameObject> coins;
	public int CoinCount { get; private set; }

	public void Initialize(List<Sprite> cardShapes)
	{
		SymbolIndex = null;
		this.cardShapes = cardShapes;
		checkImage.enabled = false;
		SetCoverVisible(false);

		Cleanup();

		coins = new List<GameObject>(); // in cleanup ?
	}

	public void AddCoin(int amount = 1)
	{
		CoinCount += amount;
		for(int i = 0; i<amount;i++)
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
		JWMGameController.Instance.WIP_OnResponseColumnAddCoinClicked(this);
	}

	public void OnRemoveCoinButtonClick()
	{
		JWMGameController.Instance.WIP_OnResponseColumnRemoveCoinClicked(this);
	}

	private void Cleanup()
	{
		for (int i = 0; i < coinContainer.childCount; i++)
		{
			Destroy(coinContainer.GetChild(i).gameObject);
		}
	}

	public void SetCoverVisible(bool visible)
	{
		coverImage.enabled = visible;
	}

	public void ShowCorrectFeedback(bool isCorrect)
	{
		checkImage.enabled = isCorrect;
	}

	public void OnSymbolButtonClick()
	{
		// if symbolkeyboard has non-null selectedSymbolIndex, set this column symbolIndex and update the symbol icon
		// then reset symbolkeyboard (set selectedSymbolIndex to null)

		JWMGameController.Instance.WIP_OnResponseColumnSymbolClicked(this);
	}

	public void SetSymbol(int symbolIndex)
	{
		symbolImage.sprite = cardShapes[symbolIndex];
		this.SymbolIndex = symbolIndex;
	}
}
