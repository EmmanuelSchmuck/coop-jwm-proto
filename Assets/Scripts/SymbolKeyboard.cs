using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbolKeyboard : MonoBehaviour
{
    public int? SelectedSymbolIndex { get; private set; }

    private SymbolKey selectedKey; // used ?
    public void Initialize(List<Sprite> cardShapes)
	{
        int i = 0;
        foreach(var symbolKey in transform.GetComponentsInChildren<SymbolKey>())
        {
            symbolKey.Initialize(symbolIndex: i, symbolIcon: cardShapes[i]);

            symbolKey.Clicked += OnKeyClicked;

            i++;
        }

        SelectedSymbolIndex = null;
    }

    private void OnKeyClicked(SymbolKey key)
	{
        selectedKey = key;
        SelectedSymbolIndex = key.SymbolIndex;
    }

    public void ResetSelection()
	{
        selectedKey = null;
        SelectedSymbolIndex = null;
    }
}
