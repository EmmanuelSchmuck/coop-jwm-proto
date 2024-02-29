using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CoinZone : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private ResponseColumn parent;

    public void Initialize(ResponseColumn parent)
	{
        this.parent = parent;
	}
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            JWMGameController.Instance.WIP_OnResponseColumnAddCoinClicked(parent);
        else if (eventData.button == PointerEventData.InputButton.Right)
            JWMGameController.Instance.WIP_OnResponseColumnRemoveCoinClicked(parent);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        JWMGameController.Instance.WIP_OnResponseColumnCoinZoneMouseEnter(parent);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        JWMGameController.Instance.WIP_OnResponseColumnCoinZoneMouseLeave(parent);
    }
}
