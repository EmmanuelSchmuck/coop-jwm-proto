using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ResponsePanel : MonoBehaviour
{
    [SerializeField] private ResponseColumn columnPrefab;


	public bool SymbolsHighlighted
	{
		set
		{
			foreach (var col in columns)
			{
				// if IsPickedOrLocked, do not highlight, but stil allow un-highlight
				col.SymbolHighlighted = value && !col.IsPickedOrLocked; 
			}
		}
	}

	public bool CoinZoneHighlighted
	{
		set
		{
			foreach (var col in columns)
			{
				col.CoinZoneHighlighted = value;
			}
		}
	}

	public bool AllSymbolsPicked => columns.All(c => c.SymbolIndex != null);
	public bool AllColumnsLocked => columns.All(c => c.IsLocked);
	public bool AllColumnsPickedOrLocked => columns.All(c => c.IsPickedOrLocked);
	public int CoinsInColumns => columns.Sum(c => c.CoinCount);

	private List<ResponseColumn> columns;
	public List<ResponseColumn> Columns => columns;
	
	private ResponseColumn hoveredColumn;
	private PlayerBoard board;
    
    public void Initialize(int sequenceLength, PlayerBoard board)
	{
		Cleanup();

		columns = new List<ResponseColumn>();

		this.board = board;

		for (int i = 0; i < sequenceLength; i++)
		{
			ResponseColumn column = Instantiate(columnPrefab, this.transform);
			columns.Add(column);
			column.Initialize(this, i);
		}
	}

	public void WIP_OnResponseColumnSymbolClicked(ResponseColumn column)
	{
		board.OnResponseSymbolPickAttempted(column);
	}

	public void OnResponseColumnAddCoin(ResponseColumn column)
	{
		board.WIP_OnResponseColumnAddCoinClicked(column);
	}

	public void OnResponseColumnRemoveCoin(ResponseColumn column)
	{
		board.WIP_OnResponseColumnRemoveCoinClicked(column);
	}

	public void OnColumnHoverEnter(ResponseColumn column)
	{
		hoveredColumn?.SetCoinButtonsVisible(false);
		hoveredColumn = column;
		hoveredColumn.SetCoinButtonsVisible(true);
	}

	public void OnColumnHoverLeave(ResponseColumn column)
	{
		column.SetCoinButtonsVisible(false);
	}

	public void SetSymbolsInteractable(bool interactable, bool onlyNonPickedSymbols = false)
	{
		foreach(var column in columns)
		{
			if (onlyNonPickedSymbols && column.SymbolIndex != null) continue;

			column.SymbolInteractable = interactable;
		}
	}
	public void SetCoinZoneInteractable(bool interactable) // only affect columns, not other buttons
	{
		foreach (var column in columns)
		{
			column.CoinZoneInteractable = interactable;
		}
	}

	public void OnStartRoundButtonClick()
	{
		board.WIP_OnStartRoundButtonClick();
	}


	//public void ShowCorrectFeedback(int[] correctIndices)
	//{
	//	for(int i = 0;i<columns.Count;i++)
	//	{
	//		columns[i].ShowCorrectFeedback(columns[i].SymbolIndex == correctIndices[i]);
	//	}
	//}

	public IEnumerable<ResponseColumn> GetCorrectColumns(int[] correctIndices)
	{
		return columns.Where((c, i) => c.SymbolIndex == correctIndices[i]);
	}


	public void AddCoinsInColumn(int amount, int columnIndex)
	{
		columns[columnIndex].AddCoin(amount);
	}

	public void SetColumnLocked(int columnIndex)
	{
		columns[columnIndex].SetLocked();
	}

	public void SetSymbolInColumn(int symbolIndex, int columnIndex)
	{
		columns[columnIndex].SetSymbol(symbolIndex);
	}

	private void Cleanup()
	{
		hoveredColumn = null;

		foreach (var column in transform.GetComponentsInChildren<ResponseColumn>())
		{
			Destroy(column.gameObject);
		}
	}
}
