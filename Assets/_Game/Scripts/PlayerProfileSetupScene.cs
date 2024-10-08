using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerProfileSystem;
using UnityEngine.SceneManagement;

public class PlayerProfileSetupScene : MonoBehaviour
{
    [SerializeField] private PlayerProfileSetup profileSetup;
    // Start is called before the first frame update
    void Start()
    {
        AppState.EnsureAppState();
        profileSetup.ValidateButtonClicked += OnValideButtonClicked;
    }

    private void OnValideButtonClicked()
	{
        AppState.HumanPlayerInfo = profileSetup.PlayerInfo;
        SceneManager.LoadScene("Game");
	}
}
