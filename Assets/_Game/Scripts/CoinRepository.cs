using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Toolbox;

public class CoinRepository : MonoBehaviour
{
    [SerializeField] private int maxCoinCount;
    [SerializeField] private bool acceptsAllCoinTypes;
    [SerializeField] private CoinType coinType;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private AnimationCurve fadeCurve;
    [SerializeField] private ButtonHighlighter highlight;
    public bool AcceptsAllCoinTypes => acceptsAllCoinTypes;
    public CoinType CoinType => coinType;
    public event System.Action<CoinRepository, bool> Interacted;
    //public event System.Action CoinDeposited;
    private readonly List<Coin> coins = new();
    public int CoinCount => coins.Count;
    public int CoinValueSum => coins.Sum(c => c.Value);
    public int MaxCointCount => maxCoinCount;
    public int TotalCoinValue => coins.Sum(c => c.Value);
    private int lastFrameClick;
    public event System.Action CoinAdded;
    public event System.Action CoinRemoved;

    public bool Highlighted
    {
        set
        {
            highlight.gameObject.SetActive(value);
        }
    }

    public bool Interactable { get; set; }

    public void FadeToVisible(bool visible)
    {
        float startAlpha = visible ? 0f : 1f;
        float targetAlpha = visible ? 1f : 0f;
        StartCoroutine(CoroutineTools.Tween(startAlpha, targetAlpha, 0.7f, t => canvasGroup.alpha = fadeCurve.Evaluate(t)));
    }

    public void AddCoin(Coin coin, bool fromInit = false)
	{
        coins.Add(coin);
        coin.transform.SetParent(this.transform);
        if(!fromInit)
		{
            SoundManager.Instance.PlaySound(SoundType.AddCoin);
            CoinAdded?.Invoke();
        }      
    }

    public bool CanAcceptCoinOfType(CoinType coinType) => (acceptsAllCoinTypes || this.coinType == coinType);

	private void Cleanup()
	{
		foreach(var coin in coins)
		{
            Destroy(coin.gameObject);
		}

        coins.Clear();

        foreach(var coin in GetComponentsInChildren<Coin>(includeInactive: true))
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
        CoinRemoved?.Invoke();
        return coin;
	}

    private void OnInteract(bool isDrag)
	{
        if(lastFrameClick == Time.frameCount)
		{
            //Debug.Log($"same frame {Time.frameCount}");
            return;
		}
        lastFrameClick = Time.frameCount;
        //Debug.Log($"Clicked {Time.frameCount} {gameObject.name}");
        
        Interacted?.Invoke(this, isDrag);
    }

    public void OnPointerDown()
	{
        //Debug.Log($"PointerDown {Time.frameCount} {gameObject.name}");
        OnInteract(false);
    }

    public void OnEndDrag()
	{
        //Debug.Log($"Drop {Time.frameCount} {gameObject.name}");
        OnInteract(true);
    }
}
