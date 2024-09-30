using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CoinRepository : MonoBehaviour
{
    [SerializeField] private int maxCoinCount;
    [SerializeField] private bool acceptsAllCoinTypes;
    [SerializeField] private CoinType coinType;
    public bool AcceptsAllCoinTypes => acceptsAllCoinTypes;
    public CoinType CoinType => coinType;
    public event System.Action<CoinRepository, bool> Clicked;
    //public event System.Action CoinDeposited;
    private readonly List<Coin> coins = new();
    public int CoinCount => coins.Count;
    public int TotalCoinValue => coins.Sum(c => c.Value);
    private int lastFrameClick;

    public void AddCoin(Coin coin, bool fromInit = false)
	{
        coins.Add(coin);
        coin.transform.SetParent(this.transform);
        if(!fromInit)
		{
            SoundManager.Instance.PlaySound(SoundType.AddCoin);
        }
    }

    public bool CanAcceptCoin(Coin coin) => CoinCount < maxCoinCount && (acceptsAllCoinTypes || coinType == coin.CoinType);

	private void Cleanup()
	{
		foreach(var coin in coins)
		{
            Destroy(coin.gameObject);
		}

        coins.Clear();

        foreach(var coin in GetComponentsInChildren<Coin>())
		{
            Destroy(coin.gameObject);
        }
	}

    public void Initialize(IEnumerable<Coin> startingCoins = null)
	{
        Cleanup();

        if (startingCoins != null)
		{
            foreach (var coin in startingCoins)
            {
                AddCoin(coin, true);
            }
        }    
	}

	public Coin TakeLastCoin()
	{
        if (coins.Count <= 0) throw new System.Exception("No coin to take !");
        Coin coin = coins.Last();
        coins.RemoveAt(coins.Count-1);
        SoundManager.Instance.PlaySound(SoundType.RemoveCoin);
        return coin;
	}

    private void OnClick(bool isDrag)
	{
        if(lastFrameClick == Time.frameCount)
		{
            Debug.Log($"same frame {Time.frameCount}");
            return;
		}
        lastFrameClick = Time.frameCount;
        Debug.Log($"Clicked {Time.frameCount} {gameObject.name}");
        
        Clicked?.Invoke(this, isDrag);
    }

    public void OnPointerDown()
	{
        Debug.Log($"PointerDown {Time.frameCount} {gameObject.name}");
        OnClick(false);
    }

    public void OnEndDrag()
	{
        Debug.Log($"Drop {Time.frameCount} {gameObject.name}");
        OnClick(true);
    }
}
