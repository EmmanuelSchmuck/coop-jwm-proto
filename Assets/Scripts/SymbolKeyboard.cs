using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbolKeyboard : MonoBehaviour
{
    public int? SelectedSymbolIndex { get; private set; }
    public bool Interactable
    {
        get => interactable;
        set
        {
            interactable = value;
            foreach (var symbolKey in transform.GetComponentsInChildren<SymbolKey>())
            {
                symbolKey.SetInteractable(interactable);
            }
        }
    }

    private bool interactable;
    public void Initialize(Sprite[] cardShapes)
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
        if (!Interactable) return;
        SelectedSymbolIndex = key.SymbolIndex;
    }

    public void ResetSelection()
	{
        SelectedSymbolIndex = null;
    }
}
