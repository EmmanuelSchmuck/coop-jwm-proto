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
    [SerializeField] private SymbolKeyboard playerA_Keyboard;
    [SerializeField] private SymbolKeyboard playerB_Keyboard;
    [SerializeField] private ResponsePanel playerA_ResponsePanel;
    [SerializeField] private ResponsePanel playerB_ResponsePanel;

    private void Start()
    {
        playerA_Keyboard.Initialize(cardShapePool);
        playerB_Keyboard.Initialize(cardShapePool);
        playerA_ResponsePanel.Initialize(sequenceLength, cardShapePool);
        playerB_ResponsePanel.Initialize(sequenceLength, cardShapePool);

        List<Sprite> shapeSequence = new List<Sprite>();

        Sprite lastShape = null;

        for (int i = 0; i < sequenceLength; i++)
        {
            Sprite shape;

            do
            {
                shape = cardShapePool.Random();
            }
            while (lastShape == shape);

            shapeSequence.Add(shape);

            lastShape = shape;
        }

        stimulusDisplay.Initialize(shapeSequence);
    }
}
