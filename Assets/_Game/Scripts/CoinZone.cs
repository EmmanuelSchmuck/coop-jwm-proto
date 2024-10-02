using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Toolbox;

namespace Deprecaded
{
	public class CoinZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private ButtonHighlighter highlight;
		[SerializeField] private CanvasGroup canvasGroup;
		[SerializeField] private AnimationCurve fadeCurve;
		[SerializeField] private CoinRepository coinRepo;
		public bool Highlighted
		{
			set
			{
				highlight.gameObject.SetActive(value);
			}
		}
		private ResponseColumn parent;

		public bool Interactable { get; set; }

		public int CoinCount => coinRepo.CoinCount;
		public int CoinValueSum => coinRepo.CoinValueSum;


		public void Initialize(ResponseColumn parent)
		{
			this.parent = parent;
			coinRepo.Initialize();
			coinRepo.CoinAdded += OnCoinAdded;
			coinRepo.CoinRemoved += OnCoinRemoved;
		}

		public void FadeToVisible(bool visible)
		{
			float startAlpha = visible ? 0f : 1f;
			float targetAlpha = visible ? 1f : 0f;
			StartCoroutine(CoroutineTools.Tween(startAlpha, targetAlpha, 0.7f, t => canvasGroup.alpha = fadeCurve.Evaluate(t)));
		}

		private void OnCoinAdded()
		{
			parent.OnAddCoinButtonClick();
		}

		private void OnCoinRemoved()
		{
			parent.OnRemoveCoinButtonClick();
		}

		//public void OnPointerClick(PointerEventData eventData)
		//{
		//	if (!Interactable) return;

		//	if (eventData.button == PointerEventData.InputButton.Left)
		//		parent.OnAddCoinButtonClick();

		//	else if (eventData.button == PointerEventData.InputButton.Right)
		//		parent.OnRemoveCoinButtonClick();
		//}

		public void AddCoin(Coin coin)
		{
			coinRepo.AddCoin(coin);
			parent.OnAddCoinButtonClick();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (!Interactable) return;

			parent.OnCoinZoneHoverEnter();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			parent.OnCoinZoneHoverLeave();
		}
	}
}