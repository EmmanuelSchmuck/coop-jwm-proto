using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResponsePanel : MonoBehaviour
{
    [SerializeField] private ResponseColumn columnPrefab;
    
    public void Initialize(int sequenceLength, List<Sprite> cardShapes)
	{
		Cleanup();

		for (int i = 0; i < sequenceLength; i++)
		{
			ResponseColumn column = Instantiate(columnPrefab, this.transform);
			column.Initialize(cardShapes);
		}
	}

	private void Cleanup()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			Destroy(transform.GetChild(i).gameObject);
		}
	}
}
