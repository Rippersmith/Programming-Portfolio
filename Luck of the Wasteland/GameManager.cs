using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int playerHealth = 100, playerMaxHealth = 100;
    public int money = 0;

    public int difficulty = 1;
    public bool costActive = true;

    public Enemy[] rEnemiesList;
    public Enemy[] easyEnemiesList;
    public Enemy[] medEnemiesList;
    public Enemy[] hardEnemiesList;

    public List<Card> deck;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
            Destroy(gameObject);

        //TODO: do thew same "mind-reading prediction" used in "Slay the Spire" to calculate total damagethey will deal each turn
    }

    public Enemy GetNewEnemy()
    {
        if (difficulty == 0)
        {
            return ReturnRandomEnemyFromArray(rEnemiesList);
        }
        else if (difficulty == 1)
        {
            return ReturnRandomEnemyFromArray(easyEnemiesList);
        }
        else if (difficulty == 2)
        {
            return ReturnRandomEnemyFromArray(medEnemiesList);
        }
        else if (difficulty == 3)
        {
            return ReturnRandomEnemyFromArray(hardEnemiesList);
        }

        return easyEnemiesList[0];
    }

    Enemy ReturnRandomEnemyFromArray(Enemy[] array)
    {
        int rnd = Random.Range(0, array.Length);
        return array[rnd];
    }

    public void SetInfoInGameManager(int playerHealth, int moneyAdded)
    {

    }

    //use this function when the player dies
    public void ResetGameManager()
    {
        deck.Clear();
        money = 0;
        playerHealth = 100;
        playerMaxHealth = 100;
    }
}
