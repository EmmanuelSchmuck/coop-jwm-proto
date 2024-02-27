using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolKeyboard : MonoBehaviour
{
    public void Initialize(List<Sprite> cardShapes)
	{
        int i = 0;
        foreach(var symbolKey in transform.GetComponentsInChildren<SymbolKey>())
        {
            symbolKey.Initialize(symbolIndex: i, symbolIcon: cardShapes[i]);

            i++;
        }
    }
}
