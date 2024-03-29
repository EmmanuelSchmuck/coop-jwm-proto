using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbolKeyboard : MonoBehaviour
{
    [SerializeField] private Transform keyContainer;
    [SerializeField] private CanvasGroup keyCanvasGroup;
    [SerializeField] private ButtonHighlighter highlight;
    public int? SelectedSymbolIndex { get; private set; }
    public bool Highlighted
    {
        set
        {
            highlight.gameObject.SetActive(value);
            //foreach (var symbolKey in keyContainer.GetComponentsInChildren<SymbolKey>())
            //{
            //    symbolKey.Highlighted = value;
            //}
        }
    }
    public event System.Action SymbolSelected;
    public bool Interactable
    {
        get => interactable;
        set
        {
            interactable = value;
            keyCanvasGroup.alpha = interactable ? 1f : 0f;
            keyCanvasGroup.interactable = interactable;
            foreach (var symbolKey in keyContainer.GetComponentsInChildren<SymbolKey>())
            {
                symbolKey.SetInteractable(interactable);
            }
        }
    }

    private bool interactable;
    public void Initialize(int symbolPoolSize)
	{
        int i = 0;
        foreach(var symbolKey in keyContainer.GetComponentsInChildren<SymbolKey>())
        {
            symbolKey.Initialize(symbolIndex: i);

            symbolKey.Clicked += OnKeyClicked;

            i++;
        }

        SelectedSymbolIndex = null;
    }

    public void SetSelectedSymbolIndex(int symbolIndex)
	{
        SelectSymbol(symbolIndex);
    }

    private void SelectSymbol(int symbolIndex)
	{
        SelectedSymbolIndex = symbolIndex;
        SymbolSelected?.Invoke();
	}

    private void OnKeyClicked(SymbolKey key)
	{
        if (!Interactable) return;
        SoundManager.Instance.PlaySound(SoundType.GenericClick);
        SelectSymbol(key.SymbolIndex);
    }

    public void ResetSelection()
	{
        SelectedSymbolIndex = null;
    }
}
