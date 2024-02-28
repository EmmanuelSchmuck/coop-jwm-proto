using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;
using System.Linq;

public class JWMGameController : MonoBehaviourSingleton<JWMGameController>
{
    [Header("Config")]
    [SerializeField] private int sequenceLength;
    [SerializeField] private bool allowSymbolRepetition;
    [SerializeField] private List<Sprite> cardShapePool;
    [Header("References")]
    [SerializeField] private StimulusDisplay stimulusDisplay;
    [SerializeField] private SymbolKeyboard playerA_Keyboard;
    [SerializeField] private SymbolKeyboard playerB_Keyboard;
    [SerializeField] private ResponsePanel playerA_ResponsePanel;
    [SerializeField] private ResponsePanel playerB_ResponsePanel;
    private int[] correctIndexSequence;

    private void Start()
    {
        playerA_Keyboard.Initialize(cardShapePool);
        playerB_Keyboard.Initialize(cardShapePool);
        playerA_ResponsePanel.Initialize(sequenceLength, cardShapePool);
        playerB_ResponsePanel.Initialize(sequenceLength, cardShapePool);

        playerA_ResponsePanel.ResponseValidated += CheckForRoundEnd;
        playerB_ResponsePanel.ResponseValidated += CheckForRoundEnd;

        List<Sprite> shapeSequence = new List<Sprite>();
        correctIndexSequence = new int[sequenceLength];

        Sprite lastShape = null;

        for (int i = 0; i < sequenceLength; i++)
        {
            Sprite shape;

            do
            {
                shape = cardShapePool.Random();
            }
            while (!allowSymbolRepetition && lastShape == shape);

            shapeSequence.Add(shape);

            correctIndexSequence[i] = cardShapePool.IndexOf(shape);

            lastShape = shape;
        }

        stimulusDisplay.Initialize(shapeSequence);

        playerB_ResponsePanel.SetSymbols(correctIndexSequence);
        playerB_ResponsePanel.SetValidated();
    }

    private void CheckForRoundEnd()
	{
        bool bothPlayersHaveValidated = playerA_ResponsePanel.IsValidated && playerB_ResponsePanel.IsValidated;

        if(bothPlayersHaveValidated)
		{
            Debug.Log("Round ended, showing correct / incorrect feedback");
            playerA_ResponsePanel.ShowCorrectFeedback(correctIndexSequence);
            playerB_ResponsePanel.ShowCorrectFeedback(correctIndexSequence);
		}

    }

    public void WIP_OnResponseColumnClicked(ResponseColumn column)
	{
        // assume this is from player A

        int? selectedSymbolIndex = playerA_Keyboard.SelectedSymbolIndex;

        if (selectedSymbolIndex == null) return;

        column.SetSymbol((int)selectedSymbolIndex);

        playerA_ResponsePanel.CheckIfCanValidate();

        // playerA_Keyboard.ResetSelection();

    }
}
