using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;

public class LobbyController : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI messageText;
    [SerializeField] private UnityEngine.UI.Button okButton;
    private bool okButtonClicked;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForPlayer());
    }

    public void OnOKButtonClick()
	{
        okButtonClicked = true;
    }

    private IEnumerator WaitForPlayer()
	{
        okButton.interactable = false;

        messageText.text = "Waiting for another player...";

        yield return new WaitForSeconds(6f);

        messageText.text = "Player found !";

        yield return new WaitForSeconds(2f);

        messageText.text = "Click the START GAME button when ready.";
        okButton.interactable = true;

        yield return new WaitUntil(() => okButtonClicked);

        messageText.text = "Waiting for other player...";

        yield return new WaitForSeconds(3f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
	}
}
