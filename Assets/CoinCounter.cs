using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinCounter : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI amountText;
    public int CoinCount { get; private set; }
	private void Start()
	{
		Initialize();
	}
	public void Initialize()
	{
		UpdateText();
	}
	private void UpdateText()
	{
		amountText.text = CoinCount.ToString();
	}
	public void RemoveCoin(int amount = 1)
	{
		if (CoinCount - amount < 0) throw new System.Exception($"Not enough coins to remove {amount} coin(s)!");
		CoinCount -= amount;
		UpdateText();
	}
    public void AddCoin(int amount = 1)
	{
		CoinCount += amount;
		UpdateText();
	}
}
