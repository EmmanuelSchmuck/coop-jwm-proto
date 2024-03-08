using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check : MonoBehaviour
{
    [SerializeField] private GameObject correctImage, incorrectImage;
    public void Show(bool isCorrect)
	{
		correctImage.SetActive(isCorrect);
		incorrectImage.SetActive(!isCorrect);
	}

	public void Hide()
	{
		correctImage.SetActive(false);
		incorrectImage.SetActive(false);
	}
}
