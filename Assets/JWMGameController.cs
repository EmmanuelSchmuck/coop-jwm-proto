using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;
using System.Linq;

public class JWMGameController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private int sequenceLength;
    [SerializeField] private List<Sprite> cardShapePool;
    [Header("References")]
    [SerializeField] private StimulusDisplay stimulusDisplay;

    private void Start()
    {
        StartGame();

        
    }

    private void StartGame()
	{
        List<Sprite> shapeSequence = new List<Sprite>();
        
        Sprite lastShape = null;
        
        for(int i = 0; i<sequenceLength;i++)
		{
            Sprite shape;

            do
			{
                shape = cardShapePool.Random();
            }
            while (lastShape == shape) ;

            shapeSequence.Add(shape);

            lastShape = shape;
        }

        stimulusDisplay.Initialize(shapeSequence);
    }


    private void Update()
    {
        
    }
}
