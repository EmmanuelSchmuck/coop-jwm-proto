using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SymbolKeyboard : MonoBehaviour
{
    [SerializeField] private GameObject cover;
    public int? SelectedSymbolIndex { get; private set; }
    public bool Interactable
    {
        get => interactable;
        set
        {
            interactable = value;
            cover.SetActive(!interactable);
            foreach (var symbolKey in transform.GetComponentsInChildren<SymbolKey>())
            {
                symbolKey.SetInteractable(interactable);
            }
        }
    }

    private bool interactable;
    public void Initialize(int symbolPoolSize)
	{
        int i = 0;
        foreach(var symbolKey in transform.GetComponentsInChildren<SymbolKey>())
        {
            symbolKey.Initialize(symbolIndex: i);

            symbolKey.Clicked += OnKeyClicked;

            i++;
        }

        SelectedSymbolIndex = null;
    }

    private void OnKeyClicked(SymbolKey key)
	{
        if (!Interactable) return;
        SoundManager.Instance.PlaySound(SoundType.GenericClick);
        SelectedSymbolIndex = key.SymbolIndex;
    }

    public void ResetSelection()
	{
        SelectedSymbolIndex = null;
    }
}
