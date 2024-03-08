using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ConfigPanel : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField sequenceLenghtInputField;
    [SerializeField] private TMPro.TMP_InputField displayDurationInputField;
    [SerializeField] private TMPro.TMP_InputField coinPerRoundInputField;
    [SerializeField] private TMPro.TMP_InputField maxCoinPerCardInputField;
    [SerializeField] private Toggle allowSymbolRepetitionToggle;
    [SerializeField] private TMPro.TMP_Dropdown rewardDependancyDropdown;
    public JWMGameConfig config;
    // Start is called before the first frame update
    void Start()
    {
        sequenceLenghtInputField.text = config.sequenceLength.ToString();
        sequenceLenghtInputField.onValueChanged.AddListener(delegate { OnSequenceLengthInputChanged(); });

        displayDurationInputField.text = config.displayDurationPerSymbol.ToString();
        displayDurationInputField.onValueChanged.AddListener(delegate { OnDisplayDurationInputChanged(); });

        coinPerRoundInputField.text = config.coinPerRound.ToString();
        coinPerRoundInputField.onValueChanged.AddListener(delegate { OnCoinperRoundInputChanged(); });

        maxCoinPerCardInputField.text = config.maxCoinPerSymbol.ToString();
        maxCoinPerCardInputField.onValueChanged.AddListener(delegate { OnMaxCoinPerCardInputChanged(); });

        allowSymbolRepetitionToggle.isOn = config.allowSymbolRepetition;
        allowSymbolRepetitionToggle.onValueChanged.AddListener(delegate { OnAllowSymbolRepetitionValueChanged(); });

        rewardDependancyDropdown.options = new List<TMPro.TMP_Dropdown.OptionData>() { };

        rewardDependancyDropdown.AddOptions(EnumUtils.GetValues<GameMode>().Select(x => x.ToString()).ToList());

        rewardDependancyDropdown.onValueChanged.AddListener(delegate { OnGamerModeInputChanged(); });
    }

    private void OnAllowSymbolRepetitionValueChanged()
	{
        config.allowSymbolRepetition = allowSymbolRepetitionToggle.isOn;
    }

    private void OnGamerModeInputChanged()
    {
        int value = rewardDependancyDropdown.value;
        config.gameMode = (GameMode)value;
        
    }

    private void OnSequenceLengthInputChanged()
	{
        string text = sequenceLenghtInputField.text;
        if (int.TryParse(text, out int result))
        {
            int validatedResult = config.ClampSequenceLength(result);
            sequenceLenghtInputField.text = validatedResult.ToString();
            config.sequenceLength = validatedResult;
        }
    }

    private void OnMaxCoinPerCardInputChanged()
    {
        string text = maxCoinPerCardInputField.text;
        if (int.TryParse(text, out int result))
        {
            int validatedResult = config.ClampMaxCoinPerSymbol(result);
            maxCoinPerCardInputField.text = validatedResult.ToString();
            config.maxCoinPerSymbol = validatedResult;
        }
    }

    private void OnCoinperRoundInputChanged()
    {
        string text = coinPerRoundInputField.text;
        if (int.TryParse(text, out int result))
        {
            int validatedResult = config.ClampCoinPerRound(result);
            coinPerRoundInputField.text = validatedResult.ToString();
            config.coinPerRound = validatedResult;
        }
    }

    private void OnDisplayDurationInputChanged()
    {
        string text = displayDurationInputField.text;
        if (float.TryParse(text, out float result))
        {
            float validatedResult = config.ClampDisplayDurationPerSymbol(result);
            displayDurationInputField.text = validatedResult.ToString();
            config.displayDurationPerSymbol = validatedResult;
        }
    }
}
