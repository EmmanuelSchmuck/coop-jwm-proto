using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;

public class ScoreCounter : MonoBehaviour
{
	[SerializeField] private float incrementAnimationDuration;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;

	private int previousScore;

    public void SetScore(int score, bool animate = true)
	{
		if(animate)
		{
			StopAllCoroutines();
			StartCoroutine(CoroutineTools.Tween(previousScore, score, incrementAnimationDuration, t => UpdateScoreText((int)t)));
		}
		else
		{
			UpdateScoreText(score);
		}

		previousScore = score;
	}

	private void UpdateScoreText(int score)
	{
		scoreText.text = $"Score: {score}";
	}
 }
