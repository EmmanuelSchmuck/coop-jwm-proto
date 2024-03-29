using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;

public class ScoreCounter : MonoBehaviour
{
	[SerializeField] private float incrementAnimationDuration;
	[SerializeField] private TMPro.TextMeshProUGUI scoreText;
	[SerializeField] private TextPopup textPopup;
	[SerializeField] private GameObject particlesPrefab;

	private int previousScore;

	public void SetScore(int score, bool animate = true)
	{
		if (animate)
		{
			StopAllCoroutines();
			StartCoroutine(CoroutineTools.Tween(previousScore, score, incrementAnimationDuration, t => UpdateScoreText((int)t)));
			textPopup.Animate($"+{score - previousScore}");
			GameObject particles = Instantiate(particlesPrefab, textPopup.transform.position, Quaternion.identity);
		}
		else
		{
			UpdateScoreText(score);
		}

		previousScore = score;
	}

	private void UpdateScoreText(int score)
	{
		scoreText.text = $"{score}";
	}
}
