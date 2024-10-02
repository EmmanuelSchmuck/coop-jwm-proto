using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Toolbox;

public class SymbolKeyboard : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private AnimationCurve fadeCurve;
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

    public void SetVisible(bool visible, bool animate = false)
    {
        //this.gameObject.SetActive(visible);
        if (animate)
        {
            this.gameObject.SetActive(true);
            this.FadeToVisible(visible);
        }
        else
		{
            this.gameObject.SetActive(visible);
        }

        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }
    private void FadeToVisible(bool visible)
    {
        float startAlpha = visible ? 0f : 1f;
        float targetAlpha = visible ? 1f : 0f;
        float duration = visible ? 0.7f : 0.3f;
        StartCoroutine(CoroutineTools.Tween(startAlpha, targetAlpha, duration, t => canvasGroup.alpha = fadeCurve.Evaluate(t)));
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
