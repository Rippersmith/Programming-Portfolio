using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMessenger : MonoBehaviour
{
    [Header("Player Turn Canvas")]
    [SerializeField] Canvas playerTurnCanvas;
    [SerializeField] TMP_Text playerTurnMessage;

    [Header("Victory Canvas")]
    [SerializeField] Canvas victoryCanvas;
    [SerializeField] TMP_Text victoryMessage;


    public void ShowPlayerTurnMessage(string playerName)
    {
        playerTurnCanvas.enabled = true;
        playerTurnMessage.text = playerName + "'s Turn!";
    }

    public void ShowVictoryMessage(string playerName)
    {
        victoryCanvas.enabled = true;
        victoryMessage.text = playerName + " Wins!!!";
    }

    public void CloseMessages()
    {
        playerTurnCanvas.enabled = false;
        victoryCanvas.enabled = false;
    }

    public void LoadNewScene(string newSceneName)
    {
        SceneManager.LoadScene(newSceneName, LoadSceneMode.Single);
    }
}
