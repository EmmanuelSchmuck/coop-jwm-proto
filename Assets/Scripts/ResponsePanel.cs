using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ResponsePanel : MonoBehaviour
{
    [SerializeField] private ResponseColumn columnPrefab;
	[SerializeField] private Button validateButton;

	public bool IsValidated { get; private set; }

	private List<ResponseColumn> columns;
	public event System.Action ResponseValidated;
	private ResponseColumn hoveredColumn;
    
    public void Initialize(int sequenceLength, List<Sprite> cardShapes)
	{
		Cleanup();

		columns = new List<ResponseColumn>();

		for (int i = 0; i < sequenceLength; i++)
		{
			ResponseColumn column = Instantiate(columnPrefab, this.transform);
			columns.Add(column);
			column.Initialize(cardShapes);
		}
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

	public void OnValidateButtonClick()
	{
		SetValidated();
	}

	public void SetValidated()
	{
		IsValidated = true;
		validateButton.gameObject.SetActive(false);
		ResponseValidated?.Invoke();
	}

	public void SetCoversVisible(bool visible)
	{
		foreach (var column in columns)
		{
			column.SetCoverVisible(visible);
		}
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

	public void CheckIfCanValidate()
	{
		bool allCoinsUsed = true;
		bool allSymbolsChosen = columns.All(c => c.SymbolIndex != null);

		bool canValidate = allCoinsUsed && allSymbolsChosen;

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
