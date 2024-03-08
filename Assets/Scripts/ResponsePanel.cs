using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ResponsePanel : MonoBehaviour
{
    [SerializeField] private ResponseColumn columnPrefab;
	[SerializeField] private Button validateButton;
	[SerializeField] private Button startRoundButton;

	public bool IsValidated { get; private set; }

	private List<ResponseColumn> columns;
	
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
			column.Initialize(this);
		}
	}

	public void WIP_OnResponseColumnSymbolClicked(ResponseColumn column)
	{
		// assume this is from player A

		int? selectedSymbolIndex = board.SelectedSymbolIndex;

		if (selectedSymbolIndex == null) return;

		column.SetSymbol((int)selectedSymbolIndex);

		CheckIfCanValidate(board.GameConfig.coinPerRound);

		// playerA_Keyboard.ResetSelection();

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

	public void SetStartRoundButtonVisible(bool visible)
	{
		startRoundButton.gameObject.SetActive(visible);
	}

	public void SetInteractable(bool interactable) // only affect columns, not other buttons
	{
		foreach(var column in columns)
		{
			column.Interactable = interactable;
		}
	}

	public void OnStartRoundButtonClick()
	{
		board.WIP_OnStartRoundButtonClick();
	}

	public void OnValidateButtonClick()
	{
		SetValidated();
	}

	public void SetValidated()
	{
		IsValidated = true;
		validateButton.gameObject.SetActive(false);
		board.OnResponseValidated();
	}

	public void ShowCorrectFeedback(int[] correctIndices)
	{
		for(int i = 0;i<columns.Count;i++)
		{
			columns[i].ShowCorrectFeedback(columns[i].SymbolIndex == correctIndices[i]);
		}
	}

	public IEnumerable<ResponseColumn> GetCorrectColumns(int[] correctIndices)
	{
		return columns.Where((c, i) => c.SymbolIndex == correctIndices[i]);
	}

	public void CheckIfCanValidate(int coinPerRound)
	{
		bool allColumnsFullOfCoins = columns.Select(c => c.CoinCount).Sum() == coinPerRound;
		bool allSymbolsChosen = columns.All(c => c.SymbolIndex != null);

		bool canValidate = allColumnsFullOfCoins && allSymbolsChosen;

		validateButton.gameObject.SetActive(canValidate);
	}

	public void AddCoinsInColumn(int amount, int columnIndex)
	{
		columns[columnIndex].AddCoin(amount);
	}

	public void SetSymbolInColumn(int symbolIndex, int columnIndex)
	{
		columns[columnIndex].SetSymbol(symbolIndex);
	}

	public void SetSymbols(int[] symbolIndices)
	{
		for(int i = 0; i<columns.Count;i++)
		{
			columns[i].SetSymbol(symbolIndices[i]);
		}
	}

	private void Cleanup()
	{
		IsValidated = false;

		hoveredColumn = null;

		validateButton.gameObject.SetActive(false);

		foreach (var column in transform.GetComponentsInChildren<ResponseColumn>())
		{
			Destroy(column.gameObject);
		}
	}
}
