using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfigSceneController : MonoBehaviour
{
    [SerializeField] private bool useEscapeToQuit;
    [SerializeField] private ConfigPanel configPanel;
    private const string gameSceneName = "Game";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (useEscapeToQuit && Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void OnStartGameButtonClick()
	{
        StartGame();   
	}

    private void StartGame()
	{
        AppState.GameConfig = configPanel.config;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnQuitGameButtonClick()
    {
        QuitGame();
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
