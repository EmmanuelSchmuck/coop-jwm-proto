using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteRendererExtensions
{
    /// <summary>
    /// Set the sorting order according to the y position, in order to fake depth on the z axis.
    /// The renderer's sprite should have its pivot point set to the very bottom.
    /// </summary>
    public static void FakeDepthSort(this SpriteRenderer spriteRenderer, float minY = -10, float maxY = 10, int minOrder = -9999, int maxOrder = 9999, float? yPositionOverride = null)
    {
        float yPosition = yPositionOverride ?? spriteRenderer.transform.position.y;
       
        spriteRenderer.sortingOrder = (int)Mathf.Lerp(minOrder, maxOrder, Mathf.InverseLerp(minY, maxY, -yPosition));
    }
}
