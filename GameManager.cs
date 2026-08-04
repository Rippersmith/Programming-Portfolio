using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    //if I come back to this project, I will probably add customizable names for the players
    public string player1Name, player2Name;

    public int matrixSideLength, winningLineLength;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);
    }
}
